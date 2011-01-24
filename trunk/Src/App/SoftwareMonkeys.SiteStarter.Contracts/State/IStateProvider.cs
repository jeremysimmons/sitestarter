using System;

namespace SoftwareMonkeys.SiteStarter.State
{
	/// <summary>
	/// Defines the interface of all state provider components.
	/// </summary>
	public interface IStateProvider
	{
		string PhysicalApplicationPath { get;set; }
	}
}
