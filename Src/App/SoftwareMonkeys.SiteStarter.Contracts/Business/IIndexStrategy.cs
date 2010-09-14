using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Defines the interface of all index strategy components.
	/// </summary>
	public interface IIndexStrategy : IStrategy
	{
		/// <summary>
		/// Gets/sets the sort expression applied to the index.
		/// </summary>
		string SortExpression {get;set;}
		
		/// <summary>
		/// Gets/sets a value indicating whether paging is enabled.
		/// </summary>
		bool EnablePaging {get;set;}
		
		/// <summary>
		/// Gets/sets the index of the current page.
		/// </summary>
		int CurrentPageIndex {get;set;}
		
		/// <summary>
		/// Gets/sets the number of entities on each page.
		/// </summary>
		int PageSize {get;set;}
		
		/// <summary>
		/// Gets/sets the absolute total number of entities found (including those on other pages).
		/// </summary>
		int AbsoluteTotal {get;set;}
		
		/// <summary>
		/// Retrieves the entities of the specified type.
		/// </summary>
		/// <returns></returns>
		T[] Index<T>()
			where T : IEntity;
		
		/// <summary>
		/// Retrieves the entities.
		/// </summary>
		/// <returns></returns>
		IEntity[] Index();
		
		/// <summary>
		/// Retrieves the page of entities at the specified location with the provided sort expression applied.
		/// </summary>
		/// <param name="filterValues"></param>
		/// <returns></returns>
		IEntity[] Index(Dictionary<string, object> filterValues);
		
		/// <summary>
		/// Retrieves the page of entities at the specified location with the provided sort expression applied.
		/// </summary>
		/// <param name="sortExpression"></param>
		/// <returns></returns>
		T[] Index<T>(Dictionary<string, object> filterValues)
			where T : IEntity;
	}
}
