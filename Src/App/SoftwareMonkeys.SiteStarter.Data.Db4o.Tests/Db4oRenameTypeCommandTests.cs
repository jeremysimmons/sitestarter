using System;
using SoftwareMonkeys.SiteStarter.Data.Tests;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o.Tests
{
	/// <summary>
	/// Description of Db4oRenameTypeCommandTests.
	/// </summary>
	public class Db4oRenameTypeCommandTests : RenameTypeCommandTests
	{
		
		public override void InitializeMockData()
		{
			MockDb4oDataProviderManager.Initialize();
		}
	}
}
