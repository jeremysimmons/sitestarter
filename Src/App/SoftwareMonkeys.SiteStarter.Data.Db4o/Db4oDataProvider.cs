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
					
					isStored = MatchReference(
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
					
					isStored = Stores[entity.GetType()].IsStored(entity);
				}
			}
			
			AppLogger.Debug("Is stored: " + isStored.ToString());
			
			return isStored;
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
		/// Retrieves the entity matching the filter .
		/// </summary>
		/// <param name="filter">The filter to apply to the query.</param>
		/// <returns>The matching entity.</returns>
		public override IEntity GetEntity(IDataFilter filter)
		{
			IEntity[] entities = GetEntities(filter);
			
			if (entities != null && entities.Length > 0)
				return entities[0];
			else
				return null;
		}
		
		/// <summary>
		/// Retrieves the entity matching the filter group.
		/// </summary>
		/// <param name="group">The group of filters to apply to the query.</param>
		/// <returns>The matching entity.</returns>
		public override IEntity GetEntity(FilterGroup group)
		{
			IEntity[] entities = GetEntities(group);
			
			if (entities != null && entities.Length > 0)
				return entities[0];
			else
				return null;
		}

		/// <summary>
		/// Retrieves all the entities matching the filter group.
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

					// Loop through the types and load them
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
			IEntity[] entities = Stores[type].GetEntitiesByTypeAndProperty(type, propertyName, propertyValue);

			if (entities == null || entities.Length == 0)
				return null;
			else
				return entities[0];
		}
		
		/// <summary>
		/// Retrieves the entity of the specified type from the data store.
		/// </summary>
		/// <param name="propertyName">Name of the property to match.</param>
		/// <param name="propertyValue">The value of the property to match.</param>
		/// <returns>The entity of the specified type found in the data store.</returns>
		public override T GetEntity<T>(string propertyName, object propertyValue)
		{
			T[] entities = Stores[typeof(T)].GetEntities<T>(propertyName, propertyValue);

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
			return (IEntity[])Stores[type].GetEntitiesByTypeAndProperty(type, propertyName, propertyValue);
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
		/// Retrieves the entity of the specified type matching the specified values.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns></returns>
		public override IEntity GetEntity(Type type, IDictionary<string, object> parameters)
		{
			
			IEntity[] entities = Stores[type].GetEntitiesByTypeAndProperties(type, parameters);
			if (entities == null || entities.Length == 0)
				return null;
			else
				return entities[0];
		}
		
		/// <summary>
		/// Retrieves all the entities of the specified type matching the specified values.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns></returns>
		public override IEntity[] GetEntities(Type type, IDictionary<string, object> parameters)
		{
			return Stores[type].GetEntitiesByTypeAndProperties(type, parameters);
		}
		
		/// <summary>
		/// Retrieves the entity of the specified type matching the specified values.
		/// </summary>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns></returns>
		public override T GetEntity<T>(IDictionary<string, object> parameters)
		{
			T[] entities = Stores[typeof(T)].GetEntities<T>(parameters);
			if (entities == null || entities.Length == 0)
				return default(T);
			else
				return entities[0];
		}
		
		/// <summary>
		/// Retrieves all the entities of the specified type matching the specified values.
		/// </summary>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns></returns>
		public override T[] GetEntities<T>(IDictionary<string, object> parameters)
		{
			return Collection<T>.ConvertAll(Stores[typeof(T)].GetEntitiesByTypeAndProperties(typeof(T), parameters));
		}

		/*/// <summary>
		/// Retrieves all entities that contain a reverse reference to the specified entity.
		/// </summary>
		/// <returns>The entities containing a reverse reference.</returns>
		public override IEntity[] GetEntitiesContainingReverseReferences(IEntity entity, PropertyInfo property)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			if (property == null)
				throw new ArgumentNullException("property");
			
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

				                                                                                                         		if (referenceEntityType.Equals(e.GetType())
				                                                                                                         		    || referenceEntityType.IsSubclassOf(e.GetType())
				                                                                                                         		    || referenceEntityType.ToString() == e.GetType().ToString())
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

		}*/
			
			
			/// <summary>
			/// Retrieves the specified page of objects from the data store.
			/// </summary>
			/// <param name="type">The type of entities to retrieve.</param>
			/// <param name="location"></param>
			/// <param name="sortExpression">The sort expression to apply before retrieving the page.</param>
			/// <returns>An array of the objects retrieved.</returns>
			public override IEntity[] GetEntitiesPage(Type type, PagingLocation location, string sortExpression)
		{
			return Stores[type].GetEntitiesPage(type, location, sortExpression);
		}

		/// <summary>
		/// Retrieves the specified page of objects from the provided IObjectSet.
		/// </summary>
		/// <param name="type">The type of entities to retrieve.</param>
		/// <param name="fieldName">The name of the field to query for.</param>
		/// <param name="propertyValue">The value of the field to query for.</param>
		/// <param name="location"></param>
		/// <param name="sortExpression">The sort expression to apply before retrieving the page.</param>
		/// <returns>An array of the objects retrieved.</returns>
		public override IEntity[] GetEntitiesPage(Type type, string fieldName, object propertyValue, PagingLocation location, string sortExpression)
		{
			return Stores[type].GetEntitiesPage(type, fieldName, propertyValue, location, sortExpression);
		}
		
		/*/// <summary>
		/// Retrieves the specified page of objects from the data store.
		/// </summary>
		/// <param name="pageIndex">The index of the page to retrieve.</param>
		/// <param name="pageSize">The size of each page.</param>
		/// <param name="sortExpression">The sort expression to apply before retrieving the page.</param>
		/// <param name="totalObjects">The total number of objects found.</param>
		/// <returns>An array of the objects retrieved.</returns>
		public override T[] GetEntitiesPage<T>(int pageIndex, int pageSize, string sortExpression, out int totalObjects)
		{
			return Stores[typeof(T)].GetEntitiesPage<T>(pageIndex,pageSize, sortExpression, out totalObjects);
		}*/
			
			/*public override T[] GetEntitiesPage<T>(string fieldName, object propertyValue, int pageIndex, int pageSize, string sortExpression, out int totalObjects)
		{
			return GetEntitiesPage<T>(fieldName, propertyValue, pageIndex, pageSize, sortExpression, totalObjects, new object[]{});
		}*/

			/*/// <summary>
			/// Retrieves the specified page of objects from the provided IObjectSet.
			/// </summary>
			/// <param name="fieldName">The name of the field to query for.</param>
			/// <param name="propertyValue">The value of the field to query for.</param>
			/// <param name="pageIndex">The index of the page to retrieve.</param>
			/// <param name="pageSize">The size of each page.</param>
			/// <param name="sortExpression">The sort expression to apply before retrieving the page.</param>
			/// <param name="totalObjects">The total number of objects found.</param>
			/// <returns>An array of the objects retrieved.</returns>
			public override T[] GetEntitiesPage<T>(string fieldName, object propertyValue, int pageIndex, int pageSize, string sortExpression, out int totalObjects)
		{
			return Stores[typeof(T)].GetEntitiesPage<T>(fieldName, propertyValue, pageIndex,pageSize, sortExpression, out totalObjects);
		}*/
			
			public override T[] GetEntitiesPage<T>(string fieldName, object propertyValue, PagingLocation location, string sortExpression)
		{
			return Stores[typeof(T)].GetEntitiesPage<T>(fieldName, propertyValue, location, sortExpression);
		}
		
		public override T[] GetEntitiesPage<T>(PagingLocation location, string sortExpression)
		{
			return Stores[typeof(T)].GetEntitiesPage<T>(location, sortExpression);
		}
		
		// TODO: Clean up
		/*public override T[] GetEntitiesPage<T>(PagingLocation location, string sortExpression, params object[] parameters)
		{
			//int total = 0;
			//T[] entities =  GetEntitiesPage<T>(location, sortExpression, parameters,);
			//location.AbsoluteTotal = total;
			return DataStores[typeof(T)].GetEntitiesPage<T>(location, sortExpression, parameters);entities;
		}*/
		/*
		public override IEntity[] GetEntitiesPage(Type type, string fieldName, object propertyValue, PagingLocation location, string sortExpression)
		{
			int total = 0;
			IEntity[] entities = GetEntitiesPage(type, fieldName, propertyValue, location.PageIndex, location.PageSize, sortExpression, out total);
			location.AbsoluteTotal = total;
			return entities;
		}*/
		
		/*public override IEntity[] GetEntitiesPage(Type type, PagingLocation location, string sortExpression)
		{
			int total = 0;
			IEntity[] entities =  GetEntitiesPage(type, location.PageIndex, location.PageSize, sortExpression, out total);
			location.AbsoluteTotal = total;
			return entities;
		}*/
		
		/*public override T[] GetEntitiesPageMatchReference<T>(string propertyName, Type referencedEntityType, Guid referencedEntityID, PagingLocation location, string sortExpression)
		{
			int total = 0;
			T[] entities = GetEntitiesPageMatchReference<T>(propertyName, referencedEntityType, referencedEntityID, location.PageIndex, location.PageSize, sortExpression, out total);
			location.AbsoluteTotal = total;
			return entities;
		}*/
		
		/// <summary>
		/// Retrieves the specified page of objects from the data store.
		/// </summary>
		/// <param name="type">The type of entities to retrieve.</param>
		/// <param name="propertyName">The name of the property to query for.</param>
		/// <param name="referenceID">The ID of the referenced entity.</param>
		/// <param name="location"></param>
		/// <param name="sortExpression">The sort expression to apply before retrieving the page.</param>
		/// <returns>An array of the objects retrieved.</returns>
		public override T[] GetEntitiesPageMatchReference<T>(string propertyName, Type referencedEntityType, Guid referencedEntityID, PagingLocation location, string sortExpression)
		{
			Collection<T> entities = null;
			Collection<T> page = new Collection<T>();
			
			int i = 0;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Querying the data store based on the provided parameters.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Property name: " + propertyName);
				AppLogger.Debug("Referenced entity ID: " + referencedEntityID);
				
				Type type = typeof(T);
				
				// TODO: See if this function can be improved performance wise
				//string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyNameReverse(typeof(T), propertyName, referencedEntityType);
				
				
				// Load references
				//EntityIDReferenceCollection references = GetReferences(referencedEntityType, referencedEntityID, mirrorPropertyName, typeof(T), false);
				
				//foreach (EntityID
				
				/*entities = new List<T>(((Db4oDataStore)Stores[type]).ObjectContainer.Query<T>(delegate(T e)
				                                                                              {
				                                                                              	bool matches = true;
				                                                                              	
				                                                                              	using (LogGroup logGroup2 = AppLogger.StartGroup("Querying entity.", NLog.LogLevel.Debug))
				                                                                              	{

				                                                                              		AppLogger.Debug("Checking type " + e.GetType().ToString());
				                                                                              		AppLogger.Debug("Entity ID: " + e.ID);
				                                                                              		
				                                                                              		matches = MatchReference(e.GetType(), e.ID, propertyName, referencedEntityType, referencedEntityID);
				                                                                              		
				                                                                              		AppLogger.Debug("Matches: " + matches);
				                                                                              	}
				                                                                              	
				                                                                              	// Check whether the current object is in the specified page
				                                                                              	bool isInPage = DataUtilities.IsInPage(i, location.PageIndex, location.PageSize);
				                                                                              	
				                                                                              	// Increment the absolute counter (only for matching items)
				                                                                              	if (matches)
				                                                                              		i++;
				                                                                              	
				                                                                              	return matches && isInPage;
				                                                                              }));*/
				
				string sortPropertyName = sortExpression.Replace("Ascending", "").Replace("Descending", "");
				SortDirection sortDirection = SortDirection.Ascending;
				if (sortExpression.IndexOf("Descending") > -1)
					sortDirection = SortDirection.Descending;
				
				Db4oDataStore store = (Db4oDataStore)Stores[type];
				
				/*entities = new List<T>(store).
					ObjectContainer.Query<T>(
						new Predicate<T>(){
							public bool Match(T e) {

								bool matches = true;
								
								using (LogGroup logGroup2 = AppLogger.StartGroup("Querying entity.", NLog.LogLevel.Debug))
								{

									AppLogger.Debug("Checking type " + e.GetType().ToString());
									AppLogger.Debug("Entity ID: " + e.ID);
									
									matches = MatchReference(e.GetType(), e.ID, propertyName, referencedEntityType, referencedEntityID);
									
									AppLogger.Debug("Matches: " + matches);
								}
								
								// Check whether the current object is in the specified page
								bool isInPage = DataUtilities.IsInPage(i, location.PageIndex, location.PageSize);
								
								// Increment the absolute counter (only for matching items)
								if (matches)
									i++;
								
								return matches && isInPage;
							},
							new QueryComparator<T>(){
								public int compare(T o1, T o2) {
									if (SortDirection == SortDirection.Ascending;
									    return EntitiesUtilities.GetPropertyValue(o1, sortPropertyName).CompareTo(EntitiesUtilities.GetPropertyValue(o2, sortPropertyName));
									    else
									    	return EntitiesUtilities.GetPropertyValue(o2, sortPropertyName).CompareTo(EntitiesUtilities.GetPropertyValue(o1, sortPropertyName));
									    
									}
							}
						}
					));*/

				entities = new Collection<T>(
					store.ObjectContainer.Query<T>(
						delegate(T e)
						{
							bool matches = true;
							//bool isInPage = false;
							
							using (LogGroup logGroup2 = AppLogger.StartGroup("Querying entity.", NLog.LogLevel.Debug))
							{

								AppLogger.Debug("Checking type " + e.GetType().ToString());
								AppLogger.Debug("Entity ID: " + e.ID);
								
								matches = MatchReference(e.GetType(), e.ID, propertyName, referencedEntityType, referencedEntityID);
								
								AppLogger.Debug("Matches: " + matches);
								
								// Check whether the current object is in the specified page
								//isInPage = EntitiesUtilities.IsInPage(i, location.PageIndex, location.PageSize);
								
								// Increment the absolute counter (only for matching items)
								//if (matches)
								//	i++;
							}
							
							
							return matches;// && isInPage;
						}));
				
				entities.Sort(sortExpression);
				
				location.AbsoluteTotal = entities.Count;
				
				page = entities.GetPage(location.PageIndex, location.PageSize);
				
				/*
				entities = new List<T>(
					store.ObjectContainer.Query<T>(
						delegate(T e)
						{
							bool matches = true;
							bool isInPage = false;
							
							using (LogGroup logGroup2 = AppLogger.StartGroup("Querying entity.", NLog.LogLevel.Debug))
							{

								AppLogger.Debug("Checking type " + e.GetType().ToString());
								AppLogger.Debug("Entity ID: " + e.ID);
								
								matches = MatchReference(e.GetType(), e.ID, propertyName, referencedEntityType, referencedEntityID);
								
								AppLogger.Debug("Matches: " + matches);
								
								// Check whether the current object is in the specified page
								isInPage = EntitiesUtilities.IsInPage(i, location.PageIndex, location.PageSize);
								
								// Increment the absolute counter (only for matching items)
								if (matches)
									i++;
							}
							
							
							return matches && isInPage;
						},
						new System.Comparison<T>(
							delegate(T o1, T o2)
							{
								if (sortDirection == SortDirection.Ascending)
									return Comparer<object>.Default.Compare(EntitiesUtilities.GetPropertyValue(o1, sortPropertyName), EntitiesUtilities.GetPropertyValue(o2, sortPropertyName));
								else
									return Comparer<object>.Default.Compare(EntitiesUtilities.GetPropertyValue(o2, sortPropertyName), EntitiesUtilities.GetPropertyValue(o1, sortPropertyName));
							}
						)));*/

				
				/*if (entities != null)
						{
							AppLogger.Debug("entities != null");
							
							location.AbsoluteTotal = i;
						}
						else
						{
							AppLogger.Debug("entities == null");
							
							location.AbsoluteTotal = 0;
						}*/
				
				AppLogger.Debug("Absolute total objects: " + location.AbsoluteTotal);
			}

			return (T[])page.ToArray();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyName">The name of the property to query for.</param>
		/// <param name="referencedEntityID">The ID of the referenced entity to match.</param>
		/// <returns>An array of the objects retrieved.</returns>
		public override T GetEntityMatchReference<T>(string propertyName, Type referencedEntityType, Guid referencedEntityID)
		{
			T[] entities = GetEntitiesMatchReference<T>(propertyName, referencedEntityType, referencedEntityID);

			if (entities.Length == 0)
				return default(T);
			else
				return entities[0];
		}
		
		/// <summary>
		/// Retrieves the specified page of objects from the data store.
		/// </summary>
		/// <param name="propertyName">The name of the property to query for.</param>
		/// <param name="referencedEntityID">The ID of the referenced entity to match.</param>
		/// <returns>An array of the objects retrieved.</returns>
		public override T[] GetEntitiesMatchReference<T>(string propertyName, Type referencedEntityType, Guid referencedEntityID)
		{
			List<T> entities = null;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Querying the data store based on the provided parameters.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Property name: " + propertyName);
				AppLogger.Debug("Referenced entity ID: " + referencedEntityID);
				
				//Type referencedEntityType = EntitiesUtilities.GetReferenceType(typeof(T), propertyName);
				
				
				if (referencedEntityType != null)
					AppLogger.Debug("Referenced entity type: " + referencedEntityType.ToString());
				else
					AppLogger.Debug("Referenced entity type: [null]");
				
				Type type = typeof(T);
				
				entities = new List<T>(((Db4oDataStore)Stores[type]).ObjectContainer.Query<T>(delegate(T e)
				                                                                              {
				                                                                              	bool matches = true;
				                                                                              	
				                                                                              	using (LogGroup logGroup2 = AppLogger.StartGroup("Querying entity to find one matching the specified reference.", NLog.LogLevel.Debug))
				                                                                              	{

				                                                                              		AppLogger.Debug("Checking type " + e.GetType().ToString());
				                                                                              		AppLogger.Debug("Entity ID: " + e.ID);
				                                                                              		
				                                                                              		
				                                                                              		//string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(e.GetType(),
//			                                                                              		                                                                    EntitiesUtilities.GetProperty(e.GetType(), propertyName, null));
				                                                                              		
				                                                                              		
				                                                                              		matches = MatchReference(e.GetType(), e.ID, propertyName, referencedEntityType, referencedEntityID);
				                                                                              		
				                                                                              		AppLogger.Debug("Matches: " + matches);
				                                                                              	}
				                                                                              	return matches;
				                                                                              }));


				
				if (entities != null)
				{
					AppLogger.Debug("entities != null");
				}
				else
				{
					AppLogger.Debug("entities == null");
				}
				
				AppLogger.Debug("Total objects: " + entities.Count);
			}

			return (T[])entities.ToArray();
		}
		
		/*	/// <summary>
		/// Retrieves all references that include the provided entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public EntityReferenceCollection<E1, E2> GetReferences<E1, E2>(E1 entity)
			where E1 : IEntity
			where E2 : IEntity
		{
			EntityReferenceCollection<E1, E2> collection = new EntityReferenceCollection<E1, E2>(entity);
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving references.", NLog.LogLevel.Debug))
			{
				foreach (EntityReference reference in GetReferences<E1, E2>(entity, false))
				{
					collection.Add(reference.SwitchFor(entity));
				}
			}
			
			return collection;
		}*/
		/*		/// <summary>
		/// Retrieves all references that include the provided entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="activateAll"></param>
		/// <returns></returns>
		public EntityReferenceCollection<E1, E2> GetReferences<E1, E2>(E1 entity, bool activateAll)
			where E1 : IEntity
			where E2 : IEntity
		{
			EntityReferenceCollection<E1, E2> returnReferences = new EntityReferenceCollection<E1, E2>(entity);
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving references.", NLog.LogLevel.Debug))
			{
				EntityReferenceCollection references = GetReferences(entity, typeof(E2), activateAll);
				
				foreach (EntityReference reference in references)
				{
					AppLogger.Debug("Adding reference: " + reference.ID.ToString());
					
					if (reference.EntityIDs == null
					    || (reference.EntityIDs[0] == Guid.Empty
					        || reference.EntityIDs[1] == Guid.Empty))
					{
						returnReferences.Add(reference.SwitchFor(entity));
					}
				}
			}
			
			return returnReferences;
		}*/
		
		/// <summary>
		/// Retrieves all references that include the provided entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="activateAll"></param>
		/// <returns></returns>
		public override EntityReferenceCollection GetReferences(Type entityType, Guid entityID, string propertyName, Type referenceType, bool activateAll)
		{
			EntityReferenceCollection collection = new EntityReferenceCollection();//(EntityReferenceCollection)Reflector.CreateGenericObject(typeof(EntityReferenceCollection<,>),
			//                        new Type[] {entity.GetType(),
			//              	referenceType},
			//              new object[] { entity });
			
			//using (LogGroup logGroup = AppLogger.StartGroup("Retrieving references.", NLog.LogLevel.Debug))
			//{
				if (entityType == null)
					throw new ArgumentNullException("entityType");
				
				if (entityID == Guid.Empty)
					throw new ArgumentNullException("entityID");
				
				if (referenceType == null)
					throw new ArgumentNullException("referenceType");
				
			//	AppLogger.Debug("Entity type: " + entityType.ToString());
			//	AppLogger.Debug("Reference type: " + referenceType.ToString());
				
				string storeName = DataUtilities.GetDataStoreName(
					entityType.Name,
					referenceType.Name);
				
			//	AppLogger.Debug("Data store name: " + storeName);
				
				Db4oDataStore dataStore = (Db4oDataStore)Data.DataAccess.Data.Stores[storeName];
				
				// Load the references from the data store
				List<EntityIDReference> list = new List<EntityIDReference>(dataStore.ObjectContainer.Query<EntityIDReference>(delegate(EntityIDReference reference)
				                                                                                                              {
				                                                                                                              	return reference.Includes(entityID, propertyName);
				                                                                                                              }));
				
				if (list.Count == 0){
			//		AppLogger.Debug("No references loaded from the data store.");
				}
				else
				{
					
					foreach (EntityIDReference idReference in list)
					{
			//			using (LogGroup logGroup2 = AppLogger.StartGroup("Processing ID reference.", NLog.LogLevel.Debug))
			//			{
			//				AppLogger.Debug("Data store name: " + storeName);
							
							if (idReference is EntityReference)
							{
								EntityReference reference = (EntityReference)idReference.SwitchFor(entityType, entityID);
								//if (idReference.EntityIDs == null)
								//	AppLogger.Debug("Loaded reference - Entity IDs: [null]");
								//else
								//{
			//					AppLogger.Debug("Loaded reference - Entity ID 1: " + reference.Entity1ID);
			//					AppLogger.Debug("Loaded reference - Entity ID 2: " + reference.Entity2ID);
								
			//					AppLogger.Debug("Loaded reference - Property 1 name: " + reference.Property1Name);
			//					AppLogger.Debug("Loaded reference - Property 2 name: " + reference.Property2Name);
								//}
								
								
								//if (idReference.TypeNames == null)
								//	AppLogger.Debug("Loaded reference - Type names: [null]");
								//{
			//					AppLogger.Debug("Loaded reference - Type name 1: " + reference.Type1Name);
			//					AppLogger.Debug("Loaded reference - Type name 2: " + reference.Type2Name);
								//}
								
								if (activateAll)
								{
			//						AppLogger.Debug("Activating reference.");
									ActivateReference(reference);
								}
								if (reference.Entity1ID != Guid.Empty
								    && reference.Entity2ID != Guid.Empty)
								{
			//						AppLogger.Debug("Adding to the collection.");
									collection.Add(reference);
								}
								else
								{
			//						AppLogger.Debug("Reference not added to the collection. IDs are empty.");
								}
							}
						//}
					}
				}
			//}
			return collection;
		}
		
		/// <summary>
		/// Retrieves all references that include the provided entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="activateAll"></param>
		/// <param name="referenceEntityID"></param>
		/// <returns></returns>
		public override EntityReference GetReference(Type entityType, Guid entityID, string propertyName, Type referenceType, Guid referenceEntityID, string mirrorPropertyName, bool activateAll)
		{
			EntityReferenceCollection collection = new EntityReferenceCollection();//(EntityReferenceCollection)Reflector.CreateGenericObject(typeof(EntityReferenceCollection<,>),
			//                        new Type[] {entity.GetType(),
			//              	referenceType},
			//              new object[] { entity });
			
			// TODO: Comment out logging to improve performance
			//using (LogGroup logGroup = AppLogger.StartGroup("Retrieving references.", NLog.LogLevel.Debug))
			//{
				
				if (entityType == null)
					throw new ArgumentNullException("entityType");
				
				if (referenceType == null)
					throw new ArgumentNullException("referenceType");
				
			//	AppLogger.Debug("Entity type: " + entityType.ToString());
			//	AppLogger.Debug("Reference type: " + referenceType.ToString());
				
				string storeName = DataUtilities.GetDataStoreName(
					entityType.Name,
					referenceType.Name);
				
			//	AppLogger.Debug("Data store name: " + storeName);
				
				Db4oDataStore dataStore = (Db4oDataStore)Data.DataAccess.Data.Stores[storeName];

				EntityIDReferenceCollection list = new EntityIDReferenceCollection();
				
				IQuery query1 = dataStore.ObjectContainer.Query();
				query1.Constrain(typeof(EntityIDReference));
				//query1.Constrain(query1.Descend("entity1ID").Constrain(entityID).Equal());
				
				query1.Descend("entity1ID").Constrain(entityID).Equal().And(
					query1.Descend("property1Name").Constrain(propertyName).Equal().And(
						query1.Descend("type1Name").Constrain(EntitiesUtilities.GetShortType(entityType.Name)).Equal().And(
							query1.Descend("entity2ID").Constrain(referenceEntityID).Equal().And(
								query1.Descend("property2Name").Constrain(mirrorPropertyName).Equal().And(
									query1.Descend("type2Name").Constrain(EntitiesUtilities.GetShortType(referenceType.Name)).Equal())))));
				
				
				//query1.Descend("entity1ID").Constrain(entityID).Equal().And(
				//	query1.Descend("entity2ID").Constrain(referenceEntityID).Equal());
				
				IQuery query2 = dataStore.ObjectContainer.Query();
				query2.Constrain(typeof(EntityIDReference));
				//query2.Constrain(query2.Descend("entity2ID").Constrain(entityID).Equal());
				
				query2.Descend("entity2ID").Constrain(entityID).Equal().And(
					query2.Descend("property2Name").Constrain(propertyName).Equal().And(
						query2.Descend("type2Name").Constrain(EntitiesUtilities.GetShortType(entityType.Name)).Equal().And(
							query2.Descend("entity1ID").Constrain(referenceEntityID).Equal().And(
								query2.Descend("property1Name").Constrain(mirrorPropertyName).Equal().And(
									query2.Descend("type1Name").Constrain(EntitiesUtilities.GetShortType(referenceType.Name)).Equal())))));
				
				//IConstraint c2 = query2.Descend("entity2ID").Constrain(entityID).Equal().And(
				//	query2.Descend("entity1ID").Constrain(referenceEntityID).Equal());
				
				//ssquery2.Constrain(c2);
				
				
				
				
				
				IObjectSet os1 = query1.Execute();
				
				while (os1.HasNext())
				{
					EntityIDReference reference = (EntityIDReference)os1.Next();
					if (reference != null)
					{
						if (reference.Includes(entityID, propertyName) &&
						    reference.Includes(referenceEntityID, mirrorPropertyName))
						{
			//				AppLogger.Debug("1 Reference matches expected. Adding to the list.");
							list.Add(reference);
						}
						else
						{
			//				AppLogger.Debug("1 Reference failed match. Skipping. IMPORTANT!!! IT SHOULD NOT HAVE FAILED!!!");
						}
					}
				}
				
				IObjectSet os2 = query2.Execute();
				
				while (os2.HasNext())
				{
					EntityIDReference reference = (EntityIDReference)os2.Next();
					if (reference != null)
					{
						if (reference.Includes(entityID, propertyName) &&
						    reference.Includes(referenceEntityID, mirrorPropertyName))
						{
			//				AppLogger.Debug("2 Reference matches expected. Adding to the list.");
							list.Add(reference);
						}
						else
						{
			//				AppLogger.Debug("2 Reference failed match. Skipping. IMPORTANT!!! IT SHOULD NOT HAVE FAILED!!!");
						}
					}
				}
				
				foreach (EntityIDReference reference in list)
				{
					if (!reference.Includes(entityID, propertyName))
					{
						throw new Exception("Retrieved invalid reference. Doesn't match expected entity with type '" + entityType.Name + "', ID '" + entityID.ToString() + "' and property name '" + propertyName + "'. Instead types are '" + reference.Type1Name + "' and '" + reference.Type2Name + "', IDs are '" + reference.Entity1ID + "' and '" + reference.Entity2ID + "' and property names are '" + reference.Property1Name + "' and '" + reference.Property2Name + "'.");
					}
					if (!reference.Includes(referenceEntityID, mirrorPropertyName))
					{
						throw new Exception("Retrieved invalid reference. Doesn't match expected reference with type '" + referenceType.Name + "', ID '" + referenceEntityID.ToString() + "' and property name '" + propertyName + "'. Instead types are '" + reference.Type1Name + "' and '" + reference.Type2Name + "', IDs are '" + reference.Entity1ID + "' and '" + reference.Entity2ID + "' and property names are '" + reference.Property1Name + "' and '" + reference.Property2Name + "'.");
					}
				}
				
				
				// Load the references from the data store
				/*List<EntityIDReference> list = new List<EntityIDReference>(dataStore.ObjectContainer.Query<EntityIDReference>(delegate(EntityIDReference reference)
				                                                                                                              {
				                                                                                                              	//return ((entityID.Equals(reference.Entity1ID) && propertyName.Equals(reference.Property1Name))
				                                                                                                              	//        || (entityID.Equals(reference.Entity2ID) && propertyName.Equals(reference.Property2Name)));
				                                                                                                              	return reference.Includes(entityID, propertyName) && reference.Includes(referenceEntityID, mirrorPropertyName);
				                                                                                                              }));*/
				
				if (list.Count == 0)
				{
			//		AppLogger.Debug("No references loaded from the data store.");
				}
				else
				{
					
					
			//		AppLogger.Debug("# references loaded: " + list.Count.ToString());
					
					if (list.Count > 1)
						throw new Exception("Multiple (" + list.Count.ToString() + ") references found when there should only be one.");
					
					int i = 0;
					
					foreach (EntityIDReference idReference in list)
					{
						i++;
						
			//			using (LogGroup logGroup2 = AppLogger.StartGroup("Processing ID reference.", NLog.LogLevel.Debug))
			//			{
			//				AppLogger.Debug("Data store name: " + storeName);
							
							EntityReference reference = null;
							
							if (!(idReference is EntityReference))
							{
								reference = new EntityReference(idReference);
							}
							
							reference = (EntityReference)idReference.SwitchFor(entityType, entityID);
							
							/*if (idReference.EntityIDs == null)
						AppLogger.Debug("Loaded reference - Entity IDs: [null]");
					else
					{
						AppLogger.Debug("Loaded reference " + i + "/" + list.Count.ToString() + " - Entity ID 1: " + reference.Entity1ID);
						AppLogger.Debug("Loaded reference " + i + "/" + list.Count.ToString() + " - Entity ID 2: " + reference.Entity2ID);
					}*/
							
							
							//	AppLogger.Debug("Loaded reference " + i + "/" + list.Count.ToString() + " - Property name 1: " + reference.Property1Name);
//					//	AppLogger.Debug("Loaded reference " + i + "/" + list.Count.ToString() + " - Property name 2: " + reference.Property2Name);
							
							
							//if (idReference.TypeNames == null)
							//{
							//	AppLogger.Debug("Loaded reference - Type names: [null]");
							
							//	AppLogger.Debug("Loaded reference " + i + "/" + list.Count.ToString() + " - Type name 1: " + reference.Type1Name);
							//	AppLogger.Debug("Loaded reference " + i + "/" + list.Count.ToString() + " - Type name 2: " + reference.Type2Name);
							//}
							
							if (activateAll)
							{
			//					AppLogger.Debug("Activating reference.");
								ActivateReference(reference);
							}
							if (reference.Entity1ID != Guid.Empty
							    && reference.Entity2ID != Guid.Empty)
							{
			//					AppLogger.Debug("Adding to the collection.");
								collection.Add(reference);
							}
							else
							{
			//					AppLogger.Debug("Reference not added to the collection. IDs are empty.");
							}
			//			//}
					}
				}
			//}
			if (collection != null && collection.Count > 0)
				return collection[0];
			else
				return null;
		}
		
		public override EntityReferenceCollection GetObsoleteReferences(IEntity entity, Guid[] idsOfEntitiesToKeep)
		{
			EntityReferenceCollection collection = new EntityReferenceCollection(entity);
			
			Type entityType = entity.GetType();
			
			foreach (PropertyInfo property in entityType.GetProperties())
			{
				if (EntitiesUtilities.IsReference(entityType, property.Name, property.PropertyType))
				{
					collection.AddRange(
						GetObsoleteReferences(entity,
						                      property.Name,
						                      DataUtilities.GetEntityType(entity, property),
						                      idsOfEntitiesToKeep
						                     )
					);
				}
			}
			return collection;
		}
		
		public override EntityReferenceCollection GetObsoleteReferences(IEntity entity, string propertyName, Type referenceType, Guid[] idsOfEntitiesToKeep)
		{
			EntityReferenceCollection collection = new EntityReferenceCollection(entity);
			
			foreach (EntityReference r in GetReferences(entity.GetType(), entity.ID, propertyName, referenceType, false))
			{
				EntityReference reference = (EntityReference)r;
				
				// If the ID is NOT found in the idsOfEntitiesToKeep array then it should be added to the list.
				// The list is references intended for deletion
				if (Array.IndexOf(idsOfEntitiesToKeep, reference.Entity2ID) == -1)
					collection.Add(reference);
			}
			
			return collection;
		}
		/*public void ActivateReferences<E1, E2>(EntityReferenceCollection<E1, E2> references)
			where E1 : IEntity
			where E2 : IEntity
		{
			foreach (EntityReference<E1, E2> reference in references)
			{
				ActivateReference(reference);
			}
		}*/
		
		
		public override void ActivateReference(EntityReference reference)
		{
			//using (LogGroup logGroup = AppLogger.StartGroup("Activating reference.", NLog.LogLevel.Debug))
			//{
			if (reference.Type1Name == string.Empty)
				throw new ArgumentNullException("reference.Type1Name");
			if (reference.Type2Name == string.Empty)
				throw new ArgumentNullException("reference.Type2Name");
			
			//AppLogger.Debug("Type 1: " + reference.Type1Name);
			//AppLogger.Debug("Type 2: " + reference.Type2Name);
			//AppLogger.Debug("ID 1: " + reference.Entity1ID);
			//AppLogger.Debug("ID 2: " + reference.Entity2ID);
			
			Type type1 = EntitiesUtilities.GetType(reference.Type1Name);
			Type type2 = EntitiesUtilities.GetType(reference.Type2Name);
			
			//AppLogger.Debug("Full type 1: " + type1.ToString());
			//AppLogger.Debug("Full type 2: " + type2.ToString());
			
			if (reference.Entity1ID == Guid.Empty || reference.Entity2ID == Guid.Empty)
			{
				//AppLogger.Debug("Skipped activation because both IDs weren't found.");
			}
			else
			{
				IEntity entity1 = DataAccess.Data.Stores[type1].GetEntityByTypeAndProperty(
					type1,
					"ID",
					reference.Entity1ID);
				
				IEntity entity2 = DataAccess.Data.Stores[type2].GetEntityByTypeAndProperty(
					type2,
					"ID",
					reference.Entity2ID);
				
				if (entity1 != null)
					reference.SourceEntity = entity1;
				else
					throw new Exception("Entity not found in data store '" + DataUtilities.GetDataStoreName(type1) + "' with ID '" + reference.Entity1ID.ToString() + "' and type " + type1.ToString() + ".");
				if (entity2 != null)
					reference.ReferenceEntity = entity2;
				else
					throw new Exception("Entity not found in data store '" + DataUtilities.GetDataStoreName(type2) + "' with ID '" + reference.Entity2ID.ToString() + "' and type " + type2.ToString() + ".");
				
			}
			
			//if (reference.SourceEntity == null)
			//	AppLogger.Debug("reference.SourceEntity == null");
			//else
			//	AppLogger.Debug("reference.SourceEntity is " + reference.SourceEntity.GetType().ToString());
			
			//if (reference.ReferenceEntity == null)
			//	AppLogger.Debug("reference.ReferenceEntity == null");
			//else
			//	AppLogger.Debug("reference.ReferenceEntity is " + reference.ReferenceEntity.GetType().ToString());
			
			//}
		}
		
		public override void Activate(IEntity[] entities)
		{
			Activate(entities, 1);
		}
		
		public override void Activate(IEntity[] entities, int depth)
		{
			foreach (IEntity entity in entities)
			{
				Activate(entity, depth);
			}
		}
		
		public override void Activate(IEntity entity)
		{
			Activate(entity, 1);
		}
		
		public override void Activate(IEntity entity, int depth)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			using (LogGroup logGroup = AppLogger.StartGroup("Activating the references on type: " + entity.GetType().ToString(), NLog.LogLevel.Debug))
			{
				Type entityType = entity.GetType();
				
				foreach (PropertyInfo property in entity.GetType().GetProperties())
				{
					if (EntitiesUtilities.IsReference(entityType, property.Name, property.PropertyType))
					{
						AppLogger.Debug("Found reference property: " + property.Name);
						AppLogger.Debug("Property type: " + property.PropertyType.ToString());
						
						Activate(entity, property.Name, property.PropertyType, depth);
					}
				}
			}
		}
		
		public override void Activate(IEntity entity, string propertyName, Type propertyType)
		{
			Activate(entity, propertyName, propertyType, 1);
		}
		
		public override void Activate(IEntity entity, string propertyName, Type propertyType, int depth)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Activating property: " + propertyName, NLog.LogLevel.Debug))
			{
				PropertyInfo property = null;
				if (propertyType != null)
					property = entity.GetType().GetProperty(propertyName, propertyType);
				else
					property = entity.GetType().GetProperty(propertyName);
				
				
				// Multiple references.
				if (property.PropertyType.GetInterface("IList") != null)
				{
					AppLogger.Debug("Multiple reference property");
					
					Type referenceType = DataUtilities.GetEntityType(entity, property);
					
					AppLogger.Debug("Reference entity type: " + referenceType.ToString());
					
					EntityReferenceCollection references = GetReferences(entity.GetType(),
					                                                     entity.ID,
					                                                     property.Name,
					                                                     referenceType,
					                                                     true);
					
					//	references.SwitchFor(entity);
					
					IEntity[] referencedEntities = DataUtilities.GetReferencedEntities(references, entity);
					
					object value = null;
					
					// If it's a collection reference then create a collection
					if (EntitiesUtilities.IsCollectionReference(entity.GetType(), property))
					{
						value = Reflector.CreateGenericObject(typeof(Collection<>),
						                                      new Type[] {referenceType},
						                                      new Object[] {referencedEntities});
						
						// If the activation depth is greater than 1
						if (depth > 1)
						{
							Activate(((Collection<IEntity>)value).ToArray(), depth-1);
						}
					}
					else // Otherwise cast the array to the referenced type
					{
						value = Collection<IEntity>.ConvertAll(referencedEntities, referenceType);
						
						// If the activation depth is greater than 1
						if (depth > 1)
						{
							Activate((IEntity[])value, depth-1);
						}
					}
					
					if (referencedEntities == null)
						AppLogger.Debug("# of entities found: [null]");
					else
						AppLogger.Debug("# of entities found:" + referencedEntities.Length);
					
					property.SetValue(entity, value, null);
					//(EntityReferenceCollection)property.GetValue(entity, null);
					
					//if (collection == null || collection.Count == 0)
					//{
					//	collection = GetReferences(entity, propertyType, true);
					//}
					
					//foreach (EntityReference reference in collection)
					//{
					//	ActivateReference(reference);
					//}
				}
				// Single reference.
				else
				{
					AppLogger.Debug("Single reference property");
					
					Type referenceType = DataUtilities.GetEntityType(entity, property);
					
					AppLogger.Debug("Reference entity type: " + referenceType.ToString());
					
					EntityReferenceCollection references = GetReferences(entity.GetType(),
					                                                     entity.ID,
					                                                     propertyName,
					                                                     referenceType,
					                                                     true);
					
					
					IEntity[] referencedEntities = DataUtilities.GetReferencedEntities(references, entity);
					
					//	object value = Reflector.CreateGenericObject(typeof(Collection<>),
					//	                                             new Type[] {referenceType},
					//	                                             new Object[] {referencedEntities});
					//
					if (referencedEntities == null)
						AppLogger.Debug("# of entities found: [null]");
					else
						AppLogger.Debug("# of entities found:" + referencedEntities.Length);
					
					//references.SwitchFor(entity);
					
					if (referencedEntities != null && referencedEntities.Length > 0)
						property.SetValue(entity, referencedEntities[0], null);
				}
			}
		}
		
		public override void Activate(IEntity entity, string propertyName)
		{
			Activate(entity, propertyName, 1);
		}

		public override void Activate(IEntity entity, string propertyName, int depth)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Activating property: " + propertyName, NLog.LogLevel.Debug))
			{
				Activate(entity, propertyName, null, depth);
			}
		}
		
		
		public override bool MatchReference(Type entityType, Guid entityID, string propertyName, Type referencedEntityType, Guid referencedEntityID, string mirrorPropertyName)
		{
			return MatchReference(entityType, entityID, propertyName, referencedEntityType, referencedEntityID);
		}
		
		public override bool MatchReference(Type entityType, Guid entityID, string propertyName, Type referencedEntityType, Guid referencedEntityID)
		{
			bool matches = false;
			
			// Logging disabled to boost performance
			
			// TODO: Comment out logging to improve performance
			//using (LogGroup logGroup = AppLogger.StartGroup("Checking whether the provided entity and specified property matches the specified referenced entity ID.", NLog.LogLevel.Debug))
			//{
				if (entityType == null)
					throw new ArgumentNullException("entityType");
				
				if (entityID == Guid.Empty)
					throw new ArgumentException("entityID");
				
				if (referencedEntityType == null)
					throw new ArgumentNullException("referencedEntityType");
				
				/*if (referencedEntityID == Guid.Empty)
					throw new ArgumentException("referencedEntityID");*/

				
				/*AppLogger.Debug("Entity type: " + entityType);
				AppLogger.Debug("Entity ID: " + entityID);
				AppLogger.Debug("Property name: " + propertyName);
				AppLogger.Debug("Referenced entity type: " + referencedEntityType.ToString());
				AppLogger.Debug("Referenced entity ID: " + referencedEntityID.ToString());
*/
				string mirrorPropertyName = String.Empty;
				if (propertyName == String.Empty)
					mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyNameReverse(entityType, propertyName, referencedEntityType);
				else
					mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(entityType, propertyName);
				
				EntityReference reference = DataAccess.Data.GetReference(entityType, entityID, propertyName, referencedEntityType, referencedEntityID, mirrorPropertyName, false);
				
				//if (reference == null)
				//	AppLogger.Debug("reference == null");
				//else
				//	AppLogger.Debug("reference != null");
				
				matches = (reference != null);
				
				//AppLogger.Debug("Matches: " + matches.ToString());
			//}
			return matches;
		}
		
		
		
		public override void Save(IEntity entity)
		{
			Stores[DataUtilities.GetDataStoreName(entity)].Save(entity);
		}

		public override void Update(IEntity entity)
		{
			Stores[DataUtilities.GetDataStoreName(entity)].Update(entity);
		}

		public override void Delete(IEntity entity)
		{
			Stores[DataUtilities.GetDataStoreName(entity)].Delete(entity);
		}

		#endregion

		#region Filter matching functions
		static public bool IsMatch(IEntity entity, IDataFilter filter)
		{
			bool isMatch = false;

			//using (LogGroup logGroup2 = AppLogger.StartGroup("Checking whether the provided entity and filter match.", NLog.LogLevel.Debug))
			//{
				isMatch = filter.IsMatch(entity);

			//	AppLogger.Debug("Is match? " + isMatch.ToString());
			//}
			
			return isMatch;
		}

		static public bool IsMatch(IEntity entity, FilterGroup filterGroup)
		{
			return filterGroup.IsMatch(entity);
		}
		#endregion
	}
}
