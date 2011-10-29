using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Entities.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	[TestFixture]
	public class SaveStrategyTests : BaseBusinessTestFixture
	{
		[Test]
		public void Test_Found_ForIEntityInterface()
		{
			StrategyInfo strategy = StrategyState.Strategies["Save", "IEntity"];
			
			Assert.IsNotNull(strategy);
		}
		
		[Test]
		public void Test_Save()
		{
			TestArticle article = CreateStrategy.New<TestArticle>(false).Create<TestArticle>();
			article.ID = Guid.NewGuid();
			
			StrategyInfo info = StrategyState.Strategies["Save", "IEntity"];
			ISaveStrategy strategy = SaveStrategy.New<TestArticle>(false);
			
			strategy.Save(article);
			
			TestArticle foundArticle = Data.DataAccess.Data.Reader.GetEntity<TestArticle>("ID", article.ID);
			
			Assert.IsNotNull(foundArticle);
		}
		
		
		[Test]
		public void Test_Save_InvalidEntityMustNotSave()
		{
			// Create the mock entity
			TestUser user = CreateStrategy.New<TestUser>(false).Create<TestUser>();
			user.ID = Guid.NewGuid();
			user.FirstName = "Test";
			user.LastName = "User";
			
			// Set a mock validator that will always fail
			user.Validator = new MockInvalidValidateEntityStrategy();
			
			Assert.IsFalse(user.IsValid, "The validator returned true when it should return false.");
			
			// Try to save the entity
			bool isValid = SaveStrategy.New(user, false).Save(user);
			
			// Ensure that the save was rejected
			Assert.IsFalse(isValid, "The save strategy didn't recognise the entity as invalid.");
			
			// Try loading the user from the data store to see if it's found
			TestUser foundUser = RetrieveStrategy.New<TestUser>(false).Retrieve<TestUser>("ID", user.ID);
			
			// Ensure the user wasn't found and therefore wasn't saved
			Assert.IsNull(foundUser, "The entity was found in the store even though it shouldn't have saved.");
		}
		
		[Test]
		public void Test_Save_RemovesUnauthorisedReferences()
		{
			MockEntity entity = new MockEntity();
			entity.ID = Guid.NewGuid();
			
			MockRestrictedEntity restrictedEntity = new MockRestrictedEntity();
			restrictedEntity.ID = Guid.NewGuid();
			
			entity.RestrictedEntities = new MockRestrictedEntity[]{
				restrictedEntity
			};
			
			SaveStrategy.New(restrictedEntity, false).Save(restrictedEntity);
			SaveStrategy.New(entity).Save(entity);
			
			MockEntity foundEntity = RetrieveStrategy.New<MockEntity>(false).Retrieve<MockEntity>("ID", entity.ID);
			
			Assert.IsNotNull(foundEntity);
			
			foundEntity.Activate();
			
			Assert.AreEqual(0, foundEntity.RestrictedEntities.Length, "Restricted entity wasn't removed from reference property.");
		}
		
	}
}
