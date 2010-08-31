using System;
using NUnit.Framework;

namespace SoftwareMonkeys.SiteStarter.State.Tests
{
	[TestFixture]
	public class StateCollectionTests : BaseStateTestFixture
	{
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
		
		
		[Test]
		public void Test_GetCount()
		{
			
			string group = "TestGroup";
			
			StateCollection<string> collection = new StateCollection<string>(StateScope.Application, group);
			
			string zero = "Zero";
			string one = "One";
			string two = "Two";
			string three = "Three";
				
			string key0 = collection.GetStateKey(group, 0);
			
			StateAccess.State.SetApplication(key0, zero);
						
			Assert.AreEqual(1, collection.GetCount());
			
			
			string key1 = group + "_1";
			
			StateAccess.State.SetApplication(key1, one);
						
			Assert.AreEqual(2, collection.GetCount());
			
			
			
			string key2 = group + "_2";
			
			StateAccess.State.SetApplication(key2, two);
			
			string result2 = collection.GetStateValue(2);
			
			Assert.AreEqual(3, collection.GetCount());
			
			
			
			string key3 = group + "_3";
			
			StateAccess.State.SetApplication(key3, three);
			
			string result3 = collection.GetStateValue(3);
			
			
			
			Assert.AreEqual(4, collection.GetCount());
		}
		
		[Test]
		public void Test_this()
		{
			StateCollection<string> collection = new StateCollection<string>(StateScope.Application, "TestGroup");
			
			string zero = "Zero";
			string one = "One";
			string two = "Two";
			string three = "Three";
				
			collection[0] = zero;
			collection[1] = one;
			collection[2] = two;
			
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
