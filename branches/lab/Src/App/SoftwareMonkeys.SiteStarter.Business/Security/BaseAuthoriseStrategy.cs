using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// The base of all authorise strategies.
	/// </summary>
	public abstract class BaseAuthoriseStrategy : BaseStrategy, IAuthoriseStrategy
	{		
		/// <summary>
		/// Checks whether the current user is authorised to perform an operation involving an entity of the specified type.
		/// </summary>
		/// <param name="shortTypeName">The type of entity involved in the operation in which authorisation is required.</param>
		/// <returns>A value indicating whether the current user is authorised to perform an operation involving an entity of the specified type.</returns>
		public abstract bool Authorise(string shortTypeName);
		
		/// <summary>
		/// Checks whether the current user is authorised to perform an operation involving to the provided entity.
		/// </summary>
		/// <param name="entity">The entity involved in the operation in which authorisation is required.</param>
		/// <returns>A value indicating whether the current user is authorised to perform an operation involving the provided entity.</returns>
		public abstract bool Authorise(IEntity entity);
		
		/// <summary>
		/// Ensures that the current user is authorised to perform an operation involving an entity of the specified type and throws an exception if unauthorised.
		/// </summary>
		/// <param name="shortTypeName">The type of entity involved in the operation in which authorisation is required.</param>
		public virtual void EnsureAuthorised(string shortTypeName)
		{
			if (!Authorise(shortTypeName))
				throw new UnauthorisedException(GetRestrictedAction(), shortTypeName);
		}
		
		/// <summary>
		/// Ensures that the current user is authorised to perform an operation involving the provided entity and throws an exception if unauthorised.
		/// </summary>
		/// <param name="entity">The entity involved in the operation in which authorisation is required.</param>
		public virtual void EnsureAuthorised(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			if (!Authorise(entity))
				throw new UnauthorisedException(GetRestrictedAction(), entity);
		}
		
		/// <summary>
		/// Retrieves the short type name specified by the Strategy attribute.
		/// </summary>
		/// <returns></returns>
		public virtual string GetRestrictedAction()
		{
			StrategyInfo info = new StrategyInfo(this);
			
			return info.Action.Replace("Authorise", "");
		}
	}
}
