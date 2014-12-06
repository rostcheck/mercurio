using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    ///  A single computer file (ex. sound file, word processor file) that we treat as a single integral Record
    /// </summary>
    public class Document : Record
    {
        private byte[] _data;

        public byte[] GetData()
        {
            return _data;
        }

        public Document(byte[] data)
        {
            this._data = data;
        }
    }
}
