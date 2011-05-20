using System;
using NUnit.Framework;

namespace SoftwareMonkeys.SiteStarter.State.Tests
{
	[TestFixture]
	public class StateNameValueCollectionTests : BaseStateTestFixture
	{
		[Test]
		public void Test_GetEnumerator()
		{
			string group = "TestGroup";
			
			StateNameValueCollection<string> collection = new StateNameValueCollection<string>(StateScope.Application, group);
			
			string one = "One";
			string two = "Two";
			
			string[] values = new String[] {one, two};
				
			collection["One"] = one;
			collection["Two"] = two	;
			
			int i = 0;
			
			foreach (string value in collection)
			{
				Assert.IsNotNull(value, "Value is null at index position " + i + ".");
				
				Assert.AreEqual(values[i], value, "Only value doesn't match what is expected.");
				
				i++;
			}
		}
		
		[Test]
		public void Test_Remove_KeysAdjusted()
		{
			
			string group = "TestGroup";
			
			StateNameValueCollection<string> collection = new StateNameValueCollection<string>(StateScope.Application, group);
			
			Assert.AreEqual(0, collection.Count, "Not empty.");
			
			string one = "One";
			string keyOne = "KeyOne";
				
			collection[keyOne] = one;
			
			Assert.AreEqual(1, collection.Keys.Count, "Invalid number of keys found.");
			Assert.AreEqual(1, collection.Count, "Invalid number of items found.");
			
			collection.Remove(collection[keyOne]);
						
			Assert.IsNotNull(collection.Keys, "collection.Keys == null");
			
			// Ensure that the key was removed
			Assert.AreEqual(0, collection.Keys.Count, "Invalid number of keys found.");
			
		}
		
		[Test]
		public void Test_Keys()
		{
			string group = "TestGroup";
			
			StateNameValueCollection<string> collection = new StateNameValueCollection<string>(StateScope.Application, group);
			
			string one = "One";
			string keyOne = "KeyOne";
				
			collection[keyOne] = one;
			
			string fullKey = group + "_" + keyOne;
			
			string result = (string)StateAccess.State.GetApplication(fullKey);
			
			Assert.AreEqual(one, result, "No value attached to key.");
			
			Assert.IsNotNull(collection.Keys, "collection.Keys == null");
			
			Assert.AreEqual(1, collection.Keys.Count, "Invalid number of keys found.");
			
			Assert.AreEqual(keyOne, collection.Keys[0], "The keys don't match.");
		}
		
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
		public void Test_Count()
		{
			
			string group = "TestGroup";
			
			StateNameValueCollection<string> collection = new StateNameValueCollection<string>(StateScope.Application, group);
			
			string zero = "Zero";
			string one = "One";
			string two = "Two";
			string three = "Three";
			
			collection["KeyZero"] = zero;
			
			Assert.AreEqual(1, collection.Count, "Wrong count value.");
			
			collection["KeyOne"] = one;
			
			Assert.AreEqual(2, collection.Count, "Wrong count value.");
				
			/*string key0 = collection.GetStateKey(group, zero);
			
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
			
			
			Assert.AreEqual(4, collection.GetCount());*/
		}
	}
}
