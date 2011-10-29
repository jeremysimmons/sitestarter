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
	[AuthoriseReferenceStrategy("MockEntity", "RestrictedEntities", "MockRestrictedEntity")]
	public class AuthoriseReferenceMockRestrictedEntityStrategy : AuthoriseReferenceStrategy
	{
		public override bool IsAuthorised()
		{
			return false;
		}
		
		public override bool IsAuthorised(IEntity entity)
		{
			return false;
		}
		
		public override bool IsAuthorised(string shortTypeName)
		{
			return false;
		}
	}
}
