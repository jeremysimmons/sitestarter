using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Tests;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using System.Collections;

namespace SoftwareMonkeys.SiteStarter.Diagnostics.Tests
{
	[TestFixture]
	public class TypeNavigatorTests : BaseDiagnosticsTestFixture
	{
		[Test]
		public void Test_HasNext_True()
		{			
			Type type = typeof(TestArticle);
			
			TypeNavigator navigator = new TypeNavigator(type);
			
			Assert.IsTrue(navigator.HasNext, "HasNext is false when it should be true.");
		}
		
		[Test]
		public void Test_HasNext_False()
		{			
			Type type = typeof(object);
			
			TypeNavigator navigator = new TypeNavigator(type);
			
			Assert.IsFalse(navigator.HasNext, "HasNext is true when it should be false.");
		}
		
		[Test]
		public void Test_Next()
		{
			Type type = typeof(TestArticle);
			
			TypeNavigator navigator = new TypeNavigator(type);
			
			ArrayList list = new ArrayList();
			
			while (navigator.HasNext)
			{
				list.Add(navigator.Next());
			}
			
			Assert.AreEqual(4, list.Count, "Invalid number of types identified.");
		}
	}
}
