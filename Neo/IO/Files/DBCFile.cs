using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Neo.IO.Files
{
	public unsafe class DbcRecord : IDataStorageRecord
    {
        private readonly int mSize;
        private readonly byte[] mData;
        private readonly Dictionary<int, string> mStringTable;

        // ReSharper disable UnusedParameter.Local
        private void AssertValid(int offset, int size)
        {
            if (offset + size > this.mSize)
            {
	            throw new IndexOutOfRangeException("Trying to read past the end of a dbc record");
            }
        }

        private void AssertString<T>() where T : struct
        {
            if (typeof(T).GetFields().Any(x => x.FieldType == typeof(string)))
            {
	            throw new TypeLoadException();
            }
        }


        public DbcRecord(int size, int offset, BinaryReader reader, Dictionary<int, string> stringTable)
        {
	        this.mSize = size;
	        this.mData = new byte[size];
            reader.BaseStream.Position = offset;
            reader.BaseStream.Read(this.mData, 0, this.mSize);
	        this.mStringTable = stringTable;
        }

        /// <summary>
        /// Use this for DBC structs with strings
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : struct
        {
            ValueType ret = new T();

            using (var ms = new MemoryStream(this.mData))
            {
	            using (var br = new BinaryReader(ms))
	            {
		            foreach (var field in typeof(T).GetFields())
		            {
			            if (field.FieldType == typeof(string)) //StringTable
			            {
				            int strId = br.ReadInt32();
				            string strVal = string.Empty;
				            this.mStringTable.TryGetValue(strId, out strVal);
				            field.SetValue(ret, strVal);
			            }
			            else if (field.FieldType == typeof(LocalisedString))
			            {
				            string[] strings = new string[16];
				            for (int i = 0; i < 16; i++)
				            {
					            int strId = br.ReadInt32();
					            string strVal = string.Empty;
					            this.mStringTable.TryGetValue(strId, out strVal);
					            strings[i] = strVal;
				            }
				            var loc = new LocalisedString(strings, br.ReadInt32());
				            field.SetValue(ret, loc);
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
            fixed (byte* data = this.mData)
            {
	            UnsafeNativeMethods.CopyMemory((byte*)ptr, data, SizeCache<T>.Size);
            }

	        return ret;
        }

        public int GetInt32(int field)
        {
            AssertValid(field * 4, 4);
            fixed (byte* ptr = this.mData)
            {
	            return *(int*)(ptr + field * 4);
            }
        }

        public uint GetUint32(int field)
        {
            AssertValid(field * 4, 4);
            fixed (byte* ptr = this.mData)
            {
	            return *(uint*)(ptr + field * 4);
            }
        }

        public float GetFloat(int field)
        {
            AssertValid(field * 4, 4);
            fixed (byte* ptr = this.mData)
            {
	            return *(float*)(ptr + field * 4);
            }
        }

        public string GetString(int field)
        {
            AssertValid(field * 4, 4);
            int offset;
            fixed (byte* ptr = this.mData)
            {
	            offset = *(int*)(ptr + field * 4);
            }

	        if (offset == 0)
	        {
		        return null;
	        }

	        string str;
	        this.mStringTable.TryGetValue(offset, out str);
            return str;
        }
    }

	public class DbcFile : IDataStorageFile
    {
        private int mRecordSize;
        private int mStringTableSize;
        private Stream mStream;
        private BinaryReader mReader;
        private Dictionary<int, string> mStringTable = new Dictionary<int, string>();
        private Dictionary<int, int> mIdLookup = new Dictionary<int, int>();
        private List<object> mCache = new List<object>();

        private const int HEADER = 20;

        public int NumRows { get; private set; }
        public int NumFields { get; private set; }

        public void Load(string file)
        {
            Stream stream = FileManager.Instance.Provider.OpenFile(file);
            if (stream == null)
            {
	            throw new FileNotFoundException(file);
            }

	        Load(stream);
        }

        public void Load(Stream stream)
        {
	        this.mStream = stream;
	        this.mReader = new BinaryReader(this.mStream);
	        this.mReader.BaseStream.Position = 0;
	        this.mReader.ReadInt32(); // signature
	        this.NumRows = this.mReader.ReadInt32();
	        this.NumFields = this.mReader.ReadInt32();
	        this.mRecordSize = this.mReader.ReadInt32();
	        this.mStringTableSize = this.mReader.ReadInt32();

	        this.mStream.Position = this.NumRows * this.mRecordSize + HEADER;
            var strBytes = this.mReader.ReadBytes(this.mStringTableSize);
            var curOffset = 0;
            var curBytes = new List<byte>();
            for (var i = 0; i < strBytes.Length; ++i)
            {
                if (strBytes[i] != 0)
                {
                    curBytes.Add(strBytes[i]);
                    continue;
                }

	            this.mStringTable.Add(curOffset, Encoding.UTF8.GetString(curBytes.ToArray()));
                curBytes.Clear();
                curOffset = i + 1;
            }

            for (var i = 0; i < this.NumRows; ++i)
            {
	            this.mIdLookup.Add(GetRow(i).GetInt32(0), i);
            }
        }

        public void Save(string file)
        {
            using (var fs = new FileStream(file, FileMode.Create))
            {
	            this.mReader.BaseStream.Position = 0;
                byte[] data = this.mReader.ReadBytes((int) this.mStream.Length);
                fs.Write(data, 0, data.Length);
            }
        }

        public void ReLoad(Stream stream)
        {
	        this.mStream.Dispose();
	        this.mReader.Dispose();
	        this.mStringTable.Clear();
	        this.mIdLookup.Clear();

            Load(stream);
        }


        public void BuildCache<T>() where T : struct
        {
            for (int i = 0; i < this.NumRows; i++)
            {
	            this.mCache.Add(GetRow(i).Get<T>());
            }
        }

        public void ClearCache()
        {
	        this.mCache.Clear();
	        this.mCache.TrimExcess();
        }


        public IDataStorageRecord GetRow(int index)
        {
            return new DbcRecord(this.mRecordSize, HEADER + index * this.mRecordSize, this.mReader, this.mStringTable);
        }

        public IDataStorageRecord GetRowById(int id)
        {
            int index;
            return this.mIdLookup.TryGetValue(id, out index) ? GetRow(index) : null;
        }

        public IList<IDataStorageRecord> GetAllRows()
        {
            List<IDataStorageRecord> files = new List<IDataStorageRecord>();
            for (int i = 0; i < this.NumRows; i++)
            {
	            files.Add(GetRow(i));
            }
	        return files;
        }

        public IList<T> GetAllRows<T>() where T : struct
        {
            if (this.mCache.Count > 0)
            {
	            return this.mCache.Cast<T>().ToList();
            }

	        List<T> files = new List<T>();
            for (int i = 0; i < this.NumRows; i++)
            {
	            files.Add(GetRow(i).Get<T>());
            }
	        return files;
        }


        public bool DeleteRow(int index)
        {
            //TODO remove strings from string table
            if (!this.mIdLookup.ContainsValue(index))
            {
	            return false;
            }

	        if (this.mCache.Count > 0)
	        {
		        this.mCache.RemoveAt(index);
	        }

	        this.mReader.BaseStream.Position = 0;
            byte[] data = this.mReader.ReadBytes((int) this.mStream.Length);

            var ms = new MemoryStream();
            int start = HEADER + index * this.mRecordSize;
            int end = data.Length - start - this.mRecordSize;
            ms.Write(data, 0, start); //Header + rows before data
            ms.Write(data, start + this.mRecordSize, end); //Skip row being removed's bytes

            ms.Position = 4;
            ms.Write(BitConverter.GetBytes(this.NumRows - 1), 0, 4); //Update the record count

            ReLoad(ms);
            return true;
        }

        public bool DeleteRowById(int id)
        {
            int index;
            return this.mIdLookup.TryGetValue(id, out index) ? DeleteRow(index) : false;
        }


        public int AddString(string value)
        {
            var maxIndex = 0;
            var maxLen = 0;

            foreach (var pair in this.mStringTable)
            {
                if (pair.Value.Equals(value))
                {
	                return pair.Key;
                }

	            if (pair.Key <= maxIndex)
	            {
		            continue;
	            }

	            maxIndex = pair.Key;
                maxLen = pair.Value.Length;
            }

            maxIndex += maxLen + 1;
	        this.mStringTable.Add(maxIndex, value);
            return maxIndex;
        }

        public void AddRow<T>(T entry)
        {
            typeof(T).GetFields().First().SetValue(entry, this.mIdLookup.Keys.Max() + 1); //Set Id

            if (this.mCache.Count > 0)
            {
	            this.mCache.Add(entry);
            }

	        var newRecord = ParseRecord(entry);
	        this.NumRows += 1;
            Update(newRecord.Item1, newRecord.Item2);
        }


        public void UpdateRow<T>(T entry)
        {
            var newRecord = ParseRecord(entry, true);
            var id = BitConverter.ToInt32(newRecord.Item1.Take(4).ToArray(), 0);
            var index = this.mIdLookup[id];

            if (this.mCache.Count > 0)
            {
	            this.mCache[index] = entry;
            }

	        this.mStream.Position = HEADER + index * this.mRecordSize;
	        this.mStream.Write(newRecord.Item1, 0, newRecord.Item1.Length); //Overwrite existing data

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

            byte[] newRecord = new byte[this.mRecordSize];
            List<string> newStrings = new List<string>();

            using (var ms = new MemoryStream(newRecord))
            {
	            using (var bw = new BinaryWriter(ms))
	            {
		            foreach (var field in type.GetFields())
		            {
			            if (field.FieldType == typeof(string))
			            {
				            int stSize = this.mStringTable.Count;
				            string strVal = Convert.ToString(field.GetValue(entry));
				            int strID = AddString(strVal);
				            bw.Write(strID);

				            if (this.mStringTable.Count > stSize) //Append to our list of new strings
				            {
					            newStrings.Add(strVal);
				            }
			            }
			            else if(field.FieldType == typeof(LocalisedString))
			            {
				            foreach(var locfield in typeof(LocalisedString).GetFields())
				            {
					            if (locfield.FieldType == typeof(string))
					            {
						            int stSize = this.mStringTable.Count;
						            string strVal = Convert.ToString(locfield.GetValue(entry));
						            int strID = AddString(strVal);
						            bw.Write(strID);

						            if (this.mStringTable.Count > stSize)
						            {
							            newStrings.Add(strVal);
						            }
					            }
					            else
					            {
						            bw.Write(Convert.ToInt32(locfield.GetValue(entry)));
					            }
				            }
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
            }

	        return new Tuple<byte[], IEnumerable<string>>(newRecord, newStrings);
        }

        private void Update(byte[] newRecord, IEnumerable<string> newStrings)
        {
	        this.mReader.BaseStream.Position = 0;
            byte[] curdata = this.mReader.ReadBytes((int) this.mStream.Length - this.mStringTableSize);
            byte[] stringtable = this.mReader.ReadBytes(this.mStringTableSize);

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
	            this.mStringTableSize += (sd.Length + 1);
            }

            bw.BaseStream.Position = 4;
            bw.Write(this.NumRows); //Number of rows
            bw.BaseStream.Position = 0x10;
            bw.Write(this.mStringTableSize); //StringTable size
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
            if (this.mStream != null)
            {
	            this.mStream.Dispose();
	            this.mStream = null;
            }

            if (this.mReader != null)
            {
	            this.mReader.Dispose();
	            this.mReader = null;
            }

            if (this.mStringTable != null)
            {
	            this.mStringTable.Clear();
	            this.mStringTable = null;
            }

            if (this.mIdLookup != null)
            {
	            this.mIdLookup.Clear();
	            this.mIdLookup = null;
            }
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
