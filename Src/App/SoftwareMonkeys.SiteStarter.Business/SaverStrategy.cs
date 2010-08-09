using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// A strategy used to save an entity to the data store.
	/// </summary>
	public class SaverStrategy<T> : BaseStrategy<T>
		where T : IEntity
	{
		public SaverStrategy()
		{
		}
		
		/// <summary>
		/// Saves the provided entity into the data store.
		/// </summary>
		/// <param name="entity">The entity to save.</param>
		public void Save(T entity)
		{
			DataStore.Saver.Save(entity);
		}
	}
}
