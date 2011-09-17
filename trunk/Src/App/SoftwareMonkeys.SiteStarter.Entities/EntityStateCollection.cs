using System;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Holds a name/value collection of entities in state.
	/// </summary>
	public class EntityStateCollection : StateNameValueCollection<EntityInfo>
	{		
		/// <summary>
		/// Gets/sets the specified entity info.
		/// </summary>
		public override EntityInfo this[string typeName]
		{
			get { return GetEntity(typeName); }
			set { SetEntity(typeName, value); }
		}
		
		/// <summary>
		/// Gets/sets the entity info for the provided type.
		/// </summary>
		public EntityInfo this[Type type]
		{
			get { return GetEntity(type.Name); }
			set { SetEntity(type.Name, value); }
		}
		
		private EntityLoader loader;
		/// <summary>
		/// Gets/sets the entity loader used to load entity types.
		/// </summary>
		public EntityLoader Creator
		{
			get {
				if (loader == null)
				{
					loader = new EntityLoader();
				}
				return loader; }
			set { loader = value; }
		}
		
		public EntityStateCollection() : base(StateScope.Application, "Entities.Entities")
		{
		}
		
		public EntityStateCollection(EntityInfo[] entities) : base(StateScope.Application, "Entities.Entities")
		{
			foreach (EntityInfo entity in entities)
			{
				SetEntity(entity.TypeName, entity);
			}
		}
		
		/// <summary>
		/// Adds the provided entity info to the collection.
		/// </summary>
		/// <param name="entity">The entity info to add to the collection.</param>
		public void Add(EntityInfo entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			this[entity.TypeName] = entity;
		}
		
		
		/// <summary>
		/// Adds the info of the provided entity to the collection.
		/// </summary>
		/// <param name="entity">The entity info to add to the collection.</param>
		public void Add(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			Add(new EntityInfo(entity));
		}
		
		/// <summary>
		/// Checks whether an entity exists with the provided name.
		/// </summary>
		/// <param name="typeName">The name of the entity type name.</param>
		/// <returns>A value indicating whether the entity exists.</returns>
		[Obsolete("Use Contains function.")]
		public bool EntityExists(string typeName)
		{
			return Contains(typeName);
		}
		
		/// <summary>
		/// Checks whether an entity exists with the provided name.
		/// </summary>
		/// <param name="typeName">The name of the entity type name.</param>
		/// <returns>A value indicating whether the entity exists.</returns>
		public bool Contains(string typeName)
		{
			bool doesContain = false;
			using (LogGroup logGroup = LogGroup.StartDebug("Checking whether the entity '" + typeName + "' exists."))
			{
				doesContain = ContainsKey(GetEntityKey(typeName));
				
				LogWriter.Debug("Does contains: " + doesContain.ToString());
			}
			return doesContain;
		}
		
		/// <summary>
		/// Retrieves the entity with the provided action and type.
		/// </summary>
		/// <param name="typeName">The type of entity involved in the entity.</param>
		/// <returns>The entity matching the provided action and type.</returns>
		public EntityInfo GetEntity(string typeName)
		{
			return GetEntity(typeName, true);
		}
		
		/// <summary>
		/// Retrieves the entity with the provided action and type.
		/// </summary>
		/// <param name="typeName">The type of entity involved in the entity.</param>
		/// <param name="throwExceptionIfNotFound"></param>
		/// <returns>The entity matching the provided action and type.</returns>
		public EntityInfo GetEntity(string typeName, bool throwExceptionIfNotFound)
		{
			EntityInfo foundEntity = null;
			
			// Disabled logging to boost performance.
			//using (LogGroup logGroup = LogGroup.StartDebug("Retrieving the entity '" + typeName + "'."))
			//{
				EntityLocator locator = new EntityLocator(this);
				
				foundEntity = locator.Locate(typeName);
				
				if (foundEntity == null)
					throw new EntityNotFoundException(typeName);
				
				//LogWriter.Debug("Found entity: " + foundEntity.ToString());
			//}
			return foundEntity;
		}

		/// <summary>
		/// Sets the entity with the provided action and type.
		/// </summary>
		/// <param name="type">The type of entity involved in the entity</param>
		/// <param name="entity">The entity that corresponds with the specified action and type.</param>
		public void SetEntity(string type, EntityInfo entity)
		{
			base[GetEntityKey(type)] = entity;
		}

		/// <summary>
		/// Retrieves the key for the specifid action and type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public string GetEntityKey(string type)
		{
			string fullKey = type;
			
			return fullKey;
		}
	}
}
