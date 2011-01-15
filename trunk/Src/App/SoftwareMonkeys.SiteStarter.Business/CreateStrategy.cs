using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business.Security;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to create new instances of entities.
	/// </summary>
	[Strategy("Create", "IEntity")]
	public class CreateStrategy : BaseStrategy, ICreateStrategy
	{
		/// <summary>
		/// Creates a new instance of the specified type of entity.
		/// </summary>
		public virtual T Create<T>()
			where T : IEntity
		{
			T entity = (T)Activator.CreateInstance(typeof(T));
			
			if (RequireAuthorisation)
				AuthoriseCreateStrategy.New(TypeName).EnsureAuthorised(TypeName);
			
			return entity;
		}
		
		/// <summary>
		/// Creates a new instance of the specified type of entity.
		/// </summary>
		/// <param name="shortTypeName">The short name of the type of entity to create an instance of.</param>
		public virtual IEntity Create()
		{
			Type type = EntityState.GetType(TypeName);
			
			if (RequireAuthorisation)
				AuthoriseCreateStrategy.New(TypeName).EnsureAuthorised(TypeName);
			
			IEntity entity = (IEntity)Activator.CreateInstance(type);
			
			if (RequireAuthorisation)
				AuthoriseCreateStrategy.New(TypeName).EnsureAuthorised(entity);
			
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
			return New(typeName, true);
		}
		
		/// <summary>
		/// Creates a new strategy for creating an instance of the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		static public ICreateStrategy New(string typeName, bool requiresAuthorisation)
		{
			
			ICreateStrategy strategy = StrategyState.Strategies.Creator.NewCreator(typeName);
			
			strategy.RequireAuthorisation = requiresAuthorisation;
			
			return strategy;
		}
		#endregion
	}
}
