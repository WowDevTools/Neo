using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WDBXLib.FileTypes;

namespace WDBXLib
{
    public class DBFile<T>
    {
        public DBHeader Header { get; private set; }
        public DataTable Data { get; set; }
        public string FilePath { get; private set; }
        public string FileName => Path.GetFileName(this.FilePath);
        public string SavePath { get; set; }
        public Dictionary<FieldInfo, int> TableStructure { get; private set; }

        public string Key { get; private set; }
        public int Build { get; private set; }
        public int LocalizationCount { get; private set; } = 17;

        public DBFile(DBHeader header, string filepath)
        {
            this.Header = header;
            this.FilePath = filepath;
            this.SavePath = filepath;

            LoadDefinition();
        }


        /// <summary>
        /// Converts the XML definition to an empty DataTable
        /// </summary>
        public void LoadDefinition()
        {
            Type type = typeof(T);
            var fields = type.GetFields();
            BuildAttribute buildAtt = (BuildAttribute)Attribute.GetCustomAttribute(type, typeof(BuildAttribute));
            AutoGenerateAttribute autogenerate = (AutoGenerateAttribute)Attribute.GetCustomAttribute(type, typeof(AutoGenerateAttribute));

            Build = buildAtt.Build;
            Key = fields.First().Name;
            TableStructure = new Dictionary<FieldInfo, int>();

            Data = new DataTable() { TableName = "Data", CaseSensitive = false };

            LocalizationCount = (Build <= 6005 ? 9 : 17); //Pre TBC had 9 locales

            foreach (var col in fields)
            {
                var columnType = Type.GetTypeCode(col.FieldType);
                Queue<TextWowEnum> languages = new Queue<TextWowEnum>(Enum.GetValues(typeof(TextWowEnum)).Cast<TextWowEnum>()); //Load locale names

                if ((PrimaryKeyAttribute)Attribute.GetCustomAttribute(col.GetType(), typeof(PrimaryKeyAttribute)) != null) //Primary key check
                    Key = col.Name;                

                int ArraySize = 1;
                if (col.FieldType.IsArray)
                {
                    columnType = Type.GetTypeCode(col.FieldType.GetElementType());
                    ArraySize = ((int[])col.GetValue(Activator.CreateInstance(type))).Length;
                }

                TableStructure.Add(col, ArraySize); //Store structure

                for (int i = 0; i < ArraySize; i++)
                {
                    string columnName = col.Name;
                    if (ArraySize > 1)
                        columnName += (i > 0 ? "_" + i.ToString() : "");

                    switch (columnType)
                    {
                        case TypeCode.SByte:
                            Data.Columns.Add(columnName, typeof(sbyte));
                            Data.Columns[columnName].DefaultValue = 0;
                            break;
                        case TypeCode.Byte:
                            Data.Columns.Add(columnName, typeof(byte));
                            Data.Columns[columnName].DefaultValue = 0;
                            break;
                        case TypeCode.Int32:
                            Data.Columns.Add(columnName, typeof(int));
                            Data.Columns[columnName].DefaultValue = 0;
                            break;
                        case TypeCode.UInt32:
                            Data.Columns.Add(columnName, typeof(uint));
                            Data.Columns[columnName].DefaultValue = 0;
                            break;
                        case TypeCode.Int64:
                            Data.Columns.Add(columnName, typeof(long));
                            Data.Columns[columnName].DefaultValue = 0;
                            break;
                        case TypeCode.UInt64:
                            Data.Columns.Add(columnName, typeof(ulong));
                            Data.Columns[columnName].DefaultValue = 0;
                            break;
                        case TypeCode.Single:
                            Data.Columns.Add(columnName, typeof(float));
                            Data.Columns[columnName].DefaultValue = 0;
                            break;
                        case TypeCode.Boolean:
                            Data.Columns.Add(columnName, typeof(bool));
                            Data.Columns[columnName].DefaultValue = 0;
                            break;
                        case TypeCode.String:
                            Data.Columns.Add(columnName, typeof(string));
                            Data.Columns[columnName].DefaultValue = string.Empty;
                            break;
                        case TypeCode.Int16:
                            Data.Columns.Add(columnName, typeof(short));
                            Data.Columns[columnName].DefaultValue = 0;
                            break;
                        case TypeCode.UInt16:
                            Data.Columns.Add(columnName, typeof(ushort));
                            Data.Columns[columnName].DefaultValue = 0;
                            break;
                        case TypeCode.Object:
                            if (col.FieldType == typeof(LocalizedString))
                            {
                                //Special case for localized strings, build up all locales and add string mask
                                for (int x = 0; x < LocalizationCount; x++)
                                {
                                    if (x == LocalizationCount - 1)
                                    {
                                        Data.Columns.Add(col.Name + "_Mask", typeof(uint)); //Last column is a mask
                                        Data.Columns[col.Name + "_Mask"].AllowDBNull = false;
                                        Data.Columns[col.Name + "_Mask"].DefaultValue = 0;
                                    }
                                    else
                                    {
                                        columnName = col.Name + "_" + languages.Dequeue().ToString(); //X columns for local strings
                                        Data.Columns.Add(columnName, typeof(string));
                                        Data.Columns[columnName].AllowDBNull = false;
                                        Data.Columns[columnName].DefaultValue = string.Empty;
                                    }
                                }
                            }
                            break;
                        default:
                            throw new Exception($"Unknown field type {columnType} for {col.Name}.");
                    }

                    //AutoGenerated Id for CharBaseInfo
                    if (autogenerate != null)
                    {
                        Data.Columns[0].ExtendedProperties.Add("AUTO_GENERATED", true);
                        Header.AutoGeneratedColumns++;
                    }

                    Data.Columns[columnName].AllowDBNull = false;
                }
            }

            //Setup the Primary Key
            Data.Columns[Key].DefaultValue = null; //Clear default value
            Data.PrimaryKey = new DataColumn[] { Data.Columns[Key] };
            Data.Columns[Key].AutoIncrement = true;
            Data.Columns[Key].Unique = true;
        }

