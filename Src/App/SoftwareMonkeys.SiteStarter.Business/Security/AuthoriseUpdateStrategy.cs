using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// Used to check whether the current user is authorised to update an entity.
	/// </summary>
	[Strategy("AuthoriseUpdate", "IEntity")]
	public class AuthoriseUpdateStrategy : BaseAuthoriseStrategy, IAuthoriseUpdateStrategy
	{
		
		/// <summary>
		/// Checks whether the current user is authorised to update an entity of the specified type.
		/// </summary>
		/// <param name="shortTypeName">The type of entity being updated.</param>
		/// <returns>A value indicating whether the current user is authorised to update an entity of the specified type.</returns>
		public override bool Authorise(string shortTypeName)
		{
			if (!AuthenticationState.IsAuthenticated)
				return false;
			
			return true;
		}
		
		/// <summary>
		/// Ensures that the current user is authorised to update an entity of the specified type.
		/// </summary>
		/// <param name="shortTypeName">The type of entity being updated.</param>
		public override void EnsureAuthorised(string shortTypeName)
		{
			if (!Authorise(shortTypeName))
				throw new UnauthorisedException("Update", shortTypeName);
		}
		
		/// <summary>
		/// Checks whether the current user is authorised to update the provided entity.
		/// </summary>
		/// <param name="entity">The entity to be updated.</param>
		/// <returns>A value indicating whether the current user is authorised to update the provided entity.</returns>
		public override bool Authorise(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			if (!AuthenticationState.IsAuthenticated)
				return false;
			
			return true;
		}
		
		/// <summary>
		/// Ensures that the current user is authorised to update the entity provided.
		/// </summary>
		/// <param name="entity">An entity being updated.</param>
		public override void EnsureAuthorised(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			if (!Authorise(entity))
				throw new UnauthorisedException("Update", entity.ShortTypeName);
		}
		
		#region New functions
		/// <summary>
		/// Creates a new strategy for authorising the updating of the specified type.
		/// </summary>
		static public IAuthoriseUpdateStrategy New<T>()
		{
			return StrategyState.Strategies.Creator.New<IAuthoriseUpdateStrategy>("AuthoriseUpdate", typeof(T).Name);
		}
		
		/// <summary>
		/// Creates a new strategy for authorising the updating the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		static public IAuthoriseUpdateStrategy New(string typeName)
		{
			return StrategyState.Strategies.Creator.New<IAuthoriseUpdateStrategy>("AuthoriseUpdate", typeName);
		}
		
		// TODO: Remove. Shouldn't be needed
		/*/// <summary>
		/// Creates a new strategy for authorising the updating of the specified type.
		/// </summary>
		/// <param name="requireAuthorisation">A value indicating whether the strategy requires authorisation.</param>
		static public IAuthoriseUpdateStrategy New<T>(bool requireAuthorisation)
		{
			IAuthoriseUpdateStrategy strategy = StrategyState.Strategies.Creator.New<IAuthoriseUpdateStrategy>("AuthoriseUpdate", typeof(T).Name);
			strategy.RequireAuthorisation = requireAuthorisation;
			return strategy;
		}
		
		/// <summary>
		/// Creates a new strategy for authorising the updating the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		/// <param name="requireAuthorisation">A value indicating whether the strategy requires authorisation.</param>
		static public IAuthoriseUpdateStrategy New(string typeName, bool requireAuthorisation)
		{
			IAuthoriseUpdateStrategy strategy = StrategyState.Strategies.Creator.New<IAuthoriseUpdateStrategy>("AuthoriseUpdate", typeName);
			strategy.RequireAuthorisation = requireAuthorisation;
			return strategy;
		}*/
		#endregion

	}
}
