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
		private ISchemaEditor schema;
		public override ISchemaEditor Schema {
			get {
				if (schema == null)
					schema = new Db4oSchemaEditor();
				return schema;
			}
		}
		
		/*public override ISchemaEditor Schema
		{
			get {
				return Schema;
			}
		}*/
		
		/*private Db4oSchemaEditor schema;
		public Db4oSchemaEditor Schema
		{
			get {
				if (schema == null)
					schema = new Db4oSchemaEditor();
				return schema;
			}
		}*/
		
		private DataStoreCollection stores;
		public override DataStoreCollection Stores
		{
			get {
				if (stores == null)
					stores = new DataStoreCollection();
				return stores; }
		}
		
		#region Adapter functions
		public override IDataReader InitializeDataReader()
		{
			Db4oDataReader reader = null;
		
			using (LogGroup logGroup = AppLogger.StartGroup("Initializing a new data reader adapter.", NLog.LogLevel.Debug))
			{
				reader = new Db4oDataReader(this, null);
			}
			
			return reader;
		}
		
		public override IDataIndexer InitializeDataIndexer()
		{
			return new Db4oDataIndexer(this, null);
		}
		
		public override IDataReferencer InitializeDataReferencer()
		{
			return new Db4oDataReferencer(this, null);
		}
		
		public override IDataActivator InitializeDataActivator()
		{
			return new Db4oDataActivator(this, null);
		}
		public override IDataSaver InitializeDataSaver()
		{
			return new Db4oDataSaver(this, null);
		}
		public override IDataDeleter InitializeDataDeleter()
		{
			return new Db4oDataDeleter(this, null);
		}
		public override IDataUpdater InitializeDataUpdater()
		{
			return new Db4oDataUpdater(this, null);
		}
		
		#endregion

		public void Initialize()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void Dispose()
		{


			foreach (Db4oDataStore store in Stores)
			{
				// Includes commit and close
				store.Dispose();
			}
		}

		public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
		{
			DataAccess.Data = this;

			base.Initialize(name, config);

			/*if ((config == null) || (config.Count == 0))
                throw new ArgumentNullException("You must supply a valid configuration dictionary.");

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Put a localized description here.");
            }

            //Let ProviderBase perform the basic initialization
            base.Initialize(name, config);

            //Perform feature-specific provider initialization here

            //Get the connection string
            string connectionStringName = config["connectionStringName"];
            if (String.IsNullOrEmpty(connectionStringName))
                throw new ProviderException("You must specify a connectionStringName attribute.");

            ConnectionStringsSection cs =
                (ConnectionStringsSection)ConfigurationManager.GetSection("connectionStrings");
            if (cs == null)
                throw new ProviderException("An error occurred retrieving the connection strings section.");

            if (cs.ConnectionStrings[connectionStringName] == null)
                throw new ProviderException("The connection string could not be found in the connection strings section.");
            else
                connectionString = cs.ConnectionStrings[connectionStringName].ConnectionString;

            if (String.IsNullOrEmpty(connectionString))
                throw new ProviderException("The connection string is invalid.");
            config.Remove("connectionStringName");

            //Check to see if unexpected attributes were set in configuration
            if (config.Count > 0)
            {
                string extraAttribute = config.GetKey(0);
                if (!String.IsNullOrEmpty(extraAttribute))
                    throw new ProviderException("The following unrecognized attribute was found in " + Name + "'s configuration: '" +
                                                extraAttribute + "'");
                else
                    throw new ProviderException("An unrecognized attribute was found in the provider's configuration.");
            }*/
		}

		public override IDataStore InitializeDataStore(string dataStoreName)
		{
			return Db4oDataStoreFactory.InitializeDataStore(dataStoreName);
		}

		/*/// <summary>
		/// Gets the name of the data store that the provided entity is stored in.
		/// </summary>
		/// <param name="type">The type of entity to get the name of the data store for.</param>
		/// <param name="entity">The entity to get the name of the data store for (in case it's not found for the type).</param>
		/// <param name="throwErrorIfNotFound">A flag indicating whether an error should be thrown when no data store attribute is found.</param>
		/// <returns>The data store that the provided entity is stored in.</returns>
		public string GetDataStoreName(Type type, IEntity entity, bool throwErrorIfNotFound)
		{
			string dataStoreName = GetDataStoreName(type, false);

			if (dataStoreName == null || dataStoreName.Length == 0)
				return GetDataStoreName(entity.GetType(), throwErrorIfNotFound);

			return String.Empty;
		}*/



			/// <summary>
			/// Gets the names of the data stores in the data directory.
			/// </summary>
			/// <returns>The names of the data stores found.</returns>
			override public string[] GetDataStoreNames()
		{
			List<String> names = new List<String>();

			foreach (string file in Directory.GetFiles(Config.Application.PhysicalPath + @"\App_Data\", "*.yap"))
			{
				names.Add(Path.GetFileNameWithoutExtension(file));
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

	}
}
