using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Diagnostics.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class LogSupervisorTests : BaseDiagnosticsTestFixture
	{
		[SetUp]
		public override void Start()
		{
			EnableTestLogging = false;
			
			base.Start();
		}
		
		[Test]
		public void Test_LoggingEnabled_DebugMode_DebugLevel_SettingsTrue_ReturnsTrue()
		{
			LogSupervisor supervisor = new LogSupervisor();
			supervisor.ModeDetector = new MockModeDetector(true);
			supervisor.SettingsManager = MockLogSettingsManager.NewSpecified(true);
			
			bool isEnabled = supervisor.LoggingEnabled(LogLevel.Debug);
			
			Assert.IsTrue(isEnabled, "Should have been true.");
		}
		
		[Test]
		public void Test_LoggingEnabled_DebugMode_DebugLevel_SettingsFalse_ReturnsFalse()
		{
			LogSupervisor supervisor = new LogSupervisor();
			supervisor.ModeDetector = new MockModeDetector(true);
			supervisor.SettingsManager =  MockLogSettingsManager.NewSpecified(false);
			
			bool isEnabled = supervisor.LoggingEnabled(LogLevel.Debug);
			
			Assert.IsFalse(isEnabled, "Should have been false.");
		}
		
		[Test]
		public void Test_LoggingEnabled_DebugMode_ErrorLevel_SettingsTrue_ReturnsTrue()
		{
			LogSupervisor supervisor = new LogSupervisor();
			supervisor.ModeDetector = new MockModeDetector(true);
			supervisor.SettingsManager = MockLogSettingsManager.NewSpecified(true);
			
			bool isEnabled = supervisor.LoggingEnabled(LogLevel.Error);
			
			Assert.IsTrue(isEnabled, "Should have been true.");
		}
		
		[Test]
		public void Test_LoggingEnabled_ReleaseMode_ErrorLevel_ReturnsTrue()
		{
			LogSupervisor supervisor = new LogSupervisor();
			supervisor.ModeDetector = new MockModeDetector(false);
			
			bool isEnabled = supervisor.LoggingEnabled(LogLevel.Error);
			
			Assert.IsTrue(isEnabled, "Should have been true.");
		}
		
		[Test]
		public void Test_LoggingEnabled_DebugMode_InfoLevel_ReturnsTrue()
		{
			LogSupervisor supervisor = new LogSupervisor();
			supervisor.ModeDetector = new MockModeDetector(true);
			
			bool isEnabled = supervisor.LoggingEnabled(LogLevel.Info);
			
			Assert.IsTrue(isEnabled, "Should have been true.");
		}
		
		[Test]
		public void Test_LoggingEnabled_ReleaseMode_InfoLevel_ReturnsTrue()
		{
			LogSupervisor supervisor = new LogSupervisor();
			supervisor.ModeDetector = new MockModeDetector(false);
			
			bool isEnabled = supervisor.LoggingEnabled(LogLevel.Info);
			
			Assert.IsTrue(isEnabled, "Should have been true.");
		}
		
		[Test]
		public void Test_LoggingEnabled_DebugMode_InfoLevel_SettingsTrue_ReturnsTrue()
		{
			LogSupervisor supervisor = new LogSupervisor();
			supervisor.ModeDetector = new MockModeDetector(true);
			supervisor.SettingsManager = MockLogSettingsManager.NewSpecified(true);
			
			bool isEnabled = supervisor.LoggingEnabled(LogLevel.Info);
			
			Assert.IsTrue(isEnabled, "Should have been true.");
		}
		
		[Test]
		public void Test_IsEnabled_MockType_DebugMode_InfoLevel_SettingsTrue_ReturnsTrue()
		{
			LogSupervisor supervisor = new LogSupervisor();
			supervisor.ModeDetector = new MockModeDetector(true);
			supervisor.SettingsManager = MockLogSettingsManager.NewSpecified(true);
			
			bool isEnabled = supervisor.IsEnabled("MockType", LogLevel.Info);
			
			Assert.IsTrue(isEnabled, "Should have been true.");
		}
		
		[Test]
		public void Test_IsEnabled_MockType_DebugMode_DebugLevel_SettingsNotSpecified_ReturnsFalse()
		{
			LogSupervisor supervisor = new LogSupervisor();
			supervisor.ModeDetector = new MockModeDetector(true);
			supervisor.SettingsManager = MockLogSettingsManager.NewNotSpecified();
			
			bool isEnabled = supervisor.IsEnabled("MockType", LogLevel.Debug);
			
			Assert.IsFalse(isEnabled, "Should have been true.");
		}
		
		[Test]
		public void Test_CanPop_False()
		{
			LogGroup logGroup1 = LogGroup.Start("Test");
			
			LogGroup logGroup2 = LogGroup.Start("Test2");
			
			logGroup2.Parent = logGroup1;
			
			LogSupervisor supervisor = new LogSupervisor();
			bool canPop = supervisor.CanPop(logGroup2, logGroup1);
			
			Assert.IsFalse(canPop);
		}
		
		
		[Test]
		public void Test_CanPop_True()
		{
			LogGroup logGroup1 = LogGroup.Start("Test");
			
			LogGroup logGroup2 = LogGroup.Start("Test2");
			
			logGroup2.Parent = logGroup1;
			
			LogSupervisor supervisor = new LogSupervisor();
			
			bool canPop = supervisor.CanPop(logGroup2, logGroup2);
			
			Assert.IsTrue(canPop);
		}
	}
}
