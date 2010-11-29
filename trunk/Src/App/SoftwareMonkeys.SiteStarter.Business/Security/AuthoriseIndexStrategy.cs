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
			return AuthoriseRetrieveStrategy.New(shortTypeName).Authorise(shortTypeName);
			
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
		public virtual bool Authorise(ref IEntity[] entities)
		{
			int originalCount = entities.Length;
			
			string typeName = entities.GetType().GetElementType().Name;
			
			if (Authorise(typeName))
			{
				Collection<IEntity> collection = new Collection<IEntity>();
				collection.AddRange(entities);
				
				for (int i = 0; i < collection.Count; i++)
				{
					if (!Authorise(collection[i]))
					{
						collection.RemoveAt(i);
						i--;
					}
				}
				
				entities = collection.ToArray();

				return true;
			}
			else
				return false;
		}
		
		/// <summary>
		/// Checks whether the current user is authorised to view provided entity in an index.
		/// </summary>
		/// <param name="entity">An entity in the index.</param>
		/// <returns>A value indicating whether the current user is authorised to access an index of entities including the one provided.</returns>
		public override bool Authorise(IEntity entity)
		{
			return AuthoriseRetrieveStrategy.New(entity.ShortTypeName).Authorise(entity);
		}
		
		/// <summary>
		/// Ensures that the current user is authorised to access an index of entities including the one provided.
		/// </summary>
		/// <param name="entity">An entity involved in the index.</param>
		public override void EnsureAuthorised(IEntity entity)
		{
			AuthoriseRetrieveStrategy.New(entity.ShortTypeName).EnsureAuthorised(entity);
		}
		
		/// <summary>
		/// Ensures that the current user is authorised to access an index of entities including the one provided.
		/// </summary>
		/// <param name="entities">The entities in the index.</param>
		public virtual void EnsureAuthorised(ref IEntity[] entities)
		{
			if (entities == null)
				throw new ArgumentNullException("entities");
			
			if (entities.Length > 0)
			{
				string shortTypeName = entities[0].ShortTypeName;
				
				if (!Authorise(ref entities))
					throw new UnauthorisedException("Index", shortTypeName);
				//throw new UnauthorisedException("Index", this.TypeName);
			}
		}
		
		/// <summary>
		/// Ensures that the current user is authorised to access an index of entities including the one provided.
		/// </summary>
		/// <param name="entities">The entities in the index.</param>
		public virtual void EnsureAuthorised<T>(ref T[] entities)
			where T : IEntity
		{
			IEntity[] e = Collection<IEntity>.ConvertAll(entities);
			
			EnsureAuthorised(ref e);
			
			entities = Collection<T>.ConvertAll(e);
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
