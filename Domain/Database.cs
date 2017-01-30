using System;
using System.Collections.Generic;
using System.Linq;

namespace Mercurio.Domain
{
    /// <summary>
    /// Implements Mercurio's internal database object
    /// </summary>
    public class Database : IDatabase
    {
        private Dictionary<Guid, Record> _records;

        public Database()
        {
        }

        public event EventHandler NeedsSerialization;

        //TODO: Add functionality to fire the event (sending changes w/ it) when data is changed
        public void AddRecord(Record record)
        {
            if (record == null)
                throw new ArgumentNullException("record");
            
            if (_records.ContainsKey(record.Id))
                ChangeRecord(record);
            else
                _records.Add(record.Id, record);
        }

        public void ChangeRecord(Record changedRecord)
        {
            if (changedRecord == null)
                throw new ArgumentNullException("changedRecord");

            if (!_records.ContainsKey(changedRecord.Id))
                throw new ArgumentException("No matching record with id " + changedRecord.Id.ToString() + " found to change");

            _records[changedRecord.Id] = changedRecord;
        }

        public void DeleteRecord(Guid recordId)
        {
            if (!_records.ContainsKey(recordId))
                throw new ArgumentException("No matching record with id " + recordId.ToString() + " found to delete");
            _records.Remove(recordId);
        }

        public List<Record> GetRecords()
        {
            return new List<Record>(_records.Values);
        }

        public List<Record> GetRecordsWhere(Func<Record, bool> predicate)
        {
            return new List<Record>(_records.Values.Where(predicate));
        }

//        public event EventHandler NeedsSerialization; // Fire when changes have occurred

        private List<RecordChange> DetectChanges(Record originalRecord, Record newRecord)
        {
            var changes = new List<RecordChange>();
            foreach (var field in originalRecord.GetDataElements())
            {
                //TODO: determine the AtomicRecordChanges for the record
            }
            return changes;
        }
    }
}
