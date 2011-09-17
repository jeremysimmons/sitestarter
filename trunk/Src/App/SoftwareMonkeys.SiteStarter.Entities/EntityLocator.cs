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
			// Create the entity info variable to hold the return value
			EntityInfo entityInfo = null;
			
			// Disabled logging to boost performance
			//using (LogGroup logGroup = LogGroup.StartDebug("Locating type '" + typeName + "' in application state."))
			//{
				string key = Entities.GetEntityKey(typeName);
				
				// Check the direct key to see if a entity exists
				if (Entities.ContainsKey(key))
				{
					entityInfo = Entities.GetStateValue(key);
				}
				
			//	LogWriter.Debug("Entity: " + (entityInfo == null ? "[null]" : entityInfo.FullType));
			//}
			
			return entityInfo;
		}
		
	}
}
