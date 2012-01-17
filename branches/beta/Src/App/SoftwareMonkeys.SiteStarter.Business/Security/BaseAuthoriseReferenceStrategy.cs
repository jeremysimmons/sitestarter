using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// The base clas of all authorise reference strategies.
	/// </summary>
	public abstract class BaseAuthoriseReferenceStrategy : BaseAuthoriseStrategy, IAuthoriseReferenceStrategy
	{
		private IEntity sourceEntity;
		public IEntity SourceEntity
		{
			get { return sourceEntity; }
			set { sourceEntity = value; }
		}
		
		private string sourceProperty = String.Empty;
		/// <summary>
		/// Gets/sets the name of the name of the property on the source entity containing the referenced entity(ies).
		/// </summary>
		public string SourceProperty
		{
			get { return sourceProperty; }
			set { sourceProperty = value; }
		}
		
		private string mirrorProperty = String.Empty;
		/// <summary>
		/// Gets/sets the name of the name of the property on the referenced entity that reciprocates the reference.
		/// </summary>
		public string MirrorProperty
		{
			get { return mirrorProperty; }
			set { mirrorProperty = value; }
		}
		
		public BaseAuthoriseReferenceStrategy()
		{
		}
		
		public override IEntity[] Authorise(IEntity[] toEntities)
		{
			List<IEntity> entities = new List<IEntity>();
			
			using (LogGroup logGroup = LogGroup.StartDebug("Authorising the creation of a reference on the '" + SourceProperty + "' property of the '" + SourceEntity.ShortTypeName + "' type."))
			{
				LogWriter.Debug("Initial referenced entities: " + toEntities.Length);
				
				foreach (IEntity e in toEntities)
				{
					IEntity authorisedEntity = Authorise(e);
					if (authorisedEntity != null)
						entities.Add(e);
				}
				
				LogWriter.Debug("Total authorised: " + entities.Count.ToString());
			}
			return entities.ToArray();
		}
		
		public void Authorise()
		{
			Authorise(SourceEntity, SourceProperty);
		}
		
		public virtual void Authorise(IEntity fromEntity, string referencePropertyName)
		{
			PropertyInfo property = fromEntity.GetType().GetProperty(referencePropertyName);
			
			if (property == null)
				throw new ArgumentException("Can't find '" + referencePropertyName + "' property on type '" + fromEntity.GetType().FullName + "'.");
			
			Authorise(fromEntity, property);
		}
		
		public virtual void Authorise(IEntity fromEntity, PropertyInfo property)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Checking whether the user is authorised to create a reference on the '" + property.Name + "' property of the '" + fromEntity.GetType().FullName + "' entity type."))
			{
				Type type = EntitiesUtilities.GetReferenceType(fromEntity, property.Name);
				
				ApplyAuthorisation(fromEntity, property);
			}
		}
		
		public virtual bool IsAuthorised()
		{
			return IsAuthorised(SourceEntity, SourceProperty);
		}
		
		public virtual bool IsAuthorised(IEntity entity, string propertyName)
		{
			Type type = EntitiesUtilities.GetReferenceType(entity, propertyName);
			
			return IsAuthorised(type.Name);
		}
		
		public virtual void ApplyAuthorisation(IEntity fromEntity, PropertyInfo property)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Applying authorisation on '" + property.Name + "' property of '" + fromEntity.GetType().FullName + "'."))
			{
				object value = property.GetValue(fromEntity, null);
				object newValue = null;
				
				Type type = fromEntity.GetType();
				
				Type referenceType = EntitiesUtilities.GetReferenceType(fromEntity, property);
				
				if (EntitiesUtilities.IsSingleReference(type, property))
				{
					LogWriter.Debug("Is single reference.");
					
					if (IsAuthorised((IEntity)value))
						newValue = value;
				}
				else
				{
					LogWriter.Debug("Multiple reference.");
					
					if (value != null)
					{
						IEntity[] toEntities = (IEntity[])value;
						
						ArrayList entities = new ArrayList();
						
						LogWriter.Debug("# before: " + toEntities.Length.ToString());
						
						foreach (IEntity entity in toEntities)
						{
							if (IsAuthorised(entity))
								entities.Add(entity);
						}
						
						LogWriter.Debug("# after: " + entities.Count.ToString());
						
						newValue = entities.ToArray(referenceType);
					}
					else
						LogWriter.Debug("Property value is null. Skipping authorisation check.");
				}
				
				if (value != newValue)
				{
					LogWriter.Debug("Setting new value to property.");
					
					property.SetValue(fromEntity, newValue, null);
				}
			}
		}
		
		public override IEntity Authorise(IEntity toEntity)
		{
			IEntity authorisedEntity = null;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Authorising the creation of a reference on the '" + SourceProperty + "' property of the '" + SourceEntity.ShortTypeName + "' type to referenced entity '" + toEntity.ToString() + "' of type '"  + toEntity.ShortTypeName + "'."))
			{
				if (IsAuthorised(toEntity))
				{
					LogWriter.Debug("Entity IS authorised.");
					
					authorisedEntity = toEntity;
				}
				else
					LogWriter.Debug("Entity is NOT authorised.");
			}
			
			return authorisedEntity;
		}
	}
}
