using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace SoftwareMonkeys.SiteStarter.Data
{
	public class XmlBackupManager
	{
		public int TotalRecordsFound;
		public int TotalRecordsExported;
		public int TotalRecordsZipped;
		

		private string applicationPath;
		public string ApplicationPath
		{
			get { return applicationPath; }
			set { applicationPath = value; }
		}

		private string dataDirectoryName;
		public string DataDirectoryName
		{
			get { return dataDirectoryName; }
			set { dataDirectoryName = value; }
		}

		public string DataDirectoryPath
		{
			get
			{
				return ApplicationPath + Path.DirectorySeparatorChar
					+ DataDirectoryName;
			}
		}

		private string backupsDirectoryName;
		public string BackupsDirectoryName
		{
			get { return backupsDirectoryName; }
			set { backupsDirectoryName = value; }
		}

		public string BackupsDirectoryPath
		{
			get
			{
				return ApplicationPath + Path.DirectorySeparatorChar
					+ DataDirectoryName + Path.DirectorySeparatorChar
					+ BackupsDirectoryName;
			}
		}

		private string tmpDirectoryName;
		public string TmpDirectoryName
		{
			get { return tmpDirectoryName; }
			set { tmpDirectoryName = value; }
		}

		public string TmpDirectoryPath
		{
			get
			{
				return ApplicationPath + Path.DirectorySeparatorChar
					+ DataDirectoryName + Path.DirectorySeparatorChar
					+ TmpDirectoryName;
			}
		}

		private string urlVariation;
		public string UrlVariation
		{
			get { return urlVariation; }
			set { urlVariation = value; }
		}

		private bool preparingForUpdate = false;
		public bool PreparingForUpdate
		{
			get { return preparingForUpdate; }
			set { preparingForUpdate = value; }
		}


		public XmlBackupManager(string applicationPath, string dataDirectoryName, string backupsDirectoryName, string urlVariation)
			: this(applicationPath, dataDirectoryName, backupsDirectoryName, urlVariation, false)
		{
		}

		public XmlBackupManager(string applicationPath, string dataDirectoryName, string backupsDirectoryName, string urlVariation, bool preparingForUpdate)
		{
			ApplicationPath = applicationPath;
			DataDirectoryName = dataDirectoryName;
			BackupsDirectoryName = backupsDirectoryName;
			TmpDirectoryName = "Tmp";
			UrlVariation = urlVariation;
			PreparingForUpdate = preparingForUpdate;
		}


		private void ExportObjects()
		{
			foreach (string dataStoreName in DataAccess.Data.GetDataStoreNames())
			{
				object[] objects = DataAccess.Data.Stores[dataStoreName].Indexer.GetEntities();


				if (objects != null)
				{
					foreach (Object obj in objects)
					{
						TotalRecordsFound++;

						ExportObject(obj);
					}
				}
			}
		}

		private void ExportObject(object obj)
		{
			Type type = obj.GetType();
			string objectNamespace = type.Namespace;
			string objectName = type.Name;
			
			string idPropertyName = "ID";
			string directoryPath = String.Empty;

			
			if (obj is EntityReference || obj is EntityIDReference)
			{
				EntityIDReference reference = (EntityIDReference)obj;
				string[] types = new String[] {reference.Type1Name, reference.Type2Name};
				
				Array.Sort(types);
				
				objectName = types[0] + "-" + types[1];
				
				directoryPath = EnsureExportDirectoryExists(objectName);
			}
			else
			{
				directoryPath = EnsureExportDirectoryExists(objectNamespace, objectName);
			}
			
			

			//Panel resultPanel = new Panel();

			// resultPanel.Controls.Add(new LiteralControl("<b>Starting export:</b> " + objectNamespace + "." + objectName + "<br/>"));
			//try
			//{
				// TODO: Ensure this is obsolete
				// Type type = Db4oHelper.GetType(objectNamespace + "." + objectName);

				//     foreach (object obj in Db4oHelper.GetObjects(type))
				//   {
				Guid id = GetID(obj, idPropertyName);

				if (id == Guid.Empty)
				{
					id = Guid.NewGuid();
				}

				if (obj is IEntity)
				{
					((IEntity)obj).ID = id;
				}

				//     }

				// resultPanel.Controls.Add(new LiteralControl(" -- Found - ID: " + id + "<br/>"));

				string filePath = directoryPath + @"\" + id + ".xml";
				
				//resultPanel.Controls.Add(new LiteralControl(" --- Path: " + filePath + "<br/>"));

				SerializeToFile(obj, filePath);

				//resultPanel.Controls.Add(new LiteralControl(" -- Exported" + "<br/>"));

				//		} // TODO: Check if needed. Should be obsolete
				//		else
				//		{
				//			resultPanel.Controls.Add(new LiteralControl(" -- Found - ID: [None found. This object is not an entity. Couldn't be exported.]" + "<br/>"));
				//		}

			//}
			//catch (Exception ex)
			//{
			//	throw ex;
				// resultPanel.Controls.Add(new LiteralControl("Error: [Referring to " + objectNamespace + "." + objectName + "] " + ex.ToString() + "<br/>"));
			//}

			//OutputPanel.Controls.Add(resultPanel);

			TotalRecordsExported++;
		}

		private Guid GetID(object obj, string idPropertyName)
		{
			PropertyInfo property = obj.GetType().GetProperty(idPropertyName);
			if (property == null)
				return Guid.Empty;

			object value = property.GetValue(obj, null);

			if (value is Guid)
				return (Guid)value;
			else
				return Guid.Empty;

		}

		private void SerializeToFile(object obj, string filePath)
		{
			XmlSerializer serializer = new XmlSerializer(obj.GetType());
			using (StreamWriter writer = File.CreateText(filePath))
			{
				serializer.Serialize(writer, obj);

				writer.Close();
			}

		}
		
		private string EnsureExportDirectoryExists(string objectName)
		{
			string path = TmpDirectoryPath + Path.DirectorySeparatorChar
				+ objectName;

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			return path;
		}


		private string EnsureExportDirectoryExists(string objectNamespace, string objectName)
		{
			string path = TmpDirectoryPath + Path.DirectorySeparatorChar
				+ objectNamespace + "." + objectName;

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			return path;
		}


		private void ExportVersionFile()
		{
			string versionFilePath = Path.Combine(ApplicationPath, "Version.number");

			string newVersionFilePath = TmpDirectoryPath + Path.DirectorySeparatorChar + "Version.number";
			
			if (!Directory.Exists(Path.GetDirectoryName(newVersionFilePath)))
				Directory.CreateDirectory(Path.GetDirectoryName(newVersionFilePath));

			if (File.Exists(versionFilePath))
				File.Copy(versionFilePath, newVersionFilePath, true);
		}


		private void ExportConfigFile(string configName)
		{
			string configFileName = String.Empty;
			//string variation = WebUtilities.GetLocationVariation(Request.Url);
			if (UrlVariation != String.Empty)
				configFileName = configName + "." + UrlVariation + ".config";
			else
				configFileName = configName + ".config";

			string configFilePath = DataDirectoryPath + Path.DirectorySeparatorChar
				+ configFileName;

			string newConfigFilePath = TmpDirectoryPath + Path.DirectorySeparatorChar
				+ configFileName;

			if (!Directory.Exists(Path.GetDirectoryName(newConfigFilePath)))
				Directory.CreateDirectory(Path.GetDirectoryName(newConfigFilePath));

			if (File.Exists(configFilePath))
				File.Copy(configFilePath, newConfigFilePath, true);
		}

		private string GetApplicationFilePath(string fileName)
		{
			return Path.Combine(ApplicationPath, "App_Data" + Path.DirectorySeparatorChar + fileName);
		}

		private string GetTmpFilePath(string fileName)
		{
			return ApplicationPath + Path.DirectorySeparatorChar + DataDirectoryName + Path.DirectorySeparatorChar + TmpDirectoryName + fileName;
		}


		private void ZipData()
		{
			
			string zipPath = GetBackupZipPath();
			string tmpPath = TmpDirectoryPath;

			if (!Directory.Exists(tmpPath))
			{
				//try
				//{
				Directory.CreateDirectory(tmpPath);
				//}

			}

			if (!Directory.Exists(Path.GetDirectoryName(zipPath)))
			{
				//try
				//{
				Directory.CreateDirectory(Path.GetDirectoryName(zipPath));
				//}

			}


			// Create the zip file
			//Crc32 crc = new Crc32();
			ZipOutputStream zipFile = new ZipOutputStream(File.Create(zipPath));
			zipFile.UseZip64 = UseZip64.Off;
			zipFile.SetLevel(9);

			ZipFromPath(zipFile, TmpDirectoryPath);

			// Close the writer
			zipFile.Finish();
			zipFile.Close();

			if (PreparingForUpdate)
			{
				string legacyPath = Path.Combine(
					Path.GetDirectoryName(Path.GetDirectoryName(tmpPath)),
					"Legacy");

				if (Directory.Exists(legacyPath))
					Directory.Delete(legacyPath, true);

				Directory.Move(tmpPath, legacyPath);
			}
			else
			{
				DeleteTemporaryFiles();
			}
		}

		private void ZipFromPath(ZipOutputStream zipFile, string path)
		{
			foreach (string filePath in Directory.GetFiles(path))
			{
				string[] parts = Path.GetDirectoryName(filePath).Split('\\');
				string newFileName = filePath.Replace(TmpDirectoryPath, String.Empty);
				
				try
				{
					FileStream stream = File.OpenRead(filePath);

					byte[] buffer = new byte[stream.Length];
					stream.Read(buffer, 0, buffer.Length);
					ZipEntry entry = new ZipEntry(newFileName);

					entry.DateTime = DateTime.Now;

					entry.Size = stream.Length;
					stream.Close();

					//crc.Reset();
					//crc.Update(buffer);

					//entry.Crc  = crc.Value;

					zipFile.PutNextEntry(entry);

					zipFile.Write(buffer, 0, buffer.Length);

					TotalRecordsZipped++;
				}
				catch (FileNotFoundException ex)
				{
					throw ex;
					// Just ignore files that have been moved or deleted. They'll get removed from the list next time the release is edited.
				}
				catch (Exception ex)
				{
					throw ex;
					//Error error = new Error(ex);
					//error.IsLocal = true;
					//My.ErrorEngine.SaveError(error);
				}
			}

			foreach (string directory in Directory.GetDirectories(path))
			{
				ZipFromPath(zipFile, directory);
			}
		}

		public void DeleteTemporaryFiles()
		{
			Directory.Delete(TmpDirectoryPath, true);
		}

		public void ClearBackups()
		{
			string backupsPath = ApplicationPath + Path.DirectorySeparatorChar + DataDirectoryName + Path.DirectorySeparatorChar + BackupsDirectoryName;
			foreach (string filename in Directory.GetFiles(backupsPath, "*.zip"))
			{
				if (Path.GetExtension(filename).ToLower() == ".zip")
				{
					File.Delete(filename);
				}
			}

		}

		/*private string GetTempPath()
        {
            string path = Path.Combine(GetBackupsPath(), "Tmp");
            return path;
        }*/

		/*private string GetBackupsPath()
        {
            if (BackupsPath == String.Empty)
            {
                string backupDirectory = String.Empty;
                if (ConfigurationSettings.AppSettings["Backup.Directory"] == null)
                    BackupsPath = String.Empty;
                else
                    backupDirectory = ConfigurationSettings.AppSettings["Backup.Directory"];
                string path = Path.Combine(Server.MapPath(Request.ApplicationPath), backupDirectory.Replace("/", @"\"));
                //path = Path.Combine(path, DateTime.Now.ToString().Replace(":", "-").Replace("/", "-").Replace(" ", "-"));

                BackupsPath = path;
            }
            return BackupsPath;
        }*/

		private string GetBackupZipPath()
		{
			//string tmpPath = GetTempPath();
			string fileName = @"Backup--" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "--" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + ".zip";
			string zipPath = ApplicationPath + Path.DirectorySeparatorChar + DataDirectoryName + Path.DirectorySeparatorChar + BackupsDirectoryName + Path.DirectorySeparatorChar + fileName;
			//zipShortPath;//Config.Application.PhysicalPath + zipShortPath;

			

			return zipPath;
		}

		public void ExecuteBackup()
		{

			ExportConfigFile("Application");
			ExportConfigFile("Mappings");
			ExportVersionFile();
			ExportObjects();

			ZipData();
		}

		public string GetBackupZipWebPath(string relativeApplicationPath)
		{
			string zipWebPath = GetBackupZipPath().Replace("\\", "/").Replace(Config.Application.PhysicalPath, relativeApplicationPath + "/");
			return zipWebPath;
		}
	}
}
