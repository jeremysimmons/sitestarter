﻿using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// 
	/// </summary>
	public class ReactionTests : BaseBusinessTestFixture
	{
		public ReactionTests()
		{
		}
		
		[Test]
		public void Test_React()
		{
			TestArticle article = CreateStrategy.New<TestArticle>(false).Create<TestArticle>();
			article.ID = Guid.NewGuid();
			article.Title = "Test Article";
			
			MockSaveTestArticleStrategy strategy = MockSaveTestArticleStrategy.New(false);
			
			strategy.Save(article);
			
			Assert.IsTrue((bool)StateAccess.State.Session["MockSaveTestArticleReaction_Reacted"], "The reaction flag wasn't set to true.");
		}
	}
}
