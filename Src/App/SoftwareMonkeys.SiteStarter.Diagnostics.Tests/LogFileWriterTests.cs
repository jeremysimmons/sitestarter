using System;
using System.IO;
using NLog;
using NUnit.Framework;

namespace SoftwareMonkeys.SiteStarter.Diagnostics.Tests
{
	/// <summary>
	/// Description of LogFileWriterTests.
	/// </summary>
	[TestFixture]
	public class LogFileWriterTests : BaseDiagnosticsTestFixture
	{
		[SetUp]
		public override void Start()
		{
			// Disable test logging because it'll interfere
			EnableTestLogging = false;
			
			base.Start();
		}
		
		[Test]
		public void Test_Write_DoesntCommitUntilTime()
		{
			LogFileWriter writer = new LogFileWriter();
			writer.BufferDuration = 5;
			writer.PreviousCommitTime = DateTime.Now;
			
			Guid entryID = Guid.NewGuid();
			Guid parentID = Guid.Empty;
			
			string data = LogWriter.CreateLogEntry(LogLevel.Error, "Test message", "TestType", "TestMethod", entryID, parentID, 0);
			
			string logFilePath = LogFileWriter.LogFilePath;
			
			// Delete the existing log if its found
			if (File.Exists(logFilePath))
				File.Delete(logFilePath);
			
			writer.PreviousCommitTime = DateTime.Now;
			
			writer.Write(entryID, data);
			
			//Assert.IsFalse(File.Exists(logFilePath), "The log file was created when it shouldn't have been written to yet.");
			
			string content = LoadLogContent(logFilePath);
			
			bool entryFound = content.IndexOf(entryID.ToString()) > -1;
			
			Assert.IsFalse(entryFound, "The entry was found when it shouldn't have been committed yet.");
		}
		
		[Test]
		public void Test_Write_CommitsWhenTime()
		{
			LogFileWriter writer = new LogFileWriter();
			writer.BufferDuration = 5;
			writer.PreviousCommitTime = DateTime.Now.Subtract(new TimeSpan(0, 0, 10));
			
			Guid entryID = Guid.NewGuid();
			Guid parentID = Guid.Empty;
			
			string data = LogWriter.CreateLogEntry(LogLevel.Error, "Test message", "TestType", "TestMethod", entryID, parentID, 0);
			
			string logFilePath = LogFileWriter.LogFilePath;
			
			// Delete the existing log if its found
			if (File.Exists(logFilePath))
				File.Delete(logFilePath);
			
			writer.Write(entryID, data);
			
			string content = LoadLogContent(logFilePath);
			
			bool entryFound = content.IndexOf(entryID.ToString()) > -1;
			
			Assert.IsTrue(entryFound, "The entry wasn't found when it should have been.");
		}
		
		[Test]
		public void Test_Write_ClearsBufferData()
		{
			LogFileWriter writer = new LogFileWriter();
			writer.BufferDuration = 5;
			writer.PreviousCommitTime = DateTime.Now;
			
			Guid entryID = Guid.NewGuid();
			Guid parentID = Guid.Empty;
			
			string data = LogWriter.CreateLogEntry(LogLevel.Error, "Test message", "TestType", "TestMethod", entryID, parentID, 0);
			
			string logFilePath = LogFileWriter.LogFilePath;
			
			// Delete the existing log if its found
			if (File.Exists(logFilePath))
				File.Delete(logFilePath);
			
			writer.PreviousCommitTime = DateTime.Now;
			
			writer.BufferData = data;
			writer.CommitBuffer();
			
			Assert.AreEqual(String.Empty, writer.BufferData, "CommitBuffer didn't clear the buffer data.");
		}
		
		[Test]
		public void Test_Write_SetsPreviousCommitTime()
		{
			LogFileWriter writer = new LogFileWriter();
			writer.BufferDuration = 5;
			writer.PreviousCommitTime = DateTime.Now.Subtract(new TimeSpan(0, 0, 30));
			
			DateTime previousCommitTime = writer.PreviousCommitTime;
			
			Guid entryID = Guid.NewGuid();
			Guid parentID = Guid.Empty;
			
			string data = LogWriter.CreateLogEntry(LogLevel.Error, "Test message", "TestType", "TestMethod", entryID, parentID, 0);
			
			string logFilePath = LogFileWriter.LogFilePath;
			
			// Delete the existing log if its found
			if (File.Exists(logFilePath))
				File.Delete(logFilePath);
			
			writer.PreviousCommitTime = DateTime.Now;
			
			writer.BufferData = data;
			writer.CommitBuffer();
			
			Assert.AreNotEqual(previousCommitTime, writer.PreviousCommitTime, "PreviousCommitTime property wasn't updated on CommitBuffer call.");
		}
		
	}
}
