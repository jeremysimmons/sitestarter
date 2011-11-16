using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Data.Tests;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o.Tests
{
	[TestFixture]
	public class Db4oDataSaverTests : DataSaverTests
	{
		public override void InitializeMockData()
		{
			new MockDb4oDataProviderInitializer(this).Initialize();
		}
		
		[Test]
		public override void Test_Save_SetsCountPropertyForReference()
		{
			base.Test_Save_SetsCountPropertyForReference();
		}
		
		[Test]
		public override void Test_Save_EntityReference()
		{
			base.Test_Save_EntityReference();
		}
	}
}
