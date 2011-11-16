using System;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// 
	/// </summary>
	public class EditCommandInfo : CommandInfo
	{
		public EditCommandInfo(string typeName) : base("Edit", typeName, "Update")
		{}
	}
}
