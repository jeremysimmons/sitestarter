using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Defines the interface of all delete reactions.
	/// </summary>
	public interface IDeleteReaction : IReaction
	{
		/// <summary>
		/// Reacts to a delete action.
		/// </summary>
		/// <param name="entity">The entity that was deleted.</param>
		void React(IEntity entity);
	}
}
