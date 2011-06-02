﻿using System;
using NUnit.Framework;
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
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			
			StrategyInfo info = StrategyState.Strategies["Save", "IEntity"];
			ISaveStrategy strategy = SaveStrategy.New<TestArticle>(false);
			strategy.Validator = new ValidateStrategy();
			
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
	}
}
