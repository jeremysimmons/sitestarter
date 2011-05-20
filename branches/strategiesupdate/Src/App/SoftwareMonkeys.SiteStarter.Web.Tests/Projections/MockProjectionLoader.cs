using System;
using SoftwareMonkeys.SiteStarter.Tests;
using System.Web.UI;

namespace SoftwareMonkeys.SiteStarter.Web.Tests.Projections
{
	/// <summary>
	/// 
	/// </summary>
	public class MockProjectionLoader : ControlLoader
	{
		private BaseTestFixture fixture;
		public BaseTestFixture Fixture
		{
			get { return fixture; }
			set { fixture = value; }
		}
		
		private string mockProjectionFilePath;
		/// <summary>
		/// The path of the mock projection.
		/// </summary>
		public string MockProjectionFilePath
		{
			get { return mockProjectionFilePath;  }
			set { mockProjectionFilePath = value; }
		}
		
		public override string ApplicationPath
		{
			get { return TestUtilities.GetTestApplicationPath(Fixture, "MockApplication"); }
			set { base.ApplicationPath = value; }
		}
		
		public MockProjectionLoader(BaseTestFixture fixture, string mockProjectionFilePath) : base(null)
		{
			MockProjectionFilePath = mockProjectionFilePath;
		}
		
		public override System.Web.UI.Control LoadControl(string virtualPath)
		{
			return new MockProjection();
		}
	}
}
