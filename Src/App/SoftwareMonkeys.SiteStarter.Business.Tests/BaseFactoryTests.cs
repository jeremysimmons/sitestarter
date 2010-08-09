using System;
using NUnit;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Business;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// Description of BaseFactoryTests.
	/// </summary>
	[TestFixture]
	public class BaseFactoryTests : BaseBusinessTestFixture
	{
		public BaseFactoryTests()
		{
		}
		
		
		[Test]
		public void Test_Execute_GetEntities_Paged_Page1()
		{
			TestRecord.RegisterType();
			
			List<TestRecord> list = new List<TestRecord>();
			
			for (int i = 0; i < 30; i++)
			{
				TestRecord record = new TestRecord();
				record.ID = Guid.NewGuid();
				record.Text = "Record " + i;
				
				DataAccess.Data.Saver.Save(record);
				
				list.Add(record);
			}
			
			if (DataAccess.Data == null)
				throw new InvalidOperationException("Data provider has not been initialized. Run setup.");
			
			PagingLocation pagingLocation = new PagingLocation(0, 20);
			
			//object[] parameters = new object[] {pagingLocation,
			//	"NameAscending"};
			
			string sortExpression = "NameAscending";
			
			EntityFactory factory = new EntityFactory();
			
			TestRecord[] entities = factory.GetPage<TestRecord>(pagingLocation, sortExpression);//Execute("Index", "TestRecord", parameters);
			
			Assert.IsNotNull("entities", "entities == null");
			
			Assert.AreEqual(20, entities.Length, "Entity count mismatch.");
			
			Assert.AreEqual(30, pagingLocation.AbsoluteTotal, "Absolute count mismatch.");
			
			foreach (TestRecord record in list)
				DataAccess.Data.Deleter.Delete(record);
		}
		
		
		
		[Test]
		public void Test_Execute_GetEntities_Paged_Page2()
		{
			TestRecord.RegisterType();
			
			List<TestRecord> list = new List<TestRecord>();
			
			for (int i = 0; i < 30; i++)
			{
				TestRecord record = new TestRecord();
				record.ID = Guid.NewGuid();
				record.Text = "Record " + i;
				
				DataAccess.Data.Saver.Save(record);
				
				list.Add(record);
			}
			
			if (DataAccess.Data == null)
				throw new InvalidOperationException("Data provider has not been initialized. Run setup.");
			
			PagingLocation pagingLocation = new PagingLocation(1, 20);
			
			//object[] parameters = new object[] {pagingLocation,
			//	"NameAscending"};
			
			string sortExpression = "NameAscending";
			
			EntityFactory factory = new EntityFactory();
			
			TestRecord[] entities = factory.GetPage<TestRecord>(pagingLocation, sortExpression);//Execute("Index", "TestRecord", parameters);
			
			Assert.IsNotNull("entities", "entities == null");
			
			Assert.AreEqual(10, entities.Length, "Entity count mismatch.");
			
			Assert.AreEqual(30, pagingLocation.AbsoluteTotal, "Absolute count mismatch.");
			
			foreach (TestRecord record in list)
				DataAccess.Data.Deleter.Delete(record);
		}
	}
}
