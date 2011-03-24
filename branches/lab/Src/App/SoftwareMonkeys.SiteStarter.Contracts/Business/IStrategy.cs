using System;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Defines the interface of all business strategy components.
	/// </summary>
	public interface IStrategy
	{
		/// <summary>
		/// Gets/sets the name of the type involved in the strategy.
		/// </summary>
		string TypeName {get;set;}
		
		/// <summary>
		/// Gets/sets a value indicating whether authorisation is required in order for the strategy to execute.
		/// Note: Must default to true, and should only be set to false when used internally.
		/// </summary>
		bool RequireAuthorisation { get;set; }
	}
}
