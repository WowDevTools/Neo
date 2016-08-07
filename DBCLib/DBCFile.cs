using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DBCLib
{
    public interface IDBCRowConverter<T> where T : new()
    {
        T Convert(object value);
    }

    public class DBCFile<T> where T : new()
    {
        public DBCFile(string path)
        {
            mReader = new BinaryReader(File.OpenRead(path));
            FileName = path;
            mCreationType = typeof(T);
        }

        public unsafe bool LoadData()
        {
            if (IsLoaded)
                return true;

            byte[] signature = mReader.ReadBytes(4);
            string str = Encoding.UTF8.GetString(signature);
            if (str != "WDBC")
                throw new Exception("Invalid signature in DBC file!");

            var fields = mCreationType.GetFields();
            int fieldCount = DBCHelper.FieldCount(fields, mCreationType);

            uint numRecords = mReader.ReadUInt32();
            uint numFields = mReader.ReadUInt32();
            uint recordSize = mReader.ReadUInt32();
            uint stringSize = mReader.ReadUInt32();

            mReader.BaseStream.Position = numRecords * recordSize + 20;
            byte[] stringData = mReader.ReadBytes((int)stringSize);
            string fullStr = Encoding.UTF8.GetString(stringData);
            string[] strings = fullStr.Split(new string[] { "\0" }, StringSplitOptions.None);
            Dictionary<int, string> stringTable = new Dictionary<int, string>();
            int curPos = 0;
            foreach (var strEnt in strings)
            {
                stringTable.Add(curPos, strEnt);
                curPos += Encoding.UTF8.GetByteCount(strEnt) + 1;
            }
            mReader.BaseStream.Position = 20;

            if (numFields != fieldCount)
                throw new Exception("numFields != fieldCount in " + FileName + "!");

            for (uint i = 0; i < numRecords; ++i)
            {
                var t = Activator.CreateInstance(mCreationType);
                long posStart = mReader.BaseStream.Position;
                foreach (var field in fields)
                {
                    switch (Type.GetTypeCode(field.FieldType))
                    {
                        case TypeCode.Int32:
                            {
                                int value = mReader.ReadInt32();
                                field.SetValue(t, value);
                                break;
                            }

                        case TypeCode.UInt32:
                            {
                                uint uvalue = mReader.ReadUInt32();
                                field.SetValue(t, uvalue);
                                break;
                            }

                        case TypeCode.String:
                            {
                                int offset = mReader.ReadInt32();
                                string strFromTable;
                                if (!stringTable.TryGetValue(offset, out strFromTable))
                                    throw new InvalidOperationException("Invalid index into stringtable found!");

                                string strVal = strFromTable;
                                field.SetValue(t, strVal);
                                break;
                            }
                        case TypeCode.Byte:
                            {
                                byte bvalue = mReader.ReadByte();
                                field.SetValue(t, bvalue);
                                break;
                            }

                        case TypeCode.Object:
                            {
                                if (field.FieldType == typeof(LocalizedString))
                                {
                                    string strValue = "";
                                    for (uint j = 0; j < 16; ++j)
                                    {
                                        int offset = mReader.ReadInt32();
                                        string strFromTable;
                                        if (strValue == "" && offset != 0 && stringTable.TryGetValue(offset, out strFromTable))
                                        {
                                            strValue = strFromTable;
                                            LocalePosition = j;
                                        }
                                    }

                                    // Flag de localisation, utile du côté client ?
                                    LocaleFlag = mReader.ReadUInt32();

                                    field.SetValue(t, (LocalizedString)strValue);
                                }
                                else if (field.FieldType.IsArray)
                                {
                                    int len;
                                    Array array;

                                    switch (Type.GetTypeCode(field.FieldType.GetElementType()))
                                    {
                                        case TypeCode.Int32:
                                            len = ((int[])field.GetValue(t)).Length;
                                            array = new int[len];
                                            for (var q = 0; q < len; ++q)
                                                array.SetValue(mReader.ReadInt32(), q);
                                            field.SetValue(t, array);
                                            break;
                                        case TypeCode.UInt32:
                                            len = ((uint[])field.GetValue(t)).Length;
                                            array = new uint[len];
                                            for (var q = 0; q < len; ++q)
                                                array.SetValue(mReader.ReadUInt32(), q);
                                            field.SetValue(t, array);
                                            break;
                                        case TypeCode.Single:
                                            len = ((float[])field.GetValue(t)).Length;
                                            array = new float[len];
                                            for (var q = 0; q < len; ++q)
                                                array.SetValue(mReader.ReadSingle(), q);
                                            field.SetValue(t, array);
                                            break;
                                        default:
                                            throw new NotImplementedException();
                                    }
                                }
                                break;
                            }
                        case TypeCode.Single:
                            {
                                float fvalue = mReader.ReadSingle();
                                field.SetValue(t, fvalue);
                                break;
                            }
                    }
                }


                long posEnd = mReader.BaseStream.Position;
                var firstVal = fields[0].GetValue(t);

                var classAttribs = typeof(T).GetCustomAttributes(typeof(NoPrimaryAttribute), false);
                uint id;
                if (classAttribs.Length == 0)
                    id = (uint)Convert.ChangeType(firstVal, typeof(uint));
                else
                    id = (uint)mRecords.Count();

                if (mConverter == null)
                    mRecords.Add(id, (T)t);
                else
                    mRecords.Add(id, mConverter.Convert(t));
            }


            mReader.Close(); // On peut sauvegarder par dessus notre mReader :D !

            IsLoaded = true;

            return true;
        }

        private Dictionary<uint, T> mRecords = new Dictionary<uint, T>();
        private BinaryReader mReader;
        private Type mCreationType;
        private IDBCRowConverter<T> mConverter = null;

        public void SetLoadType(Type load, IDBCRowConverter<T> converter)
        {
            mCreationType = load;
            mConverter = converter;
        }

        public void AddEntry(uint id, T value)
        {
            if (mRecords.ContainsKey(id))
                throw new Exception("Cette clé existe déjà !");

            IsEdited = true;
            mRecords.Add(id, value);
        }

        public void ReplaceEntry(uint id, T value)
        {
            if (!mRecords.ContainsKey(id))
                throw new Exception("Cette clé n'existe pas !");

            IsEdited = true;
            mRecords[id] = value;
        }

        public void RemoveEntry(uint id)
        {
            if (!mRecords.ContainsKey(id))
                throw new Exception("Cette clé n'existe pas !");

            IsEdited = true;
            mRecords.Remove(id);
        }

        public bool TryRemoveEntry(uint id)
        {
            bool isRemoved = mRecords.Remove(id);

            IsEdited &= isRemoved;

            return isRemoved;
        }

        public void SaveDBC(IComparer<T> comparison)
        {
            //if (!IsEdited)
            //    return;

            string path = FileName;

            DBCWriter<T> wr = new DBCWriter<T>();
            wr.WriteDBC(this, path, comparison);
        }

        public void SaveDBC()
        {
            SaveDBC(null);
        }

        public T this[uint id]
        {
            get { return mRecords[id]; }
            set { mRecords[id] = value; IsEdited = true; }
        }

        public bool ContainsKey(uint id)
        {
            return mRecords.ContainsKey(id);
        }

        public uint MaxKey { get { return mRecords.Keys.Max(); } }

        public Dictionary<uint, T>.ValueCollection Records { get { return mRecords.Values; } }
        public string FileName { get; private set; }
        public uint LocalePosition { get; private set; }
        public uint LocaleFlag { get; private set; }
        private bool IsLoaded = false;
        private bool IsEdited = false;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class NoPrimaryAttribute : Attribute
    {
    }
}
