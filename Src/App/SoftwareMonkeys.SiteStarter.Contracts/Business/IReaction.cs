using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Defines the interface of all business reaction components.
	/// </summary>
	public interface IReaction
	{
		/// <summary>
		/// Gets/sets the name of the type involved in the reaction.
		/// </summary>
		string TypeName {get;set;}
		
		string Action { get; }
		
		void React(IEntity entity);
	}
}
