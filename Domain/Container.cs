using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// A collection of Records
    /// </summary>
    public class Container
    {
        private string _name;

        private Container(string name)
        {
            Name = name;
        }

        public static Container Create(string name)
        {
            return new Container(name);
        }

        public string Name { get; private set; }

        public Record CreateDocument(string documentName)
        {
            throw new NotImplementedException();
           // return Record.Create(recordName, )
        }

        public void DeleteRecord(string recordId)
        {
            throw new NotImplementedException();
        }

        public void ChangeRecord(Record changedRecord)
        {
            throw new NotImplementedException();
        }

        public void AddIdentity(Identity identity, AccessPermissionType accessPermissionType)
        {
            throw new NotImplementedException();
        }

    }
}
