using System;

namespace SoftwareMonkeys.SiteStarter.Web.Parts
{
	/// <summary>
	/// Indicates that a projection for the provided type and action could not be found.
	/// </summary>
	public class PartNotFoundException : ApplicationException
	{
		private string action;
		/// <summary>
		/// Gets/sets the action being performed by the projection.
		/// </summary>
		public string Action
		{
			get { return action; }
			set { action = value; }
		}
		
		private string typeName;
		/// <summary>
		/// Gets/sets the name of the type involved in the projection.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		private PartFormat format = PartFormat.Html;
		/// <summary>
		/// Gets/sets the format of the projection output.
		/// </summary>
		public PartFormat Format
		{
			get { return format; }
			set { format = value; }
		}
		
		public PartNotFoundException(string action, string typeName, PartFormat format) : base("A projection could not be found to carry out with action '" + action + "', entity type '" + typeName + "' and output format '" + format + "'.")
		{
			Action = action;
			TypeName = typeName;
			Format = format;
		}
	}
}
