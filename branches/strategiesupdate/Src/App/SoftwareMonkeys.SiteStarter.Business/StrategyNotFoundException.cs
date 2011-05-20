using System;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Indicates that a strategy for the provided type and action could not be found.
	/// </summary>
	public class StrategyNotFoundException : ApplicationException
	{
		private string action;
		/// <summary>
		/// Gets/sets the action being performed by the strategy.
		/// </summary>
		public string Action
		{
			get { return action; }
			set { action = value; }
		}
		
		private string typeName;
		/// <summary>
		/// Gets/sets the name of the type involved in the strategy.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		public StrategyNotFoundException(string action, string typeName) : base("A strategy could not be found to carry out the '" + action + "' action with the '" + typeName + "' entity type.")
		{
			Action = action;
			TypeName = typeName;
		}
	}
}
