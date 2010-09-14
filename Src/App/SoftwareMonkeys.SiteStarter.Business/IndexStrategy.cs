using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;
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
		private int absoluteTotal;
		/// <summary>
		/// Gets/sets the absolute total number of entities found (including those on other pages).
		/// </summary>
		public int AbsoluteTotal
		{
			get { return absoluteTotal; }
			set { absoluteTotal = value; }
		}
		
		private bool enablePaging;
		/// <summary>
		/// Gets/sets a value indicating whether paging is enabled on the index.
		/// </summary>
		public bool EnablePaging
		{
			get { return enablePaging; }
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
		
		private int currentPageIndex;
		/// <summary>
		/// Gets/sets the current page index.
		/// </summary>
		public int CurrentPageIndex
		{
			get { return currentPageIndex; }
			set { currentPageIndex = value; }
		}
		
		private int pageSize;
		/// <summary>
		/// Gets/sets the number of items on each page.
		/// </summary>
		public int PageSize
		{
			get { return pageSize; }
			set { pageSize = value; }
		}
		
		/// <summary>
		/// Retrieves the entities of the specified type.
		/// </summary>
		/// <returns></returns>
		public T[] Index<T>()
			where T : IEntity
		{
			CheckPageSize();
			
			Collection<T> entities = new Collection<T>();
						
			if (EnablePaging)
			{
				PagingLocation location = new PagingLocation(CurrentPageIndex, PageSize);
				
				entities.AddRange(DataAccess.Data.Indexer.GetPageOfEntities<T>(location, SortExpression));
				
				AbsoluteTotal = location.AbsoluteTotal;
			}
			else
			{
				entities.AddRange(DataAccess.Data.Indexer.GetEntities<T>());
				
				AbsoluteTotal = entities.Count;
			}
			
			
			return entities.ToArray();
		}
		
		/// <summary>
		/// Retrieves the entities of the specified type with the sort expression applied.
		/// </summary>
		/// <param name="sortExpression"></param>
		/// <returns></returns>
		public IEntity[] Index()
		{
			CheckPageSize();
			CheckTypeName();
			
			Type type = EntitiesUtilities.GetType(TypeName);
			
			return Collection<IEntity>.ConvertAll(Reflector.InvokeGenericMethod(this, // Source object
			                                                                    "Index", // Method name
			                                                                    new Type[] {type}, // Generic types
			                                                                    new object[] {})); // Method arguments);
		}
		
		/// <summary>
		/// Retrieves the entities matching the provided filter values.
		/// </summary>
		/// <param name="filterValues"></param>
		/// <returns></returns>
		public IEntity[] Index(Dictionary<string, object> filterValues)
		{
			CheckPageSize();
			
			Type type = EntitiesUtilities.GetType(TypeName);
			
			if (type == null)
				throw new InvalidOperationException("Type not found.");
			
			return Collection<IEntity>.ConvertAll(
				Reflector.InvokeGenericMethod(this, // Source object
				                              "Index", // Method name
				                              new Type[] {type}, // Generic types
				                              new object[] {filterValues})); // Method arguments);
		}
		
		/// <summary>
		/// Retrieves the entities matching the provided filter values.
		/// </summary>
		/// <param name="filterValues"></param>
		/// <returns></returns>
		public T[] Index<T>(Dictionary<string, object> filterValues)
			where T : IEntity
		{
			CheckPageSize();
			
			T[] entities = new T[]{};
			
			if (EnablePaging)
			{
				PagingLocation location = new PagingLocation(CurrentPageIndex, PageSize);
				
				entities = Collection<T>.ConvertAll(DataAccess.Data.Indexer.GetPageOfEntities<T>(new FilterGroup(typeof(T), filterValues), location, SortExpression));
				
				AbsoluteTotal = location.AbsoluteTotal;
			}
			else
			{
				entities = Collection<T>.ConvertAll(DataAccess.Data.Indexer.GetEntities<T>(new FilterGroup(typeof(T), filterValues), SortExpression));
				
				AbsoluteTotal = entities.Length;
			}
			
			return entities;
		}
		
		public void CheckPageSize()
		{
			if (PageSize <= 0)
				throw new InvalidOperationException("The page size must be greater than 0 but is " + PageSize);
		}
	}
}
