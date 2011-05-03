﻿using System;
using SoftwareMonkeys.SiteStarter.Data.Tests;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o.Tests
{
	/// <summary>
	/// Description of Db4oRenamePropertyCommandTests.
	/// </summary>
	public class Db4oRenamePropertyCommandTests : RenamePropertyCommandTests
	{
		public override void InitializeMockData()
		{
			new MockDb4oDataProviderInitializer(this).Initialize();
		}
	}
}