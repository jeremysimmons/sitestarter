using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Tests;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	[TestFixture]
	public class DataUtilitiesTests : BaseDataTestFixture
	{
		public string ApplicationPath
		{
			get { return  TestUtilities.GetTestingPath(this); }
		}
		
		
		#region Singleton tests
		[Test]
		public void Test_Data_EnsureInitialized()
		{
			DataProvider provider = DataAccess.Data;

			Assert.IsNotNull((object)provider);
		}
		#endregion
		
		[Test]
		public void Test_GetType_AliasName()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the DataUtilities.GetType function.", NLog.LogLevel.Debug))
			{
				TestArticle e1 = new TestArticle();
				e1.ID = Guid.NewGuid();
				e1.Title = "Test 1";
				

				TestArticlePage e2 = new TestArticlePage();
				e2.ID = Guid.NewGuid();
				e2.Title = "Test 2";

				e1.Pages = new TestArticlePage[] { e2 };
				
				Type type =  EntitiesUtilities.GetType("ITestArticle");

				Assert.IsNotNull(type, "Null type was returned.");
				
				if (type != null)
					Assert.AreEqual("TestArticle", type.Name, "The wrong data type was returned.");
			}
		}
		
		/*[Test]
	public void Test_GetDataStoreNameForReference_Many_Null()
	{
		using (LogGroup logGroup = LogGroup.Start("Testing the GetDataStoreNameForReference function for a multiple reference.", NLog.LogLevel.Debug))
		{
	        	TestArticle e1 = new TestArticle();
			e1.ID = Guid.NewGuid();
			e1.Title = "Test 1";
			

			TestArticlePage e2 = new TestArticlePage();
			e2.ID = Guid.NewGuid();
			e2.Title = "Test 2";
			
			//e2.ArticleID = e1.ID;
			
			string name = DataUtilities.GetDataStoreNameForReference(e2, e2.GetType().GetProperty("ArticleID"));

			
			Assert.AreEqual("Testing_Articles", name, "The wrong data store name was returned.");
		}
	}
	
		[Test]
	public void Test_GetDataStoreNameForReference_Many_NotNull()
	{
		using (LogGroup logGroup = LogGroup.Start("Testing the GetDataStoreNameForReference function for a multiple reference.", NLog.LogLevel.Debug))
		{
	        	TestArticle e1 = new TestArticle();
			e1.ID = Guid.NewGuid();
			e1.Title = "Test 1";
			

			TestArticlePage e2 = new TestArticlePage();
			e2.ID = Guid.NewGuid();
			e2.Title = "Test 2";
			
			e2.ArticleID = e1.ID;
			
			string name = DataUtilities.GetDataStoreNameForReference(e2, e2.GetType().GetProperty("ArticleID"));

			
			Assert.AreEqual("Testing_Articles", name, "The wrong data store name was returned.");
		}
	}*/
		
		[Test]
		public void Test_GetEntityType()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the DataUtilities.GetEntityType function.", NLog.LogLevel.Debug))
			{
				
				TestUser user = new TestUser();
				user.ID = Guid.NewGuid();
				user.FirstName = "Test";
				user.LastName = "User";
				
				TestRole role = new TestRole();
				role.ID = Guid.NewGuid();
				role.Name = "Test Role";
				
				user.Roles = Collection<TestRole>.Add(user.Roles, role);
				role.Users = Collection<TestUser>.Add(role.Users, user);
				
				PropertyInfo rolesProperty = user.GetType().GetProperty("Roles");
				
				Type entityType = DataUtilities.GetEntityType(user, rolesProperty);
				
				Assert.AreEqual(entityType.FullName, role.GetType().FullName, "The types don't match #1.");
				
				
				
				PropertyInfo usersProperty = role.GetType().GetProperty("Users");
				
				Type entityType2 = DataUtilities.GetEntityType(role, usersProperty);
				
				Assert.AreEqual(entityType2.FullName, user.GetType().FullName, "The types don't match #2.");
			}
		}
		
		
		[Test]
		public void Test_GetType_Alias()
		{
			
			//TestUser user = new TestUser();
			//user.ID = Guid.NewGuid();
			//user.FirstName = "Test";
			//user.LastName = "User";
			
			//TestRole role = new TestRole();
			//role.ID = Guid.NewGuid();
			//role.Name = "Test Role";
			
			//user.Roles.Add(role);
			
			//PropertyInfo rolesProperty = user.GetType().GetProperty("Roles");
			
			Type type = EntitiesUtilities.GetType("ITestUser");
			
			Assert.AreEqual(type.FullName, typeof(TestUser).FullName, "The types don't match.");
		}
		
		[Test]
		public void Test_GetType_Name()
		{
			
			//TestUser user = new TestUser();
			//user.ID = Guid.NewGuid();
			//user.FirstName = "Test";
			//user.LastName = "User";
			
			//TestRole role = new TestRole();
			//role.ID = Guid.NewGuid();
			//role.Name = "Test Role";
			
			//user.Roles.Add(role);
			
			//PropertyInfo rolesProperty = user.GetType().GetProperty("Roles");
			
			Type type = EntitiesUtilities.GetType("TestUser");
			
			Assert.AreEqual(type.FullName, typeof(TestUser).FullName, "The types don't match.");
			
		}
		
		[Test]
		public void Test_GetDataStoreName_FromType()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the GetDataStoreName function from a provided type.", NLog.LogLevel.Debug))
			{
				TestArticle e1 = new TestArticle();
				e1.ID = Guid.NewGuid();
				e1.Title = "Test 1";
				

				TestArticlePage e2 = new TestArticlePage();
				e2.ID = Guid.NewGuid();
				e2.Title = "Test 2";
				
				//e2.ArticleID = e1.ID;
				
				string name = DataUtilities.GetDataStoreName(typeof(TestArticle));

				
				Assert.AreEqual(e1.ShortTypeName, name, "The wrong data store name was returned.");
			}
		}
		
		[Test]
		public void Test_GetDataStoreName_FromEntity()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the GetDataStoreName function from a provided type.", NLog.LogLevel.Debug))
			{
				TestArticle e1 = new TestArticle();
				e1.ID = Guid.NewGuid();
				e1.Title = "Test 1";
				

				TestArticlePage e2 = new TestArticlePage();
				e2.ID = Guid.NewGuid();
				e2.Title = "Test 2";
				
				//e2.ArticleID = e1.ID;
				
				string name = DataUtilities.GetDataStoreName(e1);

				
				Assert.AreEqual(e1.ShortTypeName, name, "The wrong data store name was returned.");
			}
		}
		
		[Test]
		public void Test_GetDataStoreName_FromNames()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the GetDataStoreName function from a provided type.", NLog.LogLevel.Debug))
			{
				TestArticle e1 = new TestArticle();
				e1.ID = Guid.NewGuid();
				e1.Title = "Test 1";
				

				TestArticlePage e2 = new TestArticlePage();
				e2.ID = Guid.NewGuid();
				e2.Title = "Test 2";
				
				//e2.ArticleID = e1.ID;
				
				string name = DataUtilities.GetDataStoreName(e1.GetType().Name, e2.GetType().Name);

				
				Assert.AreEqual(e1.ShortTypeName + "-" + e2.ShortTypeName, name, "The wrong data store name was returned.");
			}
		}
		
	}
}
