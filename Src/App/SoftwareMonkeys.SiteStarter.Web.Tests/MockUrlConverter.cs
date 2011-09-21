using System;

namespace SoftwareMonkeys.SiteStarter.Web.Tests
{
	/// <summary>
	/// 
	/// </summary>
	public class MockUrlConverter : UrlConverter
	{
		public MockUrlConverter()
		{
			Protocol = "http";
			Host = "localhost";
		}
	}
}
