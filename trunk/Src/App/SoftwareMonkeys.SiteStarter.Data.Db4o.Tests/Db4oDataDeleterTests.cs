using System;
using SoftwareMonkeys.SiteStarter.Data.Tests;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Tests;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o.Tests
{
	[TestFixture]
	public class Db4oDataDeleterTests : DataDeleterTests
	{		
		public override void InitializeMockData()
		{
			MockDb4oDataProviderManager.Initialize();
		}
	}
}
