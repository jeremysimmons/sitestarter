using System;
using SoftwareMonkeys.SiteStarter.Business.Security;

namespace SoftwareMonkeys.SiteStarter.Business.Tests.Security
{
	/// <summary>
	/// 
	/// </summary>
	[AuthoriseStrategy("Update", "MockPublicEntity")]
	public class AuthoriseUpdateMockPublicEntityStrategy : AuthoriseUpdateStrategy
	{
		public AuthoriseUpdateMockPublicEntityStrategy()
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
