﻿using System;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data.Tests.Entities;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	/// <summary>
	/// Description of DataDeleterTests.
	/// </summary>
	[TestFixture]
	public class DataDeleterTests
	{
		public DataDeleterTests()
		{
		}
		
		[Test]
		public void Test_Delete_EntityAndReference()
		{
			TestUtilities.ClearTestEntities();
			
			TestUser user = new TestUser();
			Guid userID = user.ID = Guid.NewGuid();
			user.FirstName = "Test";
			user.LastName = "User";
			
			TestRole role = new TestRole();
			Guid roleID = role.ID = Guid.NewGuid();
			role.Name = "Test Role";
			
			user.Roles = new TestRole[] {role};
			
			DataAccess.Data.Saver.Save(user);
			
			EntityReferenceCollection references = DataAccess.Data.Referencer.GetReferences(typeof(TestUser).Name, typeof(TestRole).Name);
			
			Assert.AreEqual(1, references.Count, "Incorrect number of references found.");
			
			DataAccess.Data.Deleter.Delete(user);
			
			EntityReferenceCollection references2 = DataAccess.Data.Referencer.GetReferences(typeof(TestUser).Name, typeof(TestRole).Name);
			
			Assert.AreEqual(0, references2.Count, "Reference not deleted.");
			
			
			TestUtilities.ClearTestEntities();
		}
		
		
		[Test]
		public void Test_Delete_RemoveReferences()
		{
			
			using (LogGroup logGroup = AppLogger.StartGroup("Testing saving of an EntityIDReference.", NLog.LogLevel.Debug))
			{
				TestUtilities.ClearTestEntities();
				
				TestUser user = new TestUser();
				user.ID = Guid.NewGuid();
				user.FirstName = "Test";
				user.LastName = "User";
				
				TestRole role = new TestRole();
				role.ID = Guid.NewGuid();
				role.Name = "Test role";
				
				// This should remain commented out to check for exclusion
				user.Roles = new TestRole[]{role};
				
				DataAccess.Data.Saver.Save(user);
				DataAccess.Data.Saver.Save(role);
				
				DataAccess.Data.Deleter.Delete(role);
				
				TestUser user2 = DataAccess.Data.Reader.GetEntity<TestUser>("ID", user.ID);
				
				DataAccess.Data.Activator.Activate(user2, "Roles");
				
				Assert.IsNotNull(user2.Roles);
				
				if (user2.Roles != null)
					Assert.AreEqual(0, user2.Roles.Length, "Incorrect number of roles. The role should have been removed.");
				//Assert.AreEqual(newFirstName, user3.FirstName, "First name mismatch.");
				
				//IDataStore store = DataAccess.Data.Stores["Testing_Articles-Testing_Articles"];
				
				//Assert.IsNotNull(store, "The data store wasn't created/initialized.");
				
				TestUtilities.ClearTestEntities();
			}
		}
		
		
	}
}
