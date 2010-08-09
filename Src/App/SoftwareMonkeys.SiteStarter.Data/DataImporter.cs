using System;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Data;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Provides a base implementation for all data importers, which are used for imorting data from XML.
	/// </summary>
	public class DataImporter : DataAdapter, IDataImporter
	{
		private bool autoRenewSchema = true;
		/// <summary>
		/// Gets/sets a boolean flag indicating whether the schema of imported entities should be automatically renewed upon import.
		/// Note: It is recommended that this remain true always except in some testing and/or development environments. Leaving it false
		/// may cause a loss of data on properties or types that no longer exist.
		/// </summary>
		public bool AutoRenewSchema
		{
			get { return autoRenewSchema; }
			set { autoRenewSchema = value; }
		}
		
		private string importableDirectoryPath;
		/// <summary>
		/// Gets/sets the path to the directory containing importable data.
		/// </summary>
		public string ImportableDirectoryPath
		{
			get { return importableDirectoryPath;  }
			set { importableDirectoryPath = value; }
		}
		
		private string importedDirectoryPath;
		/// <summary>
		/// Gets/sets the path to the directory containing imported data.
		/// </summary>
		public string ImportedDirectoryPath
		{
			get { return importedDirectoryPath;  }
			set { importedDirectoryPath = value; }
		}
		
		/// <summary>
		/// Configures the default imported directory path.
		/// </summary>
		public DataImporter()
		{
			if (Configuration.Config.IsInitialized)
			{
				string basePath = Configuration.Config.Application.PhysicalPath + Path.DirectorySeparatorChar +
					"App_Data" + Path.DirectorySeparatorChar;
				
				// Importable directory
				ImportableDirectoryPath = basePath + "Importable";
				
				// Imported directory
				ImportedDirectoryPath = basePath + "Imported";
			}
		}
		
		
		/// <summary>
		/// Imports the data from the XML files found in the importable directory (each in their own type specific subdirectory).
		/// </summary>
		public void ImportFromXml()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Importing all objects in the working directory.", NLog.LogLevel.Debug))
			{
				foreach (string file in LoadEntitiesFileList())
				{

					IEntity entity = LoadEntityFromFile(file);
					
					AppLogger.Debug("Entity type: " + entity.GetType().ToString());

					if (!DataAccess.Data.IsStored((IEntity)entity))
					{
						AppLogger.Debug("New entity. Importing.");
						
						DataAccess.Data.Saver.Save(entity);
					}
					else
					{
						AppLogger.Debug("Entity already exists in store. Skipping.");
					}

					MoveToImported(entity);
				}
			}
		}
		
		
		/// <summary>
		/// Retrieves a list of all the importable entity file paths.
		/// </summary>
		/// <returns>An array of file paths.</returns>
		public string[] LoadEntitiesFileList()
		{
			string directory = ImportableDirectoryPath;
			
			List<string> list = new List<string>();

			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving a list of importable entities from the specified directory.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Directory: " + directory);
				
				if (Directory.Exists(directory))
				{
					AppLogger.Debug("Directory exists");
					
					foreach (string subDirectory in Directory.GetDirectories(directory))
					{
						foreach (string file in Directory.GetFiles(subDirectory, "*.xml"))
						{
							list.Add(file);
						}
					}
				}
			}
			
			return list.ToArray();
		}
		
		/// <summary>
		/// Instantiates and deserializes the entity in the specified file.
		/// </summary>
		/// <param name="filePath">The path to the entity file.</param>
		/// <returns>The deserialized entity object.</returns>
		public IEntity LoadEntityFromFile(string filePath)
		{
			IEntity entity = null;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Loading an entity from XML file.", NLog.LogLevel.Debug))
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(filePath);
				
				// Renew/update the schema so it matches the current application version
				if (AutoRenewSchema)
				{
					Provider.Schema.RenewSchema(doc);
				}

				string shortTypeName = doc.DocumentElement.Name;

				Type type = EntitiesUtilities.GetType(shortTypeName);

				if (type == null)
					throw new Exception("Cannot find object type with corresponding short type name of '" + shortTypeName + "'.");

				AppLogger.Debug("Loading type: " + type.ToString());

				entity = (IEntity)XmlUtilities.DeserializeFromDocument(doc, type);
				
			}
			
			return entity;
		}
		
		/// <summary>
		/// Moves the provided entity from the importable directory to the imported directory.
		/// </summary>
		/// <param name="entity">The entity to move.</param>
		public void MoveToImported(IEntity entity)
		{
			
			using (LogGroup logGroup = AppLogger.StartGroup("Marking the provided entity as imported by moving it to the imported directory.", NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity", "The provided entity cannot be null.");

				AppLogger.Debug("Entity type: " + entity.GetType());

				string fileName = String.Empty;
				string toFileName = String.Empty;


				string typeName = entity.GetType().ToString();
				
				DataExporter exporter = (DataExporter)Provider.InitializeDataExporter();
				exporter.ExportDirectoryPath = Path.GetDirectoryName(exporter.ExportDirectoryPath);

				fileName = CreateImportableEntityPath(entity);
				toFileName = CreateImportedEntityPath(entity);
				
				AppLogger.Debug("Imported directory specified. Continuing.");
				

				AppLogger.Debug("From path: " + fileName);
				AppLogger.Debug("To path: " + toFileName);

				if (!Directory.Exists(Path.GetDirectoryName(toFileName)))
					Directory.CreateDirectory(Path.GetDirectoryName(toFileName));


				if (File.Exists(fileName))
				{
					if (File.Exists(toFileName))
						File.Delete(toFileName);

					File.Move(fileName, toFileName);

					AppLogger.Debug("Moved");
				}
				else
				{
					AppLogger.Debug("File not found. Not moved.");
				}
			}
		}
		// TODO: Remove if not needed
		/*
		/// <summary>
		/// Moves the provided entity to the specified imported directory. This is called so data doesn't get imported twice.
		/// </summary>
		/// <param name="importedDirectoryPath">The directory containing imported entity XML files after they're imported.</param>
		/// <param name="reference">The reference to move.</param>
		public void MoveReferenceToImported(string importedDirectoryPath, EntityReference reference)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Moving the provided entity to the imported directory.", NLog.LogLevel.Debug))
			{
			}
		}*/
		
		/// <summary>
		/// Creates the file path for the provided entity in the folder specified by the ImportedDirectoryPath property.
		/// </summary>
		/// <param name="entity">The entity to create the file path for.</param>
		/// <returns>The file path for the provided entity.</returns>
		public string CreateImportedEntityPath(IEntity entity)
		{
			// This function is passed through to the exporter adapter, but with the ImportedDirectoryPath

			DataExporter exporter = (DataExporter)Provider.InitializeDataExporter();
			exporter.ExportDirectoryPath = ImportedDirectoryPath;
			return exporter.CreateEntityPath(entity);
		}
		/// <summary>
		/// Creates the file path for the provided entity in the folder specified by the ImportableDirectoryPath property.
		/// </summary>
		/// <param name="entity">The entity to create the file path for.</param>
		/// <returns>The file path for the provided entity.</returns>
		public string CreateImportableEntityPath(IEntity entity)
		{
			// This function is passed through to the exporter adapter, but using the ImportableDirectoryPath

			DataExporter exporter = (DataExporter)Provider.InitializeDataExporter();
			exporter.ExportDirectoryPath = ImportableDirectoryPath;
			return exporter.CreateEntityPath(entity);
		}
	}
}
