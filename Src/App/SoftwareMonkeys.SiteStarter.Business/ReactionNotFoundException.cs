using System;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Indicates that a reaction for the provided type and action could not be found.
	/// </summary>
	public class ReactionNotFoundException : ApplicationException
	{
		private string action;
		/// <summary>
		/// Gets/sets the action being performed by the reaction.
		/// </summary>
		public string Action
		{
			get { return action; }
			set { action = value; }
		}
		
		private string typeName;
		/// <summary>
		/// Gets/sets the name of the type involved in the reaction.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		public ReactionNotFoundException(string action, string typeName) : base("A reaction could not be found to carry out the '" + action + "' action with the '" + typeName + "' entity type.")
		{
			Action = action;
			TypeName = typeName;
		}
	}
}
