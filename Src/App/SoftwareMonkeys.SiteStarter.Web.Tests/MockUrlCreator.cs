using System;
using SoftwareMonkeys.SiteStarter.Tests;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Web.Tests
{
	/// <summary>
	/// A mock URL creator for use during testing.
	/// </summary>
	public class MockUrlCreator : UrlCreator
	{
		private BaseTestFixture executingTestFixture;
		public BaseTestFixture ExecutingTestFixture
		{
			get { return executingTestFixture; }
			set { executingTestFixture = value; }
		}
		
		public MockUrlCreator(BaseTestFixture executingTestFixture) : base(false)
		{
			ExecutingTestFixture = executingTestFixture;
			Initialize();
		}
		
		public override void Initialize()
		{
			Initialize(
				TestUtilities.GetTestApplicationPath(ExecutingTestFixture, "TestApplication"),
				true,
				"http://localhost/TestApplication"
			);
		}
		
		public override string CreateUrl(string action, string typeName, string propertyName, string dataKey)
		{
			return "http://localhost/TestApplication/" + EntitiesUtilities.FormatUniqueKey(typeName)
				+ "/" + EntitiesUtilities.FormatUniqueKey(action)
				+ "/" + EntitiesUtilities.FormatUniqueKey(propertyName)
				+ "/" + EntitiesUtilities.FormatUniqueKey(dataKey);
		}
	}
}
