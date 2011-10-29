using System;
using System.Collections.Generic;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// 
	/// </summary>
	[AuthoriseStrategy("References", "IEntity")]
	public class AuthoriseReferencesStrategy : BaseAuthoriseStrategy, IAuthoriseReferencesStrategy
	{
		public AuthoriseReferencesStrategy()
		{
		}
		
		public override bool IsAuthorised(string typeName)
		{
			return AuthenticationState.UserIsInRole("Administrator");
		}
		
		public override bool IsAuthorised(IEntity entity)
		{
			return AuthenticationState.UserIsInRole("Administrator");
		}
		
		public override IEntity Authorise(IEntity entity)
		{
			IEntity output = null;
			
			bool isAuthorised = false;
			using (LogGroup logGroup = LogGroup.StartDebug("Authorising the creation of references on the provided '" + entity.ShortTypeName + "' entity."))
			{
				foreach (PropertyInfo property in entity.GetType().GetProperties())
				{
					if (EntitiesUtilities.IsReference(entity.GetType(), property))
						AuthoriseReferenceStrategy.New(entity, property).Authorise();
				}
				
				isAuthorised = IsAuthorised(entity);
				
				LogWriter.Debug("Is authorised: " + isAuthorised);
				
				if (isAuthorised)
					output = entity;
			}
			
			return output;
		}
		
		public static IAuthoriseReferencesStrategy New(IEntity entity)
		{
			return New(entity.ShortTypeName);
		}
		
		public static IAuthoriseReferencesStrategy New(string typeName)
		{
			IAuthoriseReferencesStrategy strategy = null;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Instantiating a new AuthoriseReferenceStrategy for '" + typeName + "' type."))
			{
				 strategy = StrategyState.Strategies.Creator.New<IAuthoriseReferencesStrategy>("AuthoriseReferences", typeName);
				 
				 LogWriter.Debug("Strategy type: " + (strategy == null ? "[null]" : strategy.GetType().FullName));
			}
			
			return strategy;
		}
	}
}
