using System;
using System.Xml;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Configuration.Tests
{
	
	/// <summary>
	/// Holds the configuration settings for the application.
	/// </summary
	[XmlType(Namespace = "urn:SoftwareMonkeys.SiteStarter.Config")]
	[XmlRoot(Namespace = "urn:SoftwareMonkeys.SiteStarter.Config")]
	public class MockAppConfig : BaseConfig, IAppConfig
	{
		#region Installation settings
		private bool isDownForMaintenance;
		/// <summary>
		/// Gets/sets a value indicating whether the site is down for maintenance.
		/// </summary>
		public bool IsDownForMaintenance
		{
			get { return isDownForMaintenance; }
			set { isDownForMaintenance = value; }
		}

		private DateTime installDate = DateTime.Now;
		/// <summary>
		/// Gets/sets the date the application was installed.
		/// </summary>
		public DateTime InstallDate
		{
			get { return installDate; }
			set { installDate = value; }
		}

		private string applicationPath = String.Empty;
		/// <summary>
		/// Gets/sets the path the application was installed to.
		/// </summary>
		public string ApplicationPath
		{
			get { return applicationPath.TrimEnd('/'); }
			set { applicationPath = value; }
		}

		private string applicationUrl;
		/// <summary>
		/// Gets/sets the URL the application was installed to.
		/// </summary>
		public string ApplicationUrl
		{
			get { return applicationUrl; }
			set { applicationUrl = value; }
		}

		private string physicalApplicationPath = String.Empty;
		/// <summary>
		/// Gets/sets the physical path of the application.
		/// </summary>
		public string PhysicalApplicationPath
		{
			get { return physicalApplicationPath; }
			set { physicalApplicationPath = value; }
		}
		#endregion

		#region Load/save functions
		static private string physicalConfigPath;
		/// <summary>
		/// Gets the physical path to the config file.
		/// </summary>
		static public string PhysicalConfigPath
		{
			get
			{
				return physicalConfigPath;
			}
			set { physicalConfigPath = value; }
		}
		#endregion

		#region IConfig Members

		private string[] traceData;
		public string[] TraceData
		{
			get { return traceData; }
			set { traceData = value; }
		}

		private string smtpServer;
		public string SmtpServer
		{
			get { return smtpServer; }
			set { smtpServer = value; }
		}

		private int sessionTimeout;
		public int SessionTimeout
		{
			get { return sessionTimeout; }
			set { sessionTimeout = value; }
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
		#endregion
		// TODO: Remove if not needed
		/*
		private bool enableVirtualServer;
		/// <summary>
		/// Gets/sets a flag indicating whether application sharing is enabled.
		/// </summary>
		bool IAppConfig.EnableVirtualServer
		{
			get { return enableVirtualServer; }
			set { enableVirtualServer = value; }
		}
		
		private bool enableVirtualServerRegistration;
		/// <summary>
		/// Gets/sets a flag indicating whether application sharing users can register their own account.
		/// </summary>
		bool IAppConfig.EnableVirtualServerRegistration
		{
			get { return enableVirtualServerRegistration; }
			set { enableVirtualServerRegistration = value; }
		}
		
		private bool autoApproveVirtualServerRegistration;
		/// <summary>
		/// Gets/sets a flag indicating whether application sharing registrations are automatically approved or not.
		/// </summary>
		bool IAppConfig.AutoApproveVirtualServerRegistration
		{
			get { return autoApproveVirtualServerRegistration; }
			set { autoApproveVirtualServerRegistration = value; }
		}
		
		private string[] defaultVirtualServerKeywords;
		/// <summary>
		/// Gets/sets a list of the default keywords given to new virtual servers.
		/// </summary>
		string[] IAppConfig.DefaultVirtualServerKeywords
		{
			get { return defaultVirtualServerKeywords; }
			set { defaultVirtualServerKeywords = value; }
		}*/

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
		
		private string name = "Application";
		/// <summary>
		/// Gets/sets the name of the config.
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
	}
}
