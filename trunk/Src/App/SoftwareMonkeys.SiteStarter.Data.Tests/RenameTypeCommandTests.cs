using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	/// <summary>
	/// Tests the RenamePropertyCommand.
	/// </summary>
	[TestFixture]
	public class RenameTypeCommandTests : BaseDataTestFixture
	{
		public RenameTypeCommandTests()
		{
		}
		
		[Test]
		public void Test_Execute_Entity()
		{
			TestUser user = new TestUser();
			user.ID = Guid.NewGuid();
			user.FirstName = "-- First Name -- ";
			user.LastName = "-- Last Name --";
			//user.Username = "Username";
		
			XmlDocument document = XmlUtilities.SerializeToDocument(user);
			
			RenameTypeCommand command = new RenameTypeCommand();
			command.TypeName = user.ShortTypeName;
			command.NewTypeName = "TestAccount";
			
			command.Execute(document);
			
			Assert.AreEqual("TestAccount", document.DocumentElement.Name, "The type name wasn't updated on the document element node.");
			
			TestAccount account = (TestAccount)XmlUtilities.DeserializeFromDocument(document, typeof(TestAccount));
			
			Assert.AreEqual(user.FirstName, account.FirstName, "The properties don't match.");
		}
		
		
		[Test]
		public void Test_Execute_Reference()
		{
			TestUser user = new TestUser();
			user.ID = Guid.NewGuid();
			user.FirstName = "-- First Name -- ";
			user.LastName = "-- Last Name --";
			//user.Username = "Username";
			
			TestRole role = new TestRole();
			role.ID = Guid.NewGuid();
			role.Name = "-- Test Role --";
			
			user.Roles = new TestRole[] {role};
			
			string newTypeName = "TestAccount";
		
			EntityReference reference = DataAccess.Data.Referencer.GetActiveReferences(user)[0];
			
			XmlDocument document = XmlUtilities.SerializeToDocument(reference);
			
			RenameTypeCommand command = new RenameTypeCommand();
			command.TypeName = user.ShortTypeName;
			command.NewTypeName = newTypeName;
			
			command.Execute(document);
			
			EntityReference updatedReference = (EntityReference)XmlUtilities.DeserializeFromDocument(document, typeof(EntityReference));
			
			Assert.AreEqual(newTypeName, updatedReference.Type1Name, "The type name wasn't changed.");
		}
	}
}
