using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Defines the interface of all save strategies that enforce a unique property rule.
	/// </summary>
	public interface IUniqueSaveStrategy : ISaveStrategy
	{
	}
}
