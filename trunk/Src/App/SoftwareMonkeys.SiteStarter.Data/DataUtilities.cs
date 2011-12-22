using System;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Configuration;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Description of DataUtilities.
	/// </summary>
	public static class DataUtilities
	{
		static public string DataStoreName = "Data.db4o";
		
		/// <summary>
		/// Gets the name of the data store that the provided entity is stored in.
		/// </summary>
		/// <param name="type">The type of entity to get the data store name for.</param>
		/// <returns>The data store that the provided entity is stored in.</returns>
		static public string GetDataStoreName(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			
			if (type.Name == "EntityReference")
				throw new InvalidOperationException("An instance of the reference is required. Pass the entity to GetDataStore(IEntity) overload.");

			return GetDataStoreName(type, true);
		}
		

		/// <summary>
		/// Gets the name of the data store that the provided entity is stored in.
		/// </summary>
		/// <param name="entity">The entity to get the data store name for.</param>
		/// <returns>The data store that the provided entity is stored in.</returns>
		static public string GetDataStoreName(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");

			return GetDataStoreName(entity, true);
		}
		
		/// <summary>
		/// Gets the name of the data store that the provided entity is stored in.
		/// </summary>
		/// <param name="type">The type of entity to get the data store name for.</param>
		/// <param name="throwErrorIfNotFound">A flag indicating whether an error should be thrown when no data store attribute is found.</param>
		/// <returns>The data store that the provided entity is stored in.</returns>
		static public string GetDataStoreName(Type type, bool throwErrorIfNotFound)
		{
			if (type.Name == "EntityReference")
				throw new InvalidOperationException("An instance of the reference is required. Pass the entity to GetDataStore(IEntity) overload.");

			
			return DataStoreName;
			//return type.Name;
		}

		/// <summary>
		/// Gets the name of the data store that the provided entity is stored in.
		/// </summary>
		/// <param name="entity">The entity to get the data store name for.</param>
		/// <param name="throwErrorIfNotFound">A flag indicating whether an error should be thrown when no data store attribute is found.</param>
		/// <returns>The data store that the provided entity is stored in.</returns>
		static public string GetDataStoreName(IEntity entity, bool throwErrorIfNotFound)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			// Use the same store for each type to boost performance
			return "Data.db4o";
			
			// TODO: Clean up
			/*string dataStoreName = String.Empty;
			Type type = entity.GetType();
			
			using (LogGroup logGroup = LogGroup.Start("Retrieving data store name for entity of type '" + type.ToString() + "'.", LogLevel.Debug))
			{
				if (EntitiesUtilities.IsReference(entity))
				{
					LogWriter.Debug("Provided entity is an EntityReference");
					
					EntityReference reference = (EntityReference)entity;
					
					dataStoreName = GetDataStoreName(new string[] {reference.Type1Name, reference.Type2Name});//GetType(names[0]), GetType(names[1]));//dataStoreNames[0] + "-" + dataStoreNames[1];
				}
				else
				{
					LogWriter.Debug("Provided entity is NOT an EntityReference.");
					
					dataStoreName = GetDataStoreName(entity.GetType());
				}
				
				LogWriter.Debug("Data store name: " + dataStoreName);
			}
			
			return dataStoreName;*/
			
		}
		
		static public string GetDataStoreName(params string[] typeNames)
		{
			return DataStoreName;
			/*string returnName;
			
			//using (LogGroup logGroup = LogGroup.Start("Retrieving the data store name for provided types.", LogLevel.Debug))
			//{
			if (typeNames == null)
				throw new ArgumentNullException("typeNames");
			
			if (typeNames.Length != 2)
				throw new ArgumentException("Incorrect number of type names provided. Expected 2 but was " + typeNames.Length + ".");
			else
			{
				if (typeNames[0].Trim().Length == 0)
					throw new ArgumentException("The type name at position 0 is empty.");
				if (typeNames[1].Trim().Length == 0)
					throw new ArgumentException("The type name at position 1 is empty.");
				
				//LogWriter.Debug("Type name 0: " + typeNames[0]);
				//LogWriter.Debug("Type name 1: " + typeNames[1]);
			}
			
			Array.Sort(typeNames);
			
			//LogWriter.Debug("Sorted names.");
			
			//LogWriter.Debug("Type name 0: " + typeNames[0]);
			//LogWriter.Debug("Type name 1: " + typeNames[1]);
			
			Type type0 = EntityState.GetType(typeNames[0]);
			Type type1 = EntityState.GetType(typeNames[1]);
			
			//LogWriter.Debug("Type 0: " + type0.ToString());
			//LogWriter.Debug("Type 1: " + type1.ToString());
			
			string[] dataStoreNames = new String[]{
				GetDataStoreName(type0),
				GetDataStoreName(type1)
			};
			
			returnName = dataStoreNames[0] + "-" + dataStoreNames[1];
			
			//LogWriter.Debug("Data store name: " + returnName);
			//}
			
			return returnName;*/
		}
		
		static public Type GetEntityType(IEntity entity, PropertyInfo property)
		{
			Type type = null;

			using (LogGroup group = LogGroup.Start("Retrieving the type of entity being referenced by the provided property.", LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				if (property == null)
					throw new ArgumentNullException("property");
				
				LogWriter.Debug("Entity type: " + entity.GetType().ToString());
				LogWriter.Debug("Property name: " + property.Name);
				LogWriter.Debug("Property type: " + property.PropertyType.Name);
					
				type = EntitiesUtilities.GetReferenceType(entity.GetType(), property);
			}

			if (type == null)
				LogWriter.Debug("return type == null");
			else
				LogWriter.Debug("return type == " + type.ToString());
			
			return type;
		}
		
		// TODO: Check if function should be removed
		static public void StripReferences(IEntity entity)
		{
			using (LogGroup logGroup2 = LogGroup.Start("Clearing all the object references so that they don't cascade automatically.", LogLevel.Debug))
			{
				if (entity is EntityReference)
				{
					((EntityReference)entity).Deactivate();
				}
				else
				{
					//  Clear all the references from the entity once they're ready to be saved separately
					foreach (PropertyInfo property in entity.GetType().GetProperties())
					{
						LogWriter.Debug("Property name: " + property.Name);
						LogWriter.Debug("Property type: " + property.PropertyType.ToString());
						
						// If the property is a reference
						// OR the actual provided entity is a reference AND the property holds an IEntity instance
						if (EntitiesUtilities.IsReference(entity.GetType(), property.Name, property.PropertyType))
						{
							LogWriter.Debug("Cleared property. (Set to null)");
							Reflector.SetPropertyValue(entity, property.Name, null);
						}
					}
				}
			}
		}
	}
}
