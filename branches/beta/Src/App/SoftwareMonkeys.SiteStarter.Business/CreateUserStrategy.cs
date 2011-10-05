using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to create new instances of entities.
	/// </summary>
	[Strategy("Create", "User")]
	public class CreateUserStrategy : CreateStrategy
	{
		/// <summary>
		/// Creates a new instance of the specified type of entity.
		/// </summary>
		public override T Create<T>()
		{
			return (T)Create();
		}
		
		/// <summary>
		/// Creates a new instance of the specified type of entity.
		/// </summary>
		/// <param name="shortTypeName">The short name of the type of entity to create an instance of.</param>
		public override IEntity Create()
		{
			Type type = EntityState.GetType(TypeName);
			
			if (RequireAuthorisation)
				AuthoriseCreateStrategy.New(TypeName).EnsureAuthorised(TypeName);
			
			User user = (User)base.Create();
			user.IsApproved = Config.Application.Settings.GetBool("AutoApproveNewUsers");
			
			return user;
		}
		
		#region New functions		
		/// <summary>
		/// Creates a new strategy for creating an instance of the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		static public ICreateStrategy New()
		{
			return New(true);
		}
		
		/// <summary>
		/// Creates a new strategy for creating an instance of the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		static public ICreateStrategy New(bool requiresAuthorisation)
		{
			
			ICreateStrategy strategy = StrategyState.Strategies.Creator.NewCreator("User");
			
			strategy.RequireAuthorisation = requiresAuthorisation;
			
			return strategy;
		}
		#endregion
	}
}
