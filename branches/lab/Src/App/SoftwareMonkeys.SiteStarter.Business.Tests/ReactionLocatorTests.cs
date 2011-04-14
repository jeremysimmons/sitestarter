using System;
using NUnit.Framework;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class ReactionLocatorTests : BaseBusinessTestFixture
	{
		public ReactionLocatorTests()
		{
		}
		
		[Test]
		public void Test_Locate()
		{
			ReactionLocator locator = new ReactionLocator();
			
			ReactionInfoCollection reactions = locator.Locate("Save", "TestArticle");
			
			Assert.IsNotNull(reactions);
			
			Assert.Greater(reactions.Count, 0, "No reactions found.");
		}
	}
}
