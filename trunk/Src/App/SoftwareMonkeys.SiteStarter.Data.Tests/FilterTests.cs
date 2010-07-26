using System;
using SoftwareMonkeys.SiteStarter.Data;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Data.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	/// <summary>
	/// Description of FilterTests.
	/// </summary>
	[TestFixture]
	public class FilterTests
	{
		public FilterTests()
		{
		}
		
		
		[Test]
		public void Test_FilterGroup_Or()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing a filter group query with the OR operator.", NLog.LogLevel.Debug))
			{
				TestEntity e1 = new TestEntity();
				e1.Name = "Test E1";
				
				FilterGroup filterGroup = new FilterGroup();
				filterGroup.Operator = FilterOperator.Or;
				
				PropertyFilter filter1 = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
				filter1.PropertyName = "Name";
				filter1.PropertyValue = e1.Name;
				filter1.AddType(typeof(TestEntity));
				
				PropertyFilter filter2 = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
				filter2.PropertyName = "Name";
				filter2.PropertyValue = "Another value";
				filter2.AddType(typeof(TestEntity));
				
				filterGroup.Filters = new BaseFilter[] {filter1, filter2};
				
				DataAccess.Data.Stores[typeof(TestEntity)].Saver.Save(e1);
				
				IEntity[] found = (IEntity[])DataAccess.Data.Indexer.GetEntities(filterGroup);
				
				Assert.IsNotNull(found, "Null value returned.");
				
				if (found != null)
					Assert.IsTrue(found.Length > 0, "No results found.");
				
				
				DataAccess.Data.Deleter.Delete(e1);
			}
		}

		[Test]
		public void Test_FilterGroup_And_Exclusion()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing a filter group query with the AND operator.", NLog.LogLevel.Debug))
			{
				TestEntity e1 = new TestEntity();
				e1.Name = "Test E1";
				
				FilterGroup filterGroup = new FilterGroup();
				filterGroup.Operator = FilterOperator.And;
				
				PropertyFilter filter1 = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
				filter1.PropertyName = "Name";
				filter1.PropertyValue = e1.Name;
				filter1.AddType(typeof(TestEntity));
				
				PropertyFilter filter2 = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
				filter2.PropertyName = "Name";
				filter2.PropertyValue = "Another value";
				filter2.AddType(typeof(TestEntity));
				
				filterGroup.Filters = new BaseFilter[] {filter1, filter2};
				
				DataAccess.Data.Saver.Save(e1);
				
				IEntity[] found = (IEntity[])DataAccess.Data.Indexer.GetEntities(filterGroup);
				
				Assert.IsNotNull(found, "Null array returned.");
				
				Collection<TestEntity> foundList = new Collection<TestEntity>(found);
				
				if (found != null)
					Assert.AreEqual(0, foundList.Count, "Entity retrieved when it shouldn't have matched.");
				
				
				DataAccess.Data.Deleter.Delete(e1);
			}
		}

	}
}
