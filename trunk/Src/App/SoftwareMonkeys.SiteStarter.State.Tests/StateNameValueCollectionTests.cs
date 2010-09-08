using System;
using NUnit.Framework;

namespace SoftwareMonkeys.SiteStarter.State.Tests
{
	[TestFixture]
	public class StateNameValueCollectionTests : BaseStateTestFixture
	{
		[Test]
		public void Test_this()
		{
			string input = "Hello world";
			
			StateNameValueCollection<string> collection = new StateNameValueCollection<string>(StateScope.Application, "TestGroup");
			
			
			collection["Test"] = input;
			
			
			string result = collection["Test"];
			
			Assert.AreEqual(input, result, "Value wasn't persisted correctly.");
			
		}
		
		[Test]
		public void Test_this_SeparatedByGroupAndScope()
		{
			string input = "Hello world";
			
			string input2 = "Hello world 2";
			
			StateNameValueCollection<string> collection = new StateNameValueCollection<string>(StateScope.Application, "TestGroup");
			
			
			collection["Test"] = input;
			
			StateNameValueCollection<string> collection2 = new StateNameValueCollection<string>(StateScope.Session, "TestGroup2");
			
			
			collection2["Test"] = input2;
			
			
			string result = collection["Test"];
			
			Assert.AreEqual(input, result, "Value wasn't persisted correctly.");
			
			string result2 = collection2["Test"];
			
			Assert.AreEqual(input2, result2, "Value wasn't persisted correctly.");
			
		}
		[Test]
		public void Test_GetCount()
		{
			
			string group = "TestGroup";
			
			StateNameValueCollection<string> collection = new StateNameValueCollection<string>(StateScope.Application, group);
			
			string zero = "Zero";
			string one = "One";
			string two = "Two";
			string three = "Three";
				
			string key0 = collection.GetStateKey(group, zero);
			
			StateAccess.State.SetApplication(key0, zero);
						
			Assert.AreEqual(1, collection.GetCount());
			
			
			string key1 = collection.GetStateKey(group, one);
			
			StateAccess.State.SetApplication(key1, one);
						
			Assert.AreEqual(2, collection.GetCount());
			
			
			
			string key2 = collection.GetStateKey(group, two);;
			
			StateAccess.State.SetApplication(key2, two);
						
			Assert.AreEqual(3, collection.GetCount());
			
			
			
			string key3 = collection.GetStateKey(group, three);
			
			StateAccess.State.SetApplication(key3, three);
			
			
			Assert.AreEqual(4, collection.GetCount());
		}
	}
}
