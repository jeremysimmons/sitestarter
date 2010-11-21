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
		/// Gets/sets the entity for the specifid action and type.
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
			
			string key = GetEntityKey(entity.TypeName);
			
			this[key] = entity;
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
		/// Checks whether a entity exists with the provided key.
		/// </summary>
		/// <param name="key">The key of the entity to check for.</param>
		/// <returns>A value indicating whether the entity exists.</returns>
		public bool EntityExists(string key)
		{
			return StateValueExists(key);
		}
		
		/// <summary>
		/// Retrieves the entity with the provided action and type.
		/// </summary>
		/// <param name="typeName">The type of entity involved in the entity</param>
		/// <returns>The entity matching the provided action and type.</returns>
		public EntityInfo GetEntity(string typeName)
		{
			
			EntityLocator locator = new EntityLocator(this);
			
			EntityInfo foundEntity = locator.Locate(typeName);
			
			if (foundEntity == null)
				throw new EntityNotFoundException(typeName);
			
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
