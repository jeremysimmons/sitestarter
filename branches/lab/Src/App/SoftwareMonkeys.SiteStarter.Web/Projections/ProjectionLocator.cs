using System;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	/// <summary>
	/// Used to locate a projection for a particular scenario.
	/// </summary>
	public class ProjectionLocator
	{
		private ProjectionStateCollection projections;
		/// <summary>
		/// Gets/sets the projections that are available to the projection locator.
		/// Note: Defaults to ProjectionState.Projections.
		/// </summary>
		public ProjectionStateCollection Projections
		{
			get {
				if (projections == null)
					projections = ProjectionState.Projections;
				return projections; }
			set { projections = value; }
		}
		
		/// <summary>
		/// Sets the provided projections to the Projections property.
		/// </summary>
		/// <param name="projections"></param>
		public ProjectionLocator(ProjectionStateCollection projections)
		{
			Projections = projections;
		}
		
		/// <summary>
		/// Empty constructor.
		/// </summary>
		public ProjectionLocator()
		{}
		
		/// <summary>
		/// Locates the projection info for performing the specified action with the specified type.
		/// </summary>
		/// <param name="action">The action that is to be performed by the projection.</param>
		/// <param name="typeName">The short type name of the entity that is involved in the action.</param>
		/// <param name="format">The output format of the projection to locate.</param>
		/// <returns>The projection info for the specified scenario.</returns>
		public ProjectionInfo Locate(string action, string typeName, ProjectionFormat format)
		{
			// Get the specified type
			Type type = null;
			
			if (Entities.EntityState.Entities.EntityExists(typeName))
				type = Entities.EntityState.Entities[typeName].GetEntityType();
			
			// Create a direct projection key for the specified type
			string key = Projections.GetProjectionKey(action, typeName, format);
			
			// Create the projection info variable to hold the return value
			ProjectionInfo projectionInfo = null;
			
			// Check the direct key to see if a projection exists
			if (Projections.ProjectionExists(key))
			{
				projectionInfo = Projections[key];
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
		public ProjectionInfo LocateFromHeirarchy(string action, Type type, ProjectionFormat format)
		{
			ProjectionInfo projectionInfo = LocateFromInterfaces(action, type, format);
			
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
		public ProjectionInfo LocateFromInterfaces(string action, Type type, ProjectionFormat format)
		{
			ProjectionInfo projectionInfo = null;
			
			Type[] interfaceTypes = type.GetInterfaces();
			
			// Loop backwards through the interface types
			for (int i = interfaceTypes.Length-1; i >= 0; i --)
			{
				Type interfaceType = interfaceTypes[i];
				
				string key = Projections.GetProjectionKey(action, interfaceType.Name, format);
				
				if (Projections.ProjectionExists(key))
				{
					projectionInfo = Projections[key];
					
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
		public ProjectionInfo LocateFromBaseTypes(string action, Type type, ProjectionFormat format)
		{
			ProjectionInfo projectionInfo = null;
			
			TypeNavigator navigator = new TypeNavigator(type);
			
			while (navigator.HasNext && projectionInfo == null)
			{
				Type nextType = navigator.Next();
				
				string key = Projections.GetProjectionKey(action, nextType.Name, format);
				
				// If a projection exists for the base type then use it
				if (Projections.ProjectionExists(key))
				{
					projectionInfo = Projections[key];
					
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
