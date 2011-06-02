using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	[TestFixture]
	public class UpdateStrategyTests : BaseBusinessTestFixture
	{
		[Test]
		public void Test_Found_ForIEntityInterface()
		{
			StrategyInfo strategy = StrategyState.Strategies["Update", "IEntity"];
			
			Assert.IsNotNull(strategy);
		}
		
		[Test]
		public void Test_Update()
		{
			TestArticle article = CreateStrategy.New<TestArticle>(false).Create<TestArticle>();
			article.ID = Guid.NewGuid();
			article.Title = "Mock Title";
			
			Assert.IsNotNull(article.Activator);
			
			Data.DataAccess.Data.Saver.Save(article);
			
			Assert.IsNotNull(article.Activator);
			
			string newTitle = "Updated";
			article.Title = newTitle;
			
			IUpdateStrategy strategy = UpdateStrategy.New<TestArticle>(false);
			
			strategy.Update(article);
			
			TestArticle foundArticle = Data.DataAccess.Data.Reader.GetEntity<TestArticle>("ID", article.ID);
			
			Assert.IsNotNull(foundArticle);
			
			Assert.AreEqual(newTitle, foundArticle.Title, "Title wasn't updated.");
		}
		
		[Test]
		public void Test_Update_AutoActivateReferences()
		{
			// Create the mock entities
			TestUser user = CreateStrategy.New<TestUser>(false).Create<TestUser>();
			user.ID = Guid.NewGuid();
			user.FirstName = "Test";
			user.LastName = "User";
			
			TestRole role = CreateStrategy.New<TestRole>(false).Create<TestRole>();
			role.ID = Guid.NewGuid();
			role.Name = "Test Role";
			
			// Assign the user to the role
			user.Roles = new TestRole[] {role};
			
			// Save the entities
			SaveStrategy.New(role, false).Save(role);
			SaveStrategy.New(user, false).Save(user);
			
			// Retrieve the mock user from the data store
			TestUser foundUser = RetrieveStrategy.New<TestUser>(false).Retrieve<TestUser>("ID", user.ID);
			
			// Change a standard property value
			foundUser.FirstName = "Test2";
			
			// Update WITHOUT having activated the user manually
			// This update should automatically activate the user entity before updating and
			// should therefore persist the references
			UpdateStrategy.New(foundUser, false).Update(foundUser);
			
			// Retrieve the mock user again
			TestUser foundUser2 = RetrieveStrategy.New<TestUser>(false).Retrieve<TestUser>("ID", user.ID);
			
			// Manually activate the user
			foundUser2.Activate();
			
			// Assert that the referenced roles are found on the user which indicates
			// that the update strategy did automatically activate the entity and persist
			// the references
			Assert.IsNotNull(foundUser2.Roles, "Roles property is null.");
			Assert.AreEqual(1, foundUser2.Roles.Length, "Invalid number of roles assigned to user.");
			
		}
		
		[Test]
		public void Test_Update_InvalidEntityMustNotUpdate()
		{
			// Create the mock entity
			TestUser user = CreateStrategy.New<TestUser>(false).Create<TestUser>();
			user.ID = Guid.NewGuid();
			user.FirstName = "Test";
			user.LastName = "User";
			
			// Save the entity
			SaveStrategy.New(user, false).Save(user);
			
			// Change a standard property value
			user.FirstName = "Test2";
			
			// Set a mock validator that will always fail
			user.Validator = new MockInvalidValidateEntityStrategy();
			
			Assert.IsFalse(user.IsValid, "The validator returned true when it should return false.");
			
			// Update the invalid entity
			bool isValid = UpdateStrategy.New(user, false).Update(user);
			
			Assert.IsFalse(isValid, "The update strategy didn't recognise the entity as invalid.");
			
			TestUser foundUser = RetrieveStrategy.New<TestUser>(false).Retrieve<TestUser>("ID", user.ID);
			
			Assert.AreNotEqual(foundUser.FirstName, user.FirstName, "The entity was updated despite being invalid.");
		}
		
		[Test]
		[ExpectedException(typeof(InactiveEntityException))]
		public void Test_Update_InactiveEntityCausesException()
		{
			// Create the mock entity
			TestUser user = CreateStrategy.New<TestUser>(false).Create<TestUser>();
			user.ID = Guid.NewGuid();
			user.FirstName = "Test";
			user.LastName = "User";
			
			// Save the entity
			SaveStrategy.New(user, false).Save(user);
			
			// Change a standard property value
			user.FirstName = "Test2";
			
			// Set AutoActivate to false otherwise it'll auto activate and won't be tested properly
			user.AutoActivate = false;
			
			// Set IsActivated to false
			user.IsActivated = false;
			
			// Update the inactive entity
			UpdateStrategy.New(user, false).Update(user);
			
			
		}
	}
}
