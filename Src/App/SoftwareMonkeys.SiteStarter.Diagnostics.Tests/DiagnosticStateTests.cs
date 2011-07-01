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
			// If no error occurred then it succeeded
		}
		
		
	}
}
