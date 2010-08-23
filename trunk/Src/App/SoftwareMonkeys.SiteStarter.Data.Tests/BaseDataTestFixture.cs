using SoftwareMonkeys.SiteStarter.Tests;
using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration.Tests;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	/// <summary>
	/// Provides a base implementation for all data test fixtures and test fixtures that require a mock data tier to function.
	/// </summary>
	public abstract class BaseDataTestFixture : BaseConfigurationTestFixture
	{
		/// <summary>
		/// Starts a test by initializing the mock environment, registering test entities, and ensuring the testing directory is clear.
		/// </summary>
		[SetUp]
		public void Start()
		{
			TestUtilities.ClearTestingDirectory();
			InitializeMockState();
			InitializeMockConfiguration();
			InitializeMockData();
			TestUtilities.RegisterTestEntities();
		}
		
		/// <summary>
		/// Ends a test by disposing the mock test environment and deleting mock data.
		/// </summary>
		[TearDown]
		public void End()
		{
			DisposeMockData();
			DisposeMockConfiguration();
			DisposeMockState();
			TestUtilities.ClearTestingDirectory();
			TestUtilities.ClearTestEntities();
		}
		
		
		/// <summary>
		/// Initializes the mock data system. Should be overridden and data source/provider specific mock data provider should be initialized.
		/// </summary>
		public virtual void InitializeMockData()
		{
			MockDataProviderManager.Initialize();
		}
		
		/// <summary>
		/// Disposes the mock data system and deletes all mock data.
		/// </summary>
		public virtual void DisposeMockData()
		{
			/*foreach (string dataStoreName in DataAccess.Data.GetDataStoreNames())
			{
				DataAccess.Data.Stores[dataStoreName].Dispose();
				DataAccess.Data.Stores.Remove(DataAccess.Data.Stores[dataStoreName]);
			}*/
			
			DataAccess.Data.Dispose();
		}
	}
}
