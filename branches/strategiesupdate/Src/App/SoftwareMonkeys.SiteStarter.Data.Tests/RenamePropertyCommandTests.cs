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
	public class RenamePropertyCommandTests : BaseDataTestFixture
	{
		public RenamePropertyCommandTests()
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
			
			RenamePropertyCommand command = new RenamePropertyCommand();
			command.TypeName = user.ShortTypeName;
			command.PropertyName = "FirstName";
			command.NewPropertyName = "Username";
			
			command.Execute(document);
			
			TestUser updatedUser = (TestUser)XmlUtilities.DeserializeFromDocument(document, typeof(TestUser));
			
			// This should have moved the "FirstName" property to "Username" and therefore the new "Username" property value
			// should be equal with the original "FirstName" property
			Assert.AreEqual(user.FirstName, updatedUser.Username, "The property wasn't renamed properly.");
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
		
			EntityReference reference = DataAccess.Data.Referencer.GetActiveReferences(user)[0];
			
			XmlDocument document = XmlUtilities.SerializeToDocument(reference);
			
			RenamePropertyCommand command = new RenamePropertyCommand();
			command.TypeName = user.ShortTypeName;
			command.PropertyName = "Roles";
			command.NewPropertyName = "GrantedRoles";
			
			command.Execute(document);
			
			EntityReference updatedReference = (EntityReference)XmlUtilities.DeserializeFromDocument(document, typeof(EntityReference));
			
			Assert.AreEqual("GrantedRoles", updatedReference.Property1Name, "The property name wasn't changed.");
		}
	}
}
