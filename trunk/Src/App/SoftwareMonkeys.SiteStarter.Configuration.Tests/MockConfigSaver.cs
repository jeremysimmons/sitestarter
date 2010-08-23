using System;

namespace SoftwareMonkeys.SiteStarter.Configuration.Tests
{
	/// <summary>
	/// Provides a mock/test implementation for the config saver component.
	/// </summary>
	public class MockConfigSaver : ConfigSaver
	{
		/// <summary>
		/// Provides a blank implementation for use during testing.
		/// </summary>
		/// <param name="config"></param>
		public override void Save(IConfig config)
		{
			
		}
	}
}
