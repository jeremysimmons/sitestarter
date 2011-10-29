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
		public override bool IsAuthorised(string shortTypeName)
		{
			return AuthenticationState.UserIsInRole("Administrator");
		}
	
		public override bool IsAuthorised(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
		
			return AuthenticationState.UserIsInRole("Administrator");
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
