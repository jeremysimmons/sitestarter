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

namespace SoftwareMonkeys.SiteStarter.Data.Db4o
{
	public class Db4oDataProvider : DataProvider
	{
		private DataStoreCollection stores;
		public override DataStoreCollection Stores
		{
			get {
				if (stores == null)
					stores = new DataStoreCollection();
				return stores; }
		}

		public void Initialize()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void Dispose()
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
				else
				{
					AppLogger.Debug("Creation failed. " + filterType.ToString() + " isn't a supported filter.");
					throw new NotSupportedException(filterType.ToString() + " isn't yet supported.");
				}
			}
		}

		#region Data access functions
		/// <summary>
		/// Retrieves all the entities of the specified type from the data store.
		/// </summary>
		/// <param name="filter">The filter to apply to the query.</param>
		/// <returns>The entities of the specified type found in the data store.</returns>
		public override IEntity[] GetEntities(IDataFilter filter)
		{
			List<IEntity> entities = new List<IEntity>();

			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving entities by type and filter.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Filter type: " + filter.GetType().ToString());
				
				foreach (Type type in filter.Types)
				{
					
					AppLogger.Debug("Includes type: " + type.ToString());
					entities.AddRange(((Db4oDataStore)Stores[type]).ObjectContainer.Query<IEntity>(delegate(IEntity entity)
					                                                                               {
					                                                                               	return IsMatch(entity, filter);
					                                                                               	
					                                                                               }));
				}

				foreach (IEntity entity in entities)
				{
					using (LogGroup logGroup2 = AppLogger.StartGroup("Entity found.", NLog.LogLevel.Debug))
					{
						//IEntity entity = (IEntity)os.Next();
						AppLogger.Debug("Entity ID: " + entity.ID);
						AppLogger.Debug("Entity .ToString(): " + entity.ToString());
					}
				}

				if (entities.Count == 0)
					AppLogger.Debug("No entities retrieved.");
			}

			return (IEntity[])entities.ToArray();
		}

		/// <summary>
		/// Retrieves all the entities of the specified type from the data store.
		/// </summary>
		/// <param name="group">The group of filters to apply to the query.</param>
		/// <returns>The entities of the specified type found in the data store.</returns>
		public override IEntity[] GetEntities(FilterGroup group)
		{
			List<IEntity> entities = new List<IEntity>();

			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving entities by type and filter.", NLog.LogLevel.Debug))
			{

				if (group != null && group.Filters != null)
				{

					AppLogger.Debug("Group operator: " + group.Operator.ToString());

					List<Type> allTypes = new List<Type>();

					foreach (IDataFilter filter in group.Filters)
					{
						if (filter.Types != null)
							allTypes.AddRange(filter.Types);
					}

					foreach (Type type in allTypes)
					{

						AppLogger.Debug("Includes type: " + type.ToString());

						Db4oDataStore store = (Db4oDataStore)Stores[type];


						entities.AddRange(store.ObjectContainer.Query<IEntity>(delegate(IEntity entity)
						                                                       {
						                                                       	return IsMatch(entity, group);

						                                                       }));
					}

					foreach (IEntity entity in entities)
					{
						using (LogGroup logGroup2 = AppLogger.StartGroup("Entity found.", NLog.LogLevel.Debug))
						{
							//IEntity entity = (IEntity)os.Next();
							AppLogger.Debug("Entity ID: " + entity.ID);
							AppLogger.Debug("Entity .ToString(): " + entity.ToString());
						}
					}

					if (entities.Count == 0)
						AppLogger.Debug("No entities retrieved.");
				}
			}

			return (IEntity[])entities.ToArray();
		}
		
		/// <summary>
		/// Retrieves the entities of the specified type with the specified IDs from the data store.
		/// </summary>
		/// <param name="type">The entity type to retrieve.</param>
		/// <param name="ids">The IDs of the entities to retrieve.</param>
		/// <returns>The entities of the specified type with the specified IDs.</returns>
		public override IEntity[] GetEntities(Type type, Guid[] ids)
		{
			return (IEntity[])Reflector.InvokeGenericMethod(this, "GetEntities",
			                                              new Type[] {type},
			                                              new object[] {ids});
		}
		
		
		/// <summary>
		/// Retrieves the entity of the specified type with the specified IDs from the data store.
		/// </summary>
		/// <param name="ids">The IDs of the entities to retrieve.</param>
		/// <returns>The entity of the specified type with the specified IDs.</returns>
		public override T[] GetEntities<T>(Guid[] ids)
		{
			return (T[])Stores[typeof(T)].GetEntities<T>(ids);
		}
		
		/// <summary>
		/// Retrieves the entities of the specified type from the data store.
		/// </summary>
		/// <param name="type">The entity type to retrieve.</param>
		/// <returns>The entities of the specified type found in the data store.</returns>
		public override IEntity[] GetEntities(Type type)
		{
			return (IEntity[])Reflector.InvokeGenericMethod(this, "GetEntities",
			                                              new Type[] {type},
			                                              new object[] {});
		}
		
		/// <summary>
		/// Retrieves the entities of the specified type from the data store.
		/// </summary>
		/// <returns>The entities of the specified type found in the data store.</returns>
		public override T[] GetEntities<T>()
		{
			return (T[])Stores[typeof(T)].GetEntities<T>();
		}

		/// <summary>
		/// Retrieves the entity of the specified type from the data store.
		/// </summary>
		/// <param name="propertyName">Name of the property to match.</param>
		/// <param name="propertyValue">The value of the property to match.</param>
		/// <returns>The entity of the specified type found in the data store.</returns>
		public override IEntity GetEntity(Type type, string propertyName, object propertyValue)
		{
			return (IEntity)Reflector.InvokeGenericMethod(this, "GetEntity",
			                                              new Type[] {type},
			                                              new object[] {propertyName, propertyValue});
		}
		
		/// <summary>
		/// Retrieves the entity of the specified type from the data store.
		/// </summary>
		/// <param name="propertyName">Name of the property to match.</param>
		/// <param name="propertyValue">The value of the property to match.</param>
		/// <returns>The entity of the specified type found in the data store.</returns>
		public override T GetEntity<T>(string propertyName, object propertyValue)
		{
			T[] entities = GetEntities<T>(propertyName, propertyValue);

			if (entities == null || entities.Length == 0)
				return default(T);
			else
				return entities[0];
		}
		
		/// <summary>
		/// Retrieves the entities of the specified type from the data store.
		/// </summary>
		/// <param name="propertyName">Name of the property to match.</param>
		/// <param name="propertyValue">The value of the property to match.</param>
		/// <returns>The entities of the specified type found in the data store.</returns>
		public override IEntity[] GetEntities(Type type, string propertyName, object propertyValue)
		{
			return (IEntity[])Reflector.InvokeGenericMethod(this, "GetEntities",
			                                              new Type[] {type},
			                                              new object[] {propertyName, propertyValue});
		}
		
		/// <summary>
		/// Retrieves all the entities of the specified type from the data store.
		/// </summary>
		/// <param name="propertyName">Name of the property to match.</param>
		/// <param name="propertyValue">The value of the property to match.</param>
		/// <returns>The entities of the specified type found in the data store.</returns>
		public override T[] GetEntities<T>(string propertyName, object propertyValue)
		{
			return (T[])Stores[typeof(T)].GetEntities<T>(propertyName, propertyValue);
		}

		/// <summary>
		/// Retrieves all entities that contain a reverse reference to the specified entity.
		/// </summary>
		/// <returns>The entities containing a reverse reference.</returns>
		public override IEntity[] GetEntitiesContainingReverseReferences(IEntity entity, PropertyInfo property)
		{
			List<IEntity> entities = null;
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving entities containing reverse references.", NLog.LogLevel.Debug))
			{
				//PropertyInfo property = DataUtilities.GetProperty(entity, tity.GetType().GetProperty(propertyName);

				AppLogger.Debug("Entity type: " + entity.GetType().ToString());
				AppLogger.Debug("Entity ID: " + entity.ID);
				AppLogger.Debug("Property name: " + property.Name);
				AppLogger.Debug("Property/reference type: " + property.PropertyType.ToString());
				
				Type referenceEntityType = DataUtilities.GetReferenceType(entity, property);
				AppLogger.Debug("Reference entity type: " + referenceEntityType.ToString());

				AppLogger.Debug("Looking for any entities with the above ID found on the reverse/mirror property of the referenced entity.");
				
				entities = new List<IEntity>(((Db4oDataStore)Stores[referenceEntityType]).ObjectContainer.Query<IEntity>(delegate(IEntity e)
				                                                                                                         {
				                                                                                                         	bool isMatch = false;

				                                                                                                         	using (LogGroup logGroup2 = AppLogger.StartGroup("Querying data store: " + Stores[referenceEntityType].Name, NLog.LogLevel.Debug))
				                                                                                                         	{
				                                                                                                         		AppLogger.Debug("Checking entity - Type: " + e.GetType());
				                                                                                                         		AppLogger.Debug("Checking entity - ID: " + e.ID.ToString());

				                                                                                                         		if (referenceEntityType.Equals(e.GetType()) || referenceEntityType.IsSubclassOf(e.GetType()))
				                                                                                                         		{
				                                                                                                         			AppLogger.Debug("Types match.");

				                                                                                                         			PropertyInfo mirrorProperty = e.GetType().GetProperty(DataUtilities.GetMirrorPropertyName(property));

				                                                                                                         			object mirrorValue = mirrorProperty.GetValue(e, null);

				                                                                                                         			AppLogger.Debug("Checking entity - Mirror type: " + mirrorValue.GetType().ToString());

				                                                                                                         			AppLogger.Debug("Checking entity - Mirror value: " + mirrorValue.ToString());

				                                                                                                         			if (mirrorValue != null)
				                                                                                                         			{
				                                                                                                         				if (mirrorValue is Guid)
				                                                                                                         				{
				                                                                                                         					isMatch = (Guid)mirrorValue == entity.ID;
				                                                                                                         					if (isMatch)
				                                                                                                         						AppLogger.Debug("IDs match.");
				                                                                                                         					else
				                                                                                                         						AppLogger.Debug("IDs don't match.");
				                                                                                                         					
				                                                                                                         				}
				                                                                                                         				else
				                                                                                                         				{
				                                                                                                         					isMatch = Array.IndexOf((Guid[])mirrorValue, entity.ID) > -1;
				                                                                                                         					
				                                                                                                         					if (isMatch)
				                                                                                                         						AppLogger.Debug("ID found in reference array.");
				                                                                                                         					else
				                                                                                                         						AppLogger.Debug("ID not found in reference array.");
				                                                                                                         				}
				                                                                                                         			}
				                                                                                                         			else
				                                                                                                         				AppLogger.Debug("Mirror value == null");
				                                                                                                         		}
				                                                                                                         		else
				                                                                                                         			AppLogger.Debug("Entity type '" + e.GetType() + "' does not match reference entity type '" + referenceEntityType.ToString());

				                                                                                                         		if (isMatch)
				                                                                                                         			AppLogger.Debug("Match found.");
				                                                                                                         		else
				                                                                                                         			AppLogger.Debug("Match failed.");

				                                                                                                         	}

				                                                                                                         	return isMatch;
				                                                                                                         }));
			}
			return (IEntity[])entities.ToArray();

		}

		public override void Save(IEntity entity)
		{
			Stores[entity.GetType()].Save(entity);
		}

		public override void Update(IEntity entity)
		{
			Stores[entity.GetType()].Update(entity);
		}

		public override void Delete(IEntity entity)
		{
			Stores[entity.GetType()].Delete(entity);
		}

		#endregion

		#region Filter matching functions
		static public bool IsMatch(IEntity entity, IDataFilter filter)
		{
			bool isMatch = false;

			using (LogGroup logGroup2 = AppLogger.StartGroup("Checking whether the provided entity and filter match.", NLog.LogLevel.Debug))
			{
				isMatch = filter.IsMatch(entity);

				AppLogger.Debug("Is match? " + isMatch.ToString());
			}
			
			return isMatch;
		}

		static public bool IsMatch(IEntity entity, FilterGroup filterGroup)
		{

			bool allMatch = true;
			bool anyMatch = false;

			foreach (IDataFilter filter in filterGroup.Filters)
			{
				if (filter.IsMatch(entity))
				{
					anyMatch = true;
				}
				else
				{
					allMatch = false;
				}
			}

			if (filterGroup.Operator == FilterOperator.Or)
				return anyMatch;
			else
				return allMatch;
		}
		#endregion
	}
}
