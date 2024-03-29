﻿using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Business.Tests.Security;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class StrategyInfoTests : BaseBusinessTestFixture
	{
		[Test]
		public void Test_Constructor_StrategyParameter()
		{
			MockIndexWidgetStrategy strategy = new MockIndexWidgetStrategy();
			
			StrategyInfo info = StrategyInfo.ExtractInfo(strategy.GetType())[0];
			
			Assert.AreEqual("Widget", info.TypeName, "Type name doesn't match what's expected.");
			Assert.AreEqual("Index", info.Action, "Action doesn't match what's expected.");
			//Assert.AreEqual("Index_Widget", info.Key, "Key doesn't match what's expected.");
			Assert.AreEqual(strategy.GetType().FullName + ", " + strategy.GetType().Assembly.GetName().Name, info.StrategyType, "Strategy type doesn't match what's expected.");
		}


		[Test]
		public void Test_ExtractInfo()
		{
			MockIndexWidgetStrategy strategy = new MockIndexWidgetStrategy();
			
			StrategyInfo info = StrategyInfo.ExtractInfo(strategy.GetType())[0];
			
			Assert.AreEqual("Widget", info.TypeName, "Type name doesn't match what's expected.");
			Assert.AreEqual("Index", info.Action, "Action doesn't match what's expected.");
			//Assert.AreEqual("Index_Widget", info.Key, "Key doesn't match what's expected.");
			Assert.AreEqual(strategy.GetType().FullName + ", " + strategy.GetType().Assembly.GetName().Name, info.StrategyType, "Strategy type doesn't match what's expected.");
		}
		
		[Test]
		public void Test_ExtractInfo2()
		{
			DeleteSubEntityStrategy strategy = new DeleteSubEntityStrategy();
			
			StrategyInfo info = StrategyInfo.ExtractInfo(strategy.GetType())[0];
			
			Assert.AreEqual("ISubEntity", info.TypeName, "Type name doesn't match what's expected.");
			Assert.AreEqual("Delete", info.Action, "Action doesn't match what's expected.");
			//Assert.AreEqual("Index_Widget", info.Key, "Key doesn't match what's expected.");
			Assert.AreEqual(strategy.GetType().FullName + ", " + strategy.GetType().Assembly.GetName().Name, info.StrategyType, "Strategy type doesn't match what's expected.");
		}
		
		
		[Test]
		public void Test_ExtractInfo_DontInherit()
		{
			DeleteSubEntityStrategy strategy = new DeleteSubEntityStrategy();
			
			StrategyInfo[] infos = StrategyInfo.ExtractInfo(strategy.GetType());
			
			Assert.AreEqual(1, infos.Length, "Invalid number of strategy info objects found.");
			
			StrategyInfo info = infos[0];
			
			Assert.AreEqual("ISubEntity", info.TypeName, "Type name doesn't match what's expected.");
			Assert.AreEqual("Delete", info.Action, "Action doesn't match what's expected.");
			//Assert.AreEqual("Index_Widget", info.Key, "Key doesn't match what's expected.");
			Assert.AreEqual(strategy.GetType().FullName + ", " + strategy.GetType().Assembly.GetName().Name, info.StrategyType, "Strategy type doesn't match what's expected.");
		}
		
		[Test]
		public void Test_ExtractInfo_AuthoriseReferenceStrategy()
		{
			AuthoriseReferenceMockPublicEntityStrategy strategy = new AuthoriseReferenceMockPublicEntityStrategy();
			
			StrategyInfo info = StrategyInfo.ExtractInfo(strategy.GetType())[0];
			
			Assert.IsTrue(info is AuthoriseReferenceStrategyInfo, "Should have returned an AuthoriseReferenceStrategyInfo object.");
			              
			AuthoriseReferenceStrategyInfo authoriseStrategyInfo = (AuthoriseReferenceStrategyInfo)info;
			
			Assert.AreEqual("MockEntity", authoriseStrategyInfo.TypeName, "Type name doesn't match what's expected.");
			Assert.AreEqual("MockPublicEntity", authoriseStrategyInfo.ReferencedTypeName, "Type name doesn't match what's expected.");
			Assert.AreEqual("AuthoriseReference", authoriseStrategyInfo.Action, "Action doesn't match what's expected.");
			Assert.AreEqual("PublicEntities", authoriseStrategyInfo.ReferenceProperty, "Reference property doesn't match what's expected.");
			Assert.AreEqual("", authoriseStrategyInfo.MirrorProperty, "Mirror property doesn't match what's expected.");
			
			Assert.AreEqual(strategy.GetType().FullName + ", " + strategy.GetType().Assembly.GetName().Name, info.StrategyType, "Strategy type doesn't match what's expected.");
		}
	}
}
