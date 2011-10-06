using System;

namespace SoftwareMonkeys.SiteStarter.Web.Elements
{
	/// <summary>
	/// Indicates that a control for the provided type and action could not be found.
	/// </summary>
	public class ElementNotFoundException : ApplicationException
	{
		private string action;
		/// <summary>
		/// Gets/sets the action being performed by the control.
		/// </summary>
		public string Action
		{
			get { return action; }
			set { action = value; }
		}
		
		private string typeName;
		/// <summary>
		/// Gets/sets the name of the type involved in the control.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		private string name;
		/// <summary>
		/// Gets/sets the name of the control.
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}
		
		public ElementNotFoundException(string action, string typeName) : base("A control could not be found to carry out with action '" + action + "' and entity type '" + typeName + "'.")
		{
			Action = action;
			TypeName = typeName;
		}
		
		public ElementNotFoundException(string name) : base("A control could not be found with the name '" + name + "'.")
		{
			Name = name;
		}
	}
}
