using SoftwareMonkeys.SiteStarter.Tests;
using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.State;
using System.Reflection;
using System.IO;
using SoftwareMonkeys.SiteStarter.Diagnostics.Tests;

namespace SoftwareMonkeys.SiteStarter.Configuration.Tests
{
	/// <summary>
	/// Provides a base implementation for all configuration test fixtures and other test fixtures that require a mock configuration environment to run.
	/// </summary>
	public abstract class BaseConfigurationTestFixture : BaseDiagnosticsTestFixture
	{
		[SetUp]
		public override void Start()
		{
			base.Start();
			
			InitializeMockConfiguration();
		}
		
		[TearDown]
		public override void End()
		{
			DisposeMockConfiguration();
			
			base.End();
		}
		
		/// <summary>
		/// Creates a mock AppConfig object for use while testing.
		/// </summary>
		/// <returns>The mock AppConfig object ready for use.</returns>
		public IAppConfig CreateMockAppConfig()
        {
			string applicationPath = TestUtilities.GetTestingPath(this);
			string configPath = TestUtilities.GetTestingPath(this) + Path.DirectorySeparatorChar + "Application.testing.config";
			
        	IAppConfig config = new MockAppConfig();
        	config.FilePath = configPath;
        	config.PhysicalApplicationPath = applicationPath;
        	config.PathVariation = "testing";
        	config.Saver = new MockConfigSaver();
        	
        	return config;
        }
		
		/// <summary>
		/// Initializes the mock configuration environment.
		/// </summary>
		public virtual void InitializeMockConfiguration()
		{
			Config.Application = CreateMockAppConfig();
		}
		
		/// <summary>
		/// Disposes the mock configuration environment.
		/// </summary>
		public virtual void DisposeMockConfiguration()
		{
			Config.Application = null;
		}
	}
}
