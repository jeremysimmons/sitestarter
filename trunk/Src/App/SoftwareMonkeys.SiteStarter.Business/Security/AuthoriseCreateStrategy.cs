using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// Used to check whether the current user is authorised to create an entity.
	/// </summary>
	[Strategy("AuthoriseCreate", "IEntity")]
	public class AuthoriseCreateStrategy : BaseAuthoriseStrategy, IAuthoriseCreateStrategy
	{
		/// <summary>
		/// Checks whether the current user is authorised to create an entity of the specified type.
		/// </summary>
		/// <param name="shortTypeName">The type of entity being created.</param>
		/// <returns>A value indicating whether the current user is authorised to create an entity of the specified type.</returns>
		public override bool Authorise(string shortTypeName)
		{
			if (!AuthenticationState.IsAuthenticated)
				return false;
			
			return true;
		}
		
		/// <summary>
		/// Checks whether the current user is authorised to create the provided entity.
		/// </summary>
		/// <param name="entity">The entity to be created.</param>
		/// <returns>A value indicating whether the current user is authorised to create the provided entity.</returns>
		public override bool Authorise(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			if (!AuthenticationState.IsAuthenticated)
				return false;
			
			return true;
		}
		
		/// <summary>
		/// Ensures that the current user is authorised to create an entity of the specified type.
		/// </summary>
		/// <param name="shortTypeName">The type of entity being created.</param>
		public override void EnsureAuthorised(string shortTypeName)
		{
			if (!Authorise(shortTypeName))
				throw new UnauthorisedException("Create", shortTypeName);
		}
		
		/// <summary>
		/// Ensures that the current user is authorised to create the entity provided.
		/// </summary>
		/// <param name="entity">An entity being created.</param>
		public override void EnsureAuthorised(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			if (!Authorise(entity))
				throw new UnauthorisedException("Create", entity.ShortTypeName);
		}
		
		#region New functions
		/// <summary>
		/// Creates a new strategy for authorising the retrieval of an entity of the specified type.
		/// </summary>
		static public IAuthoriseCreateStrategy New<T>()
		{
			return StrategyState.Strategies.Creator.New<IAuthoriseCreateStrategy>("AuthoriseCreate", typeof(T).Name);
		}
		
		/// <summary>
		/// Creates a new strategy for authorising the create of an entity of the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		static public IAuthoriseCreateStrategy New(string typeName)
		{
			return StrategyState.Strategies.Creator.New<IAuthoriseCreateStrategy>("AuthoriseCreate", typeName);
		}
		#endregion
	}
}
