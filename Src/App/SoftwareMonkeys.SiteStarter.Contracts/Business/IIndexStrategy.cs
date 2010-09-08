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
		/// Retrieves the entities of the specified type with the provided sort expression applied.
		/// </summary>
		/// <param name="sortExpression"></param>
		/// <returns></returns>
		T[] Get<T>(string sortExpression)
			where T : IEntity;
		
		/// <summary>
		/// Retrieves the entities of the specified type with the provided sort expression applied.
		/// </summary>
		/// <param name="sortExpression"></param>
		/// <returns></returns>
		IEntity[] Get(string sortExpression);
		
		/// <summary>
		/// Retrieves the entities of the specified type.
		/// </summary>
		/// <returns></returns>
		T[] Get<T>()
			where T : IEntity;
		
		/// <summary>
		/// Retrieves the entities of the specified type.
		/// </summary>
		/// <returns></returns>
		IEntity[] Get(Type type);
		
		/// <summary>
		/// Retrieves the page of entities at the specified location with the provided sort expression applied.
		/// </summary>
		/// <param name="location"></param>
		/// <param name="sortExpression"></param>
		/// <returns></returns>
		T[] Get<T>(IPagingLocation location, string sortExpression)
			where T : IEntity;
		
		/// <summary>
		/// Retrieves the page of entities at the specified location with the provided sort expression applied.
		/// </summary>
		/// <param name="location"></param>
		/// <param name="sortExpression"></param>
		/// <returns></returns>
		IEntity[] Get(Type type, IPagingLocation location, string sortExpression);
		
		/// <summary>
		/// Retrieves the page of entities at the specified location with the provided sort expression applied.
		/// </summary>
		/// <param name="sortExpression"></param>
		/// <returns></returns>
		IEntity[] Get(Type type, string sortExpression);
		
		/// <summary>
		/// Retrieves the page of entities at the specified location with the provided sort expression applied.
		/// </summary>
		/// <param name="location"></param>
		/// <param name="sortExpression"></param>
		/// <returns></returns>
		IEntity[] Get(Type type, Dictionary<string, object> filterValues, IPagingLocation location, string sortExpression);
		
		/// <summary>
		/// Retrieves the page of entities at the specified location with the provided sort expression applied.
		/// </summary>
		/// <param name="location"></param>
		/// <param name="sortExpression"></param>
		/// <returns></returns>
		T[] Get<T>(IDictionary<string, object> filterValues, IPagingLocation location, string sortExpression)
			where T : IEntity;
		
		/// <summary>
		/// Retrieves the page of entities at the specified location with the provided sort expression applied.
		/// </summary>
		/// <param name="sortExpression"></param>
		/// <returns></returns>
		IEntity[] Get(Type type, Dictionary<string, object> filterValues, string sortExpression);
		
		/// <summary>
		/// Retrieves the page of entities at the specified location with the provided sort expression applied.
		/// </summary>
		/// <param name="sortExpression"></param>
		/// <returns></returns>
		T[] Get<T>(IDictionary<string, object> filterValues, string sortExpression)
			where T : IEntity;
	}
}
