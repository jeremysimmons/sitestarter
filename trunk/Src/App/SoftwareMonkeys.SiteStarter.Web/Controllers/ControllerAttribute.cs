
using System;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// Description of ControllerAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
	public class ControllerAttribute : Attribute
	{
		private string typeName;
		/// <summary>
		/// Gets/sets the name of the type involved in the controller.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		private string action;
		/// <summary>
		/// Gets/sets the action that can be performed by this controller.
		/// </summary>
		public string Action
		{
			get { return action; }
			set { action = value; }
		}
		
		private bool isController = true;
		/// <summary>
		/// Gets/sets a value indicating whether the corresponding class is an available controller. (Set to false to hide a controller or a base class.)
		/// </summary>
		public bool IsController
		{
			get { return isController; }
			set { isController = value; }
		}
		
		private string key;
		/// <summary>
		/// Gets/sets the key used to differentiate the controller from others that are similar.
		/// For example: A general purpose controller has no key set; A controller that enforces a particular rule such as a unique property can use
		/// the key "Unique" (or whatever suits).
		/// </summary>
		public string Key
		{
			get { return key; }
			set { key = value; }
		}
		
		/// <summary>
		/// Sets the type name and action of the controller.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="typeName"></param>
		public ControllerAttribute(string action, string typeName)
		{
			TypeName = typeName;
			Action = action;
		}
		
		
		/// <summary>
		/// Sets the type name and action of the controller.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="typeName"></param>
		/// <param name="key"></param>
		public ControllerAttribute(string action, string typeName, string key)
		{
			TypeName = typeName;
			Action = action;
			Key = key;
		}
		
		/// <summary>
		/// Sets the type name and action of the controller.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="type"></param>
		public ControllerAttribute(string action, Type type)
		{
			TypeName = type.Name;
			Action = action;
		}

		/// <summary>
		/// Sets the type name and action of the controller.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="type"></param>
		/// <param name="key"></param>
		public ControllerAttribute(string action, Type type, string key)
		{
			TypeName = type.Name;
			Action = action;
			Key = key;
		}
		
		/// <summary>
		/// Sets a value indicating whether the corresponding class is an available business controller.
		/// </summary>
		/// <param name="isController">A value indicating whether the corresponding class is an available business controller. (Set to false for base strategies, etc.)</param>
		public ControllerAttribute(bool isController)
		{
			IsController = isController;
		}
	}
}
