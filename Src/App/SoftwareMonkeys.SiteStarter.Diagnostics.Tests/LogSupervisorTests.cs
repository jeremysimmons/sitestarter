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
		[Test]
		public void Test_IsEnabled_Debug_True()
		{
			LogSupervisor supervisor = new LogSupervisor();
			supervisor.ModeDetector = new MockModeDetector(true);
			
			bool isEnabled = supervisor.LoggingEnabled(NLog.LogLevel.Debug);
			
			Assert.IsTrue(isEnabled, "Should have been true.");
		}
		
		[Test]
		public void Test_IsEnabled_Debug_False()
		{
			LogSupervisor supervisor = new LogSupervisor();
			supervisor.ModeDetector = new MockModeDetector(false);
			
			bool isEnabled = supervisor.LoggingEnabled(NLog.LogLevel.Debug);
			
			Assert.IsFalse(isEnabled, "Should have been false.");
		}
		
		[Test]
		public void Test_IsEnabled_Error_True_DebugMode()
		{
			LogSupervisor supervisor = new LogSupervisor();
			supervisor.ModeDetector = new MockModeDetector(true);
			
			bool isEnabled = supervisor.LoggingEnabled(NLog.LogLevel.Error);
			
			Assert.IsTrue(isEnabled, "Should have been true.");
		}
		
		[Test]
		public void Test_IsEnabled_Error_True_ReleaseMode()
		{
			LogSupervisor supervisor = new LogSupervisor();
			supervisor.ModeDetector = new MockModeDetector(false);
			
			bool isEnabled = supervisor.LoggingEnabled(NLog.LogLevel.Error);
			
			Assert.IsTrue(isEnabled, "Should have been true.");
		}
		
		[Test]
		public void Test_IsEnabled_Info_True_DebugMode()
		{
			LogSupervisor supervisor = new LogSupervisor();
			supervisor.ModeDetector = new MockModeDetector(true);
			
			bool isEnabled = supervisor.LoggingEnabled(NLog.LogLevel.Info);
			
			Assert.IsTrue(isEnabled, "Should have been true.");
		}
		
		[Test]
		public void Test_IsEnabled_Info_True_ReleaseMode()
		{
			LogSupervisor supervisor = new LogSupervisor();
			supervisor.ModeDetector = new MockModeDetector(false);
			
			bool isEnabled = supervisor.LoggingEnabled(NLog.LogLevel.Info);
			
			Assert.IsTrue(isEnabled, "Should have been true.");
		}
	}
}
