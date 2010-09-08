using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to save entities.
	/// </summary>
	[Strategy("Save", "IEntity")]
	public class SaveStrategy : BaseStrategy, ISaveStrategy
	{
		public SaveStrategy()
		{
		}
		
		/// <summary>
		/// Saves the provided strategy.
		/// </summary>
		/// <param name="entity">The entity to save.</param>
		public void Save(IEntity entity)
		{
			if (Validate(entity))
			{
				DataAccess.Data.Saver.Save(entity);
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
