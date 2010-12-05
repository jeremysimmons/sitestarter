using System;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Parts
{
	/// <summary>
	/// Used to locate a projection for a particular scenario.
	/// </summary>
	public class PartLocator
	{
		private PartStateCollection projections;
		/// <summary>
		/// Gets/sets the projections that are available to the projection locator.
		/// Note: Defaults to PartState.Parts.
		/// </summary>
		public PartStateCollection Parts
		{
			get {
				if (projections == null)
					projections = PartState.Parts;
				return projections; }
			set { projections = value; }
		}
		
		/// <summary>
		/// Sets the provided projections to the Parts property.
		/// </summary>
		/// <param name="projections"></param>
		public PartLocator(PartStateCollection projections)
		{
			Parts = projections;
		}
		
		/// <summary>
		/// Empty constructor.
		/// </summary>
		public PartLocator()
		{}
		
		/// <summary>
		/// Locates the projection info for performing the specified action with the specified type.
		/// </summary>
		/// <param name="action">The action that is to be performed by the projection.</param>
		/// <param name="typeName">The short type name of the entity that is involved in the action.</param>
		/// <param name="format">The output format of the projection to locate.</param>
		/// <returns>The projection info for the specified scenario.</returns>
		public PartInfo Locate(string action, string typeName, PartFormat format)
		{
			// Get the specified type
			Type type = null;
			
			if (Entities.EntityState.Entities.EntityExists(typeName))
				type = Entities.EntityState.Entities[typeName].GetEntityType();
			
			// Create a direct projection key for the specified type
			string key = Parts.GetPartKey(action, typeName, format);
			
			// Create the projection info variable to hold the return value
			PartInfo projectionInfo = null;
			
			// Check the direct key to see if a projection exists
			if (Parts.PartExists(key))
			{
				projectionInfo = Parts[key];
			}
			// If not then navigate up the heirarchy looking for a matching projection
			else if (type != null)
			{
				projectionInfo = LocateFromHeirarchy(action, type, format);
			}
			
			return projectionInfo;
		}
		
		/// <summary>
		/// Locates the projection info for performing the specified action with the specified type by looking at the base types and interfaces of the provided type.
		/// </summary>
		/// <param name="action">The action that is to be performed by the projection.</param>
		/// <param name="type">The type that is involved in the action.</param>
		/// <param name="format">The output format of the projection to location.</param>
		/// <returns>The projection info for the specified scenario.</returns>
		public PartInfo LocateFromHeirarchy(string action, Type type, PartFormat format)
		{
			PartInfo projectionInfo = LocateFromInterfaces(action, type, format);
			
			if (projectionInfo == null)
				projectionInfo = LocateFromBaseTypes(action, type, format);
			
			return projectionInfo;
		}
		
		
		/// <summary>
		/// Locates the projection info for performing the specified action with the specified type by looking at the interfaces of the provided type.
		/// </summary>
		/// <param name="action">The action that is to be performed by the projection.</param>
		/// <param name="type">The type that is involved in the action.</param>
		/// <param name="format">The output format of the projection to location.</param>
		/// <returns>The projection info for the specified scenario.</returns>
		public PartInfo LocateFromInterfaces(string action, Type type, PartFormat format)
		{
			PartInfo projectionInfo = null;
			
			Type[] interfaceTypes = type.GetInterfaces();
			
			// Loop backwards through the interface types
			for (int i = interfaceTypes.Length-1; i >= 0; i --)
			{
				Type interfaceType = interfaceTypes[i];
				
				string key = Parts.GetPartKey(action, interfaceType.Name, format);
				
				if (Parts.PartExists(key))
				{
					projectionInfo = Parts[key];
					
					break;
				}
			}
			
			return projectionInfo;
		}

		/// <summary>
		/// Locates the projection info for performing the specified action with the specified type by looking at the base types of the provided type.
		/// </summary>
		/// <param name="action">The action that is to be performed by the projection.</param>
		/// <param name="type">The type that is involved in the action.</param>
		/// <param name="format">The output format of the projection to locate.</param>
		/// <returns>The projection info for the specified scenario.</returns>
		public PartInfo LocateFromBaseTypes(string action, Type type, PartFormat format)
		{
			PartInfo projectionInfo = null;
			
			TypeNavigator navigator = new TypeNavigator(type);
			
			while (navigator.HasNext && projectionInfo == null)
			{
				Type nextType = navigator.Next();
				
				string key = Parts.GetPartKey(action, nextType.Name, format);
				
				// If a projection exists for the base type then use it
				if (Parts.PartExists(key))
				{
					projectionInfo = Parts[key];
					
					break;
				}
				// TODO: Check if needed. It shouldn't be. The other call to LocateFromInterfaces in LocateFromHeirarchy should be sufficient
				// Otherwise check the interfaces of that base type
				//else
				//{
				//	projectionInfo = LocateFromInterfaces(action, nextType);
				//}
			}
			
			return projectionInfo;
		}
	}
}
