using System;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Web.Elements
{
	/// <summary>
	/// Holds the state of all available elements.
	/// </summary>
	public class ElementStateCollection : StateNameValueCollection<ElementInfo>
	{
		/// <summary>
		/// Gets/sets the element for the specifid action and type.
		/// </summary>
		public ElementInfo this[string action, string type]
		{
			get { return GetElement(action, type); }
			set { SetElement(action, type, value); }
		}
		
		/// <summary>
		/// Gets/sets the element for the specifid action and type.
		/// </summary>
		public ElementInfo this[string action, Type type]
		{
			get { return GetElement(action, type.Name); }
			set { SetElement(action, type.Name, value); }
		}
		
		public new ElementInfo this[string name]
		{
			get { return GetElement(name); }
			set { SetElement(name, value); }
		}
		
		private ElementCreator creator;
		/// <summary>
		/// Gets/sets the element creator used to instantiate new strategies for specific types based on the info in the collection.
		/// </summary>
		public ElementCreator Creator
		{
			get {
				if (creator == null)
				{
					creator = new ElementCreator();
					creator.Elements = this;
				}
				return creator; }
			set { creator = value; }
		}
		
		
		public ElementStateCollection() : base(StateScope.Application, "Web.Elements")
		{
		}
		
		public ElementStateCollection(ElementInfo[] strategies) : base(StateScope.Application, "Web.Elements")
		{
			foreach (ElementInfo element in strategies)
			{
				Add(element);
			}
		}
		
		/// <summary>
		/// Adds the provided element info to the collection.
		/// </summary>
		/// <param name="element">The element info to add to the collection.</param>
		public void Add(ElementInfo element)
		{
			if (element == null)
				throw new ArgumentNullException("element");
			
			string key = GetElementKey(element);
			
			this[key] = element;
		}
		
		/// <summary>
		/// Checks whether a element exists with the provided key.
		/// </summary>
		/// <param name="key">The key of the element to check for.</param>
		/// <returns>A value indicating whether the element exists.</returns>
		public bool ElementExists(string key)
		{
			return ContainsKey(key);
		}
		
		/// <summary>
		/// Retrieves the element with the provided action and type.
		/// </summary>
		/// <param name="name">The name of the element.</param>
		/// <returns>The element matching the specified name.</returns>
		public ElementInfo GetElement(string name)
		{
			ElementInfo foundElement = null;
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving element '" + name + "'."))
			{
				foundElement = base[GetElementKey(name)];
				
				if (foundElement == null)
					throw new ElementNotFoundException(name);
			}
			return foundElement;
		}
		
		/// <summary>
		/// Retrieves the element with the provided action and type.
		/// </summary>
		/// <param name="action">The action that the element performs.</param>
		/// <param name="typeName">The type of entity involved in the element</param>
		/// <returns>The element matching the provided action and type.</returns>
		public ElementInfo GetElement(string action, string typeName)
		{
			ElementLocator locator = new ElementLocator(this);
			
			ElementInfo foundElement = locator.Locate(action, typeName);
			
			if (foundElement == null)
				throw new ElementNotFoundException(action, typeName);
			
			
			return foundElement;
		}

		/// <summary>
		/// Sets the element with the provided action and type.
		/// </summary>
		/// <param name="action">The action that the element performs.</param>
		/// <param name="type">The type of entity involved in the element</param>
		/// <param name="element">The element that corresponds with the specified action and type.</param>
		public void SetElement(string action, string type, ElementInfo element)
		{
			base[GetElementKey(action, type)] = element;
		}
		
		/// <summary>
		/// Sets the element with the provided name.
		/// </summary>
		/// <param name="name">The name of the element.</param>
		/// <param name="element">The element corresponding with the name.</param>
		public void SetElement(string name, ElementInfo element)
		{
			base[GetElementKey(name)] = element;
		}
		
		/// <summary>
		/// Retrieves the key for the provided element info.
		/// </summary>
		/// <param name="info">The element info.</param>
		/// <returns></returns>
		public string GetElementKey(ElementInfo info)
		{
			if (info.Action != String.Empty && info.TypeName != String.Empty)
				return GetElementKey(info.Action, info.TypeName);
			else
				return GetElementKey(info.Name);
		}

		/// <summary>
		/// Retrieves the key for the specified action and type.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public string GetElementKey(string action, string type)
		{
			string fullKey = action + "_" + type;
			
			return fullKey;
		}
		
		/// <summary>
		/// Retrieves the key for the element with the provided name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public string GetElementKey(string name)
		{
			string fullKey = name;
			
			return fullKey;
		}
	}
}
