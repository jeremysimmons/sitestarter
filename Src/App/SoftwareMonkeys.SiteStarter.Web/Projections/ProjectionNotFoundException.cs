using System;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	/// <summary>
	/// Indicates that a projection for the provided type and action could not be found.
	/// </summary>
	public class ProjectionNotFoundException : ApplicationException
	{
		private string action = String.Empty;
		/// <summary>
		/// Gets/sets the action being performed by the projection.
		/// </summary>
		public string Action
		{
			get { return action; }
			set { action = value; }
		}
		
		private string typeName = String.Empty;
		/// <summary>
		/// Gets/sets the name of the type involved in the projection.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		private string name = String.Empty;
		/// <summary>
		/// Gets/sets the name of the projction.
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}
		
		private ProjectionFormat format = ProjectionFormat.Html;
		/// <summary>
		/// Gets/sets the format of the projection output.
		/// </summary>
		public ProjectionFormat Format
		{
			get { return format; }
			set { format = value; }
		}
		
		public ProjectionNotFoundException(string name, ProjectionFormat format) : base("A projection could not be found with the name '" + name + "' and output format '" + format + "'.")
		{
			Name = name;
			Format = format;
		}
		
		public ProjectionNotFoundException(string action, string typeName, ProjectionFormat format) : base("A projection could not be found to carry out action '" + action + "', entity type '" + typeName + "' and output format '" + format + "'.")
		{
			Action = action;
			TypeName = typeName;
			Format = format;
		}
	}
}
