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
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Web.WebControls;
using SoftwareMonkeys.SiteStarter.Configuration;
using System.Reflection;
using System.Web;
using SoftwareMonkeys.SiteStarter.Tests;

namespace SoftwareMonkeys.SiteStarter.Web.Tests
{
	[TestFixture]
	public class RoleProviderTests : BaseWebTestFixture
	{
		public string ApplicationPath
		{
			get { return TestUtilities.GetTestingPath(this); }
		}
		
		public RoleProviderTests()
		{
			//Config.Initialize(ApplicationPath, "");
		}

        [Test]
        public void Test_IsUserInRole_True()
        {
        	User.RegisterType();
        	UserRole.RegisterType();
        	
        	User user = new User();
        	user.ID = Guid.NewGuid();
        	user.FirstName = "Test";
        	user.LastName = "User";
        	user.Email = "test@softwaremonkeys.net";
        	user.Username = "testuser";
        	user.Password = Crypter.EncryptPassword("pass");
        	
        	UserRole role = new UserRole();
        	role.ID = Guid.NewGuid();
        	role.Name = "TestRole";
        	
        	user.Roles = new UserRole[] {role};
        	
        	UserRoleFactory.Current.Save(role);
        	UserFactory.Current.Save(user);
        	
        	User foundUser = UserFactory.Current.GetUser(user.ID);
        	
        	UserFactory.Current.Activate(foundUser);
        	
        	Assert.IsNotNull(foundUser.Roles, "No roles found for the loaded user.");
        	
        	Assert.AreEqual(1, foundUser.Roles.Length, "Roles not found for user.");
        	
        	Assert.AreEqual(role.Name, user.Roles[0].Name, "Invalid name on the role.");
        	Assert.AreEqual(role.ID.ToString(), user.Roles[0].ID.ToString(), "Invalid ID on the role.");
        	
        	UserFactory.Current.Delete(UserFactory.Current.GetUserByUsername(user.Username));
        	UserRoleFactory.Current.Delete(UserRoleFactory.Current.GetUserRoleByName(role.Name));
        }
        
        [Test]
        public void Test_IsUserInRole_False()
        {
        	User.RegisterType();
        	UserRole.RegisterType();
        	
        	User user = new User();
        	user.ID = Guid.NewGuid();
        	user.FirstName = "Test";
        	user.LastName = "User";
        	user.Email = "test@softwaremonkeys.net";
        	user.Username = "testuser";
        	user.Password = Crypter.EncryptPassword("pass");
        	
        	UserRole role = new UserRole();
        	role.ID = Guid.NewGuid();
        	role.Name = "TestRole";
        	
        	// This test should fail to find a role because the SHOULD BE NO REFERENCE
        	// Must remain commented out
        	//user.Roles = new UserRole[] {role};
        	
        	UserRoleFactory.Current.Save(role);
        	UserFactory.Current.Save(user);
        	
        	User foundUser = UserFactory.Current.GetUser(user.ID);
        	
        	UserFactory.Current.Activate(foundUser);
        	
        	Assert.IsTrue(foundUser.Roles == null
        	              || foundUser.Roles.Length == 0,
        	              "Role(s) were found when there shouldn't be any.");
        	
        	UserFactory.Current.Delete(UserFactory.Current.GetUserByUsername(user.Username));
        	UserRoleFactory.Current.Delete(UserRoleFactory.Current.GetUserRoleByName(role.Name));
        }
	}
}
