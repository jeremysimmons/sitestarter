using System;
using System.Collections.Generic;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Diagnostics;
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
		public abstract bool IsAuthorised(string shortTypeName);
		
		/// <summary>
		/// Checks whether the current user is authorised to perform an operation involving the provided entity.
		/// </summary>
		/// <param name="entity">The entity involved in the operation in which authorisation is required.</param>
		/// <returns>A value indicating whether the current user is authorised to perform an operation involving the provided entity.</returns>
		public abstract bool IsAuthorised(IEntity entity);
		
		/// <summary>
		/// Checks whether the current user is authorised to perform an operation involving to the provided entity.
		/// </summary>
		/// <param name="entity">The entity involved in the operation in which authorisation is required.</param>
		/// <returns>The authorised entity. (Or null if not authorised)</returns>
		public virtual IEntity Authorise(IEntity entity)
		{
			if (IsAuthorised(entity))
				return entity;
			else
				return null;
		}
		
		/// <summary>
		/// Ensures that the current user is authorised to perform an operation involving an entity of the specified type and throws an exception if unauthorised.
		/// </summary>
		/// <param name="shortTypeName">The type of entity involved in the operation in which authorisation is required.</param>
		public virtual void EnsureAuthorised(string shortTypeName)
		{
			if (!IsAuthorised(shortTypeName))
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
			
			if (!IsAuthorised(entity))
				throw new UnauthorisedException(GetRestrictedAction(), entity);
		}
		
		/// <summary>
		/// Checks whether the current user is authorised to perform the desired action.
		/// </summary>
		/// <param name="entities">The entities involved in the strategy.</param>
		/// <returns>The provided entities with unauthorised entitiese removed.</returns>
		public virtual IEntity[] Authorise(IEntity[] entities)
		{
			List<IEntity> output = new List<IEntity>();
			
			using (LogGroup logGroup = LogGroup.StartDebug("Checking whether the current user is authorised to perform the desired action with the provided entities."))
			{
				if (IsAuthorised(TypeName))
				{
					foreach (IEntity entity in entities)
					{
						if (IsAuthorised(entity))
							output.Add(entity);
					}
				}
				
			}
			
			return output.ToArray();
		}
		
		/// <summary>
		/// Retrieves the short type name specified by the Strategy attribute.
		/// </summary>
		/// <returns></returns>
		public virtual string GetRestrictedAction()
		{
			return Action.Replace("Authorise", "");
		}
		
	}
}
