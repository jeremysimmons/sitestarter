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
		public T[] Index<T>()
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
		public IEntity[] Index(Type type, string sortExpression)
		{
			Collection<IEntity> entities = new Collection<IEntity>(Index(type));
			
			entities.Sort(sortExpression);
			
			return entities.ToArray();
		}
		
		/// <summary>
		/// Retrieves the entities of the specified type with the provided sort expression applied.
		/// </summary>
		/// <param name="sortExpression"></param>
		/// <returns></returns>
		public T[] Index<T>(string sortExpression)
			where T : IEntity
		{
			Collection<T> entities = new Collection<T>(Index<T>());
			
			entities.Sort(sortExpression);
			
			return entities.ToArray();
		}
		
		/// <summary>
		/// Retrieves the page of entities at the specified location with the provided sort expression applied.
		/// </summary>
		/// <param name="location"></param>
		/// <param name="sortExpression"></param>
		/// <returns></returns>
		public T[] Index<T>(PagingLocation location, string sortExpression)
			where T : IEntity
		{
			Collection<T> entities = new Collection<T>(Data.DataAccess.Data.Indexer.GetPageOfEntities<T>(location, sortExpression));
			
			entities.Sort(sortExpression);
			
			return entities.ToArray();
		}
	
		T[] IIndexStrategy.Index<T>(IPagingLocation location, string sortExpression)
		{
			return Index<T>((PagingLocation)location, sortExpression);
		}
		
		IEntity[] IIndexStrategy.Index(string sortExpression)
		{
			return (IEntity[])Index<IEntity>(sortExpression);
		}
		
		public IEntity[] Index(Type type)
		{
			return Index(type);
		}
		
		public IEntity[] Index(Type type, PagingLocation location, string sortExpression)
		{
			return DataAccess.Data.Indexer.GetPageOfEntities(type, location, sortExpression);
		}
		
		public IEntity[] Index(Type type, System.Collections.Generic.IDictionary<string, object> filterValues, string sortExpression)
		{
			return DataAccess.Data.Indexer.GetEntities(type, new FilterGroup(type, filterValues), sortExpression);
		}
		
		public T[] Index<T>(System.Collections.Generic.IDictionary<string, object> filterValues, string sortExpression) where T : IEntity
		{
			return (T[])DataAccess.Data.Indexer.GetEntities<T>(new FilterGroup(typeof(T), filterValues), sortExpression);
		}
		
		public IEntity[] Index(Type type, System.Collections.Generic.IDictionary<string, object> filterValues, IPagingLocation location, string sortExpression)
		{
			return DataAccess.Data.Indexer.GetPageOfEntities(type, new FilterGroup(type, filterValues), (PagingLocation)location, sortExpression);
		}
		
		public T[] Index<T>(System.Collections.Generic.IDictionary<string, object> filterValues, PagingLocation location, string sortExpression) where T : IEntity
		{
			return (T[])DataAccess.Data.Indexer.GetPageOfEntities<T>(new FilterGroup(typeof(T), filterValues), location, sortExpression);
		}
		
		IEntity[] IIndexStrategy.Index(Type type, IPagingLocation location, string sortExpression)
		{
			return Index(type, (PagingLocation)location, sortExpression);
		}
		
		IEntity[] IIndexStrategy.Index(Type type, System.Collections.Generic.Dictionary<string, object> filterValues, IPagingLocation location, string sortExpression)
		{
			return Index(type, filterValues, (PagingLocation)location, sortExpression);
		}
		
		T[] IIndexStrategy.Index<T>(System.Collections.Generic.IDictionary<string, object> filterValues, IPagingLocation location, string sortExpression)
		{
			return Index<T>(filterValues, (PagingLocation)location, sortExpression);
		}
		
		public IEntity[] Index(Type type, System.Collections.Generic.Dictionary<string, object> filterValues, string sortExpression)
		{
			Collection<IEntity> entities = new Collection<IEntity>(DataAccess.Data.Indexer.GetEntities(type, filterValues));
			entities.Sort(sortExpression);
			return entities.ToArray();
		}
	}
}
