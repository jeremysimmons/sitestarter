using System;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Diagnostics.Tests
{
	/// <summary>
	/// Description of MockLogSupervisor.
	/// </summary>
	public class MockLogSupervisor : LogSupervisor
	{
		static private LogSupervisor testSupervisor;
		static public LogSupervisor TestSupervisor
		{
			get {
				if (testSupervisor == null)
				{
					testSupervisor = new MockLogSupervisor();
					testSupervisor.SettingsManager = MockLogSettingsManager.TestManager;
				}
				return testSupervisor; }
			set { testSupervisor = value; }
		}
		
		public MockLogSupervisor()
		{
			SettingsManager = MockLogSettingsManager.TestManager;
		}
	}
}
