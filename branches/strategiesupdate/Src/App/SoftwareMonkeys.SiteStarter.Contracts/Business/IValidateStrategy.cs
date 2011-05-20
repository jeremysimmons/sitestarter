using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Defines the interface of all validation strategies.
	/// </summary>
	public interface IValidateStrategy : IStrategy
	{
		/// <summary>
		/// Validates the provided entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		bool Validate(IEntity entity);
	}
}
