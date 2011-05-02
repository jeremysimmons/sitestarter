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
			
			using (LogGroup logGroup = LogGroup.Start("Initializing a new data reader adapter.", NLog.LogLevel.Debug))
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
		/// Gets the names of the data stores in the data directory.
		/// </summary>
		/// <returns>The names of the data stores found.</returns>
		override public string[] GetDataStoreNames()
		{
			List<String> names = new List<String>();

			string path = DataDirectoryPath;
			
			if (Directory.Exists(path))
			{
				foreach (string file in Directory.GetFiles(path, "*.yap"))
				{
					string[] nameParts = Path.GetFileName(file).Split('.');
					string shortName = nameParts[0];
					names.Add(shortName);
				}
			}

			return names.ToArray();
		}

		/// <summary>
		/// Creates a filter for the active data source based on the specified type.
		/// </summary>
		public override IDataFilter CreateFilter(Type filterType)
		{
			using (LogGroup logGroup = LogGroup.Start("Creating filter", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Type: " + filterType.ToString());

				if (filterType.Equals(typeof(PropertyFilter)))
				{
					
					LogWriter.Debug("Filter type supported.");

					return new Db4oPropertyFilter();
				}
				else if (filterType.Equals(typeof(ReferenceFilter)))
				{
					LogWriter.Debug("Filter type supported.");

					return new Db4oReferenceFilter();
				}
				else
				{
					LogWriter.Debug("Creation failed. " + filterType.ToString() + " isn't a supported filter.");
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
			using (LogGroup logGroup = LogGroup.Start("Checking whether the provided entity has already been stored.", NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");

				LogWriter.Debug("Entity type: " + entity.GetType());

				if (EntitiesUtilities.IsReference(entity.GetType()))
				{
					LogWriter.Debug("Is reference == true");
					
					EntityIDReference reference = (EntityIDReference)entity;
					
					//if (reference.Type1Name == String.Empty)

					Type type = EntitiesUtilities.GetType(reference.Type1Name);
					Type type2 = EntitiesUtilities.GetType(reference.Type2Name);

					LogWriter.Debug("Type 1: " + type.ToString());
					LogWriter.Debug("Type 2: " + type2.ToString());
					
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
					LogWriter.Debug("Is reference != true");
					
					isStored = Stores[entity].IsStored(entity);
				}
			}
			
			LogWriter.Debug("Is stored: " + isStored.ToString());
			
			return isStored;
		}

		/// <summary>
		/// Disposes the data provider and all data stores within it.
		/// </summary>
		public override void Dispose()
		{
			using (LogGroup logGroup = LogGroup.Start("Disposing the data provider and data stores.", NLog.LogLevel.Debug))
			{
				// Dispose the batch first otherwise they'll try to commit the stores after the stores are disposed
				while (BatchState.IsRunning)
					BatchState.Batches.Pop();
				
				foreach (Db4oDataStore store in Stores)
				{
					LogWriter.Debug("Suspending store: " + store.Name);
					
					store.Dispose();
				}
			}
		}
		
		/// <summary>
		/// Disposes the data provider and suspends the data stores by moving them to a save location outside the application.
		/// Note: This is used during an update to safely clear the data stores and allow the updated data to be imported without conflicts.
		/// </summary>
		public override void Suspend()
		{
			using (LogGroup logGroup = LogGroup.Start("Suspending the data provider and data stores.", NLog.LogLevel.Debug))
			{
				// Dispose the data access system to unlock the files
				Dispose();
				
				// Create the path to the suspended directory
				string toDirectory = SuspendedDirectoryPath + Path.DirectorySeparatorChar
					+ DataAccess.Data.Schema.ApplicationVersion.ToString().Replace(".", "-");
				
				LogWriter.Debug("To directory: " + toDirectory);
				
				// Move each .yap file to the suspended directory.
				foreach (string file in Directory.GetFiles(DataDirectoryPath, "*.yap"))
				{
					LogWriter.Debug("Moving data store: " + file);
					
					string toFile = toDirectory + Path.DirectorySeparatorChar + Path.GetFileName(file);
					
					LogWriter.Debug("To: " + toFile);
					
					if (!Directory.Exists(Path.GetDirectoryName(toFile)))
						Directory.CreateDirectory(Path.GetDirectoryName(toFile));
					
					// If the to file already exists then delete it.
					// This should never actually occur in production but can occur during debugging
					if (File.Exists(toFile))
						File.Delete(toFile);
					
					File.Move(file, toFile);
				}
			}
		}
		
	}
}
