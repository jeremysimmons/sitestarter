using System;
using System.Xml.Serialization;
using System.Collections;

namespace SoftwareMonkeys.SiteStarter.Configuration
{
	/// <summary>
	/// Holds the current configuration settings used for backend operations.
	/// </summary>
    //[XmlType(Namespace = "SoftwareMonkeys.SiteStarter.Configuration")]
    //[XmlRoot(Namespace = "SoftwareMonkeys.SiteStarter.Configuration")]
	public class AppConfig : IAppConfig, IConfig
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

        private string physicalPath;
        /// <summary>
        /// Gets/sets the physical path of the application.
        /// </summary>
        public string PhysicalPath
        {
            get { return physicalPath; }
            set { physicalPath = value; }
        }

        private Guid universalProjectID;
        /// <summary>
        /// Gets/sets the universal ID of the project.
        /// </summary>
        public Guid UniversalProjectID
        {
            get { return universalProjectID; }
            set { universalProjectID = value; }
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
        
        private string smtpServer;
        /// <summary>
        /// Gets/sets the SMTP server to use for sending emails.
        /// </summary>
        public string SmtpServer
        {
        	get { return smtpServer; }
        	set { smtpServer = value; }
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
        
        private bool enableVirtualServer;
        /// <summary>
        /// Gets/sets a flag indicating whether application sharing is enabled.
        /// </summary>
        public bool EnableVirtualServer
        {
        	get { return enableVirtualServer; }
        	set { enableVirtualServer = value; }
        }
        
        private bool enableVirtualServerRegistration;
        /// <summary>
        /// Gets/sets a flag indicating whether application sharing users can register their own account.
        /// </summary>
        public bool EnableVirtualServerRegistration
        {
        	get { return enableVirtualServerRegistration; }
        	set { enableVirtualServerRegistration = value; }
        }
        
        private bool autoApproveVirtualServerRegistration;
        /// <summary>
        /// Gets/sets a flag indicating whether application sharing registrations are automatically approved or not.
        /// </summary>
        public bool AutoApproveVirtualServerRegistration
        {
        	get { return autoApproveVirtualServerRegistration; }
        	set { autoApproveVirtualServerRegistration = value; }
        }
              
        private string[] defaultVirtualServerKeywords;
        /// <summary>
        /// Gets/sets a list of the default keywords given to new virtual servers.
        /// </summary>
        public string[] DefaultVirtualServerKeywords
        {
        	get { return defaultVirtualServerKeywords; }
        	set { defaultVirtualServerKeywords = value; }
        }
    }
}
