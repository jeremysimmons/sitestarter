using System;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// 
	/// </summary>
	[AuthoriseStrategy("Reference", "IEntity")]
	public class AuthoriseReferenceStrategy : BaseAuthoriseStrategy, IAuthoriseReferenceStrategy
	{
		public AuthoriseReferenceStrategy()
		{
		}
		
		public override bool Authorise(string typeName)
		{
			// Use the authorise update strategy by default
			return AuthoriseUpdateStrategy.New(typeName).Authorise(typeName);
		}
		
		public override bool Authorise(IEntity entity)
		{
			// Use the authorise update strategy by default
			return AuthoriseUpdateStrategy.New(entity.ShortTypeName).Authorise(entity);
		}
		
		public bool Authorise(IEntity fromEntity, IEntity toEntity)
		{
			// Use the authorise update strategy by default
			return AuthoriseUpdateStrategy.New(toEntity.ShortTypeName).Authorise(toEntity);
		}
		
		public void Authorise(IEntity fromEntity, string referencePropertyName)
		{
			PropertyInfo property = fromEntity.GetType().GetProperty(referencePropertyName);
			
			if (property == null)
				throw new ArgumentException("Can't find '" + referencePropertyName + "' property on type '" + fromEntity.GetType().FullName + "'.");
			
			Authorise(fromEntity, property);
		}
		
		public void Authorise(IEntity fromEntity, PropertyInfo property)
		{
			Type type = EntitiesUtilities.GetReferenceType(fromEntity.GetType(), property.Name);
			
			object value = property.GetValue(fromEntity, null);
			
			bool isAuthorised = false;
			
			if (value is IEntity)
			{
				// Check that the user is authorised to reference the entity
				isAuthorised = !AuthoriseReferenceStrategy.New(type.Name).Authorise(fromEntity, (IEntity)value);
			}
			else
			{
				// The user is authorised if they're authorised to reference the type in general
				isAuthorised = !AuthoriseReferenceStrategy.New(type.Name).Authorise(type.Name);
				
				// Check the user authorised to reference each of the referenced entities and remove those that aren't authorised
				value = AuthoriseReferenceStrategy.New(type.Name).Authorise(fromEntity, Collection<IEntity>.ConvertAll(value));
			}
			
			// Set the reference property to exclude the unauthorised references
			property.SetValue(fromEntity, new Collection<IEntity>(value).ToArray(type), null);
		}
		
		public IEntity[] Authorise(IEntity fromEntity, IEntity[] toEntities)
		{
			// If no referenced entities were provided then return the empty array
			if (toEntities.Length == 0)
				return toEntities;
			else
			{
				Type type = toEntities[0].GetType();
				
				// Use the authorise update strategy by default
				return AuthoriseUpdateStrategy.New(type.Name).Authorise(toEntities);
			}
		}
		
		public static IAuthoriseReferenceStrategy New(string shortTypeName)
		{
			return StrategyState.Strategies.Creator.New<IAuthoriseReferenceStrategy>("AuthoriseReference", shortTypeName);
		}
		
		public static IAuthoriseReferenceStrategy New(IEntity entity)
		{
			return New(entity.ShortTypeName);
		}
		
		public static IAuthoriseReferenceStrategy New(IEntity entity, PropertyInfo property)
		{
			return New(EntitiesUtilities.GetReferenceType(entity.GetType(), property).Name);
		}
		
		
		public static IAuthoriseReferenceStrategy New(IEntity entity, string propertyName)
		{
			return New(EntitiesUtilities.GetReferenceType(entity.GetType(), propertyName).Name);
		}
	}
}
