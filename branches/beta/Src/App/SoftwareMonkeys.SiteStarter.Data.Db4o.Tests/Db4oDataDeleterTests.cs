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
			new MockDb4oDataProviderInitializer(this).Initialize();
		}
		
		[Test]
		public override void Test_Delete_RemoveReferences()
		{
			base.Test_Delete_RemoveReferences();
	}
		
		[Test]
		public override void Test_Delete_EntityAndReference_Async()
		{
			base.Test_Delete_EntityAndReference_Async();
}
	}
}
