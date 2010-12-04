using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business.Security;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to create new instances of entities.
	/// </summary>
	public class CreateStrategy : BaseStrategy, ICreateStrategy
	{
		/// <summary>
		/// Creates a new instance of the specified type of entity.
		/// </summary>
		public T Create<T>()
			where T : IEntity
		{
			T entity = (T)Activator.CreateInstance(typeof(T));
			
			return entity;
		}
		
		/// <summary>
		/// Creates a new instance of the specified type of entity.
		/// </summary>
		/// <param name="shortTypeName">The short name of the type of entity to create an instance of.</param>
		public IEntity Create(string shortTypeName)
		{
			Type type = EntitiesUtilities.GetType(shortTypeName);
			
			if (RequireAuthorisation)
				AuthoriseCreateStrategy.New(shortTypeName).EnsureAuthorised(shortTypeName);
			
			IEntity entity = (IEntity)Activator.CreateInstance(type);
			
			return entity;
		}
		
		#region New functions
		/// <summary>
		/// Creates a new strategy for creating an instance of the specified type.
		/// </summary>
		static public ICreateStrategy New<T>()
		{
			return StrategyState.Strategies.Creator.NewCreator(typeof(T).Name);
		}
		
		/// <summary>
		/// Creates a new strategy for creating an instance of the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		static public ICreateStrategy New(string typeName)
		{
			return StrategyState.Strategies.Creator.NewCreator(typeName);
		}
		#endregion
	}
}
