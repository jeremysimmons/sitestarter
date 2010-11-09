using System;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// Indicates that a controller for the provided type and action could not be found.
	/// </summary>
	public class ControllerNotFoundException : ApplicationException
	{
		private string action;
		/// <summary>
		/// Gets/sets the action being performed by the controller.
		/// </summary>
		public string Action
		{
			get { return action; }
			set { action = value; }
		}
		
		private string typeName;
		/// <summary>
		/// Gets/sets the name of the type involved in the controller.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		public ControllerNotFoundException(string action, string typeName) : base("A controller could not be found to carry out with action '" + action + "' and entity type '" + typeName + "'.")
		{
			Action = action;
			TypeName = typeName;
		}
	}
}
