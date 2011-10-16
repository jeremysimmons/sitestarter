using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Tests;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	[TestFixture]
	public class EntityFileNamerTests : BaseDataTestFixture
	{
		[Test]
		public void Test_CreateFilePath_Reference()
		{
			TestUser user = new TestUser();
			user.FirstName = "First";
			user.ID = Guid.NewGuid();
			
			TestRole role = new TestRole();
			role.ID = Guid.NewGuid();
			
			user.Roles = new TestRole[] {role};
			
			string basePath = TestUtilities.GetTestingPath(this) + Path.DirectorySeparatorChar + "Export";
			
			EntityIDReference reference = DataAccess.Data.Referencer.GetActiveReferences(user)[0];
			
			string path = new EntityFileNamer(reference, basePath).CreateFilePath();
			
			string expected = basePath + Path.DirectorySeparatorChar
				+ role.ShortTypeName + "-" + user.ShortTypeName + Path.DirectorySeparatorChar
				+ reference.ID.ToString() + ".xml";
			
			Assert.AreEqual(expected, path, "The path doesn't match what's expected.");
			
		}
		
		[Test]
		public void Test_CreateFilePath_Entity()
		{
			TestUser user = new TestUser();
			user.FirstName = "First";
			user.ID = Guid.NewGuid();
			
			string basePath = TestUtilities.GetTestingPath(this) + Path.DirectorySeparatorChar + "Export";
			
			string path = new EntityFileNamer(user, basePath).CreateFilePath();
			
			string expected = basePath + Path.DirectorySeparatorChar + user.GetType() + Path.DirectorySeparatorChar
				+ user.ID.ToString() + ".xml";
			
			Assert.AreEqual(expected, path, "The path doesn't match what's expected.");
			
		}
	}
}
