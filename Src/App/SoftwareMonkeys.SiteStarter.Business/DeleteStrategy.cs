using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Business.Security;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to delete an entity.
	/// </summary>
	[Strategy("Delete", "IEntity")]
	public class DeleteStrategy : BaseStrategy, IDeleteStrategy
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
			
			if (RequireAuthorisation)
				AuthoriseDeleteStrategy.New(entity.ShortTypeName).EnsureAuthorised(entity);
			
			DataAccess.Data.Deleter.Delete(entity);
		}
		
		
		#region New functions
		/// <summary>
		/// Creates a new strategy for deleting the specified type.
		/// </summary>
		static public IDeleteStrategy New<T>()
		{
			return StrategyState.Strategies.Creator.NewDeleter(typeof(T).Name);
		}
		
		/// <summary>
		/// Creates a new strategy for deleting the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		static public IDeleteStrategy New(string typeName)
		{
			return StrategyState.Strategies.Creator.NewDeleter(typeName);
		}
		#endregion
	}
}
