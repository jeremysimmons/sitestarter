using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Defines the base class used by all business strategy components.
	/// </summary>
	[Strategy(false)]
	public class BaseStrategy : IStrategy
	{		
		public BaseStrategy()
		{
		}
	}
}
