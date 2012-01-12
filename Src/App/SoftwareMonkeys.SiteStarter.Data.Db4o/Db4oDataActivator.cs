using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o
{
	/// <summary>
	/// Description of Db4oDataActivator.
	/// </summary>
	public class Db4oDataActivator : DataActivator
	{
		/// <summary>
		/// Sets the data provider and data store of the adapter.
		/// </summary>
		/// <param name="provider">The data provider of adapter.</param>
		/// <param name="store">The data store to tie the adapter to, or [null] to automatically select store.</param>
		public Db4oDataActivator(Db4oDataProvider provider, Db4oDataStore store)
		{
			Initialize(provider, store);
		}
		
		public override void ActivateReference(EntityReference reference)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Activating reference."))
			{
				if (reference.Type1Name == string.Empty)
					throw new ArgumentNullException("reference.Type1Name");
				if (reference.Type2Name == string.Empty)
					throw new ArgumentNullException("reference.Type2Name");
				
				LogWriter.Debug("Type 1: " + reference.Type1Name);
				LogWriter.Debug("Type 2: " + reference.Type2Name);
				LogWriter.Debug("ID 1: " + reference.Entity1ID);
				LogWriter.Debug("ID 2: " + reference.Entity2ID);
				
				Type type1 = EntityState.GetType(reference.Type1Name);
				Type type2 = EntityState.GetType(reference.Type2Name);
				
				LogWriter.Debug("Full type 1: " + type1.ToString());
				LogWriter.Debug("Full type 2: " + type2.ToString());
				
				if (reference.Entity1ID == Guid.Empty || reference.Entity2ID == Guid.Empty)
				{
					LogWriter.Debug("Skipped activation because both IDs weren't found.");
				}
				else
				{
					// If the source entity is not yet set
					if (reference.SourceEntity == null)
					{
						IEntity entity1 = DataAccess.Data.Reader.GetEntity(
							type1,
							"ID",
							reference.Entity1ID);
						
						// TODO: Check if exceptions should be thrown when the entity isn't found
						// Currently references are simply skipped if entity isn't found
						//if (entity1 == null)
						//	throw new Exception("Can't find '" + type1.Name + "' entity with ID '" + reference.Entity1ID.ToString() + "'. Make sure the entity has been saved BEFORE trying to reference it.");
						
						if (entity1 != null)
							reference.SourceEntity = entity1;
					}
					
					// If the reference entity is not yet set
					if (reference.ReferenceEntity == null)
					{
						IEntity entity2 = DataAccess.Data.Reader.GetEntity(
							type2,
							"ID",
							reference.Entity2ID);
						
						// TODO: Check if exceptions should be thrown when the entity isn't found
						// Currently references are simply skipped if entity isn't found
						//if (entity2 == null)
						//	throw new Exception("Can't find '" + type2.Name + "' entity with ID '" + reference.Entity2ID.ToString() + "'. Make sure the entity has been saved BEFORE trying to reference it.");

						if (entity2 != null)
							reference.ReferenceEntity = entity2;
					}
				}
			}
		}
		
		public override void Activate(IEntity[] entities)
		{
			Activate(entities, 1);
		}
		
		public override void Activate(IEntity[] entities, int depth)
		{
			foreach (IEntity entity in entities)
			{
				Activate(entity, depth);
			}
		}
		
		public override void Activate(IEntity entity)
		{
			Activate(entity, 1);
		}
		
		public override void Activate(IEntity entity, int depth)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			using (LogGroup logGroup = LogGroup.StartDebug("Activating the references on type: " + entity.GetType().ToString()))
			{
				Type entityType = entity.GetType();
				
				EntityReferenceCollection references = Provider.Referencer.GetReferences(entity);
				
				foreach (PropertyInfo property in entity.GetType().GetProperties())
				{
					if (EntitiesUtilities.IsReference(entityType, property.Name, property.PropertyType))
					{
						LogWriter.Debug("Found reference property: " + property.Name);
						LogWriter.Debug("Property type: " + property.PropertyType.ToString());
						
						Activate(entity, property.Name, property.PropertyType, depth, references);
					}
				}
			}
		}
		
		public override void Activate(IEntity entity, string propertyName, Type propertyType)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Activating the '" + propertyName + "' property on the '" + entity.ShortTypeName + "' type."))
			{
				Activate(entity, propertyName, propertyType, 1);
			}
		}
		
		public override void Activate(IEntity entity, string propertyName, Type propertyType, int depth)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Activating the '" + propertyName + "' property on the '" + entity.ShortTypeName + "' type."))
			{
				Type referenceType = EntitiesUtilities.GetReferenceType(entity, propertyName);
				
				// If the reference type is not null then activate the property
				if (referenceType != null)
				{
					EntityReferenceCollection references = Provider.Referencer.GetReferences(entity.GetType(), entity.ID, propertyName, referenceType, false);
					
					Activate(entity, propertyName, propertyType, depth, references);
				}
				// Otherwise skip it because a null reference type means its a dynamically typed property which has not been set
			}
		}
		
		public void Activate(IEntity entity, string propertyName, Type propertyType, int depth, EntityReferenceCollection references)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Activating the '" + propertyName + "' property on the '" + entity.ShortTypeName + "' type."))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				if (propertyName == null || propertyName == String.Empty)
					throw new ArgumentException("propertyName", "Cannot be null or String.Empty.");
				
				PropertyInfo property = EntitiesUtilities.GetProperty(entity.GetType(), propertyName, propertyType);
				
				if (property == null)
					throw new Exception("Cannot find property with name '" + propertyName + "' and type '" + (propertyType == null ? "[null]" : propertyType.ToString()) + "' on the type '" + entity.ShortTypeName + "'.");
				
				
				Type referenceType = DataUtilities.GetEntityType(entity, property);
				
				// If the reference type is not null then activate the property
				// otherwise skip it because it means it's a dynamic reference property which hasn't been set
				if (referenceType != null)
				{
					LogWriter.Debug("Reference entity type: " + referenceType.ToString());
					
					IEntity[] referencedEntities = Provider.Indexer.GetEntitiesWithReference(entity, property.Name, references);
					
					if (referencedEntities == null)
						LogWriter.Debug("# of entities found: [null]");
					else
						LogWriter.Debug("# of entities found:" + referencedEntities.Length);
					
					// Multiple references.
					if (EntitiesUtilities.IsMultipleReference(entity.GetType(), property))
					{
						LogWriter.Debug("Multiple reference property");
						
						ActivateMultipleReferenceProperty(entity, property, referencedEntities);
						
					}
					// Single reference.
					else
					{
						LogWriter.Debug("Single reference property");
						
						ActivateSingleReferenceProperty(entity, property, referencedEntities);
					}
				}
			}
		}
		
		protected virtual void ActivateMultipleReferenceProperty(IEntity entity, PropertyInfo property, IEntity[] referencedEntities)
		{
			using (LogGroup logGroup2 = LogGroup.StartDebug("Retrieving the references."))
			{
				Type referenceType = EntitiesUtilities.GetReferenceType(entity, property);

				// If the reference type is not null then activate the property
				if (referenceType != null)
				{
					object value = Collection<IEntity>.ConvertAll(referencedEntities, referenceType);
					
					property.SetValue(entity, value, null);
				}
				// Otherwise skip it because a null reference type means its a dynamically typed property which has not been set
			}
		}
		
		protected virtual void ActivateSingleReferenceProperty(IEntity entity, PropertyInfo property, IEntity[] referencedEntities)
		{
			if (referencedEntities != null && referencedEntities.Length > 0)
				property.SetValue(entity, referencedEntities[0], null);
		}
		
		public override void Activate(IEntity entity, string propertyName)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Activating the '" + propertyName + "' property on the '" + entity.ShortTypeName + "' type."))
			{
				Activate(entity, propertyName, 1);
			}
		}

		public override void Activate(IEntity entity, string propertyName, int depth)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			using (LogGroup logGroup = LogGroup.StartDebug("Activating the '" + propertyName + "' property on the '" + entity.ShortTypeName + "' type."))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				if (propertyName == null || propertyName == String.Empty)
					throw new ArgumentException("propertyName", "Cannot be null or string.Empty.");
				
				Activate(entity, propertyName, null, depth);
			}
		}
		
	}
}
