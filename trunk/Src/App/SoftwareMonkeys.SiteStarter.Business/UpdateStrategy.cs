using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to update entities.
	/// </summary>
	[Strategy("Update", "IEntity")]
	public class UpdateStrategy : BaseStrategy, IUpdateStrategy
	{
		public UpdateStrategy()
		{
		}
		
		/// <summary>
		/// Updates the provided strategy.
		/// </summary>
		/// <param name="entity">The entity to update.</param>
		public void Update(IEntity entity)
		{
			if (Validate(entity))
			{
				DataAccess.Data.Updater.Update(entity);
			}
		}
		
		/// <summary>
		/// Validates the provided entity against any business rules. Should be overridden by derived strategies.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public virtual bool Validate(IEntity entity)
		{
			return true;
		}
	}
}
