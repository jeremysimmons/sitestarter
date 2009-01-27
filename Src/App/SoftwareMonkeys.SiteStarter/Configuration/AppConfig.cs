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
	public class AppConfig : IAppConfig, IConfig
	{
        private string name;
        /// <summary>
        /// Gets/sets the name of the configuration object/file.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        #region IAppConfig Members
        private int sessionTimeout;
        /// <summary>
        /// Gets/sets the session timeout for authentication.
        /// </summary>
        public int SessionTimeout
        {
            get { return sessionTimeout; }
            set { sessionTimeout = value; }
        }

        private string applicationPath;
        /// <summary>
        /// Gets the virtual application path.
        /// </summary>
        public string ApplicationPath
        {
            get { return applicationPath; }
            set { applicationPath = value; }
        }

        private string physicalPath;
        /// <summary>
        /// Gets/sets the physical path of the application.
        /// </summary>
        public string PhysicalPath
        {
            get { return physicalPath; }
            set { physicalPath = value; }
        }
        #endregion
    }
}
