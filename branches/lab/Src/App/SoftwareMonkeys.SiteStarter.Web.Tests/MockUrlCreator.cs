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
			if (action == null || action == String.Empty)
				throw new ArgumentNullException("action");
			
			if (typeName == null || typeName == String.Empty)
				throw new ArgumentNullException("typeName");
			
			if (propertyName == null || propertyName == String.Empty)
				throw new ArgumentNullException("propertyName");
			
			if (dataKey == null || dataKey == String.Empty)
				throw new ArgumentNullException("dataKey");
			
			return "http://localhost/TestApplication/" + EntitiesUtilities.FormatUniqueKey(typeName)
				+ "/" + EntitiesUtilities.FormatUniqueKey(action)
				+ "/" + EntitiesUtilities.FormatUniqueKey(propertyName)
				+ "/" + EntitiesUtilities.FormatUniqueKey(dataKey);
		}
		
		public override string CreateUrl(string action, string typeName)
		{
			if (action == null || action == String.Empty)
				throw new ArgumentNullException("action");
			
			if (typeName == null || typeName == String.Empty)
				throw new ArgumentNullException("typeName");
			
			return "http://localhost/TestApplication/" + EntitiesUtilities.FormatUniqueKey(typeName)
				+ "/" + EntitiesUtilities.FormatUniqueKey(action);
		}
	}
}
