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
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Tests;

namespace SoftwareMonkeys.SiteStarter.Entities.Tests
{
	[TestFixture]
	public class EntitiesUtilitiesTests : BaseEntityTestFixture
	{		
		#region Tests
		[Test]
		public void Test_IsReference_True_InterfaceType()
		{
			EntityFive e5 = new EntityFive();
			
			PropertyInfo property = e5.GetType().GetProperty("InterfaceReferencedEntities");
			
			Assert.IsTrue(EntitiesUtilities.IsReference(e5.GetType(), property), "Returned false when it should have returned true.");
		}
		

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
		public void Test_GetMirrorPropertyName_Multiple_Implicit_Async_SameEntity()
		{
			TestGoal goal = new TestGoal();
			goal.ID = Guid.NewGuid();
			
			string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(goal, goal.GetType().GetProperty("Prerequisites"));
			
			Assert.AreEqual(String.Empty, mirrorPropertyName, "The mirror property name wasn't determined correctly.");
		}
		
				[Test]
		public void Test_GetMirrorPropertyNameReverse_Multiple_Implicit_Sync()
		{
			TestUser user = new TestUser();
			
			string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyNameReverse(user, "Roles", typeof(TestRole));
			
			Assert.AreEqual("Users", mirrorPropertyName, "The mirror property name wasn't determined correctly.");
		}
		
		[Test]
		public void Test_GetMirrorPropertyNameReverse_Multiple_Implicit_Async()
		{
			MockEntity entity = new MockEntity();
			MockPublicEntity publicEntity = new MockPublicEntity();
			
			string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyNameReverse(publicEntity, "", typeof(MockEntity));
			
			Assert.AreEqual("PublicEntities", mirrorPropertyName, "The mirror property name wasn't determined correctly.");
		}
		
		[Test]
		public void Test_GetMirrorPropertyName_Multiple_Implicit_Sync()
		{
			TestUser user = new TestUser();
			
			string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(user, user.GetType().GetProperty("Roles"));
			
			Assert.AreEqual("Users", mirrorPropertyName, "The mirror property name wasn't determined correctly.");
		}
		
		[Test]
		public void Test_GetMirrorPropertyName_Single_Implicit_Sync()
		{
			EntityOne e1 = new EntityOne();
			
			string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(e1, e1.GetType().GetProperty("SingleReferenceProperty"));
			
			Assert.AreEqual(String.Empty, mirrorPropertyName, "The mirror property name wasn't determined correctly.");
		}
		
		[Test]
		public void Test_GetMirrorPropertyName_Single_Explicit_Sync()
		{
			TestSample sample = new TestSample();
			
			string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(sample, sample.GetType().GetProperty("Articles"));
			
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
		
		
		#endregion
		
	}
}
