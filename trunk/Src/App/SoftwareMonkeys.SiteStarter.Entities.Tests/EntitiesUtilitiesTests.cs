using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Xml;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Entities.Tests
{
	[TestFixture]
	public class EntitiesUtilitiesTests
	{
		public string ApplicationPath
		{
			// TODO: Path MUST NOT be hard coded
			//   get { return @"f:\SoftwareMonkeys\WorkHub\Application 2\Web\"; }
			//     get { return System.Configuration.ConfigurationSettings.AppSettings["ApplicationPath"]; }
			get { return SoftwareMonkeys.SiteStarter.Configuration.Config.Application.PhysicalPath; }
		}
		
		public EntitiesUtilitiesTests()
		{
			//Config.Initialize(ApplicationPath, "");
		}

		#region Singleton tests
		[Test]
		public void Test_Current()
		{
			IAppConfig config = Configuration.Config.Application;

			Assert.IsNotNull(config);
		}
		#endregion
		
		#region Tests
		[Test]
		public void Test_IsReference_True()
		{
			TestArticle article = new TestArticle();
			
			PropertyInfo property = article.GetType().GetProperty("Categories");
			
			Assert.IsTrue(EntitiesUtilities.IsReference(article.GetType(), property), "Returned false when it should have returned true.");
		}
		
		[Test]
		public void Test_IsReference_False()
		{
			TestArticle article = new TestArticle();
			
			PropertyInfo property = article.GetType().GetProperty("ID");
			
			Assert.IsFalse(EntitiesUtilities.IsReference(article.GetType(), property), "Returned true when it should have returned false.");
		}

        [Test]
        public void Test_IsEntity_True()
        {
            TestArticle article = new TestArticle();


            Assert.IsTrue(EntitiesUtilities.IsEntity(article.GetType()), "Returned false when it should have returned true.");
        }


        [Test]
        public void Test_IsEntity_False()
        {
            object obj = String.Empty;


            Assert.IsFalse(EntitiesUtilities.IsEntity(obj.GetType()), "Returned true when it should have returned false.");
        }
				
		[Test]
		public void Test_IsMultipleReference_True()
		{
			TestArticle article = new TestArticle();
			
			PropertyInfo property = article.GetType().GetProperty("Categories");
			
			Assert.IsTrue(EntitiesUtilities.IsMultipleReference(article.GetType(), property), "Returned false when it should have returned true.");
		}
		
		[Test]
		public void Test_IsMultipleReference_False()
		{
			EntityOne e1 = new EntityOne();
			
			PropertyInfo property = e1.GetType().GetProperty("SingleReferenceProperty");
			
			Assert.IsFalse(EntitiesUtilities.IsMultipleReference(e1.GetType(), property), "Returned true when it should have returned false.");
		}
		
						
		[Test]
		public void Test_IsCollectionReference_False()
		{
			TestArticle article = new TestArticle();
			
			PropertyInfo property = article.GetType().GetProperty("Categories");
			
			Assert.IsFalse(EntitiesUtilities.IsCollectionReference(article.GetType(), property), "Returned true when it should have returned false.");
		}
		
		[Test]
		public void Test_IsCollectionReference_True()
		{
			TestArticle article = new TestArticle();
			
			PropertyInfo property = article.GetType().GetProperty("Samples");
			
			Assert.IsTrue(EntitiesUtilities.IsCollectionReference(article.GetType(), property), "Returned false when it should have returned true.");
		}
						
		[Test]
		public void Test_IsArrayReference_False()
		{
			TestArticle article = new TestArticle();
			
			PropertyInfo property = article.GetType().GetProperty("Samples");
			
			Assert.IsFalse(EntitiesUtilities.IsArrayReference(article.GetType(), property), "Returned true when it should have returned false.");
		}
		
		[Test]
		public void Test_IsArrayReference_True()
		{
			TestArticle article = new TestArticle();
			
			PropertyInfo property = article.GetType().GetProperty("Categories");
			
			Assert.IsTrue(EntitiesUtilities.IsArrayReference(article.GetType(), property), "Returned false when it should have returned true.");
		}
		
		[Test]
		public void Test_GetMirrorPropertyName_Multiple_Implicit_Async_SameEntity()
		{
			TestGoal goal = new TestGoal();
			goal.ID = Guid.NewGuid();
			
			string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(goal.GetType(), goal.GetType().GetProperty("Prerequisites"));
			
			Assert.AreEqual(String.Empty, mirrorPropertyName, "The mirror property name wasn't determined correctly.");
		}
		
				[Test]
		public void Test_GetMirrorPropertyNameReverse_Multiple_Implicit_Sync()
		{
			TestUser user = new TestUser();
			
			string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyNameReverse(user.GetType(), "Roles", typeof(TestRole));
			
			Assert.AreEqual("Users", mirrorPropertyName, "The mirror property name wasn't determined correctly.");
		}
		
		[Test]
		public void Test_GetMirrorPropertyName_Multiple_Implicit_Sync()
		{
			TestUser user = new TestUser();
			
			string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(user.GetType(), user.GetType().GetProperty("Roles"));
			
			Assert.AreEqual("Users", mirrorPropertyName, "The mirror property name wasn't determined correctly.");
		}
		
		[Test]
		public void Test_GetMirrorPropertyName_Single_Implicit_Sync()
		{
			EntityOne e1 = new EntityOne();
			
			string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(e1.GetType(), e1.GetType().GetProperty("SingleReferenceProperty"));
			
			Assert.AreEqual(String.Empty, mirrorPropertyName, "The mirror property name wasn't determined correctly.");
		}
		
		[Test]
		public void Test_GetMirrorPropertyName_Single_Explicit_Sync()
		{
			TestSample sample = new TestSample();
			
			string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(sample.GetType(), sample.GetType().GetProperty("Articles"));
			
			Assert.AreEqual("Samples", mirrorPropertyName, "The mirror property name wasn't determined correctly.");
		}
		
		[Test]
		public void Test_GetFieldName()
		{
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			
			string fieldName = EntitiesUtilities.GetFieldName(article.GetType(), "Title");
			
			Assert.AreEqual("title", fieldName, "Incorrect field name returned.");
		}
		
		
		[Test]
		public void Test_IsInPage_Pre()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the IsInPage function on an item that appears on a earlier page.", NLog.LogLevel.Debug))
			{
				int i = 9;
				int pageIndex = 1;
				int pageSize = 10;
				
				bool isInPage = EntitiesUtilities.IsInPage(i, pageIndex, pageSize);
				
				Assert.IsFalse(isInPage, "Returned true when it shouldn't have.");
			}
		}
		
		[Test]
		public void Test_IsInPage_In_Start()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the IsInPage function on an item that appears on the specified page.", NLog.LogLevel.Debug))
			{
				
				int i = 10;
				int pageIndex = 1;
				int pageSize = 10;
				
				bool isInPage = EntitiesUtilities.IsInPage(i, pageIndex, pageSize);
				
				
				Assert.IsTrue(isInPage, "Returned false when it should have been true.");
			}
		}
		
		[Test]
		public void Test_IsInPage_In_End()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the IsInPage function on an item that appears on the specified page.", NLog.LogLevel.Debug))
			{
				
				int i = 9;
				int pageIndex = 0;
				int pageSize = 10;
				
				bool isInPage = EntitiesUtilities.IsInPage(i, pageIndex, pageSize);
				
				
				Assert.IsTrue(isInPage, "Returned false when it should have been true.");
			}
		}
		
		[Test]
		public void Test_IsInPage_In()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the IsInPage function on an item that appears on the specified page.", NLog.LogLevel.Debug))
			{
				
				int i = 5;
				int pageIndex = 0;
				int pageSize = 10;
				
				bool isInPage = EntitiesUtilities.IsInPage(i, pageIndex, pageSize);
				
				
				Assert.IsTrue(isInPage, "Returned false when it should have been true.");
			}
		}
		
		[Test]
		public void Test_IsInPage_Post()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the IsInPage function on an item that appears on a later page.", NLog.LogLevel.Debug))
			{
				
				int i = 11;
				int pageIndex = 0;
				int pageSize = 10;
				
				bool isInPage = EntitiesUtilities.IsInPage(i, pageIndex, pageSize);
				
				Assert.IsFalse(isInPage, "Returned true when it shouldn't have.");
			}
		}
		
		/*[Test]
		public void Test_MatchAlias_Exact()
		{
			TestUser.RegisterType();
			
			bool matches = EntitiesUtilities.MatchAlias("TestUser", "TestUser");
			
			Assert.IsTrue(matches, "The name doesn't match the alias.");
		}
		
		[Test]
		public void Test_MatchAlias_Interfaces()
		{
			TestUser.RegisterType();
			
			bool matches = EntitiesUtilities.MatchAlias("TestUser", "ITestUser");
			
			Assert.IsTrue(matches, "The name doesn't match the alias.");
		}*/
		#endregion
		
		#region Utilities functions
	
			
			#endregion


		

	}
}
