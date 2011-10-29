using System;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// 
	/// </summary>
	public class ViewCommandInfo : CommandInfo
	{
		public ViewCommandInfo(string typeName) : base("View", typeName, "Retrieve")
		{}
	}
}
