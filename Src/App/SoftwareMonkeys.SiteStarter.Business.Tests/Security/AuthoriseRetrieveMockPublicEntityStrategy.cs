using System;
using SoftwareMonkeys.SiteStarter.Business.Security;

namespace SoftwareMonkeys.SiteStarter.Business.Tests.Security
{
	/// <summary>
	/// 
	/// </summary>
	[AuthoriseStrategy("Retrieve", "MockPublicEntity")]
	public class AuthoriseRetrieveMockPublicEntityStrategy : AuthoriseRetrieveStrategy
	{
		public AuthoriseRetrieveMockPublicEntityStrategy()
		{
		}
		
		public override bool IsAuthorised(SoftwareMonkeys.SiteStarter.Entities.IEntity entity)
		{
			return true;
		}
		
		public override bool IsAuthorised(string shortTypeName)
		{
			return true;
		}
	}
}
