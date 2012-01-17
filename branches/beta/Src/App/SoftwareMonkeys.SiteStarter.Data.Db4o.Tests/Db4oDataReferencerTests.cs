using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Data.Tests;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o.Tests
{
	[TestFixture]
	public class Db4oDataReferencerTests : DataReferencerTests
	{
		public override void InitializeMockData()
		{
			new MockDb4oDataProviderInitializer(this).Initialize();
		}
		
		[Test]
		public override void Test_SetCountProperty_TwoWayReference()
		{
			base.Test_SetCountProperty_TwoWayReference();
	}
		
		[Test]
		public override void Test_SetCountProperty_OneWayReference()
		{
			base.Test_SetCountProperty_OneWayReference();
}
		
		[Test]
		public override void Test_MatchReference_Exclusion()
		{
			base.Test_MatchReference_Exclusion();
		}
		
		[Test]
		public override void Test_GetReferences_InterfaceReferenceProperty()
		{
			base.Test_GetReferences_InterfaceReferenceProperty();
	}
		
		[Test]
		public override void Test_GetActiveReferences_ReferenceWithInterfaceType()
		{
			base.Test_GetActiveReferences_ReferenceWithInterfaceType();
		}
		
		[Test]
		public override void Test_MatchReference()
		{
			base.Test_MatchReference();
		}
	}
}