        /// <summary>
        /// Gets the Min and Max ids
        /// </summary>
        /// <returns></returns>
        public Tuple<int, int> MinMax()
        {
            int min = int.MaxValue;
            int max = int.MinValue;
            foreach (DataRow dr in Data.Rows)
            {
                int val = dr.Field<int>(Key);
                min = Math.Min(min, val);
                max = Math.Max(max, val);
            }
            return new Tuple<int, int>(min, max);
        }

        /// <summary>
        /// Generates a Bit map for all columns as the Blizzard one combines array columns
        /// </summary>
        /// <returns></returns>
        public FieldStructureEntry[] GetBits()
        {
            FieldStructureEntry[] bits = new FieldStructureEntry[Data.Columns.Count];
            if (!Header.IsTypeOf<WDB5>())
                return bits;

            int c = 0;
            int i = 0;
            foreach (var ts in TableStructure)
            {
                for (int x = 0; x < ts.Value; x++)
                {
                    bits[c] = new FieldStructureEntry(0, 0);
                    bits[c].Bits = ((WDB5)Header).FieldStructure[i]?.Bits ?? 0;
                    c++;
                }
                i++;
            }

            return bits;
        }

        #region Special Data
        /// <summary>
        /// Gets a list of Ids
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> GetPrimaryKeys()
        {
            for (int i = 0; i < Data.Rows.Count; i++)
                yield return Data.Rows[i].Field<int>(Key);
        }

        /// <summary>
        /// Produces a list of unique rows (excludes key values)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DataRow> GetUniqueRows()
        {
            DBRowComparer comparer = new DBRowComparer(Data.Columns.IndexOf(Key));
            return Data.AsEnumerable().Distinct(comparer);
        }

        /// <summary>
        /// Generates a map of unqiue rows and grouped count
        /// </summary>
        /// <returns></returns>
        public IEnumerable<dynamic> GetCopyRows()
        {
            DBRowComparer comparer = new DBRowComparer(Data.Columns.IndexOf(Key));

            return Data.AsEnumerable().GroupBy(r => r, comparer)
                                      .Where(g => g.Count() > 1)
                                      .Select(g => new
                                      {
                                          Key = g.Key,
                                          Copies = g.Where(r => r != g.Key).Select(r => r.Field<int>(comparer.IdColumnIndex)).ToArray()
                                      });
        }

