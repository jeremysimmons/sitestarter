using System;
using System.IO;
using System.Configuration;
using SoftwareMonkeys.SiteStarter.Data;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Creates zipped up backups of application data.
	/// </summary>
	public class ApplicationBackup
	{
		private int totalFilesZipped;
		/// <summary>
		/// Gets/sets the total number of files that were zipped into the backup.
		/// </summary>
		public int TotalFilesZipped
		{
			get { return totalFilesZipped; }
			set { totalFilesZipped = value; }
		}
		
		private string exportDirectoryPath;
		/// <summary>
		/// Gets/sets the path to the export directory.
		/// </summary>
		public string ExportDirectoryPath
		{
			get {
				return exportDirectoryPath;
			}
			set { exportDirectoryPath = value; }
		}
		
		
		private string backupDirectoryPath;
		/// <summary>
		/// Gets/sets the path to the backup directory.
		/// </summary>
		public string BackupDirectoryPath
		{
			get {
				return backupDirectoryPath;
			}
			set { backupDirectoryPath = value; }
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
		
		private string dataDirectoryPath;
		/// <summary>
		/// Gets/sets the path to the data directory.
		/// </summary>
		public string DataDirectoryPath
		{
			get {
				return dataDirectoryPath;
			}
			set { dataDirectoryPath = value; }
		}
		
		private bool keepLegacy;
		/// <summary>
		/// Gets/sets a boolean flag indicating whether the backed up data should be copied into the legacy directory.
		/// </summary>
		public bool KeepLegacy
		{
			get { return keepLegacy; }
			set { keepLegacy = value; }
		}
		
		/// <summary>
		/// Sets the BackupDirectoryPath property from Backup.Directory configuration settings.
		/// </summary>
		public ApplicationBackup()
		{
			Initialize();
		}
		
		private void Initialize()
		{
			if (Configuration.Config.IsInitialized && ConfigurationSettings.AppSettings != null)
			{
				
				string physicalPath = Configuration.Config.Application.PhysicalApplicationPath;
				
				DataDirectoryPath = physicalPath + Path.DirectorySeparatorChar +
					"App_Data";
				
				string exportDirectory = physicalPath + Path.DirectorySeparatorChar + ConfigurationSettings.AppSettings["Export.Directory"];
				if (exportDirectory == String.Empty)
					exportDirectory = DataDirectoryPath + Path.DirectorySeparatorChar + "Export";
				
				string backupDirectory = physicalPath + Path.DirectorySeparatorChar + ConfigurationSettings.AppSettings["Backup.Directory"];
				if (backupDirectory == String.Empty)
					backupDirectory = DataDirectoryPath + Path.DirectorySeparatorChar + "Backup";
				
				string legacyDirectory = physicalPath + Path.DirectorySeparatorChar + ConfigurationSettings.AppSettings["Legacy.Directory"];
				if (legacyDirectory == String.Empty)
					legacyDirectory = DataDirectoryPath + Path.DirectorySeparatorChar + "Legacy";
				
				
				ExportDirectoryPath = exportDirectory;
				
				BackupDirectoryPath = backupDirectory;
				
				LegacyDirectoryPath = legacyDirectory;
				
			}
		}
		/// <summary>
		/// Backs up all application data to the directory secified by the BackupDirectory property.
		/// </summary>
		/// <returns>The full path to the zipped backup file.</returns>
		public string Backup()
		{
			// Export data to XML
			Export();
			
			// Copy configuration files into export folder
			BackupConfigs();
			
			// Copy version file to export folder
			BackupVersion();
			
			// Zip up files
			string outputFile = ZipFiles();			
			
			// Copy data to legacy
			if (KeepLegacy)
				MoveToLegacy();
			// Delete the exported files (now that they've been zipped up)
			else
				DeleteExportFiles();
			
			return outputFile;
		}
		
		/// <summary>
		/// Deletes the temporary exported files.
		/// </summary>
		public void DeleteExportFiles()
		{
			Directory.Delete(ExportDirectoryPath, true);
		}
		
		/// <summary>
		/// Backs up data to the legacy folder ready for application update/restart.
		/// </summary>
		public void MoveToLegacy()
		{
			if (Directory.Exists(LegacyDirectoryPath))
				Directory.Delete(LegacyDirectoryPath, true);
			
			Directory.Move(ExportDirectoryPath, LegacyDirectoryPath);
		}
		
		/// <summary>
		/// Uses the DataExporter adapter to export data to XML.
		/// </summary>
		private void Export()
		{
			DataExporter exporter = (DataExporter)DataAccess.Data.InitializeDataExporter();
			exporter.ExportDirectoryPath = this.ExportDirectoryPath;
			exporter.ExportToXml();
		}
		
		/// <summary>
		/// Copies the current configuration files into the export folder.
		/// </summary>
		private void BackupConfigs()
		{
			if (!Directory.Exists(ExportDirectoryPath))
				Directory.CreateDirectory(ExportDirectoryPath);
			
			foreach (string file in Directory.GetFiles(DataDirectoryPath, "*.config"))
			{
				string toFile = ExportDirectoryPath + Path.DirectorySeparatorChar +
					Path.GetFileName(file);
				
				File.Copy(file, toFile, true);
			}
		}
		
		/// <summary>
		/// Backs up the version file.
		/// </summary>
		private void BackupVersion()
		{
			string file = DataDirectoryPath + Path.DirectorySeparatorChar +
				"Version.number";
			
			if (!File.Exists(file))
			{
				// Get the file from the application directory
				file = DataDirectoryPath + Path.DirectorySeparatorChar +
					".." + Path.DirectorySeparatorChar + "Version.number";
			}
			
			string toFile = ExportDirectoryPath + Path.DirectorySeparatorChar +
				Path.GetFileName(file);
			
			File.Copy(file, toFile, true);
		}
		
		/// <summary>
		/// Zips up the files in the export directory to a backup zip file.
		/// </summary>
		/// <returns>The full path to the zip file.</returns>
		private string ZipFiles()
		{
			string zipFileName = @"Backup--" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "--" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + ".zip";
			
			string zipFilePath = BackupDirectoryPath + Path.DirectorySeparatorChar +
				zipFileName;
			
			DirectoryZipper zipper = new DirectoryZipper(ExportDirectoryPath);
			
			TotalFilesZipped = zipper.ZipToFile(zipFilePath);
			
			
			return zipFilePath;
		}
	}
}
