using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Description of IDataReferencer.
	/// </summary>
	public interface IDataReferencer : IDataAdapter
	{
		bool MatchReference(Type entityType, Guid entityID, string propertyName, Type referencedEntityType, Guid referencedEntityID, string mirrorPropertyName);
		bool MatchReference(Type entityType, Guid entityID, string propertyName, Type referencedEntityType, Guid referencedEntityID);
	
		/// <summary>
		/// Retrieves all the data references between all types.
		/// </summary>
		/// <returns>A collection of the entities found in all data stores.</returns>
		EntityReferenceCollection GetReferences();
		
        /// <summary>
        /// Retrieves all the references to the entities provided.
        /// </summary>
        /// <returns>The references to the entities provided.</returns>
        EntityReferenceCollection GetReferences(Type entityType, Guid entityID, string propertyName, Type referenceType, bool fullActivation);
        
        /// <summary>
        /// Retrieves all the references to the entities provided.
        /// </summary>
        /// <returns>The references to the entities provided.</returns>
        EntityReferenceCollection GetReferences(Type entityType, Guid entityID, Type referenceType, bool fullActivation);
        
        /// <summary>
        /// Retrieves the reference from the specified entity to the entity matching the specified type and the specified ID.
        /// </summary>
        /// <returns>The reference matching the parameters.</returns>
        EntityReference GetReference(Type entityType, Guid entityID, string propertyName, Type referenceType, Guid referenceEntityID, string mirrorPropertyName, bool activateAll);
        
        EntityReferenceCollection GetObsoleteReferences(IEntity entity, string propertyName, Type referenceType, Guid[] idsOfEntitiesToKeep);
        	
        EntityReferenceCollection GetObsoleteReferences(IEntity entity, Guid[] idsOfEntitiesToKeep);
                
		void PersistReferences(EntityReferenceCollection references);
		
		void DeleteObsoleteReferences(EntityReferenceCollection references);
		
		void MaintainReferences(IEntity entity);
		
		EntityReferenceCollection GetReferences(string type1Name, string type2Name);
		
		IEntity[] GetReferencedEntities(EntityReferenceCollection references, IEntity entity);
		
		
		EntityReferenceCollection GetReferences(IEntity entity);
		
		EntityReferenceCollection GetReferences(IEntity entity, bool activateAll);
		
		/// <summary>
		/// Retrieves the active references from the provided entity. This only includes those references currently active and not those in the data store.
		/// </summary>
		/// <param name="entity">The entity containing that the references are assigned to.</param>
		/// <returns>A collection of the active entity references.</returns>
		EntityReferenceCollection GetActiveReferences(IEntity entity);
		
		/// <summary>
		/// Retrieves the active references from the provided entity. This only includes those references currently active and not those in the data store.
		/// </summary>
		/// <param name="entity">The entity containing that the references are assigned to.</param>
		/// <param name="autoBind"></param>
		/// <returns>A collection of the active entity references.</returns>
		EntityReferenceCollection GetActiveReferences(IEntity entity, bool autoBind);
		
		/// <summary>
		/// Sets the reference count properties of the provided entity (if a count property is specified by the reference attribute on the specified property).
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="referencePropertyName"></param>
		/// <param name="referenceEntityID"></param>
		/// <returns>A value indicating whether the source entity was changed.</returns>
		bool SetCountProperty(IEntity entity, string referencePropertyName, Guid referencedEntityID);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		bool SetCountProperties(IEntity entity);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		bool SetCountProperties(IEntity entity, bool autoUpdate);
		
		/// <summary>
		/// Sets the count properties on the entities referenced by the one provided.
		/// </summary>
		/// <param name="entity"></param>
		void SetMirrorCountProperties(IEntity entity);
	}
}
