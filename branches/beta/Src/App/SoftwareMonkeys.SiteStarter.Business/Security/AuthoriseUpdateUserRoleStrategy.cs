using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// Used to check whether the current role is authorised to update a role.
	/// </summary>
	[AuthoriseStrategy("Update", "UserRole")]
	public class AuthoriseUpdateUserRoleStrategy : AuthoriseUpdateStrategy
	{
		/// <summary>
		/// Checks whether the current role is authorised to update an entity of the specified type.
		/// </summary>
		/// <param name="shortTypeName">The type of entity being updated.</param>
		/// <returns>A value indicating whether the current role is authorised to update an entity of the specified type.</returns>
		public override bool Authorise(string shortTypeName)
		{
			// Everyone can, as long as its their own accoutn
			return true;
			/*bool isAuthenticated = AuthenticationState.IsAuthenticated;
			
			bool isAdministrator = AuthenticationState.UserRoleIsInRole("Administrator");
			
			bool allowRegistration = Configuration.Config.Application.Settings.GetBool("EnableUserRoleRegistration");
			
			return (isAuthenticated && isAdministrator);*/
		}
		
		/// <summary>
		/// Checks whether the current role is authorised to update the provided entity.
		/// </summary>
		/// <param name="entity">The entity to be updated.</param>
		/// <returns>A value indicating whether the current role is authorised to update the provided entity.</returns>
		public override bool Authorise(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
		
			UserRole role = (UserRole)entity;
			
			bool isAuthenticated = AuthenticationState.IsAuthenticated;
			
			bool isAdministrator = AuthenticationState.UserIsInRole("Administrator");
			
			bool isSelf = (role.ID.Equals(AuthenticationState.User.ID));
			
			return (isAuthenticated && isAdministrator) // Administrators
				|| (isSelf); // Editing own account
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
