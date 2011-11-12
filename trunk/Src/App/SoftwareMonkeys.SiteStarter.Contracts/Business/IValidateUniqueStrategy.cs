using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Defines the interface of all "unique validation" (ie. ensuring a specific property of an entity is unique) strategies.
	/// </summary>
	public interface IValidateUniqueStrategy : IValidatePropertyStrategy
	{
		
	}
}
