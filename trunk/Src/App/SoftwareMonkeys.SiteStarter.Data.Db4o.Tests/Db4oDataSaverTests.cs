using System;
using SoftwareMonkeys.SiteStarter.Data.Tests;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o.Tests
{
	public class Db4oDataSaverTests : DataSaverTests
	{
		public override void InitializeMockData()
		{
			MockDb4oDataProviderManager.Initialize();
		}
	}
}
