using System;
using System.Configuration;
using System.Configuration.Provider;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Data.Db4o;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.IO;
using SoftwareMonkeys.SiteStarter.Tests;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	public class MockDb4oDataProviderInitializer
	{
		BaseTestFixture ExecutingTestFixture;
		
		public MockDb4oDataProviderInitializer(BaseTestFixture executingTestFixture)
		{
			ExecutingTestFixture = executingTestFixture;
		}

		public void Initialize()
		{
			using (LogGroup logGroup = LogGroup.Start("Initializing data provider", NLog.LogLevel.Info))
			{
					Db4oDataProvider provider = new Db4oDataProvider();
					provider.Initialize("Db4oDataProvider", null);
					
					DataAccess.Data = provider;
					
					// The versions and directories are set here by default but can be changed by each test to suit the situation.
					provider.Schema.LegacyVersion = new Version(0, 9, 0, 0);
					provider.Schema.ApplicationVersion = new Version(1, 0, 0, 0);
					
					provider.Exporter.ExportDirectoryPath = TestUtilities.GetTestingPath(ExecutingTestFixture) + Path.DirectorySeparatorChar + "Exported";
					provider.Importer.ImportableDirectoryPath = TestUtilities.GetTestingPath(ExecutingTestFixture) + Path.DirectorySeparatorChar + "Exported";
					provider.Importer.ImportedDirectoryPath = TestUtilities.GetTestingPath(ExecutingTestFixture) + Path.DirectorySeparatorChar + "Imported";
			}
		}
	}
}
