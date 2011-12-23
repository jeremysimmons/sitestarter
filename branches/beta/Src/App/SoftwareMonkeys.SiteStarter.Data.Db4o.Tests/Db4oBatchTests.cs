using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Data.Tests;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o.Tests
{
	[TestFixture]
	public class Db4oBatchTests : BatchTests
	{
		public override void InitializeMockData()
		{
			new MockDb4oDataProviderInitializer(this).Initialize();
		}
		
		[Test]
		public override void Test_OneBatch_NoOperations()
		{
			base.Test_OneBatch_NoOperations();
		}
		
		[Test]
		public override void Test_TwoBatches_Nested()
		{
			base.Test_TwoBatches_Nested();
		}
	}
}
