using System;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Holds the state of the business entities.
	/// </summary>
	static public class EntityState
	{
		/// <summary>
		/// Gets a value indicating whether the entity state has been initialized.
		/// </summary>
		static public bool IsInitialized
		{
			get { return entities != null && entities.Count > 0; }
		}
		
		static private EntityStateCollection entities;
		/// <summary>
		/// Gets/sets a collection of all the available entities which are held in state.
		/// </summary>
		static public EntityStateCollection Entities
		{
			get {
				if (entities == null)
					entities = new EntityStateCollection();
				return entities;  }
			set { entities = value; }
		}
		
		/// <summary>
		/// Checks whether the specified type exists in the entity state.
		/// </summary>
		/// <param name="type">The type to check.</param>
		/// <returns></returns>
		static public bool IsType(Type type)
		{
			return IsType(type.Name);
		}
		
		/// <summary>
		/// Checks whether the specified type exists in the entity state.
		/// </summary>
		/// <param name="typeName">The short type name.</param>
		/// <returns></returns>
		static public bool IsType(string typeName)
		{
			if (typeName == String.Empty)
				throw new ArgumentException("A type name must be provided.", "typeName");
			
			if (typeName == "IEntity"
			    || typeName == "IUniqueEntity"
			    || typeName == "EntityReference")
				return true;
			
			EntityInfo info = GetInfo(typeName, false);
			
			return (info != null);
		}
		
		/// <summary>
		/// Retrieves the type with the provided short name.
		/// </summary>
		/// <param name="typeName">The short type name.</param>
		/// <returns></returns>
		static public Type GetType(string typeName)
		{
			if (typeName == String.Empty)
				throw new ArgumentException("A type name must be provided.", "typeName");
			
			if (typeName.IndexOf(".") > -1)
				return Type.GetType(typeName);
			
			if (typeName == "IEntity")
				return typeof(IEntity);
			
			if (typeName == "IUniqueEntity")
				return typeof(IUniqueEntity);
			
			if (typeName == "EntityReference")
				return typeof(EntityReference);
			
			
			EntityInfo info = GetInfo(typeName);
			
			if (info == null)
				throw new ArgumentException("No entity type info found with the name '" + typeName + "'.");
			
			Type type = null;
			
			type = info.GetEntityType();
			
			if (type == null)
				throw new ArgumentException("No entity type loaded with the name '" + typeName + "'.");
			
			return type;
		}
		
		static public EntityInfo GetInfo(string typeName)
		{
			return GetInfo(typeName, true);
		}
		
		static public EntityInfo GetInfo(string typeName, bool throwExceptionIfNotFound)
		{
			EntityInfo info = EntityState.Entities[typeName];
			
			if (info != null)
			{
				Type type = null;
				
				// If an alias is specified then grab the alias
				// Repeat until no alias is specified
				while (info.Alias != String.Empty)
				{
					info = EntityState.Entities[info.Alias];
				}
				
				return info;
			}
			else if (throwExceptionIfNotFound)
				throw new ArgumentException("No entity type info found with the name '" + typeName + "'.");
			else
				return null;
		}
	}
}
