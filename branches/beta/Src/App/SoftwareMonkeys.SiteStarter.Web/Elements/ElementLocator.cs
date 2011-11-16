using System;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Elements
{
	/// <summary>
	/// Used to locate a element for a particular scenario.
	/// </summary>
	public class ElementLocator
	{
		private ElementStateCollection elements;
		/// <summary>
		/// Gets/sets the elements that are available to the element locator.
		/// Note: Defaults to ElementState.Elements.
		/// </summary>
		public ElementStateCollection Elements
		{
			get {
				if (elements == null)
					elements = ElementState.Elements;
				return elements; }
			set { elements = value; }
		}
		
		/// <summary>
		/// Sets the provided elements to the Elements property.
		/// </summary>
		/// <param name="elements"></param>
		public ElementLocator(ElementStateCollection elements)
		{
			Elements = elements;
		}
		
		/// <summary>
		/// Empty constructor.
		/// </summary>
		public ElementLocator()
		{}
		
		/// <summary>
		/// Locates the element info for performing the specified action with the specified type.
		/// </summary>
		/// <param name="action">The action that is to be performed by the element.</param>
		/// <param name="typeName">The short type name of the entity that is involved in the action.</param>
		/// <returns>The element info for the specified scenario.</returns>
		public ElementInfo Locate(string action, string typeName)
		{
			// Create the element info variable to hold the return value
			ElementInfo elementInfo = null;
			
			// Disabled logging to boost performance
			//using (LogGroup logGroup = LogGroup.Start("Locating a element for the action '" + action + "' and the type '" + typeName + "'.", NLog.LogLevel.Debug))
			//{
				// Get the specified type
				Type type = null;

				type = Entities.EntityState.GetType(typeName);
				
				if (type == null)
					throw new ArgumentException("No type found with the name '" + typeName + "'.");
				
				// Create a direct element key for the specified type
				string key = Elements.GetElementKey(action, typeName);
				
			//	LogWriter.Debug("Direct key: " + key);
				
				// Check the direct key to see if a element exists
				if (Elements.ElementExists(key))
				{
			//		LogWriter.Debug("Direct key matches.");
					
					elementInfo = Elements[key];
				}
				// If not then navigate up the heirarchy looking for a matching element
				else if (type != null) // Only use heirarchy if an actual type was provided.
				{
			//		LogWriter.Debug("Direct key doesn't match. Locating through heirarchy.");
					elementInfo = LocateFromHeirarchy(action, type);
				}
				
			//	if (elementInfo == null)
			//		LogWriter.Debug("No element found.");
			//	else
			//	{
			//		LogWriter.Debug("Element type found: " + elementInfo.ElementType);
			//		LogWriter.Debug("Element key: " + elementInfo.Key);
			//	}
			//}
			return elementInfo;
		}
		
		/// <summary>
		/// Locates the element info for performing the specified action with the specified type by looking at the base types and interfaces of the provided type.
		/// </summary>
		/// <param name="action">The action that is to be performed by the element.</param>
		/// <param name="type">The type that is involved in the action.</param>
		/// <returns>The element info for the specified scenario.</returns>
		public ElementInfo LocateFromHeirarchy(string action, Type type)
		{
			ElementInfo elementInfo = null;
			
			// Disabled logging to boost performance
			//using (LogGroup logGroup = LogGroup.Start("Locating via heirarchy the element for the action '" + action + "' and type '" + type.Name + "'.", NLog.LogLevel.Debug))
			//{
				elementInfo = LocateFromInterfaces(action, type);
				
				if (elementInfo == null)
				{
			//		LogWriter.Debug("Can't locate through interfaces. Trying base types.");
					
					elementInfo = LocateFromBaseTypes(action, type);
				}
				
			//}
			return elementInfo;
		}
		
		
		/// <summary>
		/// Locates the element info for performing the specified action with the specified type by looking at the interfaces of the provided type.
		/// </summary>
		/// <param name="action">The action that is to be performed by the element.</param>
		/// <param name="type">The type that is involved in the action.</param>
		/// <returns>The element info for the specified scenario.</returns>
		public ElementInfo LocateFromInterfaces(string action, Type type)
		{
			ElementInfo elementInfo = null;
			
			// Disabled logging to boost performance
			//using (LogGroup logGroup = LogGroup.Start("Locating via interfaces the element for the action '" + action + "' and type '" + type.Name + "'.", NLog.LogLevel.Debug))
			//{
				Type[] interfaceTypes = type.GetInterfaces();
				
				// Loop backwards through the interface types
				for (int i = interfaceTypes.Length-1; i >= 0; i --)
				{
					Type interfaceType = interfaceTypes[i];
					
					string key = Elements.GetElementKey(action, interfaceType.Name);
					
					if (Elements.ElementExists(key))
					{
			//			LogWriter.Debug("Found match with key: " + key);
						
						elementInfo = Elements[key];
						
						break;
					}
				}
			//}
			
			return elementInfo;
		}

		/// <summary>
		/// Locates the element info for performing the specified action with the specified type by looking at the base types of the provided type.
		/// </summary>
		/// <param name="action">The action that is to be performed by the element.</param>
		/// <param name="type">The type that is involved in the action.</param>
		/// <returns>The element info for the specified scenario.</returns>
		public ElementInfo LocateFromBaseTypes(string action, Type type)
		{
			ElementInfo elementInfo = null;
			
			// Disabled logging to boost performance
			//using (LogGroup logGroup = LogGroup.Start("Locating via base types the element for the action '" + action + "' and type '" + type.Name + "'.", NLog.LogLevel.Debug))
			//{
				TypeNavigator navigator = new TypeNavigator(type);
				
				while (navigator.HasNext && elementInfo == null)
				{
					Type nextType = navigator.Next();
					
					string key = Elements.GetElementKey(action, nextType.Name);
					
					// If a element exists for the base type then use it
					if (Elements.ElementExists(key))
					{
			//			LogWriter.Debug("Found match with key: " + key);
						
						elementInfo = Elements[key];
						
						break;
					}
					// TODO: Check if needed. It shouldn't be. The other call to LocateFromInterfaces in LocateFromHeirarchy should be sufficient
					// Otherwise check the interfaces of that base type
					//else
					//{
					//	elementInfo = LocateFromInterfaces(action, nextType);
					//}
				}
			//}
			
			return elementInfo;
		}
	}
}
