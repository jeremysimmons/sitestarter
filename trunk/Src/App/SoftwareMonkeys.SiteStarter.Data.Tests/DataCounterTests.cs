using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Entities.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class DataCounterTests : BaseDataTestFixture
	{
		public DataCounterTests()
		{
		}
		
		[Test]
		public void Test_CountEntities_Generic_NoParameters()
		{
			TestRecord record = new TestRecord();
			record.ID = Guid.NewGuid();
			record.Name = "Test";
			
			int before = DataAccess.Data.Counter.CountEntities<TestRecord>();
			
			Assert.AreEqual(0, before);
			
			DataAccess.Data.Saver.Save(record);
			
			int after = DataAccess.Data.Counter.CountEntities<TestRecord>();
			
			Assert.AreEqual(1, after);
		}
		
		[Test]
		public void Test_CountEntitiesWithReference_OneWayReference()
		{
			
			MockEntity entity = new MockEntity();
			entity.ID = Guid.NewGuid();
			
			MockPublicEntity publicEntity = new MockPublicEntity();
			publicEntity.ID = Guid.NewGuid();
			
			entity.PublicEntities = new MockPublicEntity[]{
				publicEntity
			};
			
			int before = DataAccess.Data.Counter.CountEntitiesWithReference(typeof(MockPublicEntity),
			                                                                publicEntity.ID,
			                                                                "",
			                                                                typeof(MockEntity),
			                                                                "PublicEntities");
			
			Assert.AreEqual(0, before);
			
			DataAccess.Data.Saver.Save(publicEntity);
			DataAccess.Data.Saver.Save(entity);
			
			int after = DataAccess.Data.Counter.CountEntitiesWithReference(typeof(MockPublicEntity),
			                                                                publicEntity.ID,
			                                                                "",
			                                                                typeof(MockEntity),
			                                                                "PublicEntities");
			
			Assert.AreEqual(1, after);
		}
	}
}
