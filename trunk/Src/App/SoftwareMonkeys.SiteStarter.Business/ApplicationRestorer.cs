using System;
using System.IO;
using System.Configuration;
using SoftwareMonkeys.SiteStarter.Data;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Restores application data and configuration files.
	/// </summary>
	public class ApplicationRestorer
	{		
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
		
		public ApplicationRestorer()
		{
			Initialize();
		}
		
		private void Initialize()
		{
			if (Configuration.Config.IsInitialized && ConfigurationSettings.AppSettings != null)
			{
				string physicalPath = Configuration.Config.Application.PhysicalApplicationPath;
				
				LegacyDirectoryPath = physicalPath + Path.DirectorySeparatorChar +
					ConfigurationSettings.AppSettings["Legacy.Directory"];
				
				PersonalizationDirectoryPath = physicalPath + Path.DirectorySeparatorChar +
					"Personalization_Data";
				
			}
		}
		
		/// <summary>
		/// Restores the data and files from the legacy directory.
		/// </summary>
		public void Restore()
		{
			ImportData();
			
			ImportPersonalization();
			
			DeleteEmptyLegacyDirectory();
		}
		
		/// <summary>
		/// Imports the data from the legacy directory.
		/// </summary>
		public void ImportData()
		{
            DataAccess.Data.Importer.ImportFromXml();
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
	}
}
