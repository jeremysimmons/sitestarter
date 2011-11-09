using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Data.Tests;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o.Tests
{
	[TestFixture]
	public class Db4oDataReferencerTests : DataReferencerTests
	{
		public override void InitializeMockData()
		{
			new MockDb4oDataProviderInitializer(this).Initialize();
		}
		
		[Test]
		public override void Test_SetCountProperties()
		{
			base.Test_SetCountProperties();
		}
	}
}
