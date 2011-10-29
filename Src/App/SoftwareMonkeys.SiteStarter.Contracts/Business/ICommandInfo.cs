using System;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// 
	/// </summary>
	public interface ICommandInfo
	{
		/// <summary>
		/// The primary action.
		/// </summary>
		string Action {get;set;}
		
		/// <summary>
		/// The type involved in the action.
		/// </summary>
		string TypeName {get;set;}
		
		/// <summary>
		/// Alternative terms for the action.
		/// </summary>
		string[] AliasActions {get;set;}
		
		/// <summary>
		/// All actions including primary and aliases.
		/// </summary>
		string[] AllActions {get;}
	}
}
