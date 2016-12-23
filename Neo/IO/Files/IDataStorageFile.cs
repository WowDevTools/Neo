using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Neo.IO.Files
{
	public interface IDataStorageRecord
    {
        T Get<T>() where T : struct;
        T Get<T>(int offset) where T : struct;
        int GetInt32(int field);
        uint GetUint32(int field);
        float GetFloat(int field);
        string GetString(int field);
    }

	public interface IDataStorageFile : IDisposable
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
            get { return this._localefield.GetValue(this).ToString(); }
            set { this._localefield.SetValue(this, value); }
        }

        public LocalisedString(string[] strings, int mask)
        {
            this.mask = mask;
	        this.enUS = strings[0];
	        this.enGB = strings[1];
	        this.koKR = strings[2];
	        this.frFR = strings[3];
	        this.deDE = strings[4];
	        this.enCN = strings[5];
	        this.zhCN = strings[6];
	        this.enTW = strings[7];
	        this.zhTW = strings[8];
	        this.esES = strings[9];
	        this.esMX = strings[10];
	        this.ruRU = strings[11];
	        this.ptPT = strings[12];
	        this.ptBR = strings[13];
	        this.itIT = strings[14];
	        this.unKnown = strings[15];

            //First non-empty string is locale
            int _iLoc = Enumerable.Range(0, strings.Length).FirstOrDefault(x => !string.IsNullOrEmpty(strings[x]));
	        this._localefield = typeof(LocalisedString).GetFields()[_iLoc];
        }

        public override string ToString()
        {
            return this.Locale;
        }
    }
}
