using System;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// Thrown when an operation is attemped without authorisation.
	/// </summary>
	public class UnauthorisedException : ApplicationException
	{
		public UnauthorisedException(string action, string shortTypeName) : base("The current user is not authorised to perform the action '" + action + "' with an entity of type '" + shortTypeName + "'.")
		{}
	}
}
