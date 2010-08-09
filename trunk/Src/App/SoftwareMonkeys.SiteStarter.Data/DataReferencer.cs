
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
		
		
		
		/// <summary>
		/// Gets the references that have been removed from the entity.
		/// </summary>
		/// <param name="entity">The entity that references have been removed from.</param>
		/// <returns>A collection of the removed references.</returns>
		public abstract EntityReferenceCollection GetRemovedReferences(IEntity entity);
		
		#region Latest references functions
		
		/// <summary>
		/// Retrieves the active references from the provided entity. This only includes those references currently active and not those in the data store.
		/// </summary>
		/// <param name="entity">The entity containing that the references are assigned to.</param>
		/// <returns>A collection of the active entity references.</returns>
		public abstract EntityReferenceCollection GetActiveReferences(IEntity entity);
		
		/// <summary>
		/// Retrieves the active references from the provided property. This only includes those references currently active and not those in the data store.
		/// </summary>
		/// <param name="entity">The entity containing the property that the references are assigned to.</param>
		/// <param name="propertyName">The name of the property that the references are assigned to.</param>
		/// <param name="returnType">The type of the property that the references are assigned to.</param>
		/// <returns>A collection of the entity references.</returns>
		public abstract EntityReferenceCollection GetActiveReferences(IEntity entity, string propertyName, Type returnType);
		
		#endregion
	}
}
