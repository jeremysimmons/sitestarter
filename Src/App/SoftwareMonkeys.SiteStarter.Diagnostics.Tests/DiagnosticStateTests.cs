using System;
using System.IO;
using NUnit.Framework;

namespace SoftwareMonkeys.SiteStarter.Diagnostics.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class DiagnosticStateTests : BaseDiagnosticsTestFixture
	{
		[SetUp]
		public override void Start()
		{
			// Disable test logging because it'll interfere
			EnableTestLogging = false;
			
			base.Start();
		}
		
		[Test]
		public void Test_PopGroup_Empty()
		{
			DiagnosticState.PopGroup();
		}
		
		[Test]
		public void Test_Dispose_CommitsDataAndDisposesGroups()
		{
			// Create a group
			LogGroup group = LogGroup.Start("Test group", LogLevel.Error);
			
			// Create a sub group
			LogGroup group2 = LogGroup.Start("Another group", LogLevel.Error);
			
			// Write into the sub group
			LogWriter.Error("Test entry");
			
			// Dispose the state to test that the groups are disposed properly as well
			DiagnosticState.Dispose();
						
			// Load the log
			string content = LoadLogContent(LogFileWriter.LogFilePath);
			
			int expectedLength = 1095;
			
			Assert.IsTrue(content.IndexOf("Test group") > -1, "Didn't find first group.");
			Assert.IsTrue(content.IndexOf("Another group") > -1, "Didn't find second group.");
			Assert.IsTrue(content.IndexOf("Test entry") > -1, "Didn't find second group.");
			
			// Check that the log is the right length, to indicate that the entries were all written to the log
			Assert.AreEqual(expectedLength, content.Length, "Invalid log length.");
			
			Assert.IsNull(DiagnosticState.CurrentGroup, "Didn't dispose CurrentGroup");
		}
		
	}
}
