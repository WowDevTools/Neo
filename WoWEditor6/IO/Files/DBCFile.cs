using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace WoWEditor6.IO.Files
{
    unsafe class DbcRecord : IDataStorageRecord
    {
        private readonly int mSize;
        private readonly byte[] mData;
        private readonly Dictionary<int, string> mStringTable;

        // ReSharper disable UnusedParameter.Local
        private void AssertValid(int offset, int size)
        {
            if (offset + size > mSize)
                throw new IndexOutOfRangeException("Trying to read past the end of a dbc record");
        }

        private void AssertString<T>() where T : struct
        {
            if (typeof(T).GetFields().Any(x => x.FieldType == typeof(string)))
                throw new TypeLoadException();
        }


        public DbcRecord(int size, int offset, BinaryReader reader, Dictionary<int, string> stringTable)
        {
            mSize = size;
            mData = new byte[size];
            reader.BaseStream.Position = offset;
            reader.BaseStream.Read(mData, 0, mSize);
            mStringTable = stringTable;
        }

        /// <summary>
        /// Use this for DBC structs with strings
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : struct
        {
            ValueType ret = new T();

            using (var ms = new MemoryStream(mData))
            using (var br = new BinaryReader(ms))
            {
                foreach (var field in typeof(T).GetFields())
                {
                    if (field.FieldType == typeof(string)) //StringTable
                    {
                        int strId = br.ReadInt32();
                        string strVal = string.Empty;
                        mStringTable.TryGetValue(strId, out strVal);
                        field.SetValue(ret, strVal);
                    }
                    else
                    {
                        int sizeOf = Marshal.SizeOf(field.FieldType);
                        byte[] bytes = br.ReadBytes(sizeOf);
                        fixed (byte* dataPtr = bytes)
                        {
                            object val = Marshal.PtrToStructure(new IntPtr(dataPtr), field.FieldType);
                            field.SetValue(ret, val);
                        }
                    }
                }
            }

            return (T)ret;
        }

        /// <summary>
        /// Use this for DBC structs without strings
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="offset"></param>
        /// <returns></returns>
        public T Get<T>(int offset) where T : struct
        {
            AssertValid(offset, SizeCache<T>.Size);
            AssertString<T>();

            var ret = new T();
            var ptr = SizeCache<T>.GetUnsafePtr(ref ret);
            fixed (byte* data = mData)
                UnsafeNativeMethods.CopyMemory((byte*)ptr, data, SizeCache<T>.Size);

            return ret;
        }

        public int GetInt32(int field)
        {
            AssertValid(field * 4, 4);
            fixed (byte* ptr = mData)
                return *(int*)(ptr + field * 4);
        }

        public uint GetUint32(int field)
        {
            AssertValid(field * 4, 4);
            fixed (byte* ptr = mData)
                return *(uint*)(ptr + field * 4);
        }

        public float GetFloat(int field)
        {
            AssertValid(field * 4, 4);
            fixed (byte* ptr = mData)
                return *(float*)(ptr + field * 4);
        }

        public string GetString(int field)
        {
            AssertValid(field * 4, 4);
            int offset;
            fixed (byte* ptr = mData)
                offset = *(int*)(ptr + field * 4);

            if (offset == 0)
                return null;

            string str;
            mStringTable.TryGetValue(offset, out str);
            return str;
        }
    }

    class DbcFile : IDataStorageFile
    {
        private int mRecordSize;
        private int mStringTableSize;
        private Stream mStream;
        private BinaryReader mReader;
        private Dictionary<int, string> mStringTable = new Dictionary<int, string>();
        private Dictionary<int, int> mIdLookup = new Dictionary<int, int>();

        private const int HEADER = 20;

        public int NumRows { get; private set; }
        public int NumFields { get; private set; }

        public void Load(string file)
        {
            Stream stream = FileManager.Instance.Provider.OpenFile(file);
            if (stream == null)
                throw new FileNotFoundException(file);

            Load(stream);
        }

        public void Load(Stream stream)
        {
            mStream = stream;
            mReader = new BinaryReader(mStream);
            mReader.BaseStream.Position = 0;
            mReader.ReadInt32(); // signature
            NumRows = mReader.ReadInt32();
            NumFields = mReader.ReadInt32();
            mRecordSize = mReader.ReadInt32();
            mStringTableSize = mReader.ReadInt32();

            mStream.Position = NumRows * mRecordSize + HEADER;
            var strBytes = mReader.ReadBytes(mStringTableSize);
            var curOffset = 0;
            var curBytes = new List<byte>();
            for (var i = 0; i < strBytes.Length; ++i)
            {
                if (strBytes[i] != 0)
                {
                    curBytes.Add(strBytes[i]);
                    continue;
                }

                mStringTable.Add(curOffset, Encoding.UTF8.GetString(curBytes.ToArray()));
                curBytes.Clear();
                curOffset = i + 1;
            }

            for (var i = 0; i < NumRows; ++i)
                mIdLookup.Add(GetRow(i).GetInt32(0), i);
        }

        public void Save(string file)
        {
            using (var fs = new FileStream(file, FileMode.Create))
            {
                mReader.BaseStream.Position = 0;
                byte[] data = mReader.ReadBytes((int)mStream.Length);
                fs.Write(data, 0, data.Length);
            }
        }

        public void ReLoad(Stream stream)
        {
            mStream.Dispose();
            mReader.Dispose();
            mStringTable.Clear();
            mIdLookup.Clear();

            Load(stream);
        }


        public IDataStorageRecord GetRow(int index)
        {
            return new DbcRecord(mRecordSize, HEADER + index * mRecordSize, mReader, mStringTable);
        }

        public IDataStorageRecord GetRowById(int id)
        {
            int index;
            return mIdLookup.TryGetValue(id, out index) ? GetRow(index) : null;
        }

        public IEnumerable<IDataStorageRecord> GetAllRows()
        {
            foreach (var i in Enumerable.Range(0, NumRows))
                yield return GetRow(i);
        }

        public IEnumerable<T> GetAllRows<T>() where T : struct
        {
            foreach (var i in Enumerable.Range(0, NumRows))
                yield return GetRow(i).Get<T>();
        }


        public bool DeleteRow(int index)
        {
            //TODO remove strings from string table
            if (!mIdLookup.ContainsValue(index))
                return false;

            mReader.BaseStream.Position = 0;
            byte[] data = mReader.ReadBytes((int)mStream.Length);

            var ms = new MemoryStream();
            int start = HEADER + index * mRecordSize;
            int end = data.Length - start - mRecordSize;
            ms.Write(data, 0, start); //Header + rows before data
            ms.Write(data, start + mRecordSize, end); //Skip row being removed's bytes

            ms.Position = 4;
            ms.Write(BitConverter.GetBytes(NumRows - 1), 0, 4); //Update the record count

            ReLoad(ms);
            return true;
        }

        public bool DeleteRowById(int id)
        {
            int index;
            return mIdLookup.TryGetValue(id, out index) ? DeleteRow(index) : false;
        }


        public int AddString(string value)
        {
            var maxIndex = 0;
            var maxLen = 0;

            foreach (var pair in mStringTable)
            {
                if (pair.Value.Equals(value))
                    return pair.Key;

                if (pair.Key <= maxIndex) continue;

                maxIndex = pair.Key;
                maxLen = pair.Value.Length;
            }

            maxIndex += maxLen + 1;
            mStringTable.Add(maxIndex, value);
            return maxIndex;
        }

        public void AddRow<T>(T entry)
        {
            var newRecord = ParseRecord(entry);
            NumRows += 1;
            Update(newRecord.Item1, newRecord.Item2);
        }


        public void UpdateRow<T>(T entry)
        {
            var newRecord = ParseRecord(entry, true);
            var id = BitConverter.ToInt32(newRecord.Item1.Take(4).ToArray(), 0);
            var index = mIdLookup[id];

            mStream.Position = HEADER + index * mRecordSize;
            mStream.Write(newRecord.Item1, 0, newRecord.Item1.Length); //Overwrite existing data

            Update(new byte[0], newRecord.Item2);
        }

        #region Helpers
        /// <summary>
        /// Returns record bytes and any new strings to be added to the StringTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entry"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        private Tuple<byte[], IEnumerable<string>> ParseRecord<T>(T entry, bool update = false)
        {
            Type type = entry.GetType();

            byte[] newRecord = new byte[mRecordSize];
            List<string> newStrings = new List<string>();
            bool first = true;

            using (var ms = new MemoryStream(newRecord))
            using (var bw = new BinaryWriter(ms))
            {
                foreach (var field in type.GetFields())
                {
                    if (first && !update) //Autoincrement Id only if adding a new record
                    {
                        bw.Write(mIdLookup.Keys.Max() + 1);
                        first = false;
                        continue;
                    }

                    if (field.FieldType == typeof(string))
                    {
                        int stSize = mStringTable.Count;
                        string strVal = Convert.ToString(field.GetValue(entry));
                        int strID = AddString(strVal);
                        bw.Write(strID);

                        if (mStringTable.Count > stSize) //Append to our list of new strings
                            newStrings.Add(strVal);
                    }
                    else
                    {
                        int sizeOf = Marshal.SizeOf(field.FieldType);
                        byte[] arr = new byte[sizeOf];
                        IntPtr ptr = Marshal.AllocHGlobal(sizeOf);
                        Marshal.StructureToPtr(field.GetValue(entry), ptr, true);
                        Marshal.Copy(ptr, arr, 0, sizeOf);
                        Marshal.FreeHGlobal(ptr);
                        bw.Write(arr);
                    }
                }
            }

            return new Tuple<byte[], IEnumerable<string>>(newRecord, newStrings);
        }

        private void Update(byte[] newRecord, IEnumerable<string> newStrings)
        {
            mReader.BaseStream.Position = 0;
            byte[] curdata = mReader.ReadBytes((int)mStream.Length - mStringTableSize);
            byte[] stringtable = mReader.ReadBytes(mStringTableSize);

            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms, Encoding.UTF8);

            bw.Write(curdata); //Write header and record data
            bw.Write(newRecord); //Write new record if any
            
            bw.Write(stringtable); //Write existing string table
            foreach (var s in newStrings) //Write new strings if any
            {
                byte[] sd = Encoding.UTF8.GetBytes(s);
                bw.Write(sd);
                bw.Write((byte)0);
                mStringTableSize += (sd.Length + 1);
            }

            bw.BaseStream.Position = 4;
            bw.Write(NumRows); //Number of rows
            bw.BaseStream.Position = 0x10;
            bw.Write(mStringTableSize); //StringTable size
            bw.Flush();

            ReLoad(ms);
        }
        #endregion

        ~DbcFile()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (mStream != null)
            {
                mStream.Dispose();
                mStream = null;
            }

            if (mReader != null)
            {
                mReader.Dispose();
                mReader = null;
            }

            if (mStringTable != null)
            {
                mStringTable.Clear();
                mStringTable = null;
            }

            if (mIdLookup != null)
            {
                mIdLookup.Clear();
                mIdLookup = null;
            }
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
