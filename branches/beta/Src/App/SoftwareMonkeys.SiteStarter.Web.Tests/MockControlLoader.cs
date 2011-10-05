using System;
using SoftwareMonkeys.SiteStarter.Tests;
using System.Web.UI;

namespace SoftwareMonkeys.SiteStarter.Web.Tests
{
	/// <summary>
	/// 
	/// </summary>
	public class MockControlLoader : ControlLoader
	{
		private BaseTestFixture fixture;
		public BaseTestFixture Fixture
		{
			get { return fixture; }
			set { fixture = value; }
		}
		
		private string mockControlFilePath;
		/// <summary>
		/// The path of the mock control.
		/// </summary>
		public string MockControlFilePath
		{
			get { return mockControlFilePath;  }
			set { mockControlFilePath = value; }
		}
		
		public override string ApplicationPath
		{
			get { return TestUtilities.GetTestApplicationPath(Fixture, "MockApplication"); }
			set { base.ApplicationPath = value; }
		}
		
		public MockControlLoader(BaseTestFixture fixture) : base(null)
		{
			Fixture = fixture;
		}
		
		public MockControlLoader(BaseTestFixture fixture, string mockControlFilePath) : base(null)
		{
			MockControlFilePath = mockControlFilePath;
			Fixture = fixture;
		}
		
		public override System.Web.UI.Control LoadControl(string virtualPath)
		{
			return new MockUserControl();
		}
	}
}
