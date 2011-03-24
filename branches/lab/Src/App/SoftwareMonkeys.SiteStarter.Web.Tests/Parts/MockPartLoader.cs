using System;
using SoftwareMonkeys.SiteStarter.Tests;
using System.Web.UI;

namespace SoftwareMonkeys.SiteStarter.Web.Tests.Parts
{
	/// <summary>
	/// 
	/// </summary>
	public class MockPartLoader : ControlLoader
	{
		private BaseTestFixture fixture;
		public BaseTestFixture Fixture
		{
			get { return fixture; }
			set { fixture = value; }
		}
		
		private string mockPartFilePath;
		/// <summary>
		/// The path of the mock projection.
		/// </summary>
		public string MockPartFilePath
		{
			get { return mockPartFilePath;  }
			set { mockPartFilePath = value; }
		}
		
		public override string ApplicationPath
		{
			get { return TestUtilities.GetTestApplicationPath(Fixture, "MockApplication"); }
			set { base.ApplicationPath = value; }
		}
		
		public MockPartLoader(BaseTestFixture fixture, string mockPartFilePath) : base(null)
		{
			MockPartFilePath = mockPartFilePath;
		}
		
		public override System.Web.UI.Control LoadControl(string virtualPath)
		{
			return new MockPart();
		}
	}
}
