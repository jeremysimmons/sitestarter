using System;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Parts
{
	/// <summary>
	/// Used to locate a part for a particular scenario.
	/// </summary>
	public class PartLocator
	{
		private PartStateCollection parts;
		/// <summary>
		/// Gets/sets the parts that are available to the part locator.
		/// Note: Defaults to PartState.Parts.
		/// </summary>
		public PartStateCollection Parts
		{
			get {
				if (parts == null)
					parts = PartState.Parts;
				return parts; }
			set { parts = value; }
		}
		
		/// <summary>
		/// Sets the provided parts to the Parts property.
		/// </summary>
		/// <param name="parts"></param>
		public PartLocator(PartStateCollection parts)
		{
			Parts = parts;
		}
		
		/// <summary>
		/// Empty constructor.
		/// </summary>
		public PartLocator()
		{}
		
		/// <summary>
		/// Locates the part info for performing the specified action.
		/// </summary>
		/// <param name="action">The action that is to be performed by the part.</param>
		/// <returns>The part info for the specified scenario.</returns>
		public PartInfo Locate(string action)
		{
			return Locate(action, String.Empty);
		}
		
		/// <summary>
		/// Locates the part info for performing the specified action with the specified type.
		/// </summary>
		/// <param name="action">The action that is to be performed by the part.</param>
		/// <param name="typeName">The short type name of the entity that is involved in the action.</param>
		/// <returns>The part info for the specified scenario.</returns>
		public PartInfo Locate(string action, string typeName)
		{
			// Get the specified type
			Type type = null;
			
			if (Entities.EntityState.Entities.EntityExists(typeName))
				type = Entities.EntityState.Entities[typeName].GetEntityType();
			
			// Create a direct part key for the specified type
			string key = Parts.GetPartKey(action, typeName);
			
			// Create the part info variable to hold the return value
			PartInfo partInfo = null;
			
			// Check the direct key to see if a part exists
			if (Parts.PartExists(key))
			{
				partInfo = Parts[key];
			}
			// If not then navigate up the heirarchy looking for a matching part
			else if (type != null)
			{
				partInfo = LocateFromHeirarchy(action, type);
			}
			
			return partInfo;
		}
		
		/// <summary>
		/// Locates the part info for performing the specified action with the specified type by looking at the base types and interfaces of the provided type.
		/// </summary>
		/// <param name="action">The action that is to be performed by the part.</param>
		/// <param name="type">The type that is involved in the action.</param>
		/// <returns>The part info for the specified scenario.</returns>
		public PartInfo LocateFromHeirarchy(string action, Type type)
		{
			PartInfo partInfo = LocateFromInterfaces(action, type);
			
			if (partInfo == null)
				partInfo = LocateFromBaseTypes(action, type);
			
			return partInfo;
		}
		
		
		/// <summary>
		/// Locates the part info for performing the specified action with the specified type by looking at the interfaces of the provided type.
		/// </summary>
		/// <param name="action">The action that is to be performed by the part.</param>
		/// <param name="type">The type that is involved in the action.</param>
		/// <returns>The part info for the specified scenario.</returns>
		public PartInfo LocateFromInterfaces(string action, Type type)
		{
			PartInfo partInfo = null;
			
			Type[] interfaceTypes = type.GetInterfaces();
			
			// Loop backwards through the interface types
			for (int i = interfaceTypes.Length-1; i >= 0; i --)
			{
				Type interfaceType = interfaceTypes[i];
				
				string key = Parts.GetPartKey(action, interfaceType.Name);
				
				if (Parts.PartExists(key))
				{
					partInfo = Parts[key];
					
					break;
				}
			}
			
			return partInfo;
		}

		/// <summary>
		/// Locates the part info for performing the specified action with the specified type by looking at the base types of the provided type.
		/// </summary>
		/// <param name="action">The action that is to be performed by the part.</param>
		/// <param name="type">The type that is involved in the action.</param>
		/// <param name="format">The output format of the part to locate.</param>
		/// <returns>The part info for the specified scenario.</returns>
		public PartInfo LocateFromBaseTypes(string action, Type type)
		{
			PartInfo partInfo = null;
			
			TypeNavigator navigator = new TypeNavigator(type);
			
			while (navigator.HasNext && partInfo == null)
			{
				Type nextType = navigator.Next();
				
				string key = Parts.GetPartKey(action, nextType.Name);
				
				// If a part exists for the base type then use it
				if (Parts.PartExists(key))
				{
					partInfo = Parts[key];
					
					break;
				}
			}
			
			return partInfo;
		}
	}
}
