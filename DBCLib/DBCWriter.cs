using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DBCLib
{
    internal class DBCWriter<T> where T : new()
    {
        internal void WriteDBC(DBCFile<T> file, string path, IComparer<T> comparison)
        {
            var dirName = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dirName))
                Directory.CreateDirectory(dirName);

            try
            {
                FileStream strm = File.OpenWrite(path);
                BinaryWriter bw = new BinaryWriter(strm);
                byte[] bytes = Encoding.UTF8.GetBytes("WDBC");
                bw.Write(bytes);
                bw.Write(file.Records.Count);
                Type type = typeof(T);
                var fields = type.GetFields();
                int fieldCount = DBCHelper.FieldCount(fields, type);

                bw.Write(fieldCount);
                bw.Write(fieldCount * 4);
                bw.Write(0);

                // Ajout d'une string vide afin d'obtenir un format blizzlike
                AddStringToTable("");

                List<T> valueCollection = file.Records.ToList();
                if (comparison != null)
                {
                    valueCollection.Sort(comparison);
                }

                // Ecriture de chaque champ de chaque enregistrement
                foreach (var rec in valueCollection)
                {
                    foreach (var field in fields)
                    {
                        switch (Type.GetTypeCode(field.FieldType))
                        {
                            case TypeCode.Int32:
                                {
                                    int value = (int)field.GetValue(rec);
                                    bw.Write(value);
                                    break;
                                }

                            case TypeCode.UInt32:
                                {
                                    uint uvalue = (uint)field.GetValue(rec);
                                    bw.Write(uvalue);
                                    break;
                                }

                            case TypeCode.String:
                                {
                                    string str = field.GetValue(rec) as string;
                                    bw.Write(AddStringToTable(str));
                                    break;
                                }

                            case TypeCode.Single:
                                {
                                    float fvalue = (float)field.GetValue(rec);
                                    bw.Write(fvalue);
                                    break;
                                }

                            case TypeCode.Object:
                                {
                                    if (field.FieldType == typeof(LocalizedString))
                                    {
                                        int pos = AddStringToTable((LocalizedString)field.GetValue(rec));
                                        for (uint j = 0; j < file.LocalePosition; ++j)
                                        {
                                            bw.Write((int)0);
                                        }

                                        bw.Write(pos);
                                        for (uint j = file.LocalePosition + 1; j < 16; ++j)
                                            bw.Write((int)0);

                                        // 17ème champ de localisation
                                        bw.Write((uint)file.LocaleFlag);
                                    }
                                    else
                                    {
                                        int len;
                                        Array array;

                                        switch (Type.GetTypeCode(field.FieldType.GetElementType()))
                                        {
                                            case TypeCode.Int32:
                                                array = field.GetValue(rec) as Array;
                                                len = array.Length;
                                                for (var q = 0; q < len; ++q)
                                                    bw.Write((int)array.GetValue(q));
                                                break;
                                            case TypeCode.UInt32:
                                                array = field.GetValue(rec) as Array;
                                                len = array.Length;
                                                for (var q = 0; q < len; ++q)
                                                    bw.Write((uint)array.GetValue(q));
                                                break;
                                            case TypeCode.Single:
                                                array = field.GetValue(rec) as Array;
                                                len = array.Length;
                                                for (var q = 0; q < len; ++q)
                                                    bw.Write((float)array.GetValue(q));
                                                break;
                                            default:
                                                throw new NotImplementedException();
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }

                foreach (var str in _stringTable.Values)
                {
                    bytes = Encoding.UTF8.GetBytes(str);
                    bw.Write(bytes);
                    bw.Write((byte)0);
                }

                bw.BaseStream.Position = 16;
                if (_stringTable.Count > 0)
                    bw.Write(_stringTable.Last().Key + Encoding.UTF8.GetByteCount(_stringTable.Last().Value) + 1);

                strm.Close();
            }
            catch (IOException)
            {

            }
        }

        private KeyValuePair<int, string> _lastItem;
        private int AddStringToTable(string str)
        {
            if (str == null)
                str = "";

            int strHash = str.GetHashCode();
            int myPos = 0;

            if (_stringsHash.TryGetValue(strHash, out myPos))
                return myPos;

            if (_stringTable.Count > 0)
                myPos = _lastItem.Key + Encoding.UTF8.GetByteCount(_lastItem.Value) + 1;

            _stringTable.Add(myPos, str);
            _stringsHash.Add(strHash, myPos);
            _lastItem = new KeyValuePair<int, string>(myPos, str);

            return myPos;
        }

        private readonly Dictionary<int, string> _stringTable = new Dictionary<int, string>();
        private readonly Dictionary<int, int> _stringsHash = new Dictionary<int, int>();
    }
}
