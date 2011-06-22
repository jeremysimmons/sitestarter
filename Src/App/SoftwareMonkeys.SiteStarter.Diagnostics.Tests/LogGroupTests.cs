using System;
using System.Diagnostics;
using NUnit.Framework;

namespace SoftwareMonkeys.SiteStarter.Diagnostics.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class LogGroupTests : BaseDiagnosticsTestFixture
	{
		[SetUp]
		public override void Start()
		{
			// Disable test logging because it'll interfere
			EnableTestLogging = false;
			
			base.Start();
		}
		
		[Test]
		public void Test_Start()
		{
			using (LogGroup logGroup = LogGroup.Start("Outer group ", LogLevel.Debug))
			{
				for (int i = 0; i < 10; i++)
				{
					using (LogGroup logGroup2 = LogGroup.Start("Test group " + i, LogLevel.Debug))
					{
						if (i > 8)
							break;
					}
				}
			}
		}
		
		[Test]
		public void Test_Start2()
		{
			int startCount = DiagnosticState.GroupStack.Count;
			
			LogSupervisor supervisor = new LogSupervisor();
			
			using (LogGroup logGroup = LogGroup.Start("Outer group ", LogLevel.Debug))
			{
				
				Assert.AreEqual(logGroup.ID.ToString(), DiagnosticState.CurrentGroupID.ToString(), "Current group ID doesn't match that of outer group.");
				
				Assert.AreEqual(startCount+1, DiagnosticState.GroupStack.Count, "Invalid number of items in group stack.");
				
				LogGroup logGroup3 = null;
				
				using (LogGroup logGroup2 = LogGroup.Start("Test group", LogLevel.Debug))
				{
					
					Assert.AreEqual(2, DiagnosticState.GroupStack.Count, "Invalid number of items in group stack.");
				
					Assert.AreEqual(logGroup2.ID.ToString(), DiagnosticState.CurrentGroupID.ToString(), "Current group ID doesn't match that of test group.");
				
					Assert.AreEqual(logGroup.ID.ToString(), logGroup2.ParentID.ToString(), "Sub group's parent ID doesn't match the ID of the outer group.");
					
					// Create the group that will potentially break the logging system because it's not wrapped in "using (...) {...}"
					logGroup3 = LogGroup.Start("Break group", LogLevel.Debug);
					
					
					Assert.AreEqual(3, DiagnosticState.GroupStack.Count, "Invalid number of items in group stack.");
					
					Assert.AreEqual(logGroup3.ID.ToString(), DiagnosticState.CurrentGroupID.ToString(), "Current group ID doesn't match that of breaking group.");
				
					
					bool canPop3 = supervisor.CanPop(logGroup3, DiagnosticState.CurrentGroup);
					
					Assert.IsTrue(canPop3);
					
					bool canPop2 = supervisor.CanPop(logGroup2, DiagnosticState.CurrentGroup);
					
					Assert.IsTrue(canPop2);
				}
				
				Assert.AreEqual(1, DiagnosticState.GroupStack.Count, "Invalid number of items in group stack.");
				
				Assert.AreNotEqual(logGroup3.ID.ToString(), DiagnosticState.CurrentGroupID.ToString(), "Current group ID matches that of the breaking group even though the breaking group's parent has ended.");
				
				
				Assert.AreEqual(logGroup.ID.ToString(), DiagnosticState.CurrentGroupID.ToString(), "Current group ID doesn't match that of outer group after test group has ended.");
				
			}
		}
	}
}
