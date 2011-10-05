using System;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// 
	/// </summary>
	public class AuthoriseStrategyAttribute : StrategyAttribute
	{
		private string restrictedAction;
		/// <summary>
		/// Gets/sets the name of the action being restricted.
		/// </summary>
		public string RestrictedAction
		{
			get { return restrictedAction; }
			set { restrictedAction = value; }
		}
		
		public AuthoriseStrategyAttribute(string restrictedAction, string typeName) : base("Authorise" + restrictedAction, typeName)
		{
			RestrictedAction = restrictedAction;
		}
	}
}
