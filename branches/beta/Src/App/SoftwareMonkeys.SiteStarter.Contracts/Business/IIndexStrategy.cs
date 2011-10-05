using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Data;
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
		/// Gets/sets the current paging location.
		/// </summary>
		IPagingLocation Location { get;set; }
		
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

		/// <summary>
		/// Retrieves the entities with the specified IDs.
		/// </summary>
		/// <param name="ids">The IDs of the entities to retrieve.</param>
		/// <returns>An array of the entities matching the provided IDs.</returns>
		T[] Index<T>(Guid[] ids)
			where T : IEntity;
		
		/// <summary>
		/// Index the entity with a references that matches the provided parameters.
		/// </summary>
		/// <param name="propertyName">The name of the property containing the references.</param>
		/// <param name="referencedEntityType">The type of the entity being referenced.</param>
		/// <param name="referencedEntityID">The ID of the entity being referenced.</param>
		/// <returns>The entity matching the provided parameters.</returns>
		IEntity[] IndexWithReference(string propertyName, string referencedEntityType, Guid referencedEntityID);
		
		/// <summary>
		/// Indexes the entity with a references that matches the provided parameters.
		/// </summary>
		/// <param name="propertyName">The name of the property containing the references.</param>
		/// <param name="referencedEntityType">The type of the entity being referenced.</param>
		/// <param name="referencedEntityID">The ID of the entity being referenced.</param>
		/// <returns>The entities matching the provided parameters.</returns>
		T[] IndexWithReference<T>(string propertyName, string referencedEntityType, Guid referencedEntityID)
			where T : IEntity;
		
		
		/// <summary>
		/// Indexes the entity with a reference to one of the provided entities.
		/// </summary>
		/// <param name="propertyName">The name of the property containing the references.</param>
		/// <param name="referencedEntities">The entities to look for a reference to.</param>
		/// <returns>The entity matching the provided parameters.</returns>
		T[] IndexWithReference<T>(string propertyName, IEntity[] referencedEntities)
			where T : IEntity;
		
		/// <summary>
		/// Retrieves the entities matching the provided filter value.
		/// </summary>
		/// <param name="propertyName">The name of the property to filter the entities by.</param>
		/// <param name="propertyValue">The value of the property to match the filter values by.</param>
		/// <returns>The entities with properties matching the specified value.</returns>
		T[] Index<T>(string propertyName, object propertyValue)
			where T : IEntity;
		
		
		/// <summary>
		/// Retrieves the entities matching the provided filter group.
		/// </summary>
		/// <param name="group"></param>
		/// <returns></returns>
		T[] Index<T>(IDataFilterGroup group)
			where T : IEntity;
		
		/// <summary>
		/// Retrieves the entities matching the provided filter group.
		/// </summary>
		/// <param name="group"></param>
		/// <returns></returns>
		IEntity[] Index(IDataFilterGroup group);
	}
}
