using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Diagnostics;

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
		public override bool IsAuthorised(string shortTypeName)
		{
			bool isAuthorised = false;
			using (LogGroup logGroup = LogGroup.Start("Checking whether current user is authorised to update entities of type '" + shortTypeName + "'.", NLog.LogLevel.Debug))
			{
				if (!RequireAuthorisation)
					isAuthorised = true;
				else
				{
				if (AuthenticationState.IsAuthenticated
				    && AuthenticationState.UserIsInRole("Administrator"))
					isAuthorised = true;
				}
				
				LogWriter.Debug("Is authorised: " + isAuthorised);
			}
			return isAuthorised;
		}
		
		/// <summary>
		/// Checks whether the current user is authorised to update the provided entity.
		/// </summary>
		/// <param name="entity">The entity to be updated.</param>
		/// <returns>A value indicating whether the current user is authorised to update the provided entity.</returns>
		public override bool IsAuthorised(IEntity entity)
		{
			bool isAuthorised = false;
			using (LogGroup logGroup = LogGroup.Start("Checking whether the current user is authorised to update the provided entity.", NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				if (!RequireAuthorisation)
					isAuthorised = true;
				else
				{
					isAuthorised = IsAuthorised(entity.ShortTypeName);
				
					AuthoriseReferencesStrategy.New(entity).Authorise(entity);
				}
				
				LogWriter.Debug("Is authorised: " + isAuthorised);
				
			}
			return isAuthorised;
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
		
		/// <summary>
		/// Creates a new strategy for authorising the updating the provided entity.
		/// </summary>
		/// <param name="entity">The entity involved in the strategy.</param>
		static public IAuthoriseUpdateStrategy New(IEntity entity)
		{
			return New(entity.ShortTypeName);
		}
			#endregion

	}
}
