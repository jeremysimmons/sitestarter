using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareMonkeys.SiteStarter.Data
{
    /// <summary>
    /// Defines the interface for all data stores.
    /// </summary>
    public interface IDataServer
    {
        string Name { get; }
        void Open();
        void Close();
        void Dispose();
    }
}
