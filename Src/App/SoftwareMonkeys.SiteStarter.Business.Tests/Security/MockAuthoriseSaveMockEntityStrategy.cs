using System;
using SoftwareMonkeys.SiteStarter.Business.Security;

namespace SoftwareMonkeys.SiteStarter.Business.Tests.Security
{
	/// <summary>
	/// 
	/// </summary>
	[AuthoriseStrategy("Save", "MockEntity")]
	public class MockAuthoriseSaveMockEntityStrategy : AuthoriseSaveStrategy
	{
		public MockAuthoriseSaveMockEntityStrategy()
		{
		}
		
		public override bool IsAuthorised(string shortTypeName)
		{
			// Authorised by default because it's only used during testing
			return true;
		}
	}
}
