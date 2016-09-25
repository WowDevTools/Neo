using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.IO.Files
{
    interface IDataStorageRecord
    {
        T Get<T>() where T : struct;
        T Get<T>(int offset) where T : struct;
        int GetInt32(int field);
        uint GetUint32(int field);
        float GetFloat(int field);
        string GetString(int field);
    }

    interface IDataStorageFile : IDisposable
    {
        int NumRows { get; }
        int NumFields { get; }

        void Load(string file);
        void Load(Stream stream);
        void Save(string file);
        void ReLoad(Stream stream);

        void BuildCache<T>() where T : struct;
        void ClearCache();

        IDataStorageRecord GetRow(int index);
        IDataStorageRecord GetRowById(int id);
        IList<IDataStorageRecord> GetAllRows();
        IList<T> GetAllRows<T>() where T : struct;

        int AddString(string value);
        void AddRow<T>(T entry);

        void UpdateRow<T>(T entry);

        bool DeleteRow(int index);
        bool DeleteRowById(int id);
    }

    public struct LocalisedString
    {
        public string enUS;
        public string enGB;
        public string koKR;
        public string frFR;
        public string deDE;
        public string enCN;
        public string zhCN;
        public string enTW;
        public string zhTW;
        public string esES;
        public string esMX;
        public string ruRU;
        public string ptPT;
        public string ptBR;
        public string itIT;
        public string unKnown;
        public int mask;

        private FieldInfo _localefield;
        public string Locale
        {
            get { return _localefield.GetValue(this).ToString(); }
            set { _localefield.SetValue(this, value); }
        }

        public LocalisedString(string[] strings, int mask)
        {
            this.mask = mask;
            enUS = strings[0];
            enGB = strings[1];
            koKR = strings[2];
            frFR = strings[3];
            deDE = strings[4];
            enCN = strings[5];
            zhCN = strings[6];
            enTW = strings[7];
            zhTW = strings[8];
            esES = strings[9];
            esMX = strings[10];
            ruRU = strings[11];
            ptPT = strings[12];
            ptBR = strings[13];
            itIT = strings[14];
            unKnown = strings[15];

            //First non-empty string is locale
            int _iLoc = Enumerable.Range(0, strings.Length).FirstOrDefault(x => !string.IsNullOrEmpty(strings[x]));
            _localefield = typeof(LocalisedString).GetFields()[_iLoc];
        }

        public override string ToString()
        {
            return Locale;
        }
    }
}
