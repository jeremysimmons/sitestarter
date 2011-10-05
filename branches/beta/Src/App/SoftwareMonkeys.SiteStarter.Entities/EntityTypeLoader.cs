using System;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Used to create instances of entities.
	/// </summary>
	public class EntityTypeLoader
	{
		private EntityStateCollection entities;
		/// <summary>
		/// Gets/sets the entity info collection that the creator uses as a reference to instantiate new entities.
		/// Note: Defaults to EntityState.Entities if not set.
		/// </summary>
		public EntityStateCollection Entities
		{
			get {
				if (entities == null)
					entities = EntityState.Entities;
				return entities; }
			set { entities = value; }
		}
		
		public EntityTypeLoader()
		{
		}
		
		/// <summary>
		/// Loads the specified entity type.
		/// </summary>
		/// <param name="typeName">The name of the type involved in the action.</param>
		/// <returns>The type with the specified name.</returns>
		public Type Load(string typeName)
		{
			EntityInfo info = EntityState.Entities[typeName];
			
			return Load(info);
		}
		
		/// <summary>
		/// Loads the entity type matching the provided info.
		/// </summary>
		/// <param name="info">The info to load the type for.</param>
		/// <returns>The type matching the provided info.</returns>
		public Type Load(EntityInfo info)
		{
			return Type.GetType(info.FullType);
		}
		
		public void CheckType(string typeName)
		{
			if (typeName == null || typeName == String.Empty)
				throw new ArgumentNullException("type");
			
			// TODO: Remove if not needed
			// Should be obsolete now as even interfaces and base types get mapped
			/*if (typeName == "IEntity")
				throw new InvalidOperationException("The specified type cannot be 'IEntity'.");
			
			if (typeName == "IUniqueEntity")
				throw new InvalidOperationException("The specified type cannot be 'IUniqueEntity'.");*/
		}
	}
}
