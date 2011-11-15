using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Data.Tests;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o.Tests
{
	[TestFixture]
	public class Db4oDataUpdaterTests : DataUpdaterTests
	{
		public override void InitializeMockData()
		{
			new MockDb4oDataProviderInitializer(this).Initialize();
		}
		
		[Test]
		public override void Test_Update_SetsCountPropertyForReference_OneWay()
		{
			base.Test_Update_SetsCountPropertyForReference_OneWay();
		}
		
		[Test]
		public override void Test_Update_SetsCountPropertyForReference_TwoWay()
		{
			base.Test_Update_SetsCountPropertyForReference_TwoWay();
		}
		
		[Test]
		public override void Test_Update_RemoveObsoleteReference_Sync()
		{
			base.Test_Update_RemoveObsoleteReference_Sync();
		}
	}
}
