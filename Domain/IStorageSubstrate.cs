using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mercurio.Domain
{
    /// <summary>
    /// Represents a space that can for physically hold data (in Containers)
    /// </summary>
    public interface IStorageSubstrate
    {
        string Name { get; }

        IEnumerable<Container> GetContainers();

        void AddContainer(Container container);
    }
}
