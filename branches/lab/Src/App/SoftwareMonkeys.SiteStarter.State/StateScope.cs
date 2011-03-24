using System;

namespace SoftwareMonkeys.SiteStarter.State
{
	/// <summary>
	/// Defines all possible scopes for a state variable.
	/// </summary>
	public enum StateScope
	{
		Application = 1,
		Session = 2,
		Operation = 3
	}
}
