using System;
using SoftwareMonkeys.SiteStarter.Data.Db4o;
using SoftwareMonkeys.SiteStarter.Data.Tests;
using NUnit.Framework;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o.Tests
{
	[TestFixture]
	/// <summary>
	/// Initializes a mock db4o provider so that the tests of the base test fixture can be tested against the db4o data access adapters.
	/// </summary>
	public class Db4oDataProviderTests : DataProviderTests
	{
		public override void InitializeMockData()
		{
			MockDb4oDataProviderManager.Initialize();
		}
	}
}
