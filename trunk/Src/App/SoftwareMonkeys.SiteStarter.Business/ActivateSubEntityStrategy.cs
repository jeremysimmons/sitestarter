using System;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// 
	/// </summary>
	[Strategy("Activate", "ISubEntity")]
	[Serializable]
	public class ActivateSubEntityStrategy : ActivateStrategy
	{
		public ActivateSubEntityStrategy()
		{
		}
	}
}
