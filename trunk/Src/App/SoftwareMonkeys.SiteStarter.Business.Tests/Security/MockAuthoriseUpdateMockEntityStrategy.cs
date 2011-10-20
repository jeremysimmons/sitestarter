using System;
using SoftwareMonkeys.SiteStarter.Business.Security;

namespace SoftwareMonkeys.SiteStarter.Business.Tests.Security
{
	/// <summary>
	/// 
	/// </summary>
	[AuthoriseStrategy("Update", "MockEntity")]
	public class AuthoriseUpdateMockEntityStrategy : AuthoriseUpdateStrategy
	{
		public AuthoriseUpdateMockEntityStrategy()
		{
		}
		
		public override bool Authorise(string shortTypeName)
		{
			// Authorised by default because it's only used during testing
			return true;
		}
	}
}
