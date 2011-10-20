using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Entities.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Tests.Security
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class AuthoriseReferenceStrategyTests : BaseBusinessTestFixture
	{
		public AuthoriseReferenceStrategyTests()
		{
		}
		
		[Test]
		public void Test_Authorise_Entity_PropertyName()
		{
			MockEntity entity = new MockEntity();
			
			MockRestrictedEntity restrictedEntity = new MockRestrictedEntity();
			
			entity.RestrictedEntities = new MockRestrictedEntity[]{
				restrictedEntity
			};
			
			AuthoriseReferenceStrategy.New(entity, "RestrictedEntities").Authorise(entity, "RestrictedEntities");
			
			Assert.AreEqual(0, entity.RestrictedEntities.Length, "Restricted entity wasn't removed from reference property.");
		}
		
	}
}
