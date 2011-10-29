using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// 
	/// </summary>
	[Strategy("AuthoriseUpdate", "Projection")]
	public class AuthoriseUpdateProjectionStrategy : BaseAuthoriseStrategy
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
		
		public AuthoriseUpdateProjectionStrategy()
		{
		}
	}
}
