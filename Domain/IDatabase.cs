using System;
using System.Collections.Generic;

namespace Mercurio.Domain
{
    // Defines interface to a Mercurio database object (stores Records)
    public interface IDatabase
    {
        void DeleteRecord(Guid recordId);
        void ChangeRecord(Record changedRecord);
        void AddRecord(Record record);
        List<Record> GetRecords();
        List<Record> GetRecordsWhere(Func<Record, bool> predicate);
        event EventHandler NeedsSerialization;
    }
}
