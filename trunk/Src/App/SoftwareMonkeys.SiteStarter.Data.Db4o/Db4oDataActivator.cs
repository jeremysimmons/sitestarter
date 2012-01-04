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
				
				foreach (PropertyInfo property in entity.GetType().GetProperties())
				{
					if (EntitiesUtilities.IsReference(entityType, property.Name, property.PropertyType))
					{
						LogWriter.Debug("Found reference property: " + property.Name);
						LogWriter.Debug("Property type: " + property.PropertyType.ToString());
						
						Activate(entity, property.Name, property.PropertyType, depth);
					}
				}
			}
		}
		
		public override void Activate(IEntity entity, string propertyName, Type propertyType)
		{
			Activate(entity, propertyName, propertyType, 1);
		}
		
		public override void Activate(IEntity entity, string propertyName, Type propertyType, int depth)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Activating property: " + propertyName))
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
					
					// Multiple references.
					if (EntitiesUtilities.IsMultipleReference(entity.GetType(), property))
					{
						LogWriter.Debug("Multiple reference property");
						
						using (LogGroup logGroup2 = LogGroup.StartDebug("Retrieving the references."))
						{
							EntityReferenceCollection references = Provider.Referencer.GetReferences(entity.GetType(),
							                                                                         entity.ID,
							                                                                         property.Name,
							                                                                         referenceType,
							                                                                         true);
							
							if (references == null)
								throw new Exception("references == null");
							
							LogWriter.Debug("References #: " + references.Count);
							
							//	references.SwitchFor(entity);
							
							IEntity[] referencedEntities = Provider.Referencer.GetReferencedEntities(references, entity);
							
							LogWriter.Debug("Referenced entities #: " + referencedEntities.Length);
							
							// If the activation depth is greater than 1
							if (depth > 1)
							{
								Activate(referencedEntities, depth-1);
							}
							
							if (referencedEntities == null)
								LogWriter.Debug("# of entities found: [null]");
							else
								LogWriter.Debug("# of entities found:" + referencedEntities.Length);
							
							object value = Collection<IEntity>.ConvertAll(referencedEntities, referenceType);
							
							property.SetValue(entity, value, null);
						}
						
					}
					// Single reference.
					else
					{
						LogWriter.Debug("Single reference property");
						
						
						LogWriter.Debug("Reference entity type: " + referenceType.ToString());
						
						EntityReferenceCollection references = Provider.Referencer.GetReferences(entity.GetType(),
						                                                                         entity.ID,
						                                                                         propertyName,
						                                                                         referenceType,
						                                                                         true);
						
						
						IEntity[] referencedEntities = Provider.Referencer.GetReferencedEntities(references, entity);
						
						if (referencedEntities == null)
							LogWriter.Debug("# of entities found: [null]");
						else
							LogWriter.Debug("# of entities found:" + referencedEntities.Length);
						
						if (referencedEntities != null && referencedEntities.Length > 0)
							property.SetValue(entity, referencedEntities[0], null);
					}
				}
			}
		}
		
		public override void Activate(IEntity entity, string propertyName)
		{
			Activate(entity, propertyName, 1);
		}

		public override void Activate(IEntity entity, string propertyName, int depth)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Activating property: " + propertyName))
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
