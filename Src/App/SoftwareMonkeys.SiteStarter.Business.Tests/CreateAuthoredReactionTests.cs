using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Entities.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class CreateAuthoredReactionTests : BaseBusinessTestFixture
	{
		public CreateAuthoredReactionTests()
		{
		}
		
		[Test]
		public void Test_React()
		{
			IUser author = new User();
			author.ID = Guid.NewGuid();
			
			AuthenticationState.User = (User)author;
			
			MockAuthoredEntity entity = CreateStrategy.New<MockAuthoredEntity>(false).Create<MockAuthoredEntity>();
			
			IUser foundAuthor = entity.Author;
			
			Assert.IsNotNull(foundAuthor, "No author assigned.");
			
			Assert.AreEqual(author.ID.ToString(), foundAuthor.ID.ToString(), "IDs didn't match.");
		}
	}
}
