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
			
			AuthoriseReferenceStrategy.New(entity, "RestrictedEntities").Authorise();
			
			Assert.AreEqual(0, entity.RestrictedEntities.Length, "Restricted entity wasn't removed from reference property.");
		}
		
		[Test]
		public void Test_Authorise_RemovesUnauthorisedReferences()
		{
			MockEntity entity = new MockEntity();
			entity.ID = Guid.NewGuid();
			
			MockRestrictedEntity restrictedEntity = new MockRestrictedEntity();
			restrictedEntity.ID = Guid.NewGuid();
			
			entity.RestrictedEntities = new MockRestrictedEntity[]{
				restrictedEntity
			};
			
			AuthoriseReferencesStrategy.New(entity).Authorise(entity);
			
			Assert.AreEqual(0, entity.RestrictedEntities.Length, "Invalid number of restricted entities found.");
		}
		
		
		[Test]
		public void Test_Authorise_KeepsAuthorisedReferences()
		{
			MockEntity entity = new MockEntity();
			entity.ID = Guid.NewGuid();
			
			MockPublicEntity publicEntity = new MockPublicEntity();
			publicEntity.ID = Guid.NewGuid();
			
			entity.PublicEntities = new MockPublicEntity[]{
				publicEntity
			};
			
			AuthoriseReferencesStrategy.New(entity).Authorise(entity);
			
			Assert.AreEqual(1, entity.PublicEntities.Length, "Invalid number of public entities found.");
		}
		
	}
}
