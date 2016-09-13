using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        IDataStorageRecord GetRow(int index);
        IDataStorageRecord GetRowById(int id);
        IEnumerable<IDataStorageRecord> GetAllRows();
        IEnumerable<T> GetAllRows<T>() where T : struct;

        int AddString(string value);
        void AddRow<T>(T entry);

        bool DeleteRow(int index);
        bool DeleteRowById(int id);
    }
}
