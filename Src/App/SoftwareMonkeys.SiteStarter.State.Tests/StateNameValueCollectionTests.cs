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
	}
}
