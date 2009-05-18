using System;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Configuration
{
	/// <summary>
	/// Holds the current configuration settings used for backend operations.
	/// </summary>
    //[XmlType(Namespace = "SoftwareMonkeys.SiteStarter.Configuration")]
    //[XmlRoot(Namespace = "SoftwareMonkeys.SiteStarter.Configuration")]
	public class VirtualServerConfig : IVirtualServerConfig, IConfig
	{
        private string name = "Default";
        /// <summary>
        /// Gets/sets the name of the configuration object/file.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

    }
}
