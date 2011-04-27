using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Defines the interface of all save reactions.
	/// </summary>
	public interface ISaveReaction : IReaction
	{
		/// <summary>
		/// Reacts to a save action.
		/// </summary>
		/// <param name="entity">The entity that was saved.</param>
		void React(IEntity entity);
	}
}
