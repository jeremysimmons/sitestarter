using System;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Description of CommandAttribute.
	/// </summary>
	public class CommandAttribute : Attribute
	{
		private string action = String.Empty;
		public string Action
		{
			get { return action; }
			set { action = value; }
		}
		
		private string typeName = String.Empty;
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		public CommandAttribute(string action)
		{
			Action = action;
		}
		
		public CommandAttribute(string action, string typeName)
		{
			this.Action = action;
			this.TypeName = typeName;	
		}
		
		public CommandAttribute()
		{
		}
	}
}
