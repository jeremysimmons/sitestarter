using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Web.UI;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Web.Elements
{
	/// <summary>
	/// Holds information related to a web element.
	/// </summary>
	public class ElementInfo
	{
		private string name = String.Empty;
		/// <summary>
		/// Gets/sets the name of the element.
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}
		
		private string action = String.Empty;
		/// <summary>
		/// Gets/sets the action that the element is responsible for carrying out in relation to an entity of the specified type.
		/// </summary>
		public string Action
		{
			get { return action; }
			set { action = value; }
		}
		
		private string typeName = String.Empty;
		/// <summary>
		/// Gets/sets the name of the type that is involved in the element.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		private string elementType = String.Empty;
		/// <summary>
		/// Gets/sets the full string representation of the element object type that corresponds with the Actions and TypeName.
		/// </summary>
		public string ElementType
		{
			get { return elementType; }
			set { elementType = value; }
		}
		
		private bool enabled = true;
		public bool Enabled
		{
			get { return enabled; }
			set { enabled = value; }
		}

		private ElementCreator creator;
		/// <summary>
		/// Gets the element creator.
		/// </summary>
		[XmlIgnore]
		public ElementCreator Creator
		{
			get {
				if (creator == null)
				{
					creator = new ElementCreator();
				}
				return creator;
			}
			set { creator = value; }
		}
		
		/// <summary>
		/// Sets the action and type name indicated by the Element attribute on the provided type.
		/// </summary>
		/// <param name="type">The element type with the Element attribute to get the element information from.</param>
		public ElementInfo(Type type)
		{
			ElementAttribute attribute = null;
			foreach (Attribute a in type.GetCustomAttributes(true))
			{
				if (a.GetType().FullName == typeof(ElementAttribute).FullName)
				{
					attribute = (ElementAttribute)a;
					break;
				}
			}
			
			if (attribute == null)
				throw new ArgumentException("Can't find ElementAttribute on type: " + type.FullName);
			
			Action = attribute.Action;
			TypeName = attribute.TypeName;
			Name = attribute.Name;
			ElementType = type.FullName + ", " + type.Assembly.GetName().Name;
		}
		
		
		public ElementInfo()
		{
		}
		
		
		/// <summary>
		/// Creates a new instance of the corresponding element for use by the system.
		/// </summary>
		/// <returns>An instance of the corresponding element.</returns>
		public IElement New()
		{
			IElement element = Creator.CreateElement(this);
			
			return element;
		}
		
		
		/// <summary>
		/// Creates a new instance of the corresponding element for use by the system.
		/// </summary>
		/// <returns>An instance of the corresponding element, cast to the specified type.</returns>
		public T New<T>()
			where T : IElement
		{
			T element = New<T>(TypeName);
			
			return element;
		}
		
		/// <summary>
		/// Creates a new instance of the corresponding element for use by the system.
		/// </summary>
		/// <returns>An instance of the corresponding element, cast to the specified type.</returns>
		public T New<T>(string entityTypeName)
			where T : IElement
		{
			T element = default(T);
			
			using (LogGroup logGroup = LogGroup.Start("Creating a new element for the type: " + entityTypeName, NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Entity type name: " + entityTypeName);
				
				LogWriter.Debug("Element type: " + typeof(T).FullName);
				
				element = (T)New();
				
				element.TypeName = entityTypeName;
			};
			
			return element;
		}
	}
}
