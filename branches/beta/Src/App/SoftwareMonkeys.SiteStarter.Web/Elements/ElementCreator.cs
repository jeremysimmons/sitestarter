using System;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Web.Elements
{
	/// <summary>
	/// Used to create instances of elements.
	/// </summary>
	public class ElementCreator
	{
		private ElementStateCollection elements;
		/// <summary>
		/// Gets/sets the element info collection that the creator uses as a reference to instantiate new elements.
		/// Note: Defaults to ElementState.Elements if not set.
		/// </summary>
		public ElementStateCollection Elements
		{
			get {
				if (elements == null)
					elements = ElementState.Elements;
				return elements; }
			set { elements = value; }
		}
		
		public ElementCreator()
		{
		}
		
		/// <summary>
		/// Creates a new instance of the element with an Element attribute matching the specified type name and action.
		/// </summary>
		/// <param name="action">The action that the new element will be performing.</param>
		/// <param name="typeName">The name of the type involved in the action.</param>
		/// <returns>A element that is suitable to perform the specified action with the specified type.</returns>
		public IElement NewElement(string action, string typeName)
		{
			IElement element = null;
			
			ElementInfo info = ElementState.Elements[action, typeName];
			
			
			return element;
		}
		
		/// <summary>
		/// Creates a new instance of the element with a Element attribute matching the specified type name and action.
		/// </summary>
		/// <param name="elementInfo">The element info object that specified the element to create.</param>
		/// <returns>A element that is suitable to perform the specified action with the specified type.</returns>
		public IElement CreateElement(ElementInfo elementInfo)
		{
			IElement element = null;
			using (LogGroup logGroup = LogGroup.Start("Creating a new element based on the provided info.", NLog.LogLevel.Debug))
			{
				if (elementInfo == null)
					throw new ArgumentNullException("elementInfo");
				
				Type elementType = Type.GetType(elementInfo.ElementType);
				
				if (elementType == null)
					throw new Exception("Can't get type '" + elementInfo.ElementType);
				
				Type entityType = null;
				
				// Get the actual type specified by the type name
				if (elementInfo.TypeName != String.Empty && EntityState.IsType(elementInfo.TypeName))
					entityType = EntityState.GetType(elementInfo.TypeName);
				
				LogWriter.Debug("Element type: " + elementType.FullName);
				LogWriter.Debug("Entity type: " + (entityType != null ? entityType.FullName : String.Empty));
				
				// If the element is a generic element
				if (entityType != null && elementType.IsGenericTypeDefinition)
				{
					LogWriter.Debug("Is generic type definition.");
					
					Type gType = elementType.MakeGenericType(new Type[]{entityType});
					element = (IElement)Activator.CreateInstance(gType);
				}
				// Otherwise it's a normal element
				else
				{
					LogWriter.Debug("Is not generic type definition.");
					
					element = (IElement)Activator.CreateInstance(elementType);
				}
				
				if (element == null)
					throw new ArgumentException("Unable to create instance of element: " + entityType.ToString(), "elementInfo");
				
				LogWriter.Debug("Element created.");
			}
			
			return element;
		}
		
		#region Generic new function
		/// <summary>
		/// Creates a new instance of the specified element for the specified type.
		/// </summary>
		/// <param name="action">The action to be performed by the element.</param>
		/// <param name="typeName">The short name of the type involved in the element.</param>
		/// <returns>A new insteance of the specified element for the specified type.</returns>
		public T New<T>(string action, string typeName)
			where T : IElement
		{
			T element = Elements[action, typeName].New<T>();
			element.TypeName = typeName;
			
			return element;
		}
		
		/// <summary>
		/// Creates a new instance of the specified element for the specified type.
		/// </summary>
		/// <param name="action">The action to be performed by the element.</param>
		/// <param name="type">The type involved in the element.</param>
		/// <returns>A new insteance of the specified element for the specified type.</returns>
		public T New<T>(string action, Type type)
			where T : IElement
		{
			return New<T>(action, type.Name);
		}
		#endregion
		
		public void CheckType(string typeName)
		{
			if (typeName == null || typeName == String.Empty)
				throw new ArgumentNullException("type");
			
			if (typeName == "IEntity")
				throw new InvalidOperationException("The specified type cannot be 'IEntity'.");
			
			if (typeName == "IUniqueEntity")
				throw new InvalidOperationException("The specified type cannot be 'IUniqueEntity'.");
		}
	}
}
