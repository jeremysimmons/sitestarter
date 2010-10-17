﻿using System;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	/// <summary>
	/// Indicates that a projection for the provided type and action could not be found.
	/// </summary>
	public class ProjectionNotFoundException : ApplicationException
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
		
		public ProjectionNotFoundException(string action, string typeName) : base("A projection could not be found to carry out the '" + action + "' action with the '" + typeName + "' entity type.")
		{
			Action = action;
			TypeName = typeName;
		}
	}
}
