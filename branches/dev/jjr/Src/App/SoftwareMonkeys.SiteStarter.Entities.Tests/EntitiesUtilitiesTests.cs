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
using SoftwareMonkeys.SiteStarter.Entities.Tests.Entities;
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
		public void Test_GetReferences_Multiple_Async()
		{
			TestGoal goal = new TestGoal();
			goal.ID = Guid.NewGuid();
			
			TestGoal goal2 = new TestGoal();
			goal2.ID = Guid.NewGuid();
			
			goal.Prerequisites = new TestGoal[] {goal2};
			
			EntityReferenceCollection references = EntitiesUtilities.GetReferences(goal);
			
			Assert.IsNotNull(references, "The reference collection is null.");
			
			Assert.AreEqual(1, references.Count, "Incorrect number of references returned.");
			
			if (references != null)
			{
				Assert.AreEqual(goal.ID, references[0].Entity1ID, "The entity 1 ID wasn't set correctly.");
				Assert.AreEqual(goal2.ID, references[0].Entity2ID, "The entity 2 ID wasn't set correctly.");
				
				Assert.AreEqual("TestGoal", references[0].TypeName1, "The type name 1 wasn't set correctly.");
				Assert.AreEqual("TestGoal", references[0].TypeName2, "The type name 2 wasn't set correctly.");
				
				Assert.AreEqual("Prerequisites", references[0].Property1Name, "The property 1 name wasn't set correctly.");
				Assert.AreEqual(String.Empty, references[0].Property2Name, "The property 2 name wasn't set correctly.");
			}
		}
		
		[Test]
		public void Test_GetReferences_Multiple_Sync()
		{
			TestUser user = new TestUser();
			user.ID = Guid.NewGuid();
			
			TestRole role = new TestRole();
			role.ID = Guid.NewGuid();
			
			user.Roles = new TestRole[] {role};
			
			EntityReferenceCollection references = EntitiesUtilities.GetReferences(user);
			
			Assert.IsNotNull(references, "The reference collection is null.");
			
			Assert.AreEqual(1, references.Count, "Incorrect number of references returned.");
			
			if (references != null && references.Count > 0)
			{
				EntityReference reference = references[0];
				
				Assert.AreEqual(user.ID, references[0].Entity1ID, "The entity 1 ID wasn't set correctly.");
				Assert.AreEqual(role.ID, references[0].Entity2ID, "The entity 2 ID wasn't set correctly.");
				
				Assert.AreEqual("TestUser", references[0].TypeName1, "The type name 1 wasn't set correctly.");
				Assert.AreEqual("TestRole", references[0].TypeName2, "The type name 2 wasn't set correctly.");
				
				Assert.AreEqual("Roles", references[0].Property1Name, "The property 1 name wasn't set correctly.");
				Assert.AreEqual("Users", references[0].Property2Name, "The property 2 name wasn't set correctly.");
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