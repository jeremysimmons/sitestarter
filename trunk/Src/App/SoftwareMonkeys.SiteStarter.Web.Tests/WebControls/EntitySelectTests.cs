using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Web.WebControls;

namespace SoftwareMonkeys.SiteStarter.Web.Tests.WebControls
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class EntitySelectTests : BaseWebTestFixture
	{
		/// <summary>
		/// Tests the GetPostEntities function when AutoLoadPostEntities property is true.
		/// </summary>
		[Test]
		public void Test_GetPostedEntities_AutoLoadPostEntitiesTrue()
		{
			TestRecord record = new TestRecord();
			record.ID = Guid.NewGuid();
			
			EntitySelect<TestRecord> ctrl = new EntitySelect<TestRecord>();
			ctrl.AutoLoadPostEntities = true;			
			ctrl.EntityType = record.GetType().ToString();
			ctrl.RequireAuthorisation = false; // Only false during testing
			
			Guid[] ids = new Guid[] { record.ID };
			
			SaveStrategy.New(record, false).Save(record);
			
			IEntity[] entities = ctrl.GetPostedEntities(ids);
			
			Assert.IsNotNull(entities, "Returned null");
			
			// The entity should have been loaded automatically
			Assert.AreEqual(1, entities.Length, "Invalid number of entities returned.");
		}
		
		/// <summary>
		/// Tests the GetPostEntities function when AutoLoadPostEntities property is false.
		/// </summary>
		[Test]
		public void Test_GetPostedEntities_AutoLoadPostEntitiesFalse()
		{
			TestRecord record = new TestRecord();
			record.ID = Guid.NewGuid();
			
			EntitySelect<TestRecord> ctrl = new EntitySelect<TestRecord>();
			ctrl.AutoLoadPostEntities = false;				
			ctrl.EntityType = record.GetType().ToString();
			ctrl.RequireAuthorisation = false; // Only false during testing
			
			Guid[] ids = new Guid[] { record.ID };
			
			SaveStrategy.New(record, false).Save(record);
			
			IEntity[] entities = ctrl.GetPostedEntities(ids);
			
			Assert.IsNotNull(entities, "Returned null");
			
			// No entities should be returned because they weren't found on DataSource
			Assert.AreEqual(0, entities.Length, "Invalid number of entities returned.");
		}
	}
}
