using System;
using NUnit.Framework;

namespace SoftwareMonkeys.SiteStarter.State.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class StateStackTests : BaseStateTestFixture
	{
		[Test]
		public void Test_Count()
		{
						
			string group = "TestGroup";
			
			StateStack<string> collection = new StateStack<string>(StateScope.Application, group);
			
			string zero = "Zero";
			string one = "One";
			
			
			collection.Push(zero);
			
			
			Assert.AreEqual(1, collection.Count);
			
			string key = collection.GetStateKey();
			
			//StateAccess.State.SetApplication(key, collection);
			
			StateCollection<string> foundCollection0 = (StateCollection<string>)StateAccess.State.GetApplication(key);
						
			Assert.AreEqual(1, foundCollection0.Count);
			
			
			
			collection.Push(one);
			
			Assert.AreEqual(2, collection.Count);
			
			//StateAccess.State.SetApplication(key, collection);
						
			StateCollection<string> foundCollection2 = (StateCollection<string>)StateAccess.State.GetApplication(key);
						
			Assert.AreEqual(2, foundCollection2.Count);
			
			
			collection.Pop();
			
			
			
			Assert.AreEqual(1, collection.Count);
			
			//StateAccess.State.SetApplication(key, collection);
						
			StateCollection<string> foundCollection3 = (StateCollection<string>)StateAccess.State.GetApplication(key);
						
			Assert.AreEqual(1, foundCollection3.Count);
			
		
		}
		
		[Test]
		public void Test_Pop_Empty()
		{
			StateStack<string> stack = new StateStack<string>(StateScope.Operation, "Test");
			
			stack.Pop();
		}
	}
}
