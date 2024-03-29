﻿using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// Used to check whether the current role is authorised to save a role.
	/// </summary>
	[AuthoriseStrategy("Save", "UserRole")]
	public class AuthoriseSaveUserRoleStrategy : AuthoriseSaveStrategy
	{
		/// <summary>
		/// Checks whether the current role is authorised to save an entity of the specified type.
		/// </summary>
		/// <param name="shortTypeName">The type of entity being saved.</param>
		/// <returns>A value indicating whether the current role is authorised to save an entity of the specified type.</returns>
		public override bool IsAuthorised(string shortTypeName)
		{
			return AuthenticationState.UserIsInRole("Administrator");
		}
		
		/// <summary>
		/// Checks whether the current role is authorised to save the provided entity.
		/// </summary>
		/// <param name="entity">The entity to be saved.</param>
		/// <returns>A value indicating whether the current role is authorised to save the provided entity.</returns>
		public override bool IsAuthorised(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
		
			return IsAuthorised(entity.ShortTypeName);	
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
