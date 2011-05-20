using SoftwareMonkeys.SiteStarter.Data.Tests;
using System;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o.Tests
{
	/// <summary>
	/// 
	/// </summary>
	public class Db4oPropertyFilterTests : PropertyFilterTests
	{
		public Db4oPropertyFilterTests()
		{
		}
		
		public override void InitializeMockData()
		{
			new MockDb4oDataProviderInitializer(this).Initialize();
		}
	}
}
