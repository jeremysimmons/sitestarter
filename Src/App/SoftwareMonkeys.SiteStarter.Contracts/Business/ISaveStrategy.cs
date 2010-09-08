﻿using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Defines the interface of all save strategies.
	/// </summary>
	public interface ISaveStrategy : IStrategy
	{
		/// <summary>
		/// Saves the provided entity.
		/// </summary>
		/// <param name="entity">The entity to save.</param>
		void Save(IEntity entity);
	}
}
