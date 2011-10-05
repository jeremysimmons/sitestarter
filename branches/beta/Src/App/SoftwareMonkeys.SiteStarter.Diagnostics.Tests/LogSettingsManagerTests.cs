using System;
using NUnit.Framework;

namespace SoftwareMonkeys.SiteStarter.Diagnostics.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class LogSettingsManagerTests : BaseDiagnosticsTestFixture
	{
		[SetUp]
		public override void Start()
		{
			EnableTestLogging = false;	
			
			base.Start();
		}
		
		[Test]
		public void Test_IsEnabled_Info_NotSpecifiedInConfig_ReturnsTrue()
		{
			LogSettingsManager manager = new LogSettingsManager();
			bool isEnabled = manager.IsEnabled(LogLevel.Info);
			
			Assert.IsTrue(isEnabled, "Returned false when it should have returned true.");
		}
		
		[Test]
		public void Test_IsEnabled_Error_NotSpecifiedInConfig_ReturnsTrue()
		{
			LogSettingsManager manager = new LogSettingsManager();
			bool isEnabled = manager.IsEnabled(LogLevel.Error);
			
			Assert.IsTrue(isEnabled, "Returned false when it should have returned true.");
		}
		
		[Test]
		public void Test_IsEnabled_Debug_NotSpecifiedInConfig_ReturnsFalse()
		{
			LogSettingsManager manager = new LogSettingsManager();
			bool isEnabled = manager.IsEnabled(LogLevel.Debug);
			
			Assert.IsFalse(isEnabled, "Returned true when it should have returned false.");
		}
	}
}
