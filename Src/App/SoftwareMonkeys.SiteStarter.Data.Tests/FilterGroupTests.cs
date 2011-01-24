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
			get { return TestUtilities.GetTestingPath(this); }
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
		public void Test_IsMatch_2SubGroups_False()
		{
			
			
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Article1";
			
			// Outer group
			FilterGroup group = new FilterGroup();
			group.Operator = FilterGroupOperator.And; // BOTH child groups MUST match
			
			FilterGroup subGroup1 = new MockFailingFilterGroup();
			FilterGroup subGroup2 = new MockMatchingFilterGroup();
			
			group.Add(subGroup1);
			group.Add(subGroup2);
			
			Assert.IsFalse(subGroup1.IsMatch(article), "First sub group matches when it shouldn't.");
			Assert.IsTrue(subGroup2.IsMatch(article), "Second sub group doesn't match when it should.");
			
			Assert.IsFalse(group.IsMatch(article), "Entire group matched when it shouldn't match");
		}
		
		[Test]
		public void Test_IsMatch_2SubGroups_True()
		{
			
			
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Article1";
			
			// Outer group
			FilterGroup group = new FilterGroup();
			group.Operator = FilterGroupOperator.And; // BOTH child groups MUST match
			
			FilterGroup subGroup1 = new MockMatchingFilterGroup();
			FilterGroup subGroup2 = new MockMatchingFilterGroup();
			
			group.Add(subGroup1);
			group.Add(subGroup2);
			
			Assert.IsTrue(subGroup1.IsMatch(article), "First sub group doesn't match when it should.");
			Assert.IsTrue(subGroup2.IsMatch(article), "Second sub group doesn't match when it should.");
			
			Assert.IsTrue(group.IsMatch(article), "Entire group failed to match.");
		}
		
		[Test]
		public void Test_IsMatch_And_True()
		{
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
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Article1";
			
			FilterGroup group = new FilterGroup();
			group.Operator = FilterGroupOperator.Or;
			
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
			
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Article1";
			
			FilterGroup group = new FilterGroup();
			group.Operator = FilterGroupOperator.Or;
			
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
		[ExpectedException("System.InvalidOperationException")]
		public void Test_IsMatch_EmptyGroup()
		{
			
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Article1";
			
			FilterGroup group = new FilterGroup();
						
			Assert.IsTrue(group.IsMatch(article), "group failed to match when it should match");
		}
		
		// TODO: Check if needed.
		// Should be obsolete now due to other unit tests
		/*
		[Test]
		public void Test_2ChildGroups_IsMatch_OuterAnd_SubOr_True()
		{
			
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Article1";
			
			FilterGroup group = new FilterGroup();
			group.Operator = FilterOperator.And;
			
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
			
			FilterGroup subGroup1 = new FilterGroup();
			subGroup1.Operator = FilterOperator.And;
			subGroup1.Add(filter1);
			subGroup1.Add(filter2);
			
			
			PropertyFilter filter3 = new PropertyFilter();
			filter3.AddType(typeof(TestArticle));
			filter3.PropertyName = "ID";
			filter3.PropertyValue = article.ID;
			
			Assert.IsTrue(filter3.IsMatch(article), "filter3 matched when it should fail.");
			
			
			FilterGroup subGroup2 = new FilterGroup();
			subGroup2.Operator = FilterOperator.And;
			subGroup2.Add(filter3);
			
			group.Add(subGroup1);
			group.Add(subGroup2);
			
			Assert.IsTrue(group.IsMatch(article), "group failed when it should match");
		}*/
		
		// TODO: Check if needed.
		// Should be obsolete now due to other unit tests
		/*
		[Test]
		public void Test_2ChildGroups_IsMatch_OuterAnd_SubAnd_False()
		{
			
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Article1";
			
			// Outer group
			FilterGroup group = new FilterGroup();
			group.Operator = FilterOperator.And; // BOTH child groups MUST match
			
			// Child group 1 - filter 1
			PropertyFilter filter1 = new PropertyFilter();
			filter1.AddType(typeof(TestArticle));
			filter1.PropertyName = "Title";
			filter1.PropertyValue = article.Title; // This should match
			
			Assert.IsTrue(filter1.IsMatch(article), "filter1 failed to match article when it should.");
			
			
			// Child group 1 - filter 2
			PropertyFilter filter2 = new PropertyFilter();
			filter2.AddType(typeof(TestArticle));
			filter2.PropertyName = "Title";
			filter2.PropertyValue = "MISMATCH"; // This should fail
			
			Assert.IsFalse(filter2.IsMatch(article), "filter2 matched when it should fail.");
			
			// Child group 1
			FilterGroup subGroup1 = new FilterGroup();
			subGroup1.Operator = FilterOperator.And; // BOTH filters in this group need to succeed for it to match (which they won't in this test)
			subGroup1.Add(filter1);
			subGroup1.Add(filter2);
			
			
			// Child group 2 - filter 1
			PropertyFilter filter3 = new PropertyFilter();
			filter3.AddType(typeof(TestArticle));
			filter3.PropertyName = "ID";
			filter3.PropertyValue = article.ID;
			
			Assert.IsTrue(filter3.IsMatch(article), "filter3 failed to match when it should.");
			
			// Child grou p2
			FilterGroup subGroup2 = new FilterGroup();
			subGroup2.Operator = FilterOperator.And;
			subGroup2.Add(filter3);
			
			group.Add(subGroup1);
			group.Add(subGroup2);
			
			Assert.IsFalse(subGroup1.IsMatch(article), "First sub group matches when it shouldn't.");
			Assert.IsTrue(subGroup2.IsMatch(article), "Second sub group doesn't match when it should.");
			
			Assert.IsFalse(group.IsMatch(article), "Entire group matched when it shouldn't match");
		}*/
		#endregion

		private void ClearTestEntities()
		{	
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

