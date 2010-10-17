using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// Used to check whether the current user is authorised to index an entity.
	/// </summary>
	[Strategy("AuthoriseIndex", "IEntity")]
	public class AuthoriseIndexStrategy : BaseAuthoriseStrategy, IAuthoriseIndexStrategy
	{
		
		/// <summary>
		/// Checks whether the current user is authorised to index an entity of the specified type.
		/// </summary>
		/// <param name="shortTypeName">The type of entity being indexed.</param>
		/// <returns>A value indicating whether the current user is authorised to index an entity of the specified type.</returns>
		public override bool Authorise(string shortTypeName)
		{
			if (!AuthenticationState.IsAuthenticated)
				return false;
			
			return true;
		}
		
		/// <summary>
		/// Ensures that the current user is authorised to index an entity of the specified type.
		/// </summary>
		/// <param name="shortTypeName">The type of entity being indexed.</param>
		public override void EnsureAuthorised(string shortTypeName)
		{
			if (!Authorise(shortTypeName))
				throw new UnauthorisedException("Index", shortTypeName);
		}
		
		/// <summary>
		/// Checks whether the current user is authorised to index the provided entities.
		/// </summary>
		/// <param name="entities">The entities being indexed.</param>
		/// <returns>A value indicating whether the current user is authorised to index the provided entities.</returns>
		public bool Authorise(ref IEntity[] entities)
		{
			if (!AuthenticationState.IsAuthenticated)
				return false;
			
			Collection<IEntity> collection = new Collection<IEntity>();
			
			for (int i = 0; i < entities.Length; i++)
			{
				if (!Authorise(entities[i]))
				{
					collection.RemoveAt(i);
					i--;
				}
			}
			
			entities = collection.ToArray();
			
			return true;
		}
		
		/// <summary>
		/// Checks whether the current user is authorised to index the provided entity.
		/// </summary>
		/// <param name="entity">An entity in the index.</param>
		/// <returns>A value indicating whether the current user is authorised to access an index of entities including the one provided.</returns>
		public override bool Authorise(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			return AuthoriseRetrieveStrategy.New(entity.ShortTypeName).Authorise(entity);
		}
		
		/// <summary>
		/// Ensures that the current user is authorised to access an index of entities including the one provided.
		/// </summary>
		/// <param name="entity">An entity involved in the index.</param>
		public override void EnsureAuthorised(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			if (!Authorise(entity))
				throw new UnauthorisedException("Index", entity.ShortTypeName);
		}
		
		/// <summary>
		/// Ensures that the current user is authorised to access an index of entities including the one provided.
		/// </summary>
		/// <param name="entities">The entities in the index.</param>
		public void EnsureAuthorised(IEntity[] entities)
		{
			foreach (IEntity entity in entities)
				EnsureAuthorised(entity);
		}
		
		/// <summary>
		/// Ensures that the current user is authorised to access an index of entities including the one provided.
		/// </summary>
		/// <param name="entities">The entities in the index.</param>
		public void EnsureAuthorised<T>(T[] entities)
			where T : IEntity
		{
			foreach (T entity in entities)
				EnsureAuthorised(entity);
		}
		
		#region New functions
		/// <summary>
		/// Creates a new strategy for authorising the indexing of the specified type.
		/// </summary>
		static public IAuthoriseIndexStrategy New<T>()
		{
			return StrategyState.Strategies.Creator.New<IAuthoriseIndexStrategy>("AuthoriseIndex", typeof(T).Name);
		}
		
		/// <summary>
		/// Creates a new strategy for authorising the indexing the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		static public IAuthoriseIndexStrategy New(string typeName)
		{
			return StrategyState.Strategies.Creator.New<IAuthoriseIndexStrategy>("AuthoriseIndex", typeName);
		}
		#endregion
	}
}
