﻿using System;
using System.IO;
using System.Configuration;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.IO;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Restores application data and configuration files.
	/// </summary>
	public class ApplicationRestorer
	{
		/// <summary>
		/// Gets a value indicating whether the application currently requires restoring.
		/// Note: This is determined by checking whether the legacy directory exists.
		/// </summary>
		public bool RequiresRestore
		{
			get {
				bool does = false;
				using (LogGroup logGroup = LogGroup.Start("Checking whether the application requires restoration.", NLog.LogLevel.Debug))
				{
					CheckLegacyDirectoryPath();
					
					LogWriter.Debug("Does if directory is found: " + LegacyDirectoryPath);
					
					does = Directory.Exists(LegacyDirectoryPath);
					
					LogWriter.Debug("Requires restore: " + does.ToString());
				}
				return does;
			}
		}
		
		private string legacyDirectoryPath;
		/// <summary>
		/// Gets/sets the path to the legacy directory.
		/// </summary>
		public string LegacyDirectoryPath
		{
			get {
				return legacyDirectoryPath;
			}
			set { legacyDirectoryPath = value; }
		}
		
		private string personalizationDirectoryPath;
		/// <summary>
		/// Gets/sets the path to the personalization directory.
		/// </summary>
		public string PersonalizationDirectoryPath
		{
			get {
				return personalizationDirectoryPath;
			}
			set { personalizationDirectoryPath = value; }
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
		
		
		private string dataDirectory = "App_Data";
		/// <summary>
		/// Gets/sets the name of the data directory.
		/// </summary>
		public string DataDirectory
		{
			get { return dataDirectory; }
			set { dataDirectory = value; }
		}
		
		private Version legacyVersion;
		/// <summary>
		/// Gets the version of the legacy data.
		/// </summary>
		public Version LegacyVersion
		{
			get {
				if (legacyVersion == null)
				{
					string versionFilePath = LegacyDirectoryPath + Path.DirectorySeparatorChar + VersionUtilities.GetVersionFileName(Config.Application.PathVariation);
					
					LogWriter.Debug("Version file path: " + versionFilePath);
					
					legacyVersion = VersionUtilities.LoadVersionFromFile(versionFilePath);
				}
				return legacyVersion; }
			set { legacyVersion = value; }
		}
		
		public ApplicationRestorer(IFileMapper mapper)
		{
			FileMapper = mapper;
			Initialize();
		}
		
		private void Initialize()
		{
			if (ConfigurationSettings.AppSettings != null)
			{
				CheckFileMapper();
				
				LegacyDirectoryPath = FileMapper.MapApplicationRelativePath(ConfigurationUtilities.GetSetting("Legacy.Directory", "Legacy"));
				
				PersonalizationDirectoryPath = FileMapper.MapApplicationRelativePath(DataDirectory + "/Personalization_Data");
				
			}
		}
		
		/// <summary>
		/// Restores the data and files from the legacy directory.
		/// </summary>
		public void Restore()
		{
			// Ensure that restoration is due
			if (RequiresRestore)
			{
				if (!Config.IsInitialized)
					throw new InvalidOperationException("The configuration hasn't been initialized.");
				
				ImportData();
				
				ImportPersonalization();
				
				ImportConfigs();
				
				DeleteEmptyLegacyDirectory();
			}
		}
		
		/// <summary>
		/// Imports the data from the legacy directory.
		/// </summary>
		public void ImportData()
		{
			IDataImporter importer = DataAccess.Data.InitializeDataImporter();
			importer.ImportableDirectoryPath = LegacyDirectoryPath;
			importer.ImportFromXml();
		}
		
		/// <summary>
		/// Imports the personalization data from the legacy directory.
		/// </summary>
		public void ImportPersonalization()
		{
			
			string personalizationPath = LegacyDirectoryPath + Path.DirectorySeparatorChar + "Personalization_Data";
			
			if (Directory.Exists(personalizationPath))
			{
				string toDirectoryPath = PersonalizationDirectoryPath;
				
				if (!Directory.Exists(toDirectoryPath))
					Directory.CreateDirectory(toDirectoryPath);
				
				foreach (string file in Directory.GetFiles(personalizationPath))
				{
					string toPath = toDirectoryPath + Path.DirectorySeparatorChar + Path.GetFileName(file);
					File.Copy(file, toPath, true);
				}
			}
		}
		
		/// <summary>
		/// Imports the personalization data from the legacy directory.
		/// </summary>
		public void ImportConfigs()
		{
			using (LogGroup logGroup = LogGroup.Start("Importing the legacy configuration settings.", NLog.LogLevel.Debug))
			{
				if (Config.Application == null)
					throw new InvalidOperationException("Config.Application has not been initialized.");
				
				AppConfig legacyConfig = ConfigFactory<AppConfig>.LoadConfig(LegacyDirectoryPath, "Application", Config.Application.PathVariation);
				
				// TODO: Transfer important settings from legacy config to current config.
				
				if (legacyConfig != null)
				{
					Config.Application.Title = legacyConfig.Title;
					Config.Application.Settings = legacyConfig.Settings;
					
					Config.Application.Save();
				}
				
				if (DataAccess.Data == null)
					throw new InvalidOperationException("DataAccess.Data has not been initialized.");
				
				if (DataAccess.Data.Importer == null)
					throw new InvalidOperationException("DataAccess.Data.Importer has not been initialized.");
				
				string importedDirectory = DataAccess.Data.Importer.ImportedDirectoryPath;
				
				foreach (string file in Directory.GetFiles(LegacyDirectoryPath))
				{
					LogWriter.Debug("Moving file: " + file);
					
					string toFile = importedDirectory + Path.DirectorySeparatorChar
						+ LegacyVersion.ToString().Replace(".", "-") + Path.DirectorySeparatorChar
						+ Path.GetFileName(file);
					
					LogWriter.Debug("To: " + toFile);
					
					if (!Directory.Exists(Path.GetDirectoryName(toFile)))
						Directory.CreateDirectory(Path.GetDirectoryName(toFile));
					
					
					// If the to file already exists then delete it.
					// This should only ever happen during testing/debugging and should NEVER occur in production as each update puts files into a
					// different directory based on the legacy version number
					if (File.Exists(toFile))
						File.Delete(toFile);
					
					File.Move(file, toFile);
				}
			}
		}
		
		/// <summary>
		/// Deletes the legacy directory and all sub directories, which should be empty.
		/// </summary>
		public void DeleteEmptyLegacyDirectory()
		{
			foreach (string dir in Directory.GetDirectories(LegacyDirectoryPath))
			{
				Directory.Delete(dir); // Don't do recursive delete. Directory must be empty otherwise the import may not have succeeded.
			}
			
			Directory.Delete(LegacyDirectoryPath); // Don't do recursive delete. Directory must be empty otherwise the import may not have succeeded.
		}
		
		/// <summary>
		/// Checks that the LegacyDirectoryPath property has been set.
		/// </summary>
		public void CheckLegacyDirectoryPath()
		{
			if (LegacyDirectoryPath == null || LegacyDirectoryPath == String.Empty)
				throw new InvalidOperationException("The LegacyDirectoryPath property hasn't been set.");
		}
		
		
		/// <summary>
		/// Ensures that the file mapper has been set.
		/// </summary>
		public void CheckFileMapper()
		{
			if (FileMapper == null)
				throw new InvalidOperationException("FileMapper property has not been set.");
		}
	}
}
