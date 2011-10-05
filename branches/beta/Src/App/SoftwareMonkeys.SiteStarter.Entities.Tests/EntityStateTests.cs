using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Entities.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class EntityStateTests : BaseEntityTestFixture
	{
		[Test]
		public void Test_GetType_FullName()
		{
			Type type = EntityState.GetType(typeof(TestRecord).FullName + ", " + typeof(TestRecord).Assembly.GetName().Name);
			
			Assert.IsNotNull(type, "Type is null");
			
			Assert.AreEqual(typeof(TestRecord).FullName, type.FullName, "Doesn't match expected.");
		}
		
		[Test]
		public void Test_GetType_ShortName()
		{
			Type type = EntityState.GetType(typeof(TestRecord).Name);
			
			Assert.IsNotNull(type, "Type is null");
			
			Assert.AreEqual(typeof(TestRecord).FullName, type.FullName, "Doesn't match expected.");
		}
		
		[Test]
		public void Test_GetInfo_FullName()
		{
			EntityInfo info = EntityState.GetInfo(typeof(TestRecord).FullName + ", " + typeof(TestRecord).Assembly.GetName().Name);
			
			Assert.IsNotNull(info, "info is null");
			
			Assert.AreEqual(typeof(TestRecord).FullName + ", " + typeof(TestRecord).Assembly.GetName().Name,
			                info.FullType, "Doesn't match expected.");
		}
		
		[Test]
		public void Test_GetInfo_ShortName()
		{
			EntityInfo info = EntityState.GetInfo(typeof(TestRecord).Name);
			
			Assert.IsNotNull(info, "info is null");
			
			Assert.AreEqual(typeof(TestRecord).FullName + ", " + typeof(TestRecord).Assembly.GetName().Name,
			                info.FullType, "Doesn't match expected.");
		}
	}
}
