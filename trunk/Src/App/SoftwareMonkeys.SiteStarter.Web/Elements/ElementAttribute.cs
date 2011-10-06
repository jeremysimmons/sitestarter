
using System;

namespace SoftwareMonkeys.SiteStarter.Web.Elements
{
	/// <summary>
	/// Description of ElementAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class ElementAttribute : Attribute
	{
		private string name;
		/// <summary>
		/// Gets/sets the name of the element.
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}
		
		private string typeName;
		/// <summary>
		/// Gets/sets the name of the type involved in the element.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		private string action;
		/// <summary>
		/// Gets/sets the action that can be performed by this element.
		/// </summary>
		public string Action
		{
			get { return action; }
			set { action = value; }
		}
		
		private bool isElement = true;
		/// <summary>
		/// Gets/sets a value indicating whether the corresponding class is an available element. (Set to false to hide an element or a base class.)
		/// </summary>
		public bool IsElement
		{
			get { return isElement; }
			set { isElement = value; }
		}
		
		/// <summary>
		/// Sets the type name and action of the element.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="typeName"></param>
		public ElementAttribute(string action, string typeName)
		{
			TypeName = typeName;
			Action = action;
		}
		
		
		/// <summary>
		/// Sets the name of the element.
		/// </summary>
		/// <param name="name"></param>
		public ElementAttribute(string name)
		{
			Name = name;
		}
		
		/// <summary>
		/// Sets the type name and action of the element.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="type"></param>
		public ElementAttribute(string action, Type type)
		{
			TypeName = type.Name;
			Action = action;
		}
		
		/// <summary>
		/// Sets a value indicating whether the corresponding class is an available business element.
		/// </summary>
		/// <param name="isElement">A value indicating whether the corresponding class is an available business element. (Set to false for base strategies, etc.)</param>
		public ElementAttribute(bool isElement)
		{
			IsElement = isElement;
		}
	}
}
