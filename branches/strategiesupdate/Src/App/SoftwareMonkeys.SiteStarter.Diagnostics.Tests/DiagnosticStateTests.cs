using System;
using NUnit.Framework;

namespace SoftwareMonkeys.SiteStarter.Diagnostics.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class DiagnosticStateTests : BaseDiagnosticsTestFixture
	{
		[Test]
		public void Test_PopGroup_Empty()
		{
			DiagnosticState.PopGroup();
		}
	}
}
