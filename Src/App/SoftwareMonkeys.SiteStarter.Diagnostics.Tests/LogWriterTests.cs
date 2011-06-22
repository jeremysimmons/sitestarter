using System;
using SoftwareMonkeys.SiteStarter.State;
using System.IO;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Diagnostics.Tests
{
	/// <summary>
	/// Tests the LogWriter.
	/// </summary>
	[TestFixture]
	public class LogWriterTests : BaseDiagnosticsTestFixture
	{
		public LogWriterTests()
		{
		}
		
		[Test]
		public void Test_Dispose_UnlocksTheLogFile()
		{
			// Create the log by adding a dummy entry
			LogWriter.Error("Test entry");
			
			// Dispose the log to unlock the log file
			LogWriter.Dispose();
			
			// Get the path of the log file
			string logPath = LogFileWriter.LogFilePath;
			
			// Check that the log file was created
			Assert.IsTrue(File.Exists(logPath), "Can't find log file.");
			
			// Delete the file to test that it was unlocked
			File.Delete(logPath);
			
			// Ensure the file was deleted, confirming that it was unlocked
			Assert.IsFalse(File.Exists(logPath), "Log file wasn't deleted.");
		}
		
		[Test]
		public void Test_ConvertLevel_ConvertsNLogToInternalEnumeration()
		{
			NLog.LogLevel original = NLog.LogLevel.Error;
			
			LogLevel converted = LogWriter.ConvertLevel(original);
			
			Assert.AreEqual(LogLevel.Error, converted, "Didn't convert properly.");
		}
	}
}
