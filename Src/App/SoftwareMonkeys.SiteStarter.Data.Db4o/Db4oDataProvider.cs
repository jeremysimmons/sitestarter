using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Entities;
using System.IO;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Reflection;
using Db4objects.Db4o.Query;
using Db4objects.Db4o;
using Db4objects.Db4o.Collections;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o
{
	public class Db4oDataProvider : DataProvider
	{
		private DataStoreCollection stores;
		/// <summary>
		/// Gets/sets a collection of the available data stores.
		/// </summary>
		public override DataStoreCollection Stores
		{
			get {
				if (stores == null)
					stores = new DataStoreCollection();
				return stores; }
		}
		
		#region Adapter functions
		
		/// <summary>
		/// Initializes the data reader adapter for this provider.
		/// </summary>
		/// <returns>The data reader adapter for this provider.</returns>
		public override IDataReader InitializeDataReader()
		{
			Db4oDataReader reader = null;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Initializing a new data reader adapter.", NLog.LogLevel.Debug))
			{
				reader = new Db4oDataReader(this, null);
			}
			
			return reader;
		}
		
		/// <summary>
		/// Initializes the data indexer adapter for this provider.
		/// </summary>
		/// <returns>The data indexer adapter for this provider.</returns>
		public override IDataIndexer InitializeDataIndexer()
		{
			return new Db4oDataIndexer(this, null);
		}
		
		/// <summary>
		/// Initializes the data referencer adapter for this provider.
		/// </summary>
		/// <returns>The data referencer adapter for this provider.</returns>
		public override IDataReferencer InitializeDataReferencer()
		{
			return new Db4oDataReferencer(this, null);
		}
		
		/// <summary>
		/// Initializes the data activator adapter for this provider.
		/// </summary>
		/// <returns>The data activator adapter for this provider.</returns>
		public override IDataActivator InitializeDataActivator()
		{
			return new Db4oDataActivator(this, null);
		}
		
		/// <summary>
		/// Initializes the data saver adapter for this provider.
		/// </summary>
		/// <returns>The data saver adapter for this provider.</returns>
		public override IDataSaver InitializeDataSaver()
		{
			return new Db4oDataSaver(this, null);
		}
		
		/// <summary>
		/// Initializes the data deleter adapter for this provider.
		/// </summary>
		/// <returns>The data deleter adapter for this provider.</returns>
		public override IDataDeleter InitializeDataDeleter()
		{
			return new Db4oDataDeleter(this, null);
		}
		
		/// <summary>
		/// Initializes the data deleter adapter for this provider.
		/// </summary>
		/// <returns>The data deleter adapter for this provider.</returns>
		public override IDataUpdater InitializeDataUpdater()
		{
			return new Db4oDataUpdater(this, null);
		}
		
		/// <summary>
		/// Initializes the data exporter adapter for this provider.
		/// </summary>
		/// <returns>The data exporter adapter for this provider.</returns>
		public override IDataExporter InitializeDataExporter()
		{
			return new Db4oDataExporter(this, null);
		}
		
		/// <summary>
		/// Initializes the data schema adapter for this provider.
		/// </summary>
		/// <returns>The data schema adapter for this provider.</returns>
		public override IDataSchema InitializeDataSchema()
		{
			return new Db4oDataSchema(this, null);
		}
		
		/// <summary>
		/// Initializes the data importer adapter for this provider.
		/// </summary>
		/// <returns>The data importer adapter for this provider.</returns>
		public override IDataImporter InitializeDataImporter()
		{
			return new Db4oDataImporter(this, null);
		}
		
		
		#endregion

		/// <summary>
		/// Initializes the provided with the provided details.
		/// </summary>
		/// <param name="name">The name of the provider.</param>
		/// <param name="config">The configuration settings that apply to the provider.</param>
		public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
		{
			DataAccess.Data = this;

			base.Initialize(name, config);
		}

		/// <summary>
		/// Creates and initilizes the data store with the provided name.
		/// </summary>
		/// <param name="dataStoreName">The name of the data store to initialize.</param>
		/// <returns>The newly initialized data store.</returns>
		public override IDataStore InitializeDataStore(string dataStoreName)
		{
			IDataStore store = Db4oDataStoreFactory.InitializeDataStore(dataStoreName);
			return store;
		}

		/// <summary>
		/// Creates and initilizes the data store with the provided name.
		/// </summary>
		/// <param name="virtualServerID">The ID of the virtual server containing the data store.</param>
		/// <param name="dataStoreName">The name of the data store to initialize.</param>
		/// <returns>The newly initialized data store.</returns>
		public override IDataStore InitializeDataStore(string virtualServerID, string dataStoreName)
		{
			IDataStore store = Db4oDataStoreFactory.InitializeDataStore(virtualServerID, dataStoreName);
			return store;
		}
		
		/// <summary>
		/// Gets the names of the data stores in the data directory.
		/// </summary>
		/// <returns>The names of the data stores found.</returns>
		override public string[] GetDataStoreNames()
		{
			List<String> names = new List<String>();

			string path = Config.Application.PhysicalApplicationPath + Path.DirectorySeparatorChar + "App_Data";
			
			if (Directory.Exists(path))
			{
				foreach (string file in Directory.GetFiles(path, "*.yap"))
				{
					names.Add(Path.GetFileNameWithoutExtension(file));
				}
			}

			return names.ToArray();
		}

		/// <summary>
		/// Creates a filter for the active data source based on the specified type.
		/// </summary>
		public override IDataFilter CreateFilter(Type filterType)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Creating filter", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Type: " + filterType.ToString());

				if (filterType.Equals(typeof(PropertyFilter)))
				{
					
					AppLogger.Debug("Filter type supported.");

					return new Db4oPropertyFilter();
				}
				else if (filterType.Equals(typeof(ReferenceFilter)))
				{
					AppLogger.Debug("Filter type supported.");

					return new Db4oReferenceFilter();
				}
				else
				{
					AppLogger.Debug("Creation failed. " + filterType.ToString() + " isn't a supported filter.");
					throw new NotSupportedException(filterType.ToString() + " isn't yet supported.");
				}
			}
		}
		
		/// <summary>
		/// Checks whether the provided entity is stored in the corresponding data store.
		/// </summary>
		/// <param name="entity">The entity to look for in the corresponding data store.</param>
		/// <returns>A boolean value indicating whether the entity was found.</returns>
		public override bool IsStored(IEntity entity)
		{
			bool isStored = false;
			using (LogGroup logGroup = AppLogger.StartGroup("Checking whether the provided entity has already been stored.", NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");

				AppLogger.Debug("Entity type: " + entity.GetType());

				if (EntitiesUtilities.IsReference(entity.GetType()))
				{
					AppLogger.Debug("Is reference == true");
					
					EntityIDReference reference = (EntityIDReference)entity;
					
					//if (reference.Type1Name == String.Empty)

					Type type = EntitiesUtilities.GetType(reference.Type1Name);
					Type type2 = EntitiesUtilities.GetType(reference.Type2Name);

					AppLogger.Debug("Type 1: " + type.ToString());
					AppLogger.Debug("Type 2: " + type2.ToString());
					
					isStored = Referencer.MatchReference(
						type,
						reference.Entity1ID,
						reference.Property1Name,
						type2,
						reference.Entity2ID,
						reference.Property2Name);
				}
				else
				{
					AppLogger.Debug("Is reference != true");
					
					isStored = Stores[entity].IsStored(entity);
				}
			}
			
			AppLogger.Debug("Is stored: " + isStored.ToString());
			
			return isStored;
		}

		/// <summary>
		/// Disposes the data provider and all data stores within it.
		/// </summary>
		public override void Dispose()
		{
			foreach (Db4oDataStore store in Stores)
			{
				// Includes commit and close
				store.Dispose();
			}
		}
		
	}
}
