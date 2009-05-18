using System;
using System.Xml.Serialization;
using Db4objects.Db4o.Query;
using Db4objects.Db4o;
using System.Web;
using System.IO;
//using System.Web.Mail;
using System.Xml;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Tests
{
	/// <summary>
	/// Holds the configuration settings for the application.
	/// </summary
    [XmlType(Namespace = "urn:SoftwareMonkeys.SiteStarter.Config")]
    [XmlRoot(Namespace = "urn:SoftwareMonkeys.SiteStarter.Config")]
    public class TestConfig : IAppConfig
	{
		#region Singleton Property
		static private TestConfig current;
		/// <summary>
		/// Gets/sets an instance of the Config object containing all the current settings.
		/// </summary>
		static public TestConfig Current
		{
			get 
			{
				if (current == null)
				{
					current = TestConfig.Load();
				}
				return current;
			}
			set
			{
				current = value;
			}
		}
		#endregion

        #region Project settings
        private string projectsDirectory;
        public string ProjectsDirectory
        {
            get
            {
                return FixPath(projectsDirectory);
            }
            set { projectsDirectory = value; }
        }

        public string ProjectsDirectoryPath
        {
            get { return FixPath(DataDirectoryPath) + "/" + ProjectsDirectory; }
        }
        #endregion

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

		private string physicalPath = String.Empty;
		/// <summary>
		/// Gets/sets the physical path of the application.
		/// </summary>
		public string PhysicalPath
		{
			get { return FixPath(physicalPath); }
			set { physicalPath = value; }
        }
        #endregion

        #region Data settings
        // TODO: Remove code
       /* public string DBKey
        {
            get
            {
                string key = "DBContainer";
                return key;
            }
        }*/

        private IObjectContainer db;
        /// <summary>
        /// Gets the DB container.
        /// </summary>
        [XmlIgnore]
        public IObjectContainer DB
        {
            get
            {
                return db;
            }
            set
            {
                db = value;
            }
        }

        private string dataDirectory = "Data";
        /// <summary>
        /// Gets/sets the name of the data directory.
        /// </summary>
        public string DataDirectory
        {
            get { return FixPath(dataDirectory); }
            set { dataDirectory = value; }
        }

        /// <summary>
        /// Gets the path of the data directory.
        /// </summary>
        public string DataDirectoryPath
        {
            get
            {
                return ApplicationPath + "/" + DataDirectory;
            }
        }

        /// <summary>
        /// Gets the path to the DB file.
        /// </summary>
        public string DatabasePath
        {
            get { return MapPath(DataDirectoryPath + "/DB.yap"); }
        }

        /// <summary>
        /// Gets the path to the DB log file.
        /// </summary>
        public string DatabaseLogPath
        {
            get { return MapPath(DataDirectoryPath + "/DB.log"); }
        }

        private string backupDirectory;
        /// <summary>
        /// Gets/sets the name of the backup directory.
        /// </summary>
        public string BackupDirectory
        {
            get { return FixPath(backupDirectory); }
            set { backupDirectory = value; }
        }

        /// <summary>
        /// Gets the path of the backup directory.
        /// </summary>
        public string BackupDirectoryPath
        {
            get { return Path.Combine(DataDirectoryPath, BackupDirectory); }
        }

        /*[System.NonSerialized]
        private Guid databaseServerID = new Guid("0C4A59DD-4EB3-49e8-836F-256300DFC208");
*/
        /*[System.NonSerialized]
        private ObjectServer databaseServer;
        [XmlIgnore]
        public ObjectServer DatabaseServer
        {
            get
            {
                if (databaseServer == null)
                {
                    // This uses the HttpContext if found (normal), or a private field if not (testing)

                    //HttpContext currentContext = HttpContext.Current;
 
                    /*if (currentContext != null)
                    {
                        if(currentContext.Application.Contents[databaseServerID.ToString()] != null)
                            databaseServer = (ObjectServer)currentContext.Application[databaseServerID.ToString()];
                        else*/
        //		System.Diagnostics.Trace.WriteLine("Create database server");
        //		databaseServer = Db4o.OpenServer(DatabasePath, 0);
        //}
        //			}
        //			return databaseServer;
        //		}
        //	}*/

        [XmlIgnore]
		public IObjectServer DatabaseServer
		{
			get
			{

				IObjectServer server;
				//if(currentContext.Application.Contents[databaseServerID.ToString()] != null)
				//	server = (ObjectServer)currentContext.Application[databaseServerID.ToString()];
				//else
					server = Db4oFactory.OpenServer(TestConfig.Current.MapPath(DatabasePath), 0);

				return server;
			}
		}
		#endregion

		#region Folder settings
		private string adminDirectory = "/admin";
		/// <summary>
		/// Gets/sets the directory of the administration files.
		/// </summary>
		public string AdminDirectory
		{
			get { return adminDirectory.TrimEnd('/'); }
			set { adminDirectory = value; }
		}

		/// <summary>
		/// Gets the path to the administration directory.
		/// </summary>
		public string AdminDirectoryPath
		{
			get { return ApplicationPath + "/" + AdminDirectory; }
		}

		private string videoDirectory;
		/// <summary>
		/// Gets/sets the directory containing the videos.
		/// </summary>
		public string VideoDirectory
		{
			get { return videoDirectory; }
			set { videoDirectory = value; }
		}

		/// <summary>
		/// Gets/sets the path of the directory containing the videos.
		/// </summary>
		public string VideoDirectoryPath
		{
			get { return DataDirectoryPath + "/" + VideoDirectory; }
		}
		#endregion

		#region Utility functions
		public string MapPath(string path)
		{
			if (ApplicationPath.Length == 0)
				current = Load();

			if (path.IndexOf(":") > -1)
				return path;

			if (PhysicalPath != null && PhysicalPath.Length > 0)
			{
				if (ApplicationPath.Trim('/').Length > 0)
					path = path.Replace(ApplicationPath, "");
				path = path.Replace("/", @"\");
				return PhysicalPath + @"\" + path.TrimStart('\\');
			}

            // Server request object not available so PhysicalPath property is required
            return String.Empty;
		}

        private string FixPath(string path)
        {
            if (path == null)
                return String.Empty;
            return path.TrimEnd('/').TrimEnd('\\');
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

		/// <summary>
		/// Loads the configuration settings from the config file.
		/// </summary>
		/// <returns>The configuration settings from the config file.</returns>
		static public TestConfig Load()
		{
			if (File.Exists(PhysicalConfigPath))
            {
				FileStream stream = File.OpenRead(PhysicalConfigPath);
				XmlSerializer serializer = new XmlSerializer(typeof(TestConfig));
				TestConfig config = (TestConfig)serializer.Deserialize(stream);
				stream.Close();
				return config;
			}
			else
			{

                throw new Exception("Configuration file not found: " + PhysicalConfigPath);
				//Config config = new Config();
				//config.Save();
				//return config;
			}
		}

		/// <summary>
		/// Saves the current configuration settings to the config file.
		/// </summary>
		public void Save()
		{
            if (PhysicalConfigPath == null)
                throw new ArgumentException("A physical configuration path is required.", "PhysicalConfigPath");

            if (!Directory.Exists(Path.GetDirectoryName(PhysicalConfigPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(PhysicalConfigPath));

			FileStream stream = File.Create(PhysicalConfigPath);
			XmlSerializer serializer = new XmlSerializer(typeof(TestConfig));
			serializer.Serialize(stream, this);
			stream.Close();
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
        #endregion
        
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
    }
}
