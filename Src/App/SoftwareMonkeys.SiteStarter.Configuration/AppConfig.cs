using System;
using System.Xml.Serialization;
using System.Collections;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Configuration
{
	/// <summary>
	/// Holds the current configuration settings used for backend operations.
	/// </summary>
    //[XmlType(Namespace = "SoftwareMonkeys.SiteStarter.Configuration")]
    //[XmlRoot(Namespace = "SoftwareMonkeys.SiteStarter.Configuration")]
	public class AppConfig : BaseConfig, IAppConfig, IConfig
	{
        private string name = "Application";
        /// <summary>
        /// Gets/sets the name of the configuration object/file.
        /// </summary>
        public new string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string title;
        /// <summary>
        /// Gets/sets the title of the application.
        /// </summary>
        public string Title
        {
            get { return title; }
            set { title = value; }
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

        private string physicalApplicationPath;
        /// <summary>
        /// Gets/sets the physical path of the application.
        /// </summary>
        public string PhysicalApplicationPath
        {
            get { return physicalApplicationPath.TrimEnd(Path.DirectorySeparatorChar); }
            set { physicalApplicationPath = value; }
        }

        private Guid primaryAdministratorID;
        /// <summary>
        /// Gets/sets the universal ID of the project.
        /// </summary>
        public Guid PrimaryAdministratorID
        {
            get { return primaryAdministratorID; }
            set { primaryAdministratorID = value; }
        }
        
    	private string pathVariation;
        /// <summary>
        /// Gets/sets the variation applied to the config file path (eg. staging, local, etc.).
        /// </summary>
        public string PathVariation
        {
        	get { return pathVariation; }
        	set { pathVariation = value; }
        }

        /// <summary>
        /// Gets/sets the flexible settings collection.
        /// </summary>
        [XmlIgnore]
        IConfigurationDictionary IAppConfig.Settings
        {
            get {
            	return this.Settings; }
        	set { this.Settings = new ConfigurationDictionary(value); }
        }
        
        private ConfigurationDictionary settings = new ConfigurationDictionary();
        /// <summary>
        /// Gets/sets the flexible settings collection.
        /// </summary>
        public ConfigurationDictionary Settings
        {
            get {
            	if (settings == null)
            		settings = new ConfigurationDictionary();
            	return settings; }
            set { settings = value; }
        }
        #endregion
        
    }
}
