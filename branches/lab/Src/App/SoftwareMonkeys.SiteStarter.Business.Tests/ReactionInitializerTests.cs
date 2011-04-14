using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Tests;
using System.IO;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	[TestFixture]
	public class ReactionInitializerTests : BaseBusinessTestFixture
	{
		string MockApplicationName = "TestApplication";
		
		[Test]
		public void Test_Initialize_ReactionsProvided()
		{
			IReaction reaction = new MockSaveTestArticleReaction();
			ReactionInfo info = new ReactionInfo(reaction);
			
			ReactionInfo[] strategies = new ReactionInfo[]{info};
			
			ReactionInitializer initializer = new ReactionInitializer();
			initializer.FileNamer.ReactionsDirectoryPath = GetMockReactionsDirectoryPath(MockApplicationName);
			
			initializer.Initialize(strategies);
			
			Assert.IsTrue(ReactionState.IsInitialized, "Reactions weren't initialized or initialization wasn't detected.");
			Assert.Greater(ReactionState.Reactions.Count, 0, "Invalid number of strategies initialized.");
			
			ReactionInfoCollection foundInfos = ReactionState.Reactions[info.Action, info.TypeName];
			
			Assert.IsNotNull(foundInfos, "null value returned");
			
			Assert.Greater(foundInfos.Count, 0, "No reactions found.");
		}
		
		[Test]
		public void Test_Initialize_ReactionsScanned()
		{
			ReactionInitializer initializer = new ReactionInitializer();
			initializer.FileNamer.ReactionsDirectoryPath = GetMockReactionsDirectoryPath(MockApplicationName);
			
			string[] assemblyPaths = new string[] {
				Assembly.Load("SoftwareMonkeys.SiteStarter.Business").Location
			};
			
			initializer.Scanner.AssemblyPaths = assemblyPaths;
			
			initializer.Initialize();
			
			Assert.IsTrue(ReactionState.IsInitialized, "Reactions weren't initialized or initialization wasn't detected.");
			Assert.Greater(ReactionState.Reactions.Count, 0, "Invalid number of strategies initialized.");
		
		}
		
		[Test]
		public void Test_Initialize_ReactionsLoaded()
		{
			ReactionInitializer initializer = new ReactionInitializer();
			initializer.FileNamer.ReactionsDirectoryPath = GetMockReactionsDirectoryPath(MockApplicationName);
			
			IReaction reaction = new MockSaveTestArticleReaction();
			ReactionInfo info = new ReactionInfo(reaction);
			info.Action = "Save";
			info.TypeName = "TestArticle";
			info.ReactionType = reaction.GetType().FullName;
			
			ReactionInfo[] strategies = new ReactionInfo[]{info};
			
			ReactionSaver saver = new ReactionSaver();
			saver.FileNamer = initializer.FileNamer;
			
			saver.SaveToFile(info);
			
			initializer.Initialize();
			
			Assert.IsTrue(ReactionState.IsInitialized, "Reactions weren't initialized or initialization wasn't detected.");
			Assert.Greater(ReactionState.Reactions.Count, 0, "Invalid number of strategies initialized.");
			
			
			ReactionInfoCollection reactions = ReactionState.Reactions[info.Action, info.TypeName];
			
			Assert.IsNotNull(reactions, "null value returned");
			
			Assert.Greater(reactions.Count, 0, "No reactions loaded.");
		}
		
		public string GetMockReactionsDirectoryPath(string applicationName)
		{
			
			return TestUtilities.GetTestDataPath(this, applicationName) + Path.DirectorySeparatorChar + "Reactions";
		}
	}
}
