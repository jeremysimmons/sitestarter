
using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Description of DataReferencer.
	/// </summary>
	public abstract class DataReferencer : DataAdapter, IDataReferencer
	{
		public abstract bool MatchReference(Type entityType, Guid entityID, string propertyName, Type referencedEntityType, Guid referencedEntityID, string mirrorPropertyName);
		public abstract bool MatchReference(Type entityType, Guid entityID, string propertyName, Type referencedEntityType, Guid referencedEntityID);
		
		/// <summary>
		/// Retrieves all the references to the entities provided.
		/// </summary>
		/// <returns>The references to the entities provided.</returns>
		public abstract EntityReferenceCollection GetReferences(Type entityType, Guid entityID, string propertyName, Type referenceType, bool fullActivation);
		
		/// <summary>
		/// Retrieves the reference from the specified entity to the entity matching the specified type and the specified ID.
		/// </summary>
		/// <returns>The reference matching the parameters.</returns>
		public abstract EntityReference GetReference(Type entityType, Guid entityID, string propertyName, Type referenceType, Guid referenceEntityID, string mirrorPropertyName, bool activateAll);
		
		public abstract EntityReferenceCollection GetObsoleteReferences(IEntity entity, string propertyName, Type referenceType, Guid[] idsOfEntitiesToKeep);
		
		public abstract EntityReferenceCollection GetObsoleteReferences(IEntity entity, Guid[] idsOfEntitiesToKeep);
		
		
		public abstract void PersistReferences(EntityReferenceCollection references);
		public abstract void DeleteObsoleteReferences(EntityReferenceCollection references);
		
		public abstract void MaintainReferences(IEntity entity);
		
		public abstract EntityReferenceCollection GetReferences(string type1Name, string type2Name);
		
		
		public abstract IEntity[] GetReferencedEntities(EntityReferenceCollection references, IEntity entity);
		
		
		public abstract EntityReferenceCollection GetReferences(IEntity entity);
		
		public abstract EntityReferenceCollection GetReferences(IEntity entity, bool activateAll);
	}
}
