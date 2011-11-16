using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// Tests for the ReactionLocator.
	/// </summary>
	[TestFixture]
	public class ReactionLocatorTests : BaseBusinessTestFixture
	{
		[Test]
		public void Test_Locate()
		{
			string action = "Save";
			string typeName = "TestArticle";
			
			ReactionLocator locator = new ReactionLocator(ReactionState.Reactions);
			
			ReactionInfoCollection reactions = locator.Locate(action, typeName);
			
			foreach (ReactionInfo reaction in reactions)
			{
				Type expectedType = EntityState.GetType(typeName);
				Type actualType = EntityState.GetType(reaction.TypeName);
				
				bool doMatch = expectedType.Equals(actualType)
					|| expectedType.IsAssignableFrom(actualType)
					|| actualType.IsAssignableFrom(expectedType);
				
				Assert.IsTrue(doMatch, "The type '" + reaction.TypeName + "' on '" + reaction.GetType().FullName + "' reaction does not match expected type '" + typeName + "'.");
			}
			
			Assert.AreEqual(2, reactions.Count, "Invalid number of reactions found.");
		}
		
		[Test]
		public void Test_LocateFromHeirarchy()
		{
			string action = "Save";
			string typeName = "MockDerivedEntity";
			
			ReactionLocator locator = new ReactionLocator(ReactionState.Reactions);
			
			ReactionInfo[] reactions = locator.LocateFromHeirarchy(action, EntityState.GetType(typeName));
			
			foreach (ReactionInfo reaction in reactions)
			{
				Type expectedType = EntityState.GetType(typeName);
				Type actualType = EntityState.GetType(reaction.TypeName);
				
				bool doMatch = expectedType.Equals(actualType)
					|| expectedType.IsAssignableFrom(actualType)
					|| actualType.IsAssignableFrom(expectedType);
				
				Assert.IsTrue(doMatch, "The type '" + reaction.TypeName + "' on '" + reaction.GetType().FullName + "' reaction does not match expected type '" + typeName + "'.");
			}
			
			Assert.AreEqual(2, reactions.Length, "Invalid number of reactions found.");
		}
		
		[Test]
		public void Test_LocateFromBaseTypes()
		{
			string action = "Save";
			string typeName = "MockDerivedEntity";
			
			ReactionLocator locator = new ReactionLocator(ReactionState.Reactions);
			
			ReactionInfo[] reactions = locator.LocateFromBaseTypes(action, EntityState.GetType(typeName));
			
			foreach (ReactionInfo reaction in reactions)
			{
				Type expectedType = EntityState.GetType(typeName);
				Type actualType = EntityState.GetType(reaction.TypeName);
				
				bool doMatch = expectedType.Equals(actualType)
					|| expectedType.IsAssignableFrom(actualType)
					|| actualType.IsAssignableFrom(expectedType);
				
				Assert.IsTrue(doMatch, "The type '" + reaction.TypeName + "' on '" + reaction.GetType().FullName + "' reaction does not match expected type '" + typeName + "'.");
			}
			
			Assert.AreEqual(1, reactions.Length, "Invalid number of reactions found.");
		}
		
		[Test]
		public void Test_LocateFromInterfaces()
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Testing the LocateFromInterfaces function."))
			{
				string action = "Save";
				string typeName = "MockEntity";
				
				ReactionLocator locator = new ReactionLocator(ReactionState.Reactions);
				
				ReactionInfo[] reactions = locator.LocateFromInterfaces(action, EntityState.GetType(typeName));
				
				foreach (ReactionInfo reaction in reactions)
				{
					Type expectedType = EntityState.GetType(typeName);
					Type actualType = EntityState.GetType(reaction.TypeName);
					
					bool doMatch = expectedType.Equals(actualType)
						|| expectedType.IsAssignableFrom(actualType)
						|| actualType.IsAssignableFrom(expectedType);
					
					Assert.IsTrue(doMatch, "The type '" + reaction.TypeName + "' on '" + reaction.GetType().FullName + "' reaction does not match expected type '" + typeName + "'.");
				}
				
				Assert.AreEqual(1, reactions.Length, "Invalid number of reactions found.");
			}
		}
	}
}
