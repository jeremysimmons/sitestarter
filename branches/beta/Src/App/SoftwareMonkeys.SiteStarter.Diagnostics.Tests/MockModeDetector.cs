using System;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Diagnostics.Tests
{
	/// <summary>
	/// 
	/// </summary>
	public class MockModeDetector : ModeDetector
	{
		private bool isDebug = false;
		
		public override bool IsDebug {
			get { return isDebug; }
		}
		
		public MockModeDetector(bool isDebug)
		{
			this.isDebug = isDebug;
		}
		
	}
}
