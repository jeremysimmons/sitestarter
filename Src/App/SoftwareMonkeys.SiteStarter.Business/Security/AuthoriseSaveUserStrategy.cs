using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// Used to check whether the current user is authorised to save a user.
	/// </summary>
	[AuthoriseStrategy("Save", "User")]
	public class AuthoriseSaveUserStrategy : AuthoriseSaveStrategy
	{
		/// <summary>
		/// Checks whether the current user is authorised to save an entity of the specified type.
		/// </summary>
		/// <param name="shortTypeName">The type of entity being saved.</param>
		/// <returns>A value indicating whether the current user is authorised to save an entity of the specified type.</returns>
		public override bool IsAuthorised(string shortTypeName)
		{
			bool isAuthenticated = AuthenticationState.IsAuthenticated;
			
			bool isAdministrator = AuthenticationState.UserIsInRole("Administrator");
			
			bool allowRegistration = Configuration.Config.Application.Settings.GetBool("EnableUserRegistration");
			
			return (isAuthenticated && isAdministrator) // Administrators
				|| (allowRegistration && !isAuthenticated); // New users registering
		}
				
		#region New functions
		/// <summary>
		/// Creates a new strategy for authorising the saveing of the specified type.
		/// </summary>
		new static public IAuthoriseSaveStrategy New<T>()
		{
			return StrategyState.Strategies.Creator.New<IAuthoriseSaveStrategy>("AuthoriseSave", typeof(T).Name);
		}
		
		/// <summary>
		/// Creates a new strategy for authorising the saveing the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		new static public IAuthoriseSaveStrategy New(string typeName)
		{
			return StrategyState.Strategies.Creator.New<IAuthoriseSaveStrategy>("AuthoriseSave", typeName);
		}
		#endregion
	}
}
