using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// Used to check whether the current user is authorised to register a user account.
	/// </summary>
	[AuthoriseStrategy("Register", "User")]
	public class AuthoriseRegisterUserStrategy : AuthoriseSaveUserStrategy
	{
		/// <summary>
		/// Checks whether the current user is authorised to save an entity of the specified type.
		/// </summary>
		/// <param name="shortTypeName">The type of entity being saved.</param>
		/// <returns>A value indicating whether the current user is authorised to save an entity of the specified type.</returns>
		public override bool IsAuthorised(string shortTypeName)
		{
			bool allowRegistration = Configuration.Config.Application.Settings.GetBool("EnableUserRegistration");
			
			return allowRegistration;
		}
		
		#region New functions
		/// <summary>
		/// Creates a new strategy for authorising the saving of the specified type.
		/// </summary>
		new static public AuthoriseRegisterUserStrategy New<T>()
		{
			return StrategyState.Strategies.Creator.New<AuthoriseRegisterUserStrategy>("AuthoriseRegister", typeof(T).Name);
		}
		
		/// <summary>
		/// Creates a new strategy for authorising the saving the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		new static public AuthoriseRegisterUserStrategy New(string typeName)
		{
			return StrategyState.Strategies.Creator.New<AuthoriseRegisterUserStrategy>("AuthoriseRegister", typeName);
		}
		#endregion
	}
}
