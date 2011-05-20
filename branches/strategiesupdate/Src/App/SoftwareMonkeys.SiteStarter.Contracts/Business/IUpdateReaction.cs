using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Defines the interface of all update reactions.
	/// </summary>
	public interface IUpdateReaction : IReaction
	{
		/// <summary>
		/// Reacts to an update action.
		/// </summary>
		/// <param name="entity">The entity that was updated.</param>
		void React(IEntity entity);
	}
}
