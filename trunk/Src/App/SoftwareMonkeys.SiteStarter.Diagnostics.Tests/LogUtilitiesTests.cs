using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Xml;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Tests;

namespace SoftwareMonkeys.SiteStarter.Diagnostics.Tests
{
	// TODO: Remove if not used
	[Obsolete]
	[TestFixture]
	public class LogUtilitiesTests : BaseDiagnosticsTestFixture
	{
		public string ApplicationPath
		{
			get { return TestUtilities.GetTestingPath(this); }
		}
		
		public LogUtilitiesTests()
		{
			//Config.Initialize(ApplicationPath, "");
		}


	}
}
