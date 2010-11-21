using System;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Used to locate a entity for a particular scenario.
	/// </summary>
	public class EntityLocator
	{
		private EntityStateCollection entities;
		/// <summary>
		/// Gets/sets the entities that are available to the entity locator.
		/// Note: Defaults to EntityState.Entities.
		/// </summary>
		public EntityStateCollection Entities
		{
			get {
				if (entities == null)
					entities = EntityState.Entities;
				return entities; }
			set { entities = value; }
		}
		
		/// <summary>
		/// Sets the provided entities to the Entities property.
		/// </summary>
		/// <param name="entities"></param>
		public EntityLocator(EntityStateCollection entities)
		{
			Entities = entities;
		}
		
		/// <summary>
		/// Empty constructor.
		/// </summary>
		public EntityLocator()
		{}
		
		/// <summary>
		/// Locates the entity info for performing the specified action with the specified type.
		/// </summary>
		/// <param name="typeName">The short type name of the entity that is involved in the action.</param>
		/// <returns>The entity info for the specified scenario.</returns>
		public EntityInfo Locate(string typeName)
		{
			// Get the specified type
			//Type type = Entities.EntitiesUtilities.GetType(typeName);
			
			// Create a direct entity key for the specified type
			string key = Entities.GetEntityKey(typeName);
			
			// Create the entity info variable to hold the return value
			EntityInfo entityInfo = null;
			
			// Check the direct key to see if a entity exists
			if (Entities.EntityExists(key))
			{
				entityInfo = Entities[key];
			}
			// If not then navigate up the heirarchy looking for a matching entity
			//else
			//{
			//	entityInfo = LocateFromHeirarchy(type);
			//}
			
			return entityInfo;
		}
		
		// TODO: Clean up
		/*/// <summary>
		/// Locates the entity info for performing the specified action with the specified type by looking at the base types and interfaces of the provided type.
		/// </summary>
		/// <param name="type">The type that is involved in the action.</param>
		/// <returns>The entity info for the specified scenario.</returns>
		public EntityInfo LocateFromHeirarchy(Type type)
		{
			EntityInfo entityInfo = LocateFromInterfaces(type);
			
			if (entityInfo == null)
				entityInfo = LocateFromBaseTypes(type);
			
			return entityInfo;
		}
		
		
		/// <summary>
		/// Locates the entity info for performing the specified action with the specified type by looking at the interfaces of the provided type.
		/// </summary>
		/// <param name="action">The action that is to be performed by the entity.</param>
		/// <param name="type">The type that is involved in the action.</param>
		/// <returns>The entity info for the specified scenario.</returns>
		public EntityInfo LocateFromInterfaces(Type type)
		{
			EntityInfo entityInfo = null;
			
			Type[] interfaceTypes = type.GetInterfaces();
			
			// Loop backwards through the interface types
			for (int i = interfaceTypes.Length-1; i >= 0; i --)
			{
				Type interfaceType = interfaceTypes[i];
				
				string key = Entities.GetEntityKey(interfaceType.Name);
				
				if (Entities.EntityExists(key))
				{
					entityInfo = Entities[key];
					
					break;
				}
			}
			
			return entityInfo;
		}

		/// <summary>
		/// Locates the entity info for performing the specified action with the specified type by looking at the base types of the provided type.
		/// </summary>
		/// <param name="type">The type that is involved in the action.</param>
		/// <returns>The entity info for the specified scenario.</returns>
		public EntityInfo LocateFromBaseTypes(Type type)
		{
			EntityInfo entityInfo = null;
			
			TypeNavigator navigator = new TypeNavigator(type);
			
			while (navigator.HasNext && entityInfo == null)
			{
				Type nextType = navigator.Next();
				
				string key = Entities.GetEntityKey(nextType.Name);
				
				// If a entity exists for the base type then use it
				if (Entities.EntityExists(key))
				{
					entityInfo = Entities[key];
					
					break;
				}
				// TODO: Check if needed. It shouldn't be. The other call to LocateFromInterfaces in LocateFromHeirarchy should be sufficient
				// Otherwise check the interfaces of that base type
				//else
				//{
				//	entityInfo = LocateFromInterfaces(action, nextType);
				//}
			}
			
			return entityInfo;
		}*/
	}
}
