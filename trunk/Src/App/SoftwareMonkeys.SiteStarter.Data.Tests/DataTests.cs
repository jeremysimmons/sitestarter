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
using SoftwareMonkeys.SiteStarter.Data.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;
 
namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
    [TestFixture]
    public class DataTests
    {
        public string ApplicationPath
        {
            // TODO: Path MUST NOT be hard coded
         //   get { return @"f:\SoftwareMonkeys\WorkHub\Application 2\Web\"; }
       //     get { return System.Configuration.ConfigurationSettings.AppSettings["ApplicationPath"]; }
            get { return SoftwareMonkeys.SiteStarter.Configuration.Config.Application.PhysicalPath; }
        }

        #region Singleton tests
        [Test]
        public void Test_Data_EnsureInitialized()
        {
            DataProvider provider = DataAccess.Data;

            Assert.IsNotNull((object)provider);
        }
        #endregion

	#region Filter tests
	[Test]
	public void Test_PropertyFilter()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing a simple query with the PropertyFilter.", NLog.LogLevel.Debug))
		{
	        	TestEntity e1 = new TestEntity();
			e1.Name = "Test E1";
		
			PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
			filter.PropertyName = "Name";
		        filter.PropertyValue = e1.Name;
		        filter.AddType(typeof(TestEntity));
		
			DataAccess.Data.Stores[typeof(TestEntity)].Save(e1);
		
		        BaseEntity[] found = (BaseEntity[])DataAccess.Data.GetEntities(filter);
		        Collection<TestEntity> foundList = new Collection<TestEntity>(found);
	
			if (found != null)
				Assert.Greater(foundList.Count, 0, "No results found.");
	
	       		DataAccess.Data.Stores[typeof(TestEntity)].Delete(e1);
		}
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
		
				DataAccess.Data.Stores[typeof(TestEntity)].Save(e1);
		
		        BaseEntity[] found = (BaseEntity[])DataAccess.Data.GetEntities(filterGroup);
	
			if (found != null)
				Assert.Greater(found.Length, 0, "No results found.");
	
	
	       		DataAccess.Data.Stores[typeof(TestEntity)].Delete(e1);
		}
	}

	[Test]
	public void Test_FilterGroup_And()
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
		
				DataAccess.Data.Stores[typeof(TestEntity)].Save(e1);
		
		        BaseEntity[] found = (BaseEntity[])DataAccess.Data.GetEntities(filterGroup);
		        Collection<TestEntity> foundList = new Collection<TestEntity>(found);
	
			if (found != null)
				Assert.AreEqual(foundList.Count, 0, "Matches found when they shouldn't be.");
	
	
	       		DataAccess.Data.Stores[typeof(TestEntity)].Delete(e1);
		}
	}

	[Test]
	public void Test_PropertyFilter_Exclusion()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing exclusion with the PropertyFilter.", NLog.LogLevel.Debug))
		{
            		TestEntity e1 = new TestEntity();
			e1.ID = Guid.NewGuid();
			e1.Name = "Test E1";

			AppLogger.Debug("Entity ID: " + e1.ID);
			AppLogger.Debug("Entity name: " + e1.Name);
	
			//FilterGroup filterGroup = new FilterGroup();
			//filterGroup.Operator
	
			PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
			filter.Operator = FilterOperator.Equal;
		            filter.PropertyName = "Name";
		            filter.PropertyValue = "Another Name";
		            filter.AddType(typeof(TestEntity));
		
		
		            AppLogger.Debug("Filter.FieldName: " + filter.PropertyName);
		            AppLogger.Debug("Filter.FieldValue: " + filter.PropertyValue);
		
		            DataAccess.Data.Stores[typeof(TestEntity)].Save(e1);
		
		            BaseEntity[] found = (BaseEntity[])DataAccess.Data.GetEntities(filter);
		            Collection<TestEntity> foundList = new Collection<TestEntity>(found);
	
			if (found != null)
				Assert.AreEqual(0, foundList.Count, "Entities weren't properly excluded.");


            		DataAccess.Data.Stores[typeof(TestEntity)].Delete(e1);
		}
	}

	[Test]
	public void Test_GetByPropertyValue()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing data retrieval with the GetEntities by property value function.", NLog.LogLevel.Debug))
		{
			EntityOne e1 = new EntityOne();
			e1.Name = "Test E1";
	
			DataAccess.Data.Stores[typeof(EntityOne)].Save(e1);

			BaseEntity[] found = (BaseEntity[])DataAccess.Data.Stores[typeof(EntityOne)].GetEntities(typeof(EntityOne), "name", e1.Name);
			Collection<EntityOne> foundList = new Collection<EntityOne>(found);
	
			if (found != null)
				Assert.AreEqual(0, foundList.Count, "No results found.");
	
	
			DataAccess.Data.Stores[typeof(EntityOne)].Delete(e1);
		}
	}


	[Test]
	public void Test_GetByPropertyValue_Exclusion()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing exclusion with the GetEntities by property value function.", NLog.LogLevel.Debug))
		{
			EntityOne e1 = new EntityOne();
			e1.Name = "Test E1";
	
			//FilterGroup filterGroup = new FilterGroup();
			//filterGroup.Operator
	
			PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
			filter.Operator = FilterOperator.Equal;
            filter.PropertyName = "name";
            filter.PropertyValue = "Another Name";
	
			DataAccess.Data.Stores[typeof(EntityOne)].Save(e1);

			BaseEntity[] found = (BaseEntity[])DataAccess.Data.Stores[typeof(EntityOne)].GetEntities(typeof(EntityOne), "name", "Another Name");
			Collection<EntityOne> foundList = new Collection<EntityOne>(found);
	
			if (found != null)
				Assert.AreEqual(0, foundList.Count, "Entities weren't properly excluded.");
	
	
			DataAccess.Data.Stores[typeof(EntityOne)].Delete(e1);
		}
	}
	#endregion

    }
}
