
using NUnit.Framework;
using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	[TestFixture]
	public class ReactionScannerTests : BaseBusinessTestFixture
	{
		[Test]
		public void Test_FindReactions()
		{
			ReactionScanner scanner = new ReactionScanner();
			
			
			string[] assemblyPaths = new string[] {
				Assembly.Load("SoftwareMonkeys.SiteStarter.Business").Location,
				Assembly.Load("SoftwareMonkeys.SiteStarter.Business.Tests").Location
			};
			
			scanner.AssemblyPaths = assemblyPaths;
			
			ReactionInfo[] strategies = scanner.FindReactions();
			
			Assert.Greater(strategies.Length, 0);
		}
		
		[Test]
		public void Test_IsReaction()
		{
			
			ReactionScanner scanner = new ReactionScanner();
			
			bool isReaction = scanner.IsReaction(typeof(MockSaveTestArticleReaction));
			
			bool notReaction = scanner.IsReaction(typeof(TestUser));
			
			Assert.IsTrue(isReaction);
			Assert.IsFalse(notReaction);
		}
	}
}
