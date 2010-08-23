using System;
using SoftwareMonkeys.SiteStarter.Data.Tests;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o.Tests
{
	public class Db4oBatchTests : BatchTests
	{
		public override void InitializeMockData()
		{
			MockDb4oDataProviderManager.Initialize();
		}
	}
}
