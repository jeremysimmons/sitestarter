using System;
using System.IO;
using System.Configuration;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Configuration;

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
		
		private bool prepareForUpdate;
		/// <summary>
		/// Gets/sets a value indicating whether the application and data should be prepared for an application update.
		/// Note: Setting this to true means the data will be suspended and the exported data copied to the Legacy directory so it can be reimported (with schema modifications applied if applicable).
		/// </summary>
		public bool PrepareForUpdate
		{
			get { return prepareForUpdate; }
			set { prepareForUpdate = value; }
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
				
				string exportDirectory = physicalPath + Path.DirectorySeparatorChar + ConfigurationUtilities.GetSetting("Export.Directory", "Export").Replace("/", @"\");
				
				string backupDirectory = physicalPath + Path.DirectorySeparatorChar + ConfigurationUtilities.GetSetting("Backup.Directory", "Backup").Replace("/", @"\");
				
				string legacyDirectory = physicalPath + Path.DirectorySeparatorChar + ConfigurationUtilities.GetSetting("Legacy.Directory", "Legacy").Replace("/", @"\");
				
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
			string outputFile = String.Empty;
			
			using (LogGroup logGroup = LogGroup.Start("Backing up the application.", NLog.LogLevel.Debug))
			{
				// Export data to XML
				Export();
				
				// Copy configuration files into export folder
				BackupConfigs();
				
				// Copy version file to export folder
				BackupVersion();
				
				// Zip up files
				outputFile = ZipFiles();
				
				// Prepare for application update
				if (PrepareForUpdate)
				{
					LogWriter.Debug("Prepare for update");
					
					MoveToLegacy();
					
					Suspend();
				}
				// Delete the exported files (now that they've been zipped up)
				else
				{
					LogWriter.Debug("Prepare for update == false");
					
					DeleteExportFiles();
				}
				
				LogWriter.Info("Backup completed: " + DateTime.Now.ToString());
				LogWriter.Info("${AutoBackup}");
			}
			
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
			string filePath = DataDirectoryPath + Path.DirectorySeparatorChar +
				VersionUtilities.GetVersionFileName();
			
			if (!File.Exists(filePath))
			{
				throw new InvalidOperationException("Version.number file not found at: " + filePath);
			}
			
			string toFile = ExportDirectoryPath + Path.DirectorySeparatorChar +
				Path.GetFileName(filePath);
			
			File.Copy(filePath, toFile, true);
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
		
		/// <summary>
		/// Suspends the data stores and moves them to a safe location outside the application.
		/// Note: This is for use during an update, as the data will be imported again.
		/// The export/suspend/import mechanism is used to allow for schema modifications from one version of the application to another.
		/// </summary>
		public void Suspend()
		{
			using (LogGroup logGroup = LogGroup.Start("Suspending the application (ready for update).", NLog.LogLevel.Debug))
			{
				DataAccess.Data.Suspend();
				
				string[] suspendableTypes = new String[] {"config", "number"};
				
				string toDirectory = DataAccess.Data.SuspendedDirectoryPath + Path.DirectorySeparatorChar
					+ DataAccess.Data.Schema.ApplicationVersion.ToString().Replace(".", "-");
				
				LogWriter.Debug("To directory: " + toDirectory);
				
				foreach (string file in Directory.GetFiles(DataAccess.Data.DataDirectoryPath))
				{
					string ext = Path.GetExtension(file).Trim('.').ToLower();
					
					// If the file extension is in the suspendable types array then suspend it
					if (Array.IndexOf(suspendableTypes, ext) > -1)
					{
						LogWriter.Debug("Suspending file: " + file);
						
						string toFile = toDirectory + Path.DirectorySeparatorChar + Path.GetFileName(file);
						
						LogWriter.Debug("To: " + toFile);
						
						if (!Directory.Exists(Path.GetDirectoryName(toFile)))
							Directory.CreateDirectory(Path.GetDirectoryName(toFile));
						
						// If the to file already exists then delete it
						// This shouldn't occur in production but can during debugging
						if (File.Exists(toFile))
							File.Delete(toFile);
						
						File.Move(file, toFile);
					}
				}
				
				string[] suspendableDirectories = new String[]
				{
					"Entities",
					"Strategies",
					"Projections",
					"Controllers"
				};
				
				foreach (string shortDir in suspendableDirectories)
				{
					string dir = DataAccess.Data.DataDirectoryPath + Path.DirectorySeparatorChar + shortDir;
					string toDir = DataAccess.Data.SuspendedDirectoryPath + Path.DirectorySeparatorChar + shortDir;
					
					// If the directory already exists then delete it
					// This shouldn't occur in production but can during debugging
					if (Directory.Exists(toDir))
						Directory.Delete(toDir, true);
					
					if (Directory.Exists(dir))
						Directory.Move(dir, toDir);
				}
			}
		}
	}
}
