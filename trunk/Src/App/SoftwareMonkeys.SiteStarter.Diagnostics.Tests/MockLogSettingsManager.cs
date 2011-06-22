using System;

namespace SoftwareMonkeys.SiteStarter.Diagnostics.Tests
{
	/// <summary>
	/// 
	/// </summary>
	public class MockLogSettingsManager : LogSettingsManager
	{
		public bool Enable = false;
		public bool Specified = false;
		
		static private LogSettingsManager testManager;
		static public LogSettingsManager TestManager
		{
			get {
				if (testManager == null)
					testManager = MockLogSettingsManager.NewSpecified(true);
				return testManager; }
			set { testManager = value; }
		}
		
		public MockLogSettingsManager()
		{
		
		}
		
		public override bool IsEnabled(LogLevel level)
		{
			return Enable;
		}
		
		public override bool IsEnabled(LogLevel level, bool defaultValue)
		{
			if (Specified)
				return Enable;
			else
				return defaultValue;
		}
		           
		public override bool IsEnabled(string key, LogLevel level)
		{
			return Enable;
		}
		
		public override bool IsEnabled(string key, LogLevel level, bool defaultValue)
		{
			if (Specified)
				return Enable;
			else
				return IsEnabled(level, defaultValue);
		}
		
		static public MockLogSettingsManager NewSpecified(bool enable)
		{
			MockLogSettingsManager manager = new MockLogSettingsManager();
			manager.Specified = true;
			manager.Enable = enable;
			return manager;
		}
		
		static public MockLogSettingsManager NewNotSpecified()
		{
			MockLogSettingsManager manager = new MockLogSettingsManager();
			manager.Specified = false;
			return manager;
		}
	}
}
