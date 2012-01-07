using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	///
	/// </summary>
	[AuthoriseReferenceStrategy("IEntity", "*", "IEntity", "*")]
	public class AuthoriseReferenceStrategy : BaseAuthoriseReferenceStrategy
	{
		
		public AuthoriseReferenceStrategy()
		{
		}
		
		public override bool IsAuthorised(IEntity entity)
		{
			return AuthenticationState.UserIsInRole("Administrator");
		}
		
		public override bool IsAuthorised(string shortTypeName)
		{
			return AuthenticationState.UserIsInRole("Administrator");
		}
		
		public static IAuthoriseReferenceStrategy New(IEntity entity, PropertyInfo property)
		{
			IAuthoriseReferenceStrategy strategy = null;
			using (LogGroup logGroup = LogGroup.StartDebug("Instantiating a new AuthoriseReferenceStrategy for '" + property.Name + "' property on '" + entity.ShortTypeName + "' type."))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				if (property == null)
					throw new ArgumentNullException("property");
				
				Type referenceType = EntitiesUtilities.GetReferenceType(entity, property);
			
				// If the reference type is not null then continue	
				if (referenceType != null)
				{
					LogWriter.Debug("Referenced type: " + referenceType.FullName);
				
					string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(entity, property);
			
					LogWriter.Debug("Mirror property name: " + mirrorPropertyName);
				
					strategy = New(entity.ShortTypeName, property.Name, referenceType.Name, mirrorPropertyName);
				
					if (strategy != null)
					{
						strategy.SourceEntity = entity;
						strategy.SourceProperty = property.Name;
					}
				}
				// Otherwise skip it because it means the property is not set
				else
					LogWriter.Debug("Reference type is null. Skipping.");
			}
			return strategy;
		}
		
		public static IAuthoriseReferenceStrategy New(string entityTypeName, string propertyName, string referencedTypeName, string mirrorPropertyName)
		{
			IAuthoriseReferenceStrategy strategy = null;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Instantiating a new AuthoriseReferenceStrategy for '" + entityTypeName + "' type to '" + referencedTypeName + "'."))
			{
				LogWriter.Debug("Entity type name: " + entityTypeName);
				LogWriter.Debug("Property name: " + propertyName);
				LogWriter.Debug("Referenced type name: " + referencedTypeName);
				LogWriter.Debug("Mirror property name: " + mirrorPropertyName);
				
				strategy = StrategyState.Strategies.Creator.NewReferenceAuthoriser(entityTypeName, propertyName, referencedTypeName, mirrorPropertyName);
				
				LogWriter.Debug("Strategy type: " + (strategy == null ? "[null]" : strategy.GetType().FullName));
			}
			
			return strategy;
		}
		
		public static IAuthoriseReferenceStrategy New(IEntity entity, string propertyName)
		{
			PropertyInfo property = entity.GetType().GetProperty(propertyName);
			
			if (property == null)
				throw new ArgumentException("Can't find '" + propertyName + "' property on '" + entity.ShortTypeName + "' entity type.");
			
			return New(entity, property);
		}
	}
}
