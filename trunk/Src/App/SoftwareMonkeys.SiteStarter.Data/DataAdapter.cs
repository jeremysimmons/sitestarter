using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// The base of all data adapters.
	/// </summary>
	public abstract class DataAdapter : IDataAdapter
	{
		
		private bool autoRelease = true;
		/// <summary>
		/// Gets/sets a value indicating whether to release entities by removing binding between them and data store.
		/// </summary>
		public bool AutoRelease {
			get{
				return autoRelease;
			}
			set {
				autoRelease = value;
			}
		}
		
		private DataProvider provider;
		/// <summary>
		/// The data provider for the data adapter to use.
		/// </summary>
		public DataProvider Provider
		{
			get { return provider; }
		}
		
		/// <summary>
		/// The data store that the adapter is tied to. Access it through the GetDataStore function.
		/// </summary>
		protected IDataStore DataStore;
		
		public DataAdapter()
		{
			
		}
		
		/// <summary>
		/// Sets the data provider and the data store for the adapter to use.
		/// </summary>
		/// <param name="provider">The provider for the adapater to use.</param>
		public DataAdapter(DataProvider provider)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Constructing the data adapter.", NLog.LogLevel.Debug))
			{
				Initialize(provider, null);
			}
		}
		
		/// <summary>
		/// Sets the data provider and the data store for the adapter to use.
		/// </summary>
		/// <param name="provider">The provider for the adapater to use.</param>
		/// <param name="store">The data store for the adapter to use.</param>
		public DataAdapter(DataProvider provider, IDataStore store)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Constructing the data adapter.", NLog.LogLevel.Debug))
			{
				Initialize(provider, store);
			}
		}
		
		/// <summary>
		/// Initializes the data adapter.
		/// </summary>
		/// <param name="provider">The provider that the adapter is to use.</param>
		/// <param name="store">The data store that the adapter is tied to, or [null] to let it automatically select the appropriate store.</param>
		protected void Initialize(DataProvider provider, IDataStore store)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Initializing the data adapter.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Set the provider for the adapter to use.");
				this.provider = provider;
				AppLogger.Debug("Set the store for the adapter to use.");
				this.DataStore = store;
			}
		}
		
		
		/// <summary>
		/// Retrieves the data store with the specified name, or the data store that this adapter is tied to.
		/// </summary>
		/// <param name="dataStoreName">The name of the data store to retrieve.</param>
		/// <returns>Either the data store that this adapter is tied to, or if its [null] then the data store that is specified.</returns>
		public IDataStore GetDataStore(string dataStoreName)
		{
			IDataStore store = null;
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the data store for the provided entity.", NLog.LogLevel.Debug))
			{
				if (DataStore == null)
				{
					store = DataAccess.Data.Stores[dataStoreName];
					AppLogger.Debug("Dynamically selected data store.");
				}
				else
				{
					store = DataStore;
					AppLogger.Debug("Using the data store tied to the adapter.");
				}
				
				if (store.IsClosed)
					store = DataAccess.Data.InitializeDataStore(dataStoreName);
				
				AppLogger.Debug("Data store name: " + store.Name);
			}
			return store;
		}
		
		/// <summary>
		/// Retrieves the data store for the specified entity, or the data store that this adapter is tied to.
		/// </summary>
		/// <param name="entity">The entity to retrieve the data store for.</param>
		/// <returns>Either the data store that this adapter is tied to, or if its [null] then the data store that corresponds with the entity.</returns>
		public IDataStore GetDataStore(IEntity entity)
		{
			IDataStore store = null;
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the data store for the provided entity.", NLog.LogLevel.Debug))
			{
				if (DataStore == null)
				{
					store = DataAccess.Data.Stores[entity];
					AppLogger.Debug("Dynamically selected data store.");
				}
				else
				{
					store = DataStore;
					AppLogger.Debug("Using the data store tied to the adapter.");
				}
				
				if (store.IsClosed)
					store = DataAccess.Data.InitializeDataStore(DataUtilities.GetDataStoreName(entity));
				
				AppLogger.Debug("Data store name: " + store.Name);
			}
			return store;
		}
		
		/// <summary>
		/// Retrieves the data store for the specified entity, or the data store that this adapter is tied to.
		/// </summary>
		/// <param name="type">The type of entity to retrieve the data store for.</param>
		/// <returns>Either the data store that this adapter is tied to, or if its [null] then the data store that corresponds with the entity.</returns>
		public IDataStore GetDataStore(Type type)
		{
			IDataStore store = null;
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the data store for the provided entity.", NLog.LogLevel.Debug))
			{
				if (DataStore == null)
				{
					store = DataAccess.Data.Stores[type];
					AppLogger.Debug("Dynamically selected data store.");
				}
				else
				{
					store = DataStore;
					AppLogger.Debug("Using the data store tied to the adapter.");
				}
				
				AppLogger.Debug("Data store name: " + store.Name);
			}
			return store;
		}
		
		/// <summary>
		/// Retrieves the data store for the references between the specified types.
		/// </summary>
		/// <param name="type1Name">The name of the first type in the reference.</param>
		/// <param name="type2Name">The name of the second type in the reference.</param>
		/// <returns>Either the data store that this adapter is tied to, or if its [null] then the data store that corresponds with the entity.</returns>
		public IDataStore GetDataStore(string type1Name, string type2Name)
		{
			IDataStore store = null;
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the data store for the provided entity.", NLog.LogLevel.Debug))
			{
				
				// TODO: Clean up
				// Commented out because this function is use to get the data store for a reference.
				// Reference data store should always be retrieved dynamically, not tied like entity stores.
				//if (dataStore == null)
				//{
					store = DataAccess.Data.Stores[DataUtilities.GetDataStoreName(type1Name, type2Name)];
					AppLogger.Debug("Dynamically selected data store.");
				//}
				//else
				//{
					//store = dataStore;
					//AppLogger.Debug("Using the data store tied to the adapter.");
				//}
				
				AppLogger.Debug("Data store name: " + store.Name);
			}
			return store;
		}
		
		/// <summary>
		/// Releases the entity from the data access tier by detaching/cloning it to release all bindings.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public IEntity Release(IEntity entity)
		{
			IEntity output = null;
			
			if (!AutoRelease)
				output = entity;
			else
				output = entity.Clone();
						
			return output;
		}
		
		/// <summary>
		/// Releases the entity from the data access tier by detaching/cloning it to release all bindings.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public T Release<T>(T entity)
		{
			return (T)Release(entity);
		}
		
		/// <summary>
		/// Releases the entities from the data access tier by detaching/cloning them to release all bindings.
		/// </summary>
		/// <param name="entities"></param>
		/// <returns></returns>
		public IEntity[] Release(IEntity[] entities)
		{
			Collection<IEntity> collection = new Collection<IEntity>();
			foreach (IEntity e in entities)
			{
				if (AutoRelease)
					collection.Add(e.Clone());
				else
					collection.Add(e);
			}
			return collection.ToArray();
		}
		
		/// <summary>
		/// Releases the entities from the data access tier by detaching/cloning them to release all bindings.
		/// </summary>
		/// <param name="entities"></param>
		/// <returns></returns>
		public T[] Release<T>(T[] entities)
			where T : IEntity
		{
			return Collection<T>.ConvertAll(
				Release(
					Collection<IEntity>.ConvertAll(entities)));
			
		}
	}
}
