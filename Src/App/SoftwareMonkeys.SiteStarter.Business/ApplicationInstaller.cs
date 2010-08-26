﻿using System;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Entities;
using System.IO;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.IO;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to perform setup/installation of the application.
	/// </summary>
	public class ApplicationInstaller
	{
		/// <summary>
		/// Gets a value indicating whether the application is installed.
		/// </summary>
		public bool IsInstalled
		{
			get
			{
				bool isInstalled = false;
				using (LogGroup logGroup = AppLogger.StartGroup("Checking whether the application has been installed.", NLog.LogLevel.Debug))
				{
					CheckFileMapper();
					
					CheckApplicationPath();
										
					string path = ConfigFactory<AppConfig>.CreateConfigPath(FileMapper.MapPath(ApplicationPath + "/" + DataDirectory), "Application", PathVariation);
					
					AppLogger.Debug("Is installed if file exists: " + path);
					
					isInstalled = File.Exists(path);
					
					AppLogger.Debug("Is installed: " + isInstalled);
				}
				return isInstalled;
			}
		}
		
		private User administrator;
		/// <summary>
		/// Gets/sets the administrator user.
		/// </summary>
		public User Administrator
		{
			get { return administrator; }
			set { administrator = value; }
		}
		
		private string dataDirectory = "App_Data";
		/// <summary>
		/// Gets/sets the name of the data directory.
		/// </summary>
		public string DataDirectory
		{
			get { return dataDirectory; }
			set { dataDirectory = value; }
		}
		
		private string pathVariation;
		/// <summary>
		/// Gets/set the path variation.
		/// </summary>
		public string PathVariation
		{
			get { return pathVariation; }
			set { pathVariation = value; }
		}
		
		private string applicationPath;
		/// <summary>
		/// Gets/set the path to the application.
		/// </summary>
		public string ApplicationPath
		{
			get { return applicationPath; }
			set { applicationPath = value; }
		}
		
		private string administratorRoleName;
		/// <summary>
		/// Gets/sets the name of the administrator role.
		/// </summary>
		public string AdministratorRoleName
		{
			get { return administratorRoleName; }
			set { administratorRoleName = value; }
		}
		
		private IFileMapper fileMapper;
		/// <summary>
		/// Gets/sets the component used to map file paths.
		/// </summary>
		public IFileMapper FileMapper
		{
			get { return fileMapper;  }
			set { fileMapper = value; }
		}
		
		private IDataProviderInitializer dataProviderInitializer;
		/// <summary>
		/// Gets/sets the data provider initializer component.
		/// </summary>
		public IDataProviderInitializer DataProviderInitializer
		{
			get { return dataProviderInitializer; }
			set { dataProviderInitializer = value; }
		}
		
		/// <summary>
		/// Empty constructor.
		/// </summary>
		public ApplicationInstaller()
		{
		}
		
		public ApplicationInstaller(string applicationPath, string pathVariation)
		{
			ApplicationPath = applicationPath;
			PathVariation = pathVariation;
		}
		
		public ApplicationInstaller(string applicationPath, string pathVariation, IFileMapper fileMapper)
		{
			ApplicationPath = applicationPath;
			PathVariation = pathVariation;
			FileMapper = fileMapper;
		}
		
		
		private bool useLegacyData;
		/// <summary>
		/// Gets a value indicating whether to use legacy data.
		/// </summary>
		public bool UseLegacyData
		{
			get
			{
				return useLegacyData;
			}
			set { useLegacyData = value; }
		}
		
		/// <summary>
		/// Performs the setup and configures the application.
		/// </summary>
		public void Setup()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Performing install/setup.", NLog.LogLevel.Debug))
			{
				CheckApplicationPath();
				
				CheckPathVariation();
				
				AppConfig config = CreateConfig();
				
				config.Save(); // Needs to be saved here but will be updated again during setup
				
				AppLogger.Debug("Created application configuration object.");

				SetupMappings();
				
				AppLogger.Debug("Finished setting up core mappings.");

				Initialize();
				
				AppLogger.Debug("Initialized config and data.");
				
				InitializeSiteMap();
				
				AppLogger.Debug("Initialized site map.");
				
				InitializeDefaultData();
				
				AppLogger.Debug("Initialized default data/entities.");
				
				InitializeVersion();
				
				AppLogger.Debug("Initialized application version.");
			}
		}
		
		/// <summary>
		/// Initializes the default data/entities.
		/// </summary>
		public void InitializeDefaultData()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Initializing the default data/entities.", NLog.LogLevel.Debug))
			{
				if (!UseLegacyData)
				{
					AppLogger.Debug("Not using existing data. Creating default user and administrator.");
					
					CheckAdministrator();
					
					User user = Administrator;
					
					AppLogger.Debug("Created/retrieved primary administrator.");
					
					UserRole role = CreateAdministratorRole(user);
					
					AppLogger.Debug("Created administrator role.");
					
					Save(user, role);
					
					AppLogger.Debug("Saved administrator user and administrator role.");
				}
				else
				{
					AppLogger.Debug("Using existing data. Skipping creation. Data will be imported instead.");
				}
			}
		}

		/// <summary>
		/// Creates the administrator role and assigns it to the provided user.
		/// </summary>
		/// <param name="administrator">The default administrator to add to the administrator role.</param>
		/// <returns>The administrator role.</returns>
		private UserRole CreateAdministratorRole(User administrator)
		{
			UserRole administratorRole = null;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Creating the administrator role.", NLog.LogLevel.Debug))
			{
				administratorRole = new UserRole();
				administratorRole.ID = Guid.NewGuid();
				
				if (AdministratorRoleName == String.Empty)
					throw new InvalidOperationException("The AdministratorRoleName property has not been set.");
				
				administratorRole.Name = AdministratorRoleName;
				
			}
			return administratorRole;
		}

		/// <summary>
		/// Saves the provided user, administrator role, and application configuration.
		/// </summary>
		/// <param name="administrator">The default administrator user.</param>
		/// <param name="administratorRole">The administrator role.</param>
		private void Save(User administrator, UserRole administratorRole)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Saving the provided user, role, and config.", NLog.LogLevel.Debug))
			{
				if (SoftwareMonkeys.SiteStarter.Business.UserFactory<User>.Current.SaveUser(administrator))
				{
					administratorRole.Users = new User[]{administrator};
					
					UserRoleFactory.Current.SaveUserRole(administratorRole);
					
					Config.Application.PrimaryAdministratorID = administrator.ID;
					
					Config.Application.Save();
				}
				else
					AppLogger.Debug("User already exists. Skipping save.");
			}
		}

		/// <summary>
		/// Creates the default application configuration and assigns the provided user as the primary administrator.
		/// </summary>
		/// <returns>The application configuration component.</returns>
		private AppConfig CreateConfig()
		{
			AppConfig config = null;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Creating the default application config.", NLog.LogLevel.Debug))
			{
				CheckFileMapper();
				
				CheckPathVariation();
				
				config = ConfigFactory<AppConfig>.NewConfig(FileMapper.MapPath(ApplicationPath + "/" + DataDirectory), "Application", PathVariation);
				config.ApplicationPath = ApplicationPath;
				config.PhysicalApplicationPath = FileMapper.MapPath(ApplicationPath);
				
				AppLogger.Debug("Application path: " + config.ApplicationPath);
				AppLogger.Debug("Physical application path: " + config.PhysicalApplicationPath);
				
				// TODO: Check if needed
				//config.Settings["ApplicationVersion"] = Utilities.GetVersion();
				
				AppLogger.Debug("Version: " + config.Settings["ApplicationVersion"]);
			}
			return config;
			
		}

		/// <summary>
		/// Initializes the configuration and data access.
		/// </summary>
		public void Initialize()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Initializing the configuration and data access.", NLog.LogLevel.Debug))
			{
				// Initialize everything now that the default config has been created
				Config.Initialize(FileMapper.MapPath(ApplicationPath), PathVariation);
				
				CheckDataProviderInitializer();
				
				DataProviderInitializer.Initialize();
			}
		}

		/// <summary>
		/// Sets up the mappings for the default entities.
		/// </summary>
		private void SetupMappings()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Setting up default entity mappings.", NLog.LogLevel.Debug))
			{
				CheckPathVariation();
				
				CheckFileMapper();
				
				if (Config.Mappings == null)
					Config.Mappings = ConfigFactory<MappingConfig>.NewConfig(FileMapper.MapPath(ApplicationPath + "/" + DataDirectory), "Mappings", PathVariation);

				// TODO: Dynamically detect and register entities.
				Entities.User.RegisterType();
				UserRole.RegisterType();
				Keyword.RegisterType();

				Config.Mappings.Save();
			}
		}

		/// <summary>
		/// Initializes the site map.
		/// </summary>
		private void InitializeSiteMap()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Initializing the site map.", NLog.LogLevel.Debug))
			{
				string pathVariation = Config.Application.PathVariation;
				
				AppLogger.Debug("Path variation: " + pathVariation);

				string siteMapFile = "Menu.sitemap";
				if (pathVariation != String.Empty)
					siteMapFile = "Menu." + pathVariation + ".sitemap";
				
				AppLogger.Debug("Site map file: " + siteMapFile);

				string defaultSiteMapPath = DataAccess.Data.DataDirectoryPath + Path.DirectorySeparatorChar + "Menu.default.sitemap";
				string siteMapPath = DataAccess.Data.DataDirectoryPath + Path.DirectorySeparatorChar + siteMapFile;
				
				AppLogger.Debug("Default site map path: " + defaultSiteMapPath);
				AppLogger.Debug("Site map path: " + siteMapPath);
				
				if (!File.Exists(defaultSiteMapPath))
				{
					AppLogger.Error("Default site map file not found.");
					throw new Exception("The default site map couldn't be found at: " + defaultSiteMapPath);
				}

				File.Copy(defaultSiteMapPath, siteMapPath, true);
			}
		}
		
		/// <summary>
		/// Initializes the version by copying the Version.number file from the application root into the data directory.
		/// </summary>
		public void InitializeVersion()
		{
			CheckFileMapper();
			
			string originalPath = FileMapper.MapPath(ApplicationPath + "/"
			                                         + "Version.Number"); // Don't use VersionUtilities.GetVersionFileName() here because it applies the location variation which is not wanted here
			
			string newPath = FileMapper.MapPath(ApplicationPath + "/" + DataDirectory + "/" + VersionUtilities.GetVersionFileName(PathVariation));
			
			if (!Directory.Exists(Path.GetDirectoryName(newPath)))
				Directory.CreateDirectory(Path.GetDirectoryName(newPath));
			
			if (!File.Exists(originalPath))
				throw new Exception("Cannot find version file: " + originalPath);
			
			File.Copy(originalPath, newPath, true);
		}
		
		/// <summary>
		/// Ensures that the file mapper has been set.
		/// </summary>
		public void CheckFileMapper()
		{
			if (FileMapper == null)
				throw new InvalidOperationException("FileMapper property has not been set.");
		}
		
		/// <summary>
		/// Ensures that the path variation has been set.
		/// </summary>
		public void CheckPathVariation()
		{
			if (PathVariation == null)
				throw new InvalidOperationException("PathVariation property has not been set.");
		}
		
		/// <summary>
		/// Ensures that the application path has been set.
		/// </summary>
		public void CheckApplicationPath()
		{
			if (ApplicationPath == null)
				throw new InvalidOperationException("ApplicationPath property has not been set.");
			
			if (ApplicationPath.IndexOf(":") > -1)
				throw new InvalidOperationException("ApplicationPath should be relative to the root of the server but it is currently: " + ApplicationPath);
		}
		
		/// <summary>
		/// Ensures that the data provider initializer has been set.
		/// </summary>
		public void CheckDataProviderInitializer()
		{
			if (DataProviderInitializer == null)
				throw new InvalidOperationException("DataProviderInitializer property has not been set.");
		}
		
		/// <summary>
		/// Ensures that the administrator user has been set.
		/// </summary>
		public void CheckAdministrator()
		{
			if (Administrator == null)
				throw new InvalidOperationException("Administrator property has not been set.");
		}
	}
}
