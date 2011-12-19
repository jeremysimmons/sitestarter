using System;
using SoftwareMonkeys.SiteStarter.Diagnostics;

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
			if (type == null)
				throw new ArgumentNullException("type");
			
			return IsType(type.Name);
		}
		
		/// <summary>
		/// Checks whether the specified type exists in the entity state.
		/// </summary>
		/// <param name="typeName">The short type name.</param>
		/// <returns></returns>
		static public bool IsType(string typeName)
		{
			if (typeName == null || typeName == String.Empty)
				throw new ArgumentException("A type name must be provided.", "typeName");
			
			if (typeName == "IEntity"
			    || typeName == "IUniqueEntity"
			    || typeName == "EntityReference")
				return true;
			
			return Entities.Contains(typeName);
		}
		
		/// <summary>
		/// Retrieves the type with the provided type name.
		/// </summary>
		/// <param name="typeName">The type name. Either a short name ex. "User" or full name ex. "SoftwareMonkeys.WorkHub.Entities.User".</param>
		/// <returns></returns>
		static public Type GetType(string typeName)
		{
			Type type = null;
			
			//using (LogGroup logGroup = LogGroup.StartDebug("Retrieving the type '" + typeName + "'."))
			//{
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
			
			
			// If it's a full name
			if (typeName.IndexOf(".") > -1)
			{
				try
				{
					type = Type.GetType(typeName);
				}
				catch (Exception ex)
				{
					throw new ArgumentException("Invalid type name: " + typeName, ex);
				}
			}
			else
			{
				EntityInfo info = GetInfo(typeName, false);
				
				if (info == null)
					throw new ArgumentException("No entity type info found with the name '" + typeName + "'.");
				
				type = info.GetEntityType();
				
				if (type == null)
					throw new ArgumentException("No entity type loaded with the name '" + typeName + "'.");
			}
			
			//LogWriter.Debug("Type: " + type == null ? "[null]" : type.Name);
			//}
			return type;
		}
		
		static public EntityInfo GetInfo(string typeName)
		{
			// If it's a full type name then extract the short name from it
			
			if (typeName.LastIndexOf(",") > -1)
				typeName = typeName.Substring(0, typeName.IndexOf(","));
			
			if (typeName.LastIndexOf(".") > -1)
				typeName = typeName.Substring(typeName.LastIndexOf("."), typeName.Length-typeName.LastIndexOf(".")).Trim('.');
			
			return GetInfo(typeName, true);
		}
		
		static public EntityInfo GetInfo(string typeName, bool throwExceptionIfNotFound)
		{
			EntityInfo info = null;
			
			//using (LogGroup logGroup = LogGroup.StartDebug("Retrieving the info for the entity type '" + typeName + "'."))
			//{
				info = EntityState.Entities[typeName];
				
				if (info != null)
				{
					// If an alias is specified then grab the alias
					// Repeat until no alias is specified
					while (info.Alias != String.Empty)
					{
						info = EntityState.Entities[info.Alias];
					}
				}
				else if (throwExceptionIfNotFound)
					throw new ArgumentException("No entity type info found with the name '" + typeName + "'.");
			//}
			
			return info;
		}
	}
}
