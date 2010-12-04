using System;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	[TestFixture]
	public class DataReaderTests : BaseDataTestFixture
	{
		
		
		[Test]
		public void Test_GetEntity_ByTypeAndPropertyValue()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing a simple query with the PropertyFilter.", NLog.LogLevel.Debug))
			{
				TestUser user = new TestUser();
				Guid userID = user.ID = Guid.NewGuid();
				user.FirstName = "Test";
				user.LastName = "User";
				
				TestRole role = new TestRole();
				Guid roleID = role.ID = Guid.NewGuid();
				role.Name = "Test Role";
				
				
				user.Roles = Collection<TestRole>.Add(user.Roles, role);
				
				DataAccess.Data.Saver.Save(user);
				
				DataAccess.Data.Saver.Save(role);
				
				user = null;
				role = null;
				
				
				TestRole role2 = (TestRole)DataAccess.Data.Reader.GetEntity(typeof(TestRole), "ID", roleID);
				
				
				
				Assert.IsNotNull(role2, "role2 == null");
				
				if (role2 != null)
				{
					
					Assert.AreEqual(roleID, role2.ID, "ID of referenced user after activating doesn't match the original.");
				}
			}
		}
		
		
		[Test]
		public void Test_GetEntity_Generic_ByPropertyValue()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing a simple query with the PropertyFilter.", NLog.LogLevel.Debug))
			{
				TestUser user = new TestUser();
				Guid userID = user.ID = Guid.NewGuid();
				user.FirstName = "Test";
				user.LastName = "User";
				
				//TestRole role = new TestRole();
				//Guid roleID = role.ID = Guid.NewGuid();
				//role.Name = "Test Role";
				
				
				//user.Roles.Add(role);
				
				DataAccess.Data.Saver.Save(user);
				
				//DataAccess.Data.Saver.Save(role);
				
				user = null;
				//role = null;
				
				
				TestUser user2 = DataAccess.Data.Reader.GetEntity<TestUser>("ID", userID);
				
				
				Assert.IsNotNull(user2, "The user object is null.");
				
			}
		}
		
		
		[Test]
		public void Test_GetEntity_Bound()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing a simple query with the PropertyFilter.", NLog.LogLevel.Debug))
			{
				TestUser user = new TestUser();
				Guid userID = user.ID = Guid.NewGuid();
				user.FirstName = "Test";
				user.LastName = "User";
				
				
				DataAccess.Data.Saver.Save(user);
				
				
				IEntity user2 = DataAccess.Data.Reader.GetEntity(user);
				
				
				Assert.IsNotNull(user2, "The user object is null.");
				
			}
		}
		
	}
}
