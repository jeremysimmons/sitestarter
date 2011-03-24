using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Tests;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Web.Controllers;

namespace SoftwareMonkeys.SiteStarter.Web.Tests.Controllers
{
	[TestFixture]
	public class ControllerLocatorTests : BaseWebTestFixture
	{
		[Test]
		public void Test_Locate_Delete_IEntity()
		{
			ControllerStateCollection strategies = new ControllerStateCollection();
			
			strategies.Add(new ControllerInfo(typeof(CreateController)));
			strategies.Add(new ControllerInfo(typeof(DeleteController)));
			
			ControllerLocator locator = new ControllerLocator(strategies);
			
			ControllerInfo controller = (ControllerInfo)locator.Locate("Delete", "IEntity");
			
			Assert.IsNotNull(controller, "No controller found.");
		}
		
		
		[Test]
		public void Test_Locate_Delete_User()
		{
			
			ControllerStateCollection strategies = new ControllerStateCollection();
			
			strategies.Add(new ControllerInfo(typeof(CreateController)));
			strategies.Add(new ControllerInfo(typeof(MockController)));
			
			ControllerLocator locator = new ControllerLocator(strategies);
			
			ControllerInfo controller = (ControllerInfo)locator.Locate("Mock", "TestUser");
			
			Assert.IsNotNull(controller, "No controller found.");
		}
		
		
		[Test]
		/// <summary>
		/// Tests the LocateFromInterfaces method where the matching interface is implemented by the provided type.
		/// </summary>
		public void Test_LocateFromInterfaces_Immediate()
		{
			Type type = typeof(BaseUniqueEntity);
			string action = "Delete";
			
			ControllerStateCollection strategies = new ControllerStateCollection();
			
			strategies.Add(new ControllerInfo(typeof(CreateController)));
			strategies.Add(new ControllerInfo(typeof(DeleteController)));
			
			ControllerLocator locator = new ControllerLocator(strategies);
			
			ControllerInfo info = locator.LocateFromInterfaces(action,type);
			
			Assert.IsNotNull(info, "No controller info found.");
		}
		
		[Test]
		/// <summary>
		/// Tests the LocateFromInterfaces method where the matching interface is implemented by a base type.
		/// </summary>
		public void Test_LocateFromInterfaces_Heirarchy()
		{
			Type type = typeof(User);
			string action = "Delete";
			
			ControllerStateCollection strategies = new ControllerStateCollection();
			
			strategies.Add(new ControllerInfo(typeof(CreateController)));
			strategies.Add(new ControllerInfo(typeof(DeleteController)));
			
			ControllerLocator locator = new ControllerLocator(strategies);
			
			ControllerInfo info = locator.LocateFromInterfaces(action, type);
			
			Assert.IsNotNull(info, "No controller info found.");
		}
	}
}
