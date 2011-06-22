using System;
using System.IO;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Used to initialize the entity state and make entities available for use.
	/// </summary>
	public class EntityInitializer
	{
		private EntityFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create entity file names/paths.
		/// </summary>
		public EntityFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new EntityFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		private EntitySaver saver;
		/// <summary>
		/// Gets/sets the entity saver used to save entities to file.
		/// </summary>
		public EntitySaver Saver
		{
			get {
				if (saver == null)
				{
					saver = new EntitySaver();
					if (EntitiesDirectoryPath != null && EntitiesDirectoryPath != String.Empty)
						saver.EntitiesDirectoryPath = EntitiesDirectoryPath;
				}
				return saver; }
			set { saver = value; }
		}
		
		private EntityScanner scanner;
		/// <summary>
		/// Gets/sets the entity scanner used to find available entities in the existing assemblies.
		/// </summary>
		public EntityScanner Scanner
		{
			get {
				if (scanner == null)
				{
					scanner = new EntityScanner();
				}
				return scanner; }
			set { scanner = value; }
		}
		
		private EntityLoader loader;
		/// <summary>
		/// Gets/sets the entity loader used to find available entities in the existing assemblies.
		/// </summary>
		public EntityLoader Loader
		{
			get {
				if (loader == null)
				{
					loader = new EntityLoader();
					if (EntitiesDirectoryPath != null && EntitiesDirectoryPath != String.Empty)
						loader.EntitiesDirectoryPath = EntitiesDirectoryPath;
				}
				return loader; }
			set { loader = value; }
		}
		
		/// <summary>
		/// Gets a value indicating whether the entities have been mapped yet.
		/// </summary>
		public bool IsMapped
		{
			get {
				bool isMapped = EntityMappingsExist();
				return isMapped; }
		}
		
		/// <summary>
		/// Gets the full path to the directory containing entity mappings.
		/// </summary>
		public string EntitiesDirectoryPath
		{
			get { return FileNamer.EntitiesInfoDirectoryPath; }
		}
		
		public EntityInitializer()
		{
		}		
		
		/// <summary>
		/// Initializes the entities and loads all entities to state.
		/// </summary>
		/// <param name="entities">The entities to initialize.</param>
		public void Initialize(EntityInfo[] entities)
		{
			EntityState.Entities = new EntityStateCollection(entities);
		}
		
		/// <summary>
		/// Initializes the entities and loads all entities to state.
		/// </summary>
		public void Initialize()
		{
			using (LogGroup logGroup = LogGroup.Start("Initializing the business entities.", LogLevel.Debug))
			{
				EntityInfo[] entities = new EntityInfo[]{};
				if (IsMapped)
				{
					LogWriter.Debug("Is mapped. Loading from XML.");
					
					entities = LoadEntities();
				}
				else
				{
					LogWriter.Debug("Is not mapped. Scanning from type attributes.");
					
					entities = FindEntities();
					SaveToFile(entities);
				}
				
				Initialize(entities);
			}
		}
		
		/// <summary>
		/// Saves the mappings for the provided entities to entities mappings directory.
		/// </summary>
		/// <param name="entities">The entities to save to file.</param>
		public void SaveToFile(EntityInfo[] entities)
		{
			using (LogGroup logGroup = LogGroup.Start("Saving the provided entities to XML.", LogLevel.Debug))
			{
				foreach (EntityInfo entity in entities)
				{
					Saver.SaveToFile(entity);
				}
			}
		}
		
		
		/// <summary>
		/// Loads the available entities from file.
		/// </summary>
		/// <returns>The loaded from the entities mappings directory.</returns>
		public EntityInfo[] LoadEntities()
		{
			return Loader.LoadFromDirectory();
		}
		
		/// <summary>
		/// Finds all the entities available to the application.
		/// </summary>
		/// <returns>An array of the available entities.</returns>
		public EntityInfo[] FindEntities()
		{
			return Scanner.FindEntities();
		}
		
		/// <summary>
		/// Checks whether the entity mappings have been created and saved to file.
		/// </summary>
		/// <returns>A value indicating whether the entity mappings directory was found.</returns>
		public bool EntityMappingsExist()
		{
			string directory = EntitiesDirectoryPath;
			
			return (Directory.Exists(directory) && Directory.GetFiles(directory).Length > 0);
		}
	}
	
}
