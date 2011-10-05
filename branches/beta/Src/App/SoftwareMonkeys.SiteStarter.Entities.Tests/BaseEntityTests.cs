using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Entities.Tests
{
	[TestFixture]
	public class BaseEntityTests : BaseEntityTestFixture
	{
		public BaseEntityTests()
		{
		}
		
		[Test]
		public void Test_CopyTo_TestArticle()
		{
			TestArticle article1 = new TestArticle();
			article1.ID = Guid.NewGuid();
			article1.Title = "Test Title";
			
			
			TestArticle article2 = new TestArticle();
			
			article1.CopyTo(article2);
			
			Assert.AreEqual(article1.Title, article2.Title, "The title wasn't transferred.");
		}
		
		[Test]
		public void Test_CopyTo_TestUser()
		{
			TestUser user1 = new TestUser();
			user1.ID = Guid.NewGuid();
			user1.FirstName = "Before";
			
			
			TestUser user2 = new TestUser();
			
			user1.CopyTo(user2);
			
			Assert.AreEqual(user1.FirstName, user2.FirstName, "The first name wasn't transferred.");
		}
	}
}
