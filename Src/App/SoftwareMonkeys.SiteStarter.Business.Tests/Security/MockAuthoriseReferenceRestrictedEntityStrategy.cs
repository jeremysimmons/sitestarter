using System;
using SoftwareMonkeys.SiteStarter.Business.Security;

namespace SoftwareMonkeys.SiteStarter.Business.Tests.Security
{
	/// <summary>
	/// 
	/// </summary>
	[AuthoriseStrategy("Reference", "RestrictedEntity")]
	public class MockAuthoriseReferenceRestrictedEntityStrategy : AuthoriseReferenceStrategy
	{
		public MockAuthoriseReferenceRestrictedEntityStrategy()
		{
			TypeName = "RestrictedEntity";
			Action = "Reference";
		}
		
		public override bool Authorise(SoftwareMonkeys.SiteStarter.Entities.IEntity entity)
		{
			// The entity is restricted
			return false;
		}
		
		public override bool Authorise(string typeName)
		{
			// The type is allowed
			return true;
		}
	}
}
