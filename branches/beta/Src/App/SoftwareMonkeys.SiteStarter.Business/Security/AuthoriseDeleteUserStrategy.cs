using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// Used to check whether the current user is authorised to delete a user.
	/// </summary>
	[AuthoriseStrategy("Delete", "User")]
	public class AuthoriseDeleteUserStrategy : AuthoriseDeleteStrategy
	{
		/// <summary>
		/// Checks whether the current user is authorised to delete an entity of the specified type.
		/// </summary>
		/// <param name="shortTypeName">The type of entity being deleted.</param>
		/// <returns>A value indicating whether the current user is authorised to delete an entity of the specified type.</returns>
		public override bool Authorise(string shortTypeName)
		{
			// Everyone can, as long as its their own account
			return true;
		}
		
		/// <summary>
		/// Checks whether the current user is authorised to delete the provided entity.
		/// </summary>
		/// <param name="entity">The entity to be deleted.</param>
		/// <returns>A value indicating whether the current user is authorised to delete the provided entity.</returns>
		public override bool Authorise(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
		
			User user = (User)entity;
			
			bool isAuthenticated = AuthenticationState.IsAuthenticated;
			
			bool isAdministrator = AuthenticationState.UserIsInRole("Administrator");
			
			bool isSelf = (AuthenticationState.User != null && user.ID.Equals(AuthenticationState.User.ID));
			
			return (isAuthenticated && isAdministrator) // Administrators
				|| (isSelf); // Editing own account
		}
	}
}
