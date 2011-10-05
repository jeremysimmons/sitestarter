using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// Used to check whether the current user is authorised to retrieve an entity.
	/// </summary>
	[Strategy("AuthoriseRetrieve", "IEntity")]
	public class AuthoriseRetrieveStrategy : BaseAuthoriseStrategy, IAuthoriseRetrieveStrategy
	{
		/// <summary>
		/// Checks whether the current user is authorised to retrieve an entity of the specified type.
		/// </summary>
		/// <param name="shortTypeName">The type of entity being retrieved.</param>
		/// <returns>A value indicating whether the current user is authorised to retrieve an entity of the specified type.</returns>
		public override bool Authorise(string shortTypeName)
		{
			if (!AuthenticationState.IsAuthenticated)
				return false;
			
			return true;
		}
		
		/// <summary>
		/// Checks whether the current user is authorised to retrieve the provided entity.
		/// </summary>
		/// <param name="entity">The entity to be retrieved.</param>
		/// <returns>A value indicating whether the current user is authorised to retrieve the provided entity.</returns>
		public override bool Authorise(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			if (!AuthenticationState.IsAuthenticated)
				return false;
			
			return true;
		}
		
		#region New functions
		/// <summary>
		/// Creates a new strategy for authorising the retrieval of an entity of the specified type.
		/// </summary>
		static public IAuthoriseRetrieveStrategy New<T>()
		{
			return StrategyState.Strategies.Creator.New<IAuthoriseRetrieveStrategy>("AuthoriseRetrieve", typeof(T).Name);
		}
		
		/// <summary>
		/// Creates a new strategy for authorising the retrieve of an entity of the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		static public IAuthoriseRetrieveStrategy New(string typeName)
		{
			return StrategyState.Strategies.Creator.New<IAuthoriseRetrieveStrategy>("AuthoriseRetrieve", typeName);
		}
		
		/// <summary>
		/// Creates a new strategy for authorising the retrieve of the provided entity.
		/// </summary>
		/// <param name="entity">The entity involved in the strategy.</param>
		static public IAuthoriseRetrieveStrategy New(IEntity entity)
		{
			return New(entity.ShortTypeName);
		}
		#endregion
	}
}
