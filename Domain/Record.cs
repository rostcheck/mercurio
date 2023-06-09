﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    ///  A unique association of atomic data elements.
    /// </summary>
    public class Record
    {
        private List<DocumentVersion> _revisions;
        private List<IAtomicDataElement> _dataElements;
        private List<AtomicDataElementChange> _uncommittedRevisions;

        public Guid Id { get; private set; }

        public static Record Create(string name)
        {
            return new Record(name);
        }

        public static Record Create(string name, List<IAtomicDataElement> dataElements)
        {
            return new Record(name, dataElements);
        }

        public List<DocumentVersion> GetRevisions()
        {
            return new List<DocumentVersion>(_revisions);
        }

        private Record(string name, List<IAtomicDataElement> dataElements = null)
        {
            _revisions = new List<DocumentVersion>();
            _dataElements = dataElements ?? new List<IAtomicDataElement>();
            _uncommittedRevisions = new List<AtomicDataElementChange>();
            Id = Guid.NewGuid();
        }

        public string Name { get; private set; }

        public List<IAtomicDataElement> GetDataElements()
        {
            return new List<IAtomicDataElement>(_dataElements);
        }

        public void ChangeElement(AtomicDataElementChange dataElementChange)
        {
            throw new NotImplementedException();

            // delete any prior pending changes (sets or deletes) for this element
            //uncommittedRevisions = _uncommittedRevisions.Where(s => !s.SameElementAs(dataElementChange)).ToList();

            //if (dataElementChange.ChangeType == ChangeType.Delete && _dataElements.Where(s => s.SameElementAs(dataElementChange)).SingleOrDefault() == null)
            //{
            //    return; // Ignore deletes for non-existent records
            //}
            //_uncommittedRevisions.Add(dataElementChange);
        }

        public void CommitChanges(string revisorIdentityUniqueId)
        {
            //var priorRevision =  _revisions.Last();
            //var priorRevisionId = (priorRevision == null) ? Guid.Empty : priorRevision.Id;

            //_revisions.Add(DocumentVersion.Create(priorRevisionId, revisorIdentityUniqueId, _uncommittedRevisions));
            _uncommittedRevisions = new List<AtomicDataElementChange>();
            // TODO: publish change event (to container)
        }
    }
}
