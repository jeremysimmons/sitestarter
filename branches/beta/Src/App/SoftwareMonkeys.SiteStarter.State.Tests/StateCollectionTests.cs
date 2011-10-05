using System;
using NUnit.Framework;

namespace SoftwareMonkeys.SiteStarter.State.Tests
{
	[TestFixture]
	public class StateCollectionTests : BaseStateTestFixture
	{
		
		[Test]
		public void Test_Current()
		{
			string group = "TestGroup";
			
			StateCollection<string> collection = StateCollection<string>.Current(StateScope.Application, group);
			
			string one = "One";
			string two = "Two";
			
			collection.Add(one);
			collection.Add(two);
			
			
			
			StateCollection<string> foundCollection = StateCollection<string>.Current(StateScope.Application, group);
			
			Assert.AreEqual(collection.Count, foundCollection.Count);
			
		}
		
		[Test]
		public void Test_Remove()
		{
			string group = "TestGroup";
			
			StateCollection<string> collection = StateCollection<string>.Current(StateScope.Application, group);
			
			string one = "One";
			string two = "Two";
			
			collection.Add(one);
			collection.Add(two);
			
			
			
			StateCollection<string> foundCollection = StateCollection<string>.Current(StateScope.Application, group);
			
			foundCollection.Remove(one);
			
			StateCollection<string> foundCollection2 = StateCollection<string>.Current(StateScope.Application, group);
			
			Assert.AreEqual(1, foundCollection.Count);
			
		}
		
		[Test]
		public void Test_Remove_Empty()
		{
			string group = "TestGroup";
			
			StateCollection<string> collection = StateCollection<string>.Current(StateScope.Application, group);
			
			collection.Remove("Test");
			
		}
		/*
		[Test]
		public void Test_ResetKeys()
		{
			string group = "TestGroup";
			
			StateCollection<string> collection = new StateCollection<string>(StateScope.Application, group);
			
			string one = "One";
			string two = "Two";
				
			collection[0] = one;
			collection[1] = two;
			
			collection.Keys = new String[] {group + "_0", group + "_2"};
						
			Assert.IsNotNull(collection.Keys, "collection.Keys == null");
			
			Assert.AreEqual(2, collection.Keys.Length, "Invalid number of keys found.");
			
			for (int i = 0; i < collection.Keys.Length; i ++)
			{
				Assert.AreEqual(group + "_" + i, collection.Keys[i], "Reset didn't work properly.");
				
				i++;
			}
			
		}
		
		[Test]
		public void Test_Keys_Remove_ResetKeys()
		{
			string group = "TestGroup";
			
			StateCollection<string> collection = new StateCollection<string>(StateScope.Application, group);
			
			string one = "One";
			string two = "Two";
				
			collection[0] = one;
			collection[1] = two;
			
			Assert.IsNotNull(collection.Keys, "collection.Keys == null");
			
			Assert.AreEqual(2, collection.Keys.Length, "Invalid number of keys found.");
			
			string key = group + "_0";
			
			
			string result = (string)StateAccess.State.GetApplication(key);
			
			Assert.AreEqual(one, result);
			
			collection.RemoveAt(0);
						
			Assert.IsNotNull(collection.Keys, "collection.Keys == null");
			
			Assert.AreEqual(1, collection.Keys.Length, "Invalid number of keys found.");
			
		}*/
		
