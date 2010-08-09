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
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Entities.Tests
{
	[TestFixture]
	public class EntityIDReferenceTests
	{
		/*public string ApplicationPath
		{
			// TODO: Path MUST NOT be hard coded
			//   get { return @"f:\SoftwareMonkeys\WorkHub\Application 2\Web\"; }
			//     get { return System.Configuration.ConfigurationSettings.AppSettings["ApplicationPath"]; }
			get { return SoftwareMonkeys.SiteStarter.Configuration.Config.Application.PhysicalPath; }
		}*/
		
		[Test]
		public void Test_Includes_Match()
		{
			Guid id1 = Guid.NewGuid();
			Guid id2 = Guid.NewGuid();
			
			string propertyName1 = "Property1";
			string propertyName2 = "Property2";
			
			string type1 = "Type1";
			string type2 = "Type2";
			
			EntityIDReference reference = new EntityIDReference();
			reference.Entity1ID = id1;
			reference.Entity2ID = id2;
			reference.Type1Name = type1;
			reference.Type2Name = type2;
			reference.Property1Name = propertyName1;
			reference.Property2Name = propertyName2;
			
			bool includes = reference.Includes(id1, propertyName1);
			
			Assert.IsTrue(includes, "Returned false when it should have returned true");
			
			includes = reference.Includes(id2, propertyName2);
			
			Assert.IsTrue(includes, "Returned false when it should have returned true");
		}
		
		[Test]
		public void Test_Includes_Mismatch()
		{
			Guid id1 = Guid.NewGuid();
			Guid id2 = Guid.NewGuid();
			
			string propertyName1 = "Property1";
			string propertyName2 = "Property2";
			
			string type1 = "Type1";
			string type2 = "Type2";
			
			EntityIDReference reference = new EntityIDReference();
			reference.Entity1ID = id1;
			reference.Entity2ID = id2;
			reference.Type1Name = type1;
			reference.Type2Name = type2;
			reference.Property1Name = propertyName1;
			reference.Property2Name = propertyName2;
			
			bool includes = reference.Includes(id1, propertyName2);
			
			Assert.IsFalse(includes, "Returned true when it should have returned false");
			
			includes = reference.Includes(id2, propertyName1);
			
			Assert.IsFalse(includes, "Returned true when it should have returned false");
			
		}
		
				
		[Test]
		public void Test_SwitchFor()
		{
			TestUser user = new TestUser();
			user.ID = Guid.NewGuid();
			
			TestRole role = new TestRole();
			role.ID = Guid.NewGuid();
						
			EntityIDReference reference = new EntityIDReference();
			reference.Entity1ID = user.ID;
			reference.Entity2ID = role.ID;
			reference.Type1Name = user.ShortTypeName;
			reference.Type2Name = role.ShortTypeName;
			reference.Property1Name = "Roles"; // ie. user.Roles
			reference.Property2Name = "Users"; // ie. role.Users
			
			EntityIDReference switchedReference = reference.SwitchFor(role);
						
			Assert.AreEqual(switchedReference.Entity1ID, role.ID, "The IDs of entity 1 weren't switched.");
			Assert.AreEqual(switchedReference.Entity2ID, user.ID, "The IDs of entity 2 weren't switched.");
			Assert.AreEqual(switchedReference.Type1Name, role.ShortTypeName, "The type names of entity 1 weren't switched.");
			Assert.AreEqual(switchedReference.Type2Name, user.ShortTypeName, "The type names of entity 2 weren't switched.");
			Assert.AreEqual(switchedReference.Property1Name,
			              "Users", // ie. role.Users
			              "The property names of entity 1 weren't switched.");
			Assert.AreEqual(switchedReference.Property2Name,
			              "Roles", // ie. user.Roles
			              "The property names of entity 2 weren't switched.");
			
			
		}
	}
}
