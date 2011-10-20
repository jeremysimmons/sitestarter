using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// Used to check whether the current user is authorised to save an entity.
	/// </summary>
	[AuthoriseStrategy("Save", "IEntity")]
	public class AuthoriseSaveStrategy : BaseAuthoriseStrategy, IAuthoriseSaveStrategy
	{
		/// <summary>
		/// Checks whether the current user is authorised to save an entity of the specified type.
		/// </summary>
		/// <param name="shortTypeName">The type of entity being saved.</param>
		/// <returns>A value indicating whether the current user is authorised to save an entity of the specified type.</returns>
		public override bool Authorise(string shortTypeName)
		{
			if (!AuthenticationState.IsAuthenticated)
				return false;
			
			if (!AuthenticationState.UserIsInRole("Administrator"))
				return false;
			
			return true;
		}
		
		/// <summary>
		/// Checks whether the current user is authorised to save the provided entity.
		/// </summary>
		/// <param name="entity">The entity to be saved.</param>
		/// <returns>A value indicating whether the current user is authorised to save the provided entity.</returns>
		public override bool Authorise(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			AuthoriseReferences(entity);
			
			return Authorise(entity.ShortTypeName);
		}
		
		
		#region New functions
		/// <summary>
		/// Creates a new strategy for authorising the saveing of the specified type.
		/// </summary>
		static public IAuthoriseSaveStrategy New<T>()
		{
			return StrategyState.Strategies.Creator.New<IAuthoriseSaveStrategy>("AuthoriseSave", typeof(T).Name);
		}
		
		/// <summary>
		/// Creates a new strategy for authorising the saveing the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		static public IAuthoriseSaveStrategy New(string typeName)
		{
			return StrategyState.Strategies.Creator.New<IAuthoriseSaveStrategy>("AuthoriseSave", typeName);
		}
		#endregion
	}
}
