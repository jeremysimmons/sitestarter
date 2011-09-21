using System;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// Used to locate a controller for a particular scenario.
	/// </summary>
	public class ControllerLocator
	{
		private ControllerStateCollection controllers;
		/// <summary>
		/// Gets/sets the controllers that are available to the controller locator.
		/// Note: Defaults to ControllerState.Controllers.
		/// </summary>
		public ControllerStateCollection Controllers
		{
			get {
				if (controllers == null)
					controllers = ControllerState.Controllers;
				return controllers; }
			set { controllers = value; }
		}
		
		/// <summary>
		/// Sets the provided controllers to the Controllers property.
		/// </summary>
		/// <param name="controllers"></param>
		public ControllerLocator(ControllerStateCollection controllers)
		{
			Controllers = controllers;
		}
		
		/// <summary>
		/// Empty constructor.
		/// </summary>
		public ControllerLocator()
		{}
		
		/// <summary>
		/// Locates the controller info for performing the specified action with the specified type.
		/// </summary>
		/// <param name="action">The action that is to be performed by the controller.</param>
		/// <param name="typeName">The short type name of the entity that is involved in the action.</param>
		/// <returns>The controller info for the specified scenario.</returns>
		public ControllerInfo Locate(string action, string typeName)
		{
			// Create the controller info variable to hold the return value
			ControllerInfo controllerInfo = null;
			
			// Disabled logging to boost performance
			//using (LogGroup logGroup = LogGroup.Start("Locating a controller for the action '" + action + "' and the type '" + typeName + "'.", NLog.LogLevel.Debug))
			//{
				// Get the specified type
				Type type = null;

				type = Entities.EntityState.GetType(typeName);
				
				if (type == null)
					throw new ArgumentException("No type found with the name '" + typeName + "'.");
				
				// Create a direct controller key for the specified type
				string key = Controllers.GetControllerKey(action, typeName);
				
			//	LogWriter.Debug("Direct key: " + key);
				
				// Check the direct key to see if a controller exists
				if (Controllers.ControllerExists(key))
				{
			//		LogWriter.Debug("Direct key matches.");
					
					controllerInfo = Controllers[key];
				}
				// If not then navigate up the heirarchy looking for a matching controller
				else if (type != null) // Only use heirarchy if an actual type was provided.
				{
			//		LogWriter.Debug("Direct key doesn't match. Locating through heirarchy.");
					controllerInfo = LocateFromHeirarchy(action, type);
				}
				
			//	if (controllerInfo == null)
			//		LogWriter.Debug("No controller found.");
			//	else
			//	{
			//		LogWriter.Debug("Controller type found: " + controllerInfo.ControllerType);
			//		LogWriter.Debug("Controller key: " + controllerInfo.Key);
			//	}
			//}
			return controllerInfo;
		}
		
		/// <summary>
		/// Locates the controller info for performing the specified action with the specified type by looking at the base types and interfaces of the provided type.
		/// </summary>
		/// <param name="action">The action that is to be performed by the controller.</param>
		/// <param name="type">The type that is involved in the action.</param>
		/// <returns>The controller info for the specified scenario.</returns>
		public ControllerInfo LocateFromHeirarchy(string action, Type type)
		{
			ControllerInfo controllerInfo = null;
			
			// Disabled logging to boost performance
			//using (LogGroup logGroup = LogGroup.Start("Locating via heirarchy the controller for the action '" + action + "' and type '" + type.Name + "'.", NLog.LogLevel.Debug))
			//{
				controllerInfo = LocateFromInterfaces(action, type);
				
				if (controllerInfo == null)
				{
			//		LogWriter.Debug("Can't locate through interfaces. Trying base types.");
					
					controllerInfo = LocateFromBaseTypes(action, type);
				}
				
			//}
			return controllerInfo;
		}
		
		
		/// <summary>
		/// Locates the controller info for performing the specified action with the specified type by looking at the interfaces of the provided type.
		/// </summary>
		/// <param name="action">The action that is to be performed by the controller.</param>
		/// <param name="type">The type that is involved in the action.</param>
		/// <returns>The controller info for the specified scenario.</returns>
		public ControllerInfo LocateFromInterfaces(string action, Type type)
		{
			ControllerInfo controllerInfo = null;
			
			// Disabled logging to boost performance
			//using (LogGroup logGroup = LogGroup.Start("Locating via interfaces the controller for the action '" + action + "' and type '" + type.Name + "'.", NLog.LogLevel.Debug))
			//{
				Type[] interfaceTypes = type.GetInterfaces();
				
				// Loop backwards through the interface types
				for (int i = interfaceTypes.Length-1; i >= 0; i --)
				{
					Type interfaceType = interfaceTypes[i];
					
					string key = Controllers.GetControllerKey(action, interfaceType.Name);
					
					if (Controllers.ControllerExists(key))
					{
			//			LogWriter.Debug("Found match with key: " + key);
						
						controllerInfo = Controllers[key];
						
						break;
					}
				}
			//}
			
			return controllerInfo;
		}

		/// <summary>
		/// Locates the controller info for performing the specified action with the specified type by looking at the base types of the provided type.
		/// </summary>
		/// <param name="action">The action that is to be performed by the controller.</param>
		/// <param name="type">The type that is involved in the action.</param>
		/// <returns>The controller info for the specified scenario.</returns>
		public ControllerInfo LocateFromBaseTypes(string action, Type type)
		{
			ControllerInfo controllerInfo = null;
			
			// Disabled logging to boost performance
			//using (LogGroup logGroup = LogGroup.Start("Locating via base types the controller for the action '" + action + "' and type '" + type.Name + "'.", NLog.LogLevel.Debug))
			//{
				TypeNavigator navigator = new TypeNavigator(type);
				
				while (navigator.HasNext && controllerInfo == null)
				{
					Type nextType = navigator.Next();
					
					string key = Controllers.GetControllerKey(action, nextType.Name);
					
					// If a controller exists for the base type then use it
					if (Controllers.ControllerExists(key))
					{
			//			LogWriter.Debug("Found match with key: " + key);
						
						controllerInfo = Controllers[key];
						
						break;
					}
					// TODO: Check if needed. It shouldn't be. The other call to LocateFromInterfaces in LocateFromHeirarchy should be sufficient
					// Otherwise check the interfaces of that base type
					//else
					//{
					//	controllerInfo = LocateFromInterfaces(action, nextType);
					//}
				}
			//}
			
			return controllerInfo;
		}
	}
}
