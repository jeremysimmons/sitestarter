using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Data;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to retrieve an index (or page of an index) of entities.
	/// </summary>
	[Strategy("Index", "IEntity")]
	public class IndexStrategy : BaseStrategy, IIndexStrategy
	{	
		/// <summary>
		/// Retrieves the entities of the specified type.
		/// </summary>
		/// <returns></returns>
		public T[] Get<T>()
			where T : IEntity
		{
			Collection<T> entities = new Collection<T>(Data.DataAccess.Data.Indexer.GetEntities<T>());
			
			
			return entities.ToArray();
		}	
		
		/// <summary>
		/// Retrieves the entities of the specified type with the provided sort expression applied.
		/// </summary>
		/// <param name="sortExpression"></param>
		/// <returns></returns>
		public IEntity[] Get(Type type, string sortExpression)
		{
			Collection<IEntity> entities = new Collection<IEntity>(Get(type));
			
			entities.Sort(sortExpression);
			
			return entities.ToArray();
		}
		
		/// <summary>
		/// Retrieves the entities of the specified type with the provided sort expression applied.
		/// </summary>
		/// <param name="sortExpression"></param>
		/// <returns></returns>
		public T[] Get<T>(string sortExpression)
			where T : IEntity
		{
			Collection<T> entities = new Collection<T>(Get<T>());
			
			entities.Sort(sortExpression);
			
			return entities.ToArray();
		}
		
		/// <summary>
		/// Retrieves the page of entities at the specified location with the provided sort expression applied.
		/// </summary>
		/// <param name="location"></param>
		/// <param name="sortExpression"></param>
		/// <returns></returns>
		public T[] Get<T>(PagingLocation location, string sortExpression)
			where T : IEntity
		{
			Collection<T> entities = new Collection<T>(Data.DataAccess.Data.Indexer.GetPageOfEntities<T>(location, sortExpression));
			
			entities.Sort(sortExpression);
			
			return entities.ToArray();
		}
		
		T[] IIndexStrategy.Get<T>(IPagingLocation location, string sortExpression)
		{
			return Get<T>((PagingLocation)location, sortExpression);
		}
		
		IEntity[] IIndexStrategy.Get(string sortExpression)
		{
			return (IEntity[])Get<IEntity>(sortExpression);
		}
		
		public IEntity[] Get(Type type)
		{
			return Get(type);
		}
		
		public IEntity[] Get(Type type, PagingLocation location, string sortExpression)
		{
			return DataAccess.Data.Indexer.GetPageOfEntities(type, location, sortExpression);
		}
		
		public IEntity[] Get(Type type, System.Collections.Generic.IDictionary<string, object> filterValues, string sortExpression)
		{
			return DataAccess.Data.Indexer.GetEntities(type, new FilterGroup(type, filterValues), sortExpression);
		}
		
		public T[] Get<T>(System.Collections.Generic.IDictionary<string, object> filterValues, string sortExpression) where T : IEntity
		{
			return (T[])DataAccess.Data.Indexer.GetEntities<T>(new FilterGroup(typeof(T), filterValues), sortExpression);
		}
		
		public IEntity[] Get(Type type, System.Collections.Generic.IDictionary<string, object> filterValues, IPagingLocation location, string sortExpression)
		{
			return DataAccess.Data.Indexer.GetPageOfEntities(type, new FilterGroup(type, filterValues), (PagingLocation)location, sortExpression);
		}
		
		public T[] Get<T>(System.Collections.Generic.IDictionary<string, object> filterValues, PagingLocation location, string sortExpression) where T : IEntity
		{
			return (T[])DataAccess.Data.Indexer.GetPageOfEntities<T>(new FilterGroup(typeof(T), filterValues), location, sortExpression);
		}
		
		IEntity[] IIndexStrategy.Get(Type type, IPagingLocation location, string sortExpression)
		{
			return Get(type, (PagingLocation)location, sortExpression);
		}
		
		IEntity[] IIndexStrategy.Get(Type type, System.Collections.Generic.Dictionary<string, object> filterValues, IPagingLocation location, string sortExpression)
		{
			return Get(type, filterValues, (PagingLocation)location, sortExpression);
		}
		
		T[] IIndexStrategy.Get<T>(System.Collections.Generic.IDictionary<string, object> filterValues, IPagingLocation location, string sortExpression)
		{
			return Get<T>(filterValues, (PagingLocation)location, sortExpression);
		}
		
		public IEntity[] Get(Type type, System.Collections.Generic.Dictionary<string, object> filterValues, string sortExpression)
		{
			Collection<IEntity> entities = new Collection<IEntity>(DataAccess.Data.Indexer.GetEntities(type, filterValues));
			entities.Sort(sortExpression);
			return entities.ToArray();
		}
	}
}
