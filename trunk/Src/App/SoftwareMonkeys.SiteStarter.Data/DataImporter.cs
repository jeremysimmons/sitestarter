using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
		
		private string failedDirectoryPath;
		/// <summary>
		/// Gets/sets the path to the directory containing failed/invalid data.
		/// </summary>
		public string FailedDirectoryPath
		{
			get { return failedDirectoryPath;  }
			set { failedDirectoryPath = value; }
		}
		
		/// <summary>
		/// Configures the default imported directory path.
		/// </summary>
		public DataImporter()
		{
			if (Configuration.Config.IsInitialized)
			{
				string basePath = Configuration.Config.Application.PhysicalApplicationPath + Path.DirectorySeparatorChar +
					"App_Data" + Path.DirectorySeparatorChar;
				
				// Importable directory
				ImportableDirectoryPath = basePath + "Import";
				
				// Imported directory
				ImportedDirectoryPath = basePath + "Imported";
				
				// Failed directory
				FailedDirectoryPath = basePath + "FailedImport";
			}
		}
		
		
		/// <summary>
		/// Imports the data from the XML files found in the importable directory (each in their own type specific subdirectory).
		/// </summary>
		public void ImportFromXml()
		{
			using (LogGroup logGroup = LogGroup.Start("Importing all objects in the working directory.", LogLevel.Debug))
			{
				using (Batch batch = BatchState.StartBatch())
				{
					foreach (string file in LoadEntitiesFileList())
					{
						using (LogGroup logGroup2 = LogGroup.StartDebug("Attempting import of: " + file))
						{
							IEntity entity = LoadEntityFromFile(file);
							
							LogWriter.Debug("Entity type: " + entity.GetType().ToString());
							
							if (IsValid(entity))
							{
								LogWriter.Debug("Is valid entity.");
								
								if (!DataAccess.Data.IsStored((IEntity)entity))
								{
									LogWriter.Debug("New entity. Importing.");
									
									DataAccess.Data.Saver.Save(entity);
								}
								else
								{
									LogWriter.Debug("Entity already exists in store. Skipping.");
								}
								
								MoveToImported(entity, file);
							}
							else
							{
								LogWriter.Error("Cannot import invalid entity...\nID: " + entity.ID.ToString() + "\nType: " + entity.ShortTypeName);
								
								MoveToFailed(entity, file);
							}
						}
					}
					
					// Refresh the entities to ensure that all count properties are up to date.
					// This must be done AFTER ALL entities and references are imported.
					RefreshEntities();
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

			using (LogGroup logGroup = LogGroup.Start("Retrieving a list of importable entities from the specified directory.", LogLevel.Debug))
			{
				LogWriter.Debug("Directory: " + directory);
				
				if (Directory.Exists(directory))
				{
					LogWriter.Debug("Directory exists");
					
					// Get a list of entities
					list.AddRange(LoadStandardEntitiesFileList(directory));
					
					// Get list of references
					// (IMPORTANT: Must be imported AFTER the standard entities or the references will break)
					list.AddRange(LoadReferencesFileList(directory));
					
				}
			}
			
			return list.ToArray();
		}
		
		public string[] LoadStandardEntitiesFileList(string dir)
		{
			List<string> list = new List<string>();
			
			foreach (string subDirectory in Directory.GetDirectories(dir))
			{
				string folderName = Path.GetFileName(subDirectory);
				
				// If the folder name contains a dot then it contains standard entities
				if (folderName.IndexOf(".") > -1)
				{
					foreach (string file in Directory.GetFiles(subDirectory, "*.xml"))
					{
						if (!list.Contains(file))
							list.Add(file);
					}
				}
			}
			
			return list.ToArray();
		}
		
		public string[] LoadReferencesFileList(string dir)
		{
			List<string> list = new List<string>();
			
			foreach (string subDirectory in Directory.GetDirectories(dir))
			{
				string folderName = Path.GetFileName(subDirectory);
				
				// If the folder name contains a dash then it contains references
				if (folderName.IndexOf("-") > -1)
				{
					foreach (string file in Directory.GetFiles(subDirectory, "*.xml"))
					{
						if (!list.Contains(file))
							list.Add(file);
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
			
			using (LogGroup logGroup = LogGroup.Start("Loading an entity from XML file.", LogLevel.Debug))
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(filePath);
				
				// Renew/update the schema so it matches the current application version
				if (AutoRenewSchema)
				{
					Provider.Schema.RenewSchema(doc);
				}

				string shortTypeName = doc.DocumentElement.Name;

				if (EntityState.IsType(shortTypeName))
				{
					Type type = EntityState.GetType(shortTypeName);

					if (type == null)
						throw new Exception("Cannot find object type with corresponding short type name of '" + shortTypeName + "'.");

					LogWriter.Debug("Loading type: " + type.ToString());

					entity = (IEntity)XmlUtilities.DeserializeFromDocument(doc, type);
					
					// Dispose the XML document
					doc = null;
				}
			}
			
			return entity;
		}
		
		/// <summary>
		/// Moves the provided entity from the importable directory to the imported directory.
		/// </summary>
		/// <param name="entity">The entity to move.</param>
		public void MoveToImported(IEntity entity, string filePath)
		{
			
			using (LogGroup logGroup = LogGroup.Start("Marking the provided entity as imported by moving it to the imported directory.", LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity", "The provided entity cannot be null.");

				LogWriter.Debug("Entity type: " + entity.GetType());

				string fileName = String.Empty;
				string toFileName = String.Empty;


				string typeName = entity.GetType().ToString();
				

				//fileName = CreateImportableEntityPath(entity);
				fileName = filePath;
				toFileName = CreateImportedEntityPath(entity);
				
				LogWriter.Debug("Imported directory specified. Continuing.");
				

				LogWriter.Debug("From path: " + fileName);
				LogWriter.Debug("To path: " + toFileName);

				if (!Directory.Exists(Path.GetDirectoryName(toFileName)))
					Directory.CreateDirectory(Path.GetDirectoryName(toFileName));


				if (File.Exists(fileName))
				{
					if (File.Exists(toFileName))
						File.Delete(toFileName);

					File.Move(fileName, toFileName);

					LogWriter.Debug("Moved");
				}
				else
				{
					throw new Exception("File not found: " + fileName);
					//LogWriter.Debug("File not found. Not moved.");
				}
			}
		}
		
		/// <summary>
		/// Moves the provided entity from the importable directory to the failed directory.
		/// </summary>
		/// <param name="entity">The entity to move.</param>
		public void MoveToFailed(IEntity entity, string filePath)
		{
			
			using (LogGroup logGroup = LogGroup.Start("Marking the provided entity as failed by moving it to the failed import directory.", LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity", "The provided entity cannot be null.");

				LogWriter.Debug("Entity type: " + entity.GetType());

				string fileName = String.Empty;
				string toFileName = String.Empty;


				string typeName = entity.GetType().ToString();
				

				//fileName = CreateImportableEntityPath(entity);
				fileName = filePath;
				toFileName = CreateFailedEntityPath(entity);
				
				LogWriter.Debug("Failed directory specified. Continuing.");
				

				LogWriter.Debug("From path: " + fileName);
				LogWriter.Debug("To path: " + toFileName);

				if (!Directory.Exists(Path.GetDirectoryName(toFileName)))
					Directory.CreateDirectory(Path.GetDirectoryName(toFileName));


				if (File.Exists(fileName))
				{
					if (File.Exists(toFileName))
						File.Delete(toFileName);

					File.Move(fileName, toFileName);

					LogWriter.Debug("Moved");
				}
				else
				{
					throw new Exception("File not found: " + fileName);
					//LogWriter.Debug("File not found. Not moved.");
				}
			}
		}
		
		public bool IsValid(IEntity entity)
		{
			if (entity is EntityReference)
			{
				return IsValidReference((EntityReference)entity);
			}
			else
				return IsValidEntity(entity);
		}
		
		public bool IsValidReference(EntityReference reference)
		{
			if (reference.Entity1ID == Guid.Empty)
				return false;
			
			if (reference.Entity2ID == Guid.Empty)
				return false;
			
			if ((reference.Property1Name == null || reference.Property1Name == String.Empty)
			    && (reference.Property2Name == null || reference.Property2Name == String.Empty))
				return false;
			
			if (reference.Type1Name == null || reference.Type1Name == String.Empty)
				return false;
			
			if (reference.Type2Name == null || reference.Type2Name == String.Empty)
				return false;
			
			return true;
		}
		
		public bool IsValidEntity(IEntity entity)
		{
			return entity.ID != Guid.Empty
				&& EntityState.IsType(entity.ShortTypeName);
		}
		
		/// <summary>
		/// Creates the file path for the provided entity in the folder specified by the ImportedDirectoryPath property.
		/// </summary>
		/// <param name="entity">The entity to create the file path for.</param>
		/// <returns>The file path for the provided entity.</returns>
		public string CreateImportedEntityPath(IEntity entity)
		{
			string basePath = ImportedDirectoryPath + Path.DirectorySeparatorChar + GetPreviousVersion().ToString().Replace(".", "-");

			string fullPath = new EntityFileNamer(entity, basePath).CreateFilePath();
			
			return fullPath;
		}
		
		/// <summary>
		/// Creates the file path for the provided entity in the folder specified by the FailedDirectoryPath property.
		/// </summary>
		/// <param name="entity">The entity to create the file path for.</param>
		/// <returns>The file path for the provided entity.</returns>
		public string CreateFailedEntityPath(IEntity entity)
		{
			string basePath = FailedDirectoryPath + Path.DirectorySeparatorChar + GetPreviousVersion().ToString().Replace(".", "-");

			string fullPath = new EntityFileNamer(entity, basePath).CreateFilePath();
			
			return fullPath;
		}
		
		/// <summary>
		/// Creates the file path for the provided entity in the folder specified by the ImportableDirectoryPath property.
		/// </summary>
		/// <param name="entity">The entity to create the file path for.</param>
		/// <returns>The file path for the provided entity.</returns>
		public string CreateImportableEntityPath(IEntity entity)
		{
			Guid id = entity.ID;
			
			string basePath = ImportableDirectoryPath;

			string fullPath = new EntityFileNamer(entity, basePath).CreateFilePath();
			
			return fullPath;
		}
		
		public Version GetPreviousVersion()
		{
			
			Version legacyVersion = VersionUtilities.GetLegacyVersion(State.StateAccess.State.PhysicalApplicationPath);
			Version importVersion = VersionUtilities.GetImportVersion(State.StateAccess.State.PhysicalApplicationPath);
			
			Version version = null;
			
			if (legacyVersion.ToString() != new Version(0,0,0,0).ToString())
				version = legacyVersion;
			else
				version = importVersion;
			
			return version;
		}
		
		/// <summary>
		/// Refreshes all entities to ensure reference count properties are up to date. Simply loads each entity from the
		/// data store, activates it, then updates it back to the data store. The update sets count properties.
		/// </summary>
		public void RefreshEntities()
		{
			foreach (EntityInfo info in EntityState.Entities)
			{
				Type type = info.GetEntityType();
				
				if (type != null)
				{
					foreach (IEntity entity in Provider.Indexer.GetEntities(type))
					{
						Provider.Activator.Activate(entity);
						
						Provider.Updater.Update(entity);
					}
				}
			}
		}
	}
}
