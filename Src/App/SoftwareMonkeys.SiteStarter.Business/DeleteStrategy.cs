using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Business;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to delete an entity.
	/// </summary>
	[Strategy("Delete", "IEntity")]
	public class DeleteStrategy : IDeleteStrategy
	{
		public DeleteStrategy()
		{
		}
		
		/// <summary>
		/// Deletes the provided entity.
		/// </summary>
		/// <param name="entity">The entity to delete.</param>
		public void Delete(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			DataAccess.Data.Deleter.Delete(entity);
		}
	}
}
