using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Xml;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Entities.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Entities.Tests
{
	/// <summary>
	/// Description of EntityCollectionTests.
	/// </summary>
	[TestFixture]
	public class EntityCollectionTests
	{
		[Test]
		public void Test_Static_Add()
		{
			TestUser user = new TestUser();
			Guid userID = user.ID = Guid.NewGuid();
			user.FirstName = "Test";
			user.LastName = "User";
			
			TestRole role = new TestRole();
			Guid roleID = role.ID = Guid.NewGuid();
			role.Name = "Test Role";
			
			
			user.Roles = Collection<TestRole>.Add(user.Roles, role);
			
			Assert.AreEqual(1, user.Roles.Length, "The role doesn't appear to have been added properly.");
		}
		
		[Test]
		public void Test_Static_RemoveAt()
		{
			TestUser user = new TestUser();
			Guid userID = user.ID = Guid.NewGuid();
			user.FirstName = "Test";
			user.LastName = "User";
			
			TestRole role = new TestRole();
			Guid roleID = role.ID = Guid.NewGuid();
			role.Name = "Test Role";
			
			user.Roles = new TestRole[] {role};
			
			user.Roles = Collection<TestRole>.RemoveAt(user.Roles, 0);
			
			Assert.AreEqual(0, user.Roles.Length, "The role doesn't appear to have been removed properly.");
		}
		
		[Test]
		public void Test_Static_GetByID_Found()
		{
			TestUser user = new TestUser();
			Guid userID = user.ID = Guid.NewGuid();
			user.FirstName = "Test";
			user.LastName = "User";
			
			TestRole role = new TestRole();
			Guid roleID = role.ID = Guid.NewGuid();
			role.Name = "Test Role";
			
			user.Roles = new TestRole[] {role};
			
			TestRole role2 = Collection<TestRole>.GetByID(user.Roles, role.ID);
			
			Assert.IsNotNull(role2, "The role wasn't retrieved properly.");
		}
		
		[Test]
		public void Test_Static_GetByID_Missing()
		{
			TestUser user = new TestUser();
			Guid userID = user.ID = Guid.NewGuid();
			user.FirstName = "Test";
			user.LastName = "User";
			
			TestRole role = new TestRole();
			Guid roleID = role.ID = Guid.NewGuid();
			role.Name = "Test Role";
			
			user.Roles = new TestRole[] {role};
			
			TestRole role2 = Collection<TestRole>.GetByID(user.Roles, Guid.NewGuid());
			
			Assert.IsNull(role2, "A role was retrieved when it shouldn't have been.");
		}
	}
}
