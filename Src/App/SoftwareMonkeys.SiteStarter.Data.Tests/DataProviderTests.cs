using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using System.Diagnostics;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Tests;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	public class DataProviderTests : BaseDataTestFixture
	{		
		public DataProviderTests()
		{
		}

		#region Singleton tests
		[Test]
		public void Test_Data_EnsureInitialized()
		{
			DataProvider provider = DataAccess.Data;

			Assert.IsNotNull((object)provider);
		}
		#endregion	
		
		
		#region Tests
		
		[Test]
		public void Test_IsStored_Reference()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the IsStored function on a reference object", NLog.LogLevel.Debug))
			{
				TestArticle.RegisterType();
				TestCategory.RegisterType();

				TestArticle article = new TestArticle();
				article.ID = Guid.NewGuid();
				article.Title = "Test Article";

				TestCategory category = new TestCategory();
				category.ID = Guid.NewGuid();
				category.Name = "Test Category";

				article.Categories = new TestCategory[] { category };

				DataAccess.Data.Saver.Save(article);
				DataAccess.Data.Saver.Save(category);

				EntityReferenceCollection collection = DataAccess.Data.Referencer.GetReferences(article.GetType(), article.ID, "Categories", category.GetType(), false);

				Assert.IsNotNull(collection, "Reference collection is null.");

				if (collection != null)
				{
					Assert.AreEqual(1, collection.Count, "Incorrect number of references found.");
				}

				foreach (EntityReference reference in collection)
				{
					bool match = DataAccess.Data.IsStored(reference);

					Assert.AreEqual(true, match, "Reference wasn't detected.");
				}

				
			}
		}

				
		[Test]
		public void Test_IsStored_Reference_NotStored()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the IsStored function on a reference object", NLog.LogLevel.Debug))
			{
				TestArticle.RegisterType();
				TestCategory.RegisterType();

				TestArticle article = new TestArticle();
				article.ID = Guid.NewGuid();
				article.Title = "Test Article";

				TestCategory category = new TestCategory();
				category.ID = Guid.NewGuid();
				category.Name = "Test Category";

				article.Categories = new TestCategory[] { category };

				//DataAccess.Data.Saver.Save(article);
				//DataAccess.Data.Saver.Save(category);

				EntityReferenceCollection collection = DataAccess.Data.Referencer.GetActiveReferences(article);
				//.Data.GetReferences(article.GetType(), article.ID, "Categories", category.GetType(), false);

				Assert.IsNotNull(collection, "Reference collection is null.");

				if (collection != null)
				{
					Assert.AreEqual(1, collection.Count, "Incorrect number of references found.");
				}

				foreach (EntityReference reference in collection)
				{
					bool match = DataAccess.Data.IsStored(reference);

					Assert.AreEqual(false, match, "Reference matched when it shouldn't have.");
				}
			}
		}

		[Test]
		public void Test_Suspend()
		{
			TestUser user = new TestUser();
			user.ID = Guid.NewGuid();
			user.FirstName = "First";
			user.LastName = "Last";
			user.Email = "Email";
			user.Username = "TestUser";
			
			DataAccess.Data.Saver.Save(user);
			
			DataAccess.Data.Suspend();
			
			TestUser foundUser = DataAccess.Data.Reader.GetEntity<TestUser>("ID", user.ID);
			
			Assert.IsNull(foundUser, "The user wasn't suspended when it should have been.");
		}
		
		[Test]
		public void Test_GetDataStoreNames()
		{
			TestUser user = new TestUser();
			user.ID = Guid.NewGuid();
			user.FirstName = "First";
			user.LastName = "Last";
			user.Email = "Email";
			user.Username = "TestUser";
			
			DataAccess.Data.Saver.Save(user);
			
			string[] names = DataAccess.Data.GetDataStoreNames();
			
			string expected = DataUtilities.GetDataStoreName(user.GetType());
			
			Assert.AreEqual(1, names.Length, "Invalid number of data store names returned.");
			
			Assert.AreEqual(expected, names[0], "The data store name doesn't match what's expected.");
		}
		#endregion
	}
}
