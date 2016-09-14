using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo.IO.Files
{
    interface IDataStorageRecord
    {
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
        IDataStorageRecord GetRow(int index);
        IDataStorageRecord GetRowById(int id);
    }
}
