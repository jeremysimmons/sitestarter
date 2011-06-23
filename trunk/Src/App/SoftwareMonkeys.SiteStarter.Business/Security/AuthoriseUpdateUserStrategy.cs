using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// Used to check whether the current user is authorised to update a user.
	/// </summary>
	[AuthoriseStrategy("Update", "User")]
	public class AuthoriseUpdateUserStrategy : AuthoriseUpdateStrategy
	{
		/// <summary>
		/// Checks whether the current user is authorised to update an entity of the specified type.
		/// </summary>
		/// <param name="shortTypeName">The type of entity being updated.</param>
		/// <returns>A value indicating whether the current user is authorised to update an entity of the specified type.</returns>
		public override bool Authorise(string shortTypeName)
		{
			// Everyone can, as long as its their own account
			return true;
			/*bool isAuthenticated = AuthenticationState.IsAuthenticated;
			
			bool isAdministrator = AuthenticationState.UserIsInRole("Administrator");
			
			bool allowRegistration = Configuration.Config.Application.Settings.GetBool("EnableUserRegistration");
			
			return (isAuthenticated && isAdministrator);*/
		}
		
		/// <summary>
		/// Checks whether the current user is authorised to update the provided entity.
		/// </summary>
		/// <param name="entity">The entity to be updated.</param>
		/// <returns>A value indicating whether the current user is authorised to update the provided entity.</returns>
		public override bool Authorise(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			User user = (User)entity;
			
			bool isAuthenticated = AuthenticationState.IsAuthenticated;
			
			if (isAuthenticated)
			{
				bool isAdministrator = AuthenticationState.UserIsInRole("Administrator");
				
				bool isSelf = (user.ID.Equals(AuthenticationState.User.ID));
				
				return isAdministrator // Administrators
					|| isSelf; // Editing own account
			}
			else
				return false;
		}
		
		
		#region New functions
		/// <summary>
		/// Creates a new strategy for authorising the updating of the specified type.
		/// </summary>
		new static public IAuthoriseUpdateStrategy New<T>()
		{
			return StrategyState.Strategies.Creator.New<IAuthoriseUpdateStrategy>("AuthoriseUpdate", typeof(T).Name);
		}
		
		/// <summary>
		/// Creates a new strategy for authorising the updating the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		new static public IAuthoriseUpdateStrategy New(string typeName)
		{
			return StrategyState.Strategies.Creator.New<IAuthoriseUpdateStrategy>("AuthoriseUpdate", typeName);
		}
		#endregion
	}
}