        /// <summary>
        /// Extracts the id and the total length of strings for each row
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, short> GetStringLengths()
        {
            Dictionary<int, short> result = new Dictionary<int, short>();
            IEnumerable<string> cols = Data.Columns.Cast<DataColumn>()
                                              .Where(x => x.DataType == typeof(string))
                                              .Select(x => x.ColumnName);

            foreach (DataRow row in Data.Rows)
            {
                short total = 0;
                foreach (string c in cols)
                {
                    short len = (short)Encoding.UTF8.GetByteCount(row[c].ToString());
                    total += (short)(len > 0 ? len + 1 : 0);
                }
                result.Add(row.Field<int>(Key), total);
            }

            return result;
        }
        #endregion

        #region Get/Set
        public T GetRowById(int id)
        {
            var row = Data.Rows.Find(id);
            if (row == null)
                throw new ArgumentException("Id not found.");

            return CreateTFromRow(row);
        }

        public void SetRowById(int id, T data)
        {
            var row = Data.Rows.Find(id);
            if (row == null)
                throw new ArgumentException("Id not found.");

            row.ItemArray = CreateRowFromT(data);
        }

        public T GetRecordByIndex(int index)
        {
            if (index < 0 || index > Data.Rows.Count)
                throw new ArgumentException("Index not found.");

            return CreateTFromRow(Data.Rows[index]);
        }

        public void SetRecordByIndex(int index, T data)
        {
            if (index < 0 || index > Data.Rows.Count)
                throw new ArgumentException("Index not found.");

            var row = Data.Rows[index];
            row.ItemArray = CreateRowFromT(data);
        }

        public IEnumerable<T> GetAllRecords()
        {
            foreach (DataRow r in Data.Rows)
                yield return CreateTFromRow(r);
        }

        /// <summary>
        /// Creates a new instance of T with row values
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private T CreateTFromRow(DataRow row)
        {
            var t = Activator.CreateInstance(typeof(T));
            int x = 0;
            foreach (var ts in TableStructure)
            {
                if (ts.Key.FieldType.IsArray)
                {
                    object[] array = new object[ts.Value];
                    for(int i = 0; i < ts.Value; i++)
                    {
                        array[i] = row[x];
                        x++;
                    }

                    switch (Type.GetTypeCode(ts.Key.FieldType.GetElementType()))
                    {
                        case TypeCode.Int32:
                            ts.Key.SetValue(t, array.Cast<int>());
                            break;
                        case TypeCode.UInt32:
                            ts.Key.SetValue(t, array.Cast<uint>());
                            break;
                        case TypeCode.Single:
                            ts.Key.SetValue(t, array.Cast<float>());
                            break;
                    }
                    
                }
                else if (ts.Key.FieldType == typeof(LocalizedString))
                {
                    List<string> strings = new List<string>();
                    for (int i = 0; i < LocalizationCount - 1; i++)
                    {
                        strings.Add(row[x].ToString());
                        x++;
                    }

                    uint mask = (uint)Convert.ChangeType(row[x], TypeCode.UInt32);
                    ts.Key.SetValue(t, new LocalizedString(strings, mask));
                    x++;
                }
                else
                {
                    ts.Key.SetValue(t, row[x]);
                    x++;
                }

            }
            return (T)t;
        }

        /// <summary>
        /// Creates a new object array with T values
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private object[] CreateRowFromT(T t)
        {
            object[] data = new object[Data.Columns.Count];
            var fields = t.GetType().GetFields();

            int x = 0;
            foreach (var ts in TableStructure)
            {
                if(ts.Key.FieldType.IsArray)
                {
                    object[] array = (object[])ts.Key.GetValue(t);
                    for(int i = 0; i < array.Length; i++)
                    {
                        data[x] = array[i];
                        x++;
                    }
                }
                else if(ts.Key.FieldType == typeof(LocalizedString))
                {
                    var loc = ((LocalizedString)ts.Key.GetValue(t)).GetRawData();
                    for(int i = 0; i < loc.Length; i++)
                    {
                        data[x] = loc[i];
                        x++;
                    }
                }
                else
                {
                    data[x] = ts.Key.GetValue(t);
                    x++;
                }
            }

            return data;
        }
        #endregion

        public void Dispose()
        {
            this.Data.Reset();
            ((IDisposable)this.Data).Dispose();
        }

        public enum TextWowEnum
        {
            enUS,
            enGB,
            koKR,
            frFR,
            deDE,
            enCN,
            zhCN,
            enTW,
            zhTW,
            esES,
            esMX,
            ruRU,
            ptPT,
            ptBR,
            itIT,
            Unk,
        }
    }
}
