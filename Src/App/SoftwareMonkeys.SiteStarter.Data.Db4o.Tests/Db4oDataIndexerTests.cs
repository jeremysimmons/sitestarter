using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Data.Tests;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class Db4oDataIndexerTests : DataIndexerTests
	{
		
		public override void InitializeMockData()
		{
			new MockDb4oDataProviderInitializer(this).Initialize();
		}
		
		[Test]
		public override void Test_GetEntitiesPageMatchReference()
		{
			base.Test_GetEntitiesPageMatchReference();
		}
		
		[Test]
		public override void Test_GetEntitiesByType()
		{
			base.Test_GetEntitiesByType();
		}
		
		[Test]
		public override void Test_GetPageOfEntitiesWithReference_EmptyReferencedEntityID_Found()
		{
			base.Test_GetPageOfEntitiesWithReference_EmptyReferencedEntityID_Found();
		}
		
		[Test]
		public override void Test_GetPageOfEntitiesWithReference_EmptyReferencedEntityID_NotFound()
		{
			base.Test_GetPageOfEntitiesWithReference_EmptyReferencedEntityID_NotFound();
		}
	}
}
