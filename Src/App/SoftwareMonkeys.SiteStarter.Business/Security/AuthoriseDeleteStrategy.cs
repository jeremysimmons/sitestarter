using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// Used to check whether the current user is authorised to delete an entity.
	/// </summary>
	[AuthoriseStrategy("Delete", "IEntity")]
	public class AuthoriseDeleteStrategy : BaseAuthoriseStrategy, IAuthoriseDeleteStrategy
	{
		/// <summary>
		/// Checks whether the current user is authorised to delete an entity of the specified type.
		/// </summary>
		/// <param name="shortTypeName">The type of entity being deleted.</param>
		/// <returns>A value indicating whether the current user is authorised to delete an entity of the specified type.</returns>
		public override bool IsAuthorised(string shortTypeName)
		{
			if (!RequireAuthorisation)
				return true;
			
			return AuthenticationState.UserIsInRole("Administrator");
		}
		
		/// <summary>
		/// Checks whether the current user is authorised to delete the provided entity.
		/// </summary>
		/// <param name="shortTypeName">The type of entity being deleted.</param>
		/// <returns>A value indicating whether the current user is authorised to delete the provided entity.</returns>
		public override bool IsAuthorised(IEntity entity)
		{
			if (!RequireAuthorisation)
				return true;
			
			return AuthenticationState.UserIsInRole("Administrator");
		}
		
		#region New functions
		/// <summary>
		/// Creates a new strategy for authorising the retrieval of an entity of the specified type.
		/// </summary>
		static public IAuthoriseDeleteStrategy New<T>()
		{
			return StrategyState.Strategies.Creator.New<IAuthoriseDeleteStrategy>("AuthoriseDelete", typeof(T).Name);
		}
		
		/// <summary>
		/// Creates a new strategy for authorising the retrieve of an entity of the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		static public IAuthoriseDeleteStrategy New(string typeName)
		{
			return StrategyState.Strategies.Creator.New<IAuthoriseDeleteStrategy>("AuthoriseDelete", typeName);
		}
		#endregion
	}
}