		[Test]
		public void Test_GetEnumerator()
		{
			string group = "TestGroup";
			
			StateCollection<string> collection = new StateCollection<string>(StateScope.Application, group);
			
			string one = "One";
			string two = "Two";
			
			string[] values = new String[] {one, two};
				
			collection.Add(one);
			collection.Add(two);
			
			
			int i = 0;
			
			foreach (string value in collection)
			{
				Assert.IsNotNull(value, "Value is null at index position " + i + ".");
				
				Assert.AreEqual(values[i], value, "Only value doesn't match what is expected.");
				
				i++;
			}
		}
		/*
		[Test]
		public void Test_Keys()
		{
			string group = "TestGroup";
			
			StateCollection<string> collection = new StateCollection<string>(StateScope.Application, group);
			
			string one = "One";
			string two = "Two";
				
			collection.SetStateValue(0, one);
			
			string key = group + "_0";
			
			string result = (string)StateAccess.State.GetApplication(key);
			
			Assert.AreEqual(result, one);
			
			string expectedKey = "TestGroup_0";
			
			Assert.IsNotNull(collection.Keys, "collection.Keys == null");
			
			Assert.AreEqual(1, collection.Keys.Length, "Invalid number of keys found.");
			
			Assert.AreEqual(expectedKey, collection.Keys[0], "The keys don't match.");
		}
		
		[Test]
		public void Test_Keys_Remove()
		{
			string group = "TestGroup";
			
			StateCollection<string> collection = new StateCollection<string>(StateScope.Application, group);
			
			string one = "One";
			string two = "Two";
				
			collection.SetStateValue(0, one);
			
			string key = group + "_0";
			
			
			string result = (string)StateAccess.State.GetApplication(key);
			
			Assert.AreEqual(one, result);
			
			collection[0] = null;
						
			Assert.IsNotNull(collection.Keys, "collection.Keys == null");
			
			Assert.AreEqual(0, collection.Keys.Length, "Invalid number of keys found.");
			
		}
		
		[Test]
		public void Test_SetStateValue()
		{
			string group = "TestGroup";
			
			StateCollection<string> collection = new StateCollection<string>(StateScope.Application, group);
			
			string one = "One";
			string two = "Two";
				
			collection.SetStateValue(0, one);
			
			string key = group + "_0";
			
			string result = (string)StateAccess.State.GetApplication(key);
			
			Assert.AreEqual(result, one);
		}
		
		[Test]
		public void Test_GetStateValue()
		{
			string group = "TestGroup";
			
			StateCollection<string> collection = new StateCollection<string>(StateScope.Application, group);
			
			string zero = "Zero";
			string one = "One";
			string two = "Two";
			string three = "Three";
				
			
			string key0 = collection.GetStateKey(group, 0);
			
			StateAccess.State.SetApplication(key0, zero);
			
			string result0 = collection.GetStateValue(0);
			
			
			string key1 = group + "_1";
			
			StateAccess.State.SetApplication(key1, one);
			
			string result1 = collection.GetStateValue(1);
			
			string key2 = group + "_2";
			
			StateAccess.State.SetApplication(key2, two);
			
			string result2 = collection.GetStateValue(2);
			
			string key3 = group + "_3";
			
			StateAccess.State.SetApplication(key3, three);
			
			string result3 = collection.GetStateValue(3);
			
			
			Assert.AreEqual(result0, zero);
			
			Assert.AreEqual(result1, one);
			
			Assert.AreEqual(result2, two);
		}
		
		*/
		[Test]
		public void Test_Count()
		{
			
			string group = "TestGroup";
			
			StateCollection<string> collection = new StateCollection<string>(StateScope.Application, group);
			
			string zero = "Zero";
			string one = "One";
			
			
			collection.Add(zero);
				
			string key = collection.GetStateKey();
			
			StateAccess.State.SetApplication(key, collection);
			
			StateCollection<string> foundCollection0 = (StateCollection<string>)StateAccess.State.GetApplication(key);
						
			Assert.AreEqual(1, foundCollection0.Count);
			
			
			
			collection.Add(one);
			
			
			StateAccess.State.SetApplication(key, collection);
						
			StateCollection<string> foundCollection2 = (StateCollection<string>)StateAccess.State.GetApplication(key);
						
			Assert.AreEqual(2, foundCollection2.Count);
			
			
			
			
			
			
			Assert.AreEqual(2, collection.Count);
		}
		
		[Test]
		public void Test_this()
		{
			StateCollection<string> collection = new StateCollection<string>(StateScope.Application, "TestGroup");
			
			string zero = "Zero";
			string one = "One";
			string two = "Two";
				
			collection.Add(zero);
			collection.Add(one);
			collection.Add(two);
			
			Assert.AreEqual(3, collection.Count);
			
			Assert.AreEqual(zero, collection[0]);
			Assert.AreEqual(one, collection[1]);
			Assert.AreEqual(two, collection[2]);
		}
		
		
		[Test]
		public void Test_Add()
		{
			StateCollection<string> collection = new StateCollection<string>(StateScope.Application, "TestGroup");
			
			string zero = "Zero";
			string one = "One";
			string two = "Two";
			string three = "Three";
				
			collection.Add(zero);
			collection.Add(one);
			collection.Add(two);
			collection.Add(three);
			
			Assert.AreEqual(4, collection.Count);
			
			Assert.AreEqual(zero, collection[0]);
			Assert.AreEqual(one, collection[1]);
			Assert.AreEqual(two, collection[2]);
			Assert.AreEqual(three, collection[3]);
		}
		
		[Test]
		public void Test_Add_Separate()
		{			
			StateCollection<string> collection = new StateCollection<string>(StateScope.Application, "TestGroup1");
			
			string zero = "Zero";
			string one = "One";
			string two = "Two";
			string three = "Three";
				
			collection.Add(zero);
			collection.Add(one);
			collection.Add(two);
			collection.Add(three);
			
			StateCollection<string> collection2 = new StateCollection<string>(StateScope.Application, "TestGroup2");
						
			collection2.Add(zero + "2");
			collection2.Add(one + "2");
			collection2.Add(two + "2");
			collection2.Add(three + "2");
			
			
			
			Assert.AreEqual(4, collection.Count);
			
			Assert.AreEqual(zero, collection[0]);
			Assert.AreEqual(one, collection[1]);
			Assert.AreEqual(two, collection[2]);
			Assert.AreEqual(three, collection[3]);
			
			Assert.AreEqual(4, collection2.Count);
			
			Assert.AreEqual(zero + "2", collection2[0]);
			Assert.AreEqual(one + "2", collection2[1]);
			Assert.AreEqual(two + "2", collection2[2]);
			Assert.AreEqual(three + "2", collection2[3]);
		}
	}
}
