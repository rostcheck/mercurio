using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain
{
    /// <summary>
    /// Special-purpose substrate used for storing temporary (cleartxst) files. Does not support containers,
    /// and overwrites all content to erase it after use. Used for transferring cleartext data to application programs.
    /// </summary>
    public interface ITempStorageSubstrate
    {
        void StoreData(string fileName, string clearTextData);
        string RetrieveData(string fileName);
        void EraseData(string fileName);
    }
}
