using System;
using NUnit.Framework;
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
	}
}
