using System;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// Thrown when an operation is attemped without authorisation.
	/// </summary>
	[Serializable]
	public class UnauthorisedException : Exception
	{
		public UnauthorisedException(string action, string shortTypeName) : base("The current user is not authorised to perform the action '" + action + "' with an entity of type '" + shortTypeName + "'.")
		{}
	}
}
