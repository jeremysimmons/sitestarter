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
		public void Test_ConvertLevel_ConvertsNLogToInternalEnumeration()
		{
			NLog.LogLevel original = NLog.LogLevel.Error;
			
			LogLevel converted = LogWriter.ConvertLevel(original);
			
			Assert.AreEqual(LogLevel.Error, converted, "Didn't convert properly.");
		}
		
		[Test]
		public void Test_Write_FiveEntries()
		{
			string label = "Test entry: ";
			
			for (int i = 0; i < 5; i++)
			{
				LogWriter.Info(label + i+1.ToString());
			} 
			
			string logFilePath = LogFileWriter.LogFilePath;
			
			string content = LoadLogContent(logFilePath);
			
			for (int i = 0; i < 5; i++)
			{
				string expectedEntry = label + i+1.ToString();
				
				bool entryFound = content.IndexOf(expectedEntry.ToString()) > -1;
				
				Assert.IsTrue(entryFound, "Entry missing; index = " + i);
			}
		}
	}
}
