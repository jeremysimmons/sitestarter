using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// A business strategy used for retrieveing a single entity.
	/// </summary>
	public class RetrieverStrategy<T> : BaseStrategy<T>, IRetrieverStrategy<T>
		where T : IEntity
	{
		#region Default singleton
		static private RetrieverStrategy<T> _default;
		/// <summary>
		/// Gets the default entity retrieveer strategy used.
		/// </summary>
		static public RetrieverStrategy<T> Default
		{
			get {
				if (_default == null)
					_default = StrategyFactory.NewRetrieverStrategy<T>();
				return _default; }
		}
		#endregion
		
		public RetrieverStrategy()
		{
		}
		
		/// <summary>
		/// Retrieves the entity of the specified type with the provided unique key.
		/// </summary>
		/// <param name="uniqueKey">The unique key of the entity to retrieve.</param>
		/// <returns>The entity with the provided unique key.</returns>
		public T Retrieve(string uniqueKey)
		{
			return Retrieve("UniqueKey", EntitiesUtilities.FormatUniqueKey(uniqueKey));
		}
		
		/// <summary>
		/// Retrieves the entity of the specified type with the provided ID.
		/// </summary>
		/// <param name="entityID">The ID of the entity to retrieve.</param>
		/// <returns>The entity with the provided ID.</returns>
		public T Retrieve(Guid entityID)
		{
			return Retrieve("ID", entityID);
		}
		
		/// <summary>
		/// Retrieves the entity of the specified type with the specified property matching the provided value.
		/// </summary>
		/// <param name="propertyName">The name of the property to match to the provided value.</param>
		/// <param name="value">The name of the value to match to the specified property.</param>
		/// <returns>The entity with the specified property matching the provided value.</returns>
		public T Retrieve(string propertyName, object value)
		{
			return DataStore.Reader.GetEntity<T>(propertyName, value);
		}
	}
}
