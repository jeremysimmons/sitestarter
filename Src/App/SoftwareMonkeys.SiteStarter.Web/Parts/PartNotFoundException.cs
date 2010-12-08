using System;

namespace SoftwareMonkeys.SiteStarter.Web.Parts
{
	/// <summary>
	/// Indicates that a part for the provided type and action could not be found.
	/// </summary>
	public class PartNotFoundException : ApplicationException
	{
		private string action;
		/// <summary>
		/// Gets/sets the action being performed by the part.
		/// </summary>
		public string Action
		{
			get { return action; }
			set { action = value; }
		}
		
		private string typeName;
		/// <summary>
		/// Gets/sets the name of the type involved in the part.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		public PartNotFoundException(string action, string typeName) : base("A part could not be found to carry out with action '" + action + "', entity type '" + typeName + "'.")
		{
			Action = action;
			TypeName = typeName;
		}
		
		public PartNotFoundException(string action) : base("A part could not be found to carry out with action '" + action + "'.")
		{
			Action = action;
		}
	}
}
