using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// Specified access permission
    /// </summary>
    public enum AccessPermissionType
    {
        Add, // allows the specified Identity to add Records to a Container
        Delete, // allows the specified Identity to delete Records from a Container
        Change, // allows the specified Identity to change Records (add Revisions to) a Container
    }
}
