﻿using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// Defines the interface of all authorise index strategies.
	/// </summary>
	public interface IAuthoriseIndexStrategy : IAuthoriseStrategy
	{
		/// <summary>
		/// Checks whether the current user is authorised to index the provided entities.
		/// </summary>
		/// <param name="entities">The entities being indexed.</param>
		/// <returns>The authorised entities. (Or empty if none are authorised)</returns>
		IEntity[] Authorise(IEntity[] entities);
		
		/// <summary>
		/// Ensures that the current user is authorised to access an index of entities including the one provided.
		/// </summary>
		/// <param name="entities">The entities in the index.</param>
		void EnsureAuthorised(ref IEntity[] entities);
		
		/// <summary>
		/// Ensures that the current user is authorised to access an index of entities including the one provided.
		/// </summary>
		/// <param name="entities">The entities in the index.</param>
		void EnsureAuthorised<T>(ref T[] entities)
			where T : IEntity;
	}
}
