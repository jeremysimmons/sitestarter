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
				bool isMapped = EntityCacheExists();
				return isMapped; }
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
		/// Initializes the entities and loads all entities to state. Note: Skips initialization if already initialized.
		/// </summary>
		public void Initialize()
		{
			Initialize(false);
		}
		
		/// <summary>
		/// Initializes the entities and loads all entities to state. Note: Skips initialization if already initialized.
		/// </summary>
		public void Initialize(bool includeTestEntities)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Initializing the business entities."))
			{
				if (!EntityState.IsInitialized)
				{
					EntityInfo[] entities = new EntityInfo[]{};
					if (IsMapped)
					{
						LogWriter.Debug("Is cached. Loading from XML.");
						
						entities = LoadEntities();
					}
					else
					{
						LogWriter.Debug("Is not cached. Scanning from type attributes.");
						
						entities = FindEntities(includeTestEntities);
						SaveToFile(entities);
					}
					
					Initialize(entities);
				}
				else
					LogWriter.Debug("Already initialized.");
			}
		}
		
		/// <summary>
		/// Saves the mappings for the provided entities to entities mappings directory.
		/// </summary>
		/// <param name="entities">The entities to save to file.</param>
		public void SaveToFile(EntityInfo[] entities)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Saving the provided entities to XML."))
			{
				Saver.SaveToFile(entities);
			}
		}
		
		
		/// <summary>
		/// Loads the available entities from file.
		/// </summary>
		/// <returns>The loaded from the entities mappings directory.</returns>
		public EntityInfo[] LoadEntities()
		{
			return Loader.LoadInfoFromFile();
		}
		
		/// <summary>
		/// Finds all the entities available to the application.
		/// </summary>
		/// <param name="includeTestEntities">A flag indicating whether to find test/mock entities as well.</param>
		/// <returns>An array of the available entities.</returns>
		public EntityInfo[] FindEntities(bool includeTestEntities)
		{
			return Scanner.FindEntities(includeTestEntities);
		}
		
		/// <summary>
		/// Checks whether the entity cache has been created and saved to file.
		/// </summary>
		/// <returns>A value indicating whether the entity cache file was found.</returns>
		public bool EntityCacheExists()
		{
			string file = FileNamer.EntitiesInfoFilePath;
			
			return File.Exists(file);
		}
	}
	
}
