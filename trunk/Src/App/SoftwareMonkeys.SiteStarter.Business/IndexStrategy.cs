using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to retrieve an index (or page of an index) of entities.
	/// </summary>
	[Strategy("Index", "IEntity")]
	public class IndexStrategy : BaseStrategy, IIndexStrategy
	{		
		private bool enablePaging;
		/// <summary>
		/// Gets/sets a value indicating whether paging is enabled on the index.
		/// </summary>
		public bool EnablePaging
		{
			get {
				return enablePaging; }
			set { enablePaging = value; }
		}
		
		private string sortExpression;
		/// <summary>
		/// Gets/sets the sort expression applied to the index.
		/// </summary>
		public string SortExpression
		{
			get { return sortExpression; }
			set { sortExpression = value; }
		}
		
		private PagingLocation location;
		/// <summary>
		/// Gets/sets the current paging location.
		/// </summary>
		public PagingLocation Location
		{
			get {
				if (location == null)
					location = new PagingLocation(0, 10);
				return location; }
			set { location = value; }
		}
		
		IPagingLocation IIndexStrategy.Location
		{
			get { return location; }
			set { location = (PagingLocation)value; }
		}
		
		/// <summary>
		/// Index the entity with a references that matches the provided parameters.
		/// </summary>
		/// <param name="propertyName">The name of the property containing the references.</param>
		/// <param name="referencedEntityType">The type of the entity being referenced.</param>
		/// <param name="referencedEntityID">The ID of the entity being referenced.</param>
		/// <returns>The entity matching the provided parameters.</returns>
		public virtual IEntity[] IndexWithReference(string propertyName, string referencedEntityType, Guid referencedEntityID)
		{
			IEntity[] entities = (IEntity[])Reflector.InvokeGenericMethod(this,
			                                                              "IndexWithReference",
			                                                              new Type[] {EntityState.Entities[TypeName].GetEntityType()},
			                                                              new object[] {propertyName, referencedEntityType, referencedEntityID});
			
			if (RequireAuthorisation)
				AuthoriseIndexStrategy.New(TypeName).EnsureAuthorised(ref entities);
			
			return entities;
		}
		
		/// <summary>
		/// Indexes the entity with a references that matches the provided parameters.
		/// </summary>
		/// <param name="propertyName">The name of the property containing the references.</param>
		/// <param name="referencedEntityType">The type of the entity being referenced.</param>
		/// <param name="referencedEntityID">The ID of the entity being referenced.</param>
		/// <returns>The entity matching the provided parameters.</returns>
		public virtual T[] IndexWithReference<T>(string propertyName, string referencedEntityType, Guid referencedEntityID)
			where T : IEntity
		{
			T[] entities = new T[] {};
			
			if (EnablePaging)
			{
				entities = (T[])DataAccess.Data.Indexer.GetPageOfEntitiesWithReference<T>(propertyName, EntitiesUtilities.GetType(referencedEntityType), referencedEntityID, Location, sortExpression);
			}
			else
			{
				entities = (T[])DataAccess.Data.Indexer.GetEntitiesWithReference<T>(propertyName, EntitiesUtilities.GetType(referencedEntityType), referencedEntityID);
			}
			
			if (RequireAuthorisation)
				AuthoriseIndexStrategy.New<T>().EnsureAuthorised<T>(ref entities);
			
			return entities;
		}
		
		
		/// <summary>
		/// Retrieves the entities with the specified IDs.
		/// </summary>
		/// <param name="ids">The IDs of the entities to retrieve.</param>
		/// <returns>An array of the entities matching the provided IDs.</returns>
		public virtual T[] Index<T>(Guid[] ids)
			where T : IEntity
		{
			Collection<T> list = new Collection<T>();
			
			IRetrieveStrategy retriever = RetrieveStrategy.New<T>();
			
			foreach (Guid id in ids)
			{
				T entity = retriever.Retrieve<T>(id);
				
				list.Add(entity);
			}
			
			T[] entities = list.ToArray();
			
			if (RequireAuthorisation)
				AuthoriseIndexStrategy.New<T>().EnsureAuthorised(ref entities);
			
			return entities;
		}
		
		/// <summary>
		/// Retrieves the entities of the specified type.
		/// </summary>
		/// <returns></returns>
		public virtual T[] Index<T>()
			where T : IEntity
		{
			Collection<T> collection = new Collection<T>();
			
			if (EnablePaging)
			{
				collection.AddRange(DataAccess.Data.Indexer.GetPageOfEntities<T>(Location, SortExpression));
			}
			else
			{
				collection.AddRange(DataAccess.Data.Indexer.GetEntities<T>());
			}
			
			
			T[] entities = collection.ToArray();
			
			if (RequireAuthorisation)
				AuthoriseIndexStrategy.New<T>().EnsureAuthorised(ref entities);
			
			return entities;
		}
		
		/// <summary>
		/// Retrieves the entities of the specified type with the sort expression applied.
		/// </summary>
		/// <param name="sortExpression"></param>
		/// <returns></returns>
		public virtual IEntity[] Index()
		{
			CheckTypeName();
			
			Type type = EntitiesUtilities.GetType(TypeName);
			
			IEntity[] entities = Collection<IEntity>.ConvertAll(Reflector.InvokeGenericMethod(this, // Source object
			                                                                                  "Index", // Method name
			                                                                                  new Type[] {type}, // Generic types
			                                                                                  new object[] {})); // Method arguments);
			
			if (RequireAuthorisation)
				AuthoriseIndexStrategy.New(TypeName).EnsureAuthorised(ref entities);
			
			return entities;
		}
		
		/// <summary>
		/// Retrieves the entities matching the provided filter values.
		/// </summary>
		/// <param name="filterValues"></param>
		/// <returns></returns>
		public virtual IEntity[] Index(Dictionary<string, object> filterValues)
		{
			Type type = EntitiesUtilities.GetType(TypeName);
			
			if (type == null)
				throw new InvalidOperationException("Type not found.");
			
			IEntity[] entities = Collection<IEntity>.ConvertAll(
				Reflector.InvokeGenericMethod(this, // Source object
				                              "Index", // Method name
				                              new Type[] {type}, // Generic types
				                              new object[] {filterValues})); // Method arguments);
			
			if (RequireAuthorisation)
				AuthoriseIndexStrategy.New(TypeName).EnsureAuthorised(ref entities);
			
			return entities;
		}
		
		/// <summary>
		/// Retrieves the entities matching the provided filter value.
		/// </summary>
		/// <param name="propertyName">The name of the property to filter the entities by.</param>
		/// <param name="propertyValue">The value of the property to match the filter values by.</param>
		/// <returns>The entities with properties matching the specified value.</returns>
		public virtual T[] Index<T>(string propertyName, object propertyValue)
			where T : IEntity
		{
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters.Add(propertyName, propertyValue);
			
			return Index<T>(parameters);
		}
		
		/// <summary>
		/// Retrieves the entities matching the provided filter values.
		/// </summary>
		/// <param name="filterValues"></param>
		/// <returns></returns>
		public virtual T[] Index<T>(Dictionary<string, object> filterValues)
			where T : IEntity
		{
			return Index<T>(new FilterGroup(typeof(T), filterValues));
		}
		
		
		/// <summary>
		/// Retrieves the entities matching the provided filter group.
		/// </summary>
		/// <param name="group"></param>
		/// <returns></returns>
		public virtual T[] Index<T>(FilterGroup group)
			where T : IEntity
		{
			T[] entities = new T[]{};
			
			if (EnablePaging)
			{
				entities = Collection<T>.ConvertAll(DataAccess.Data.Indexer.GetPageOfEntities<T>(group, Location, SortExpression));
				
				
			}
			else
			{
				entities = Collection<T>.ConvertAll(DataAccess.Data.Indexer.GetEntities<T>(group, SortExpression));
				
			}
			
			if (RequireAuthorisation)
				AuthoriseIndexStrategy.New<T>().EnsureAuthorised(ref entities);
			
			return entities;
		}
		
		/// <summary>
		/// Retrieves the entities matching the provided filter group.
		/// </summary>
		/// <param name="group"></param>
		/// <returns></returns>
		T[] IIndexStrategy.Index<T>(IFilterGroup group)
		{
			return this.Index<T>((FilterGroup)group);
		}
		
		
		#region New functions
		/// <summary>
		/// Creates a new strategy for indexing the specified type.
		/// </summary>
		static public IIndexStrategy New<T>()
		{
			return StrategyState.Strategies.Creator.NewIndexer(typeof(T).Name);
		}
		
		/// <summary>
		/// Creates a new strategy for indexing the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		static public IIndexStrategy New(string typeName)
		{
			return StrategyState.Strategies.Creator.NewIndexer(typeName);
		}
		
		/// <summary>
		/// Creates a new strategy for indexing a single page of entities the specified type.
		/// </summary>
		static public IIndexStrategy New<T>(PagingLocation location, string sortExpression)
		{
			IIndexStrategy strategy = StrategyState.Strategies.Creator.NewIndexer(typeof(T).Name);
			strategy.EnablePaging = true;
			strategy.Location = location;
			strategy.SortExpression = sortExpression;
			
			return strategy;
		}
		
		/// <summary>
		/// Creates a new strategy for indexing a single page of entities the specified type.
		/// </summary>
		static public IIndexStrategy New<T>(PagingLocation location, string sortExpression, bool requireAuthorisation)
		{
			IIndexStrategy strategy = StrategyState.Strategies.Creator.NewIndexer(typeof(T).Name);
			strategy.EnablePaging = true;
			strategy.Location = location;
			strategy.RequireAuthorisation = requireAuthorisation;
			strategy.SortExpression = sortExpression;
			
			return strategy;
		}
		
		/// <summary>
		/// Creates a new strategy for indexing the specified type.
		/// </summary>
		/// <param name="requiresAutorisation">A value indicating whether or not the strategy requires authorisation.</param>
		static public IIndexStrategy New<T>(bool requiresAuthorisation)
		{
			IIndexStrategy strategy = StrategyState.Strategies.Creator.NewIndexer(typeof(T).Name);
			strategy.RequireAuthorisation = requiresAuthorisation;
			return strategy;
		}
		
		/// <summary>
		/// Creates a new strategy for indexing the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		/// <param name="requiresAutorisation">A value indicating whether or not the strategy requires authorisation.</param>
		static public IIndexStrategy New(string typeName, bool requiresAuthorisation)
		{
			IIndexStrategy strategy = StrategyState.Strategies.Creator.NewIndexer(typeName);
			strategy.RequireAuthorisation = requiresAuthorisation;
			return strategy;
		}
		
		/// <summary>
		/// Creates a new strategy for indexing a single page of entities the specified type.
		/// </summary>
		static public IIndexStrategy New(PagingLocation location, string typeName, string sortExpression, bool requireAuthorisation)
		{
			IIndexStrategy strategy = StrategyState.Strategies.Creator.NewIndexer(typeName);
			strategy.EnablePaging = true;
			strategy.Location = location;
			strategy.RequireAuthorisation = requireAuthorisation;
			strategy.SortExpression = sortExpression;
			
			return strategy;
		}
		#endregion
	}
}
