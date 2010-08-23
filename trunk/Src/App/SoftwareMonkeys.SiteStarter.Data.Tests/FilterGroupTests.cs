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
	[TestFixture]
	public class FilterGroupTests : BaseDataTestFixture
	{
		public string ApplicationPath
		{
			get { return TestUtilities.GetTestingPath(); }
		}

		#region Singleton tests
		[Test]
		public void Test_Data_EnsureInitialized()
		{
			DataProvider provider = DataAccess.Data;

			Assert.IsNotNull((object)provider);
		}
		#endregion


		/*[TearDown]
        public void CleanUp()
        {
            ClearTestEntities();

        }*/

		#region Tests
		
		[Test]
		public void Test_IsMatch_And_True()
		{
			TestArticle.RegisterType();
			
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Article1";
			
			FilterGroup group = new FilterGroup();
			
			PropertyFilter filter1 = new PropertyFilter();
			filter1.AddType(typeof(TestArticle));
			filter1.PropertyName = "Title";
			filter1.PropertyValue = article.Title;
			
			Assert.IsTrue(filter1.IsMatch(article), "filter1 failed to match article when it should.");
			
			
			PropertyFilter filter2 = new PropertyFilter();
			filter2.AddType(typeof(TestArticle));
			filter2.PropertyName = "ID";
			filter2.PropertyValue = article.ID;
			
			Assert.IsTrue(filter2.IsMatch(article), "filter2 failed to match article when it should.");
			
			group.Add(filter1);
			group.Add(filter2);
			
			Assert.IsTrue(group.IsMatch(article), "group failed to match when it should");
		}
		
		[Test]
		public void Test_IsMatch_And_False()
		{
			
			TestArticle.RegisterType();
			
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Article1";
			
			FilterGroup group = new FilterGroup();
			
			PropertyFilter filter1 = new PropertyFilter();
			filter1.AddType(typeof(TestArticle));
			filter1.PropertyName = "Title";
			filter1.PropertyValue = article.Title;
			
			Assert.IsTrue(filter1.IsMatch(article), "filter1 failed to match article when it should.");
			
			
			PropertyFilter filter2 = new PropertyFilter();
			filter2.AddType(typeof(TestArticle));
			filter2.PropertyName = "Title";
			filter2.PropertyValue = "MISMATCH"; // This one should fail
			
			Assert.IsFalse(filter2.IsMatch(article), "filter2 matched when it should fail.");
			
			group.Add(filter1);
			group.Add(filter2);
			
			Assert.IsFalse(group.IsMatch(article), "group matched when it should fail");
		}
		
		[Test]
		public void Test_IsMatch_Or_True_BothMatch()
		{
			TestArticle.RegisterType();
			
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Article1";
			
			FilterGroup group = new FilterGroup();
			group.Operator = FilterOperator.Or;
			
			PropertyFilter filter1 = new PropertyFilter();
			filter1.AddType(typeof(TestArticle));
			filter1.PropertyName = "Title";
			filter1.PropertyValue = article.Title;
			
			Assert.IsTrue(filter1.IsMatch(article), "filter1 failed to match article when it should.");
			
			
			PropertyFilter filter2 = new PropertyFilter();
			filter2.AddType(typeof(TestArticle));
			filter2.PropertyName = "ID";
			filter2.PropertyValue = article.ID;
			
			Assert.IsTrue(filter2.IsMatch(article), "filter2 failed to match article when it should.");
			
			group.Add(filter1);
			group.Add(filter2);
			
			Assert.IsTrue(group.IsMatch(article), "group failed to match when it should");
		}
		
		[Test]
		public void Test_IsMatch_Or_True_OneMatches()
		{
			
			TestArticle.RegisterType();
			
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Article1";
			
			FilterGroup group = new FilterGroup();
			group.Operator = FilterOperator.Or;
			
			PropertyFilter filter1 = new PropertyFilter();
			filter1.AddType(typeof(TestArticle));
			filter1.PropertyName = "Title";
			filter1.PropertyValue = article.Title;
			
			Assert.IsTrue(filter1.IsMatch(article), "filter1 failed to match article when it should.");
			
			
			PropertyFilter filter2 = new PropertyFilter();
			filter2.AddType(typeof(TestArticle));
			filter2.PropertyName = "Title";
			filter2.PropertyValue = "MISMATCH"; // This one should fail
			
			Assert.IsFalse(filter2.IsMatch(article), "filter2 matched when it should fail.");
			
			group.Add(filter1);
			group.Add(filter2);
			
			Assert.IsTrue(group.IsMatch(article), "group failed when it should match");
		}
		
		[Test]
		public void Test_GetEntityByFilterGroup_PropertyAndReference()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the ReferenceFilter.IsMatch function.", NLog.LogLevel.Debug))
			{
				ClearTestEntities();
				
				TestArticle.RegisterType();
				TestCategory.RegisterType();
				
				TestArticle article = new TestArticle();
				article.ID = Guid.NewGuid();
				article.Title = "Article1";
				
				TestCategory category = new TestCategory();
				category.ID = Guid.NewGuid();
				category.Name = "Category1";
				
				
				TestArticle article2 = new TestArticle();
				article2.ID = Guid.NewGuid();
				article2.Title = "Article2";
				
				TestCategory category2 = new TestCategory();
				category2.ID = Guid.NewGuid();
				category2.Name = "Category2";
				
				article.Categories = new TestCategory[] {category};
				
				article2.Categories = new TestCategory[] {category2};
				
				DataAccess.Data.Saver.Save(article2);
				DataAccess.Data.Saver.Save(category2);
				DataAccess.Data.Saver.Save(article);
				DataAccess.Data.Saver.Save(category);
				
				FilterGroup filterGroup = new FilterGroup();
				filterGroup.Operator = FilterOperator.And;
				
				PropertyFilter filter1 = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
				filter1.PropertyName = "Title";
				filter1.PropertyValue = article.Title;
				filter1.AddType(typeof(TestArticle));

				ReferenceFilter filter2 = (ReferenceFilter)DataAccess.Data.CreateFilter(typeof(ReferenceFilter));
				filter2.PropertyName = "Categories";
				filter2.ReferencedEntityID = category.ID;
				filter2.ReferenceType = typeof(TestCategory);
				filter2.AddType(typeof(TestArticle));
				
				filterGroup.Add(filter1);
				filterGroup.Add(filter2);
				
				
				
				
				Assert.IsTrue(filter1.IsMatch(article), "filter1 (property filter) doesn't match test article.");
				Assert.IsTrue(filter2.IsMatch(article), "filter2 (reference filter) doesn't match test article.");

				
				Assert.IsFalse(filter1.IsMatch(article2), "filter1 (property filter) matches article2 when it shouldn't.");
				Assert.IsFalse(filter2.IsMatch(article2), "filter2 (reference filter) matches article2 when it shouldn't.");

				
				TestArticle foundArticle = (TestArticle)DataAccess.Data.Reader.GetEntity(filterGroup);
				
				
				Assert.IsNotNull(foundArticle, "foundArticle == null");
				
				Assert.AreEqual(article.ID.ToString(), foundArticle.ID.ToString(), "Found article ID doesn't match expected.");
				
				
				//bool isMatch = filter.IsMatch(article);
				//Assert.IsTrue(isMatch, "The IsMatch function returned false when it should have been true.");
				ClearTestEntities();
			}
		}
		
		
		[Test]
		public void Test_GetEntityByFilterGroup_PropertyAndReference_Exclude()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the ReferenceFilter.IsMatch function.", NLog.LogLevel.Debug))
			{
				ClearTestEntities();
				
				TestArticle.RegisterType();
				TestCategory.RegisterType();
				
				TestArticle article = new TestArticle();
				article.ID = Guid.NewGuid();
				article.Title = "Article1";
				
				TestCategory category = new TestCategory();
				category.ID = Guid.NewGuid();
				category.Name = "Category1";
				
				
				TestArticle article2 = new TestArticle();
				article2.ID = Guid.NewGuid();
				article2.Title = "Article2";
				
				TestCategory category2 = new TestCategory();
				category2.ID = Guid.NewGuid();
				category2.Name = "Category2";
				
				article.Categories = new TestCategory[] {category};
				
				article2.Categories = new TestCategory[] {category2};
				
				DataAccess.Data.Saver.Save(article2);
				DataAccess.Data.Saver.Save(category2);
				DataAccess.Data.Saver.Save(article);
				DataAccess.Data.Saver.Save(category);
				
				
				
				FilterGroup failingFilterGroup = new FilterGroup();
				failingFilterGroup.Operator = FilterOperator.And;
				
				PropertyFilter failingFilter1 = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
				failingFilter1.PropertyName = "Title";
				failingFilter1.PropertyValue = article.Title;
				failingFilter1.AddType(typeof(TestArticle));

				ReferenceFilter failingFilter2 = (ReferenceFilter)DataAccess.Data.CreateFilter(typeof(ReferenceFilter));
				failingFilter2.PropertyName = "Categories";
				failingFilter2.ReferencedEntityID = category2.ID;
				failingFilter2.ReferenceType = typeof(TestCategory);
				failingFilter2.AddType(typeof(TestArticle));
				
				failingFilterGroup.Add(failingFilter1);
				failingFilterGroup.Add(failingFilter2);
				
				
				
				//Assert.IsFalse(failingFilter1.IsMatch(article2), "failingFilter1 (property filter) matches when it shouldn't.");
				//Assert.IsFalse(failingFilter2.IsMatch(article2), "failingFilter2 (reference filter) matches when it shouldn't.");

				
				//Assert.IsTrue(failingFilter1.IsMatch(article), "failingFilter1 (property filter) doesn't match secondary article when it should.");
				//Assert.IsTrue(failingFilter2.IsMatch(article), "failingFilter2 (reference filter) doesn't match secondary article when it should.");

				
				// This next one SHOULD FAIL to return anything
				TestArticle excludedArticle = (TestArticle)DataAccess.Data.Reader.GetEntity(failingFilterGroup);
				
				Assert.IsNull(excludedArticle, "The excludedArticle should be null because it should have failed to match.");
				
				
				//bool isMatch = filter.IsMatch(article);
				//Assert.IsTrue(isMatch, "The IsMatch function returned false when it should have been true.");
				ClearTestEntities();
			}
		}
		#endregion

		private void ClearTestEntities()
		{
			TestArticle.RegisterType();
			TestCategory.RegisterType();
			TestUser.RegisterType();
			TestRole.RegisterType();
			TestEntity.RegisterType();
			EntityOne.RegisterType();
			EntityTwo.RegisterType();
			EntityThree.RegisterType();
			EntityFour.RegisterType();
			
			Type[] types = new Type[] {
				typeof(TestArticle),
				typeof(TestCategory),
				typeof(TestUser),
				typeof(TestRole),
				typeof(EntityOne),
				typeof(EntityTwo),
				typeof(TestEntity), typeof(EntityThree), typeof(EntityFour) };

			Collection<IEntity> entities = new Collection<IEntity>();
			foreach (Type type in types)
				entities.Add((IEntity[])DataAccess.Data.Indexer.GetEntities(type));

			foreach (IEntity entity in entities)
			{
				DataAccess.Data.Deleter.Delete(entity);
			}
			
			string[] referenceStoreNames = new String[]{
				DataUtilities.GetDataStoreName("TestArticle", "TestCategory"),
				DataUtilities.GetDataStoreName("TestUser", "TestRole")
			};
			
			foreach (string storeName in referenceStoreNames)
			{
				foreach (IEntity entity in DataAccess.Data.Stores[storeName].Indexer.GetEntities<EntityIDReference>())
				{
					DataAccess.Data.Stores[storeName].Deleter.Delete(entity);
				}
			}
		}
	}


}

