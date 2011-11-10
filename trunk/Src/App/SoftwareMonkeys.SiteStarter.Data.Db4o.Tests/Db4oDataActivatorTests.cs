using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Data.Tests;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class Db4oDataActivatorTests : DataActivatorTests
	{
		
		public override void InitializeMockData()
		{
			new MockDb4oDataProviderInitializer(this).Initialize();
		}
		
		[Test]
		public override void Test_Activate()
		{
			base.Test_Activate();
		}
	}
}
