using System;
using SoftwareMonkeys.SiteStarter.Data.Tests;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o.Tests
{
	/// <summary>
	/// Description of Db4oReferenceFilterTests.
	/// </summary>
	public class Db4oReferenceFilterTests : ReferenceFilterTests
	{
		public override void InitializeMockData()
		{
			MockDb4oDataProviderManager.Initialize();
		}
	}
}
