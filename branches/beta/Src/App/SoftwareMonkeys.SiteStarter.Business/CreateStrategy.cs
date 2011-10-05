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
			return (T)Create();
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
			
			// Assign an activation strategy
			entity.Activator = ActivateStrategy.New(entity);
			
			// Assign a validation strategy
			entity.Validator = ValidateStrategy.New(entity);
			
			return entity;
		}
		
		#region New functions
		/// <summary>
		/// Creates a new strategy for creating an instance of the specified type.
		/// </summary>
		/// <param name="requireAuthorisation">A flag indicating whether the strategy requires the current user to be authorised.</param>
		static public ICreateStrategy New<T>(bool requireAuthorisation)
		{
			ICreateStrategy strategy = New<T>();
			strategy.RequireAuthorisation = requireAuthorisation;
			return strategy;
		}
		
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
