using System;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Entities.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Tests.Security
{
	/// <summary>
	/// 
	/// </summary>
	[AuthoriseReferenceStrategy("MockEntity", "PublicEntities", "MockPublicEntity")]
	public class AuthoriseReferenceMockPublicEntityStrategy : AuthoriseReferenceStrategy
	{
				
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
