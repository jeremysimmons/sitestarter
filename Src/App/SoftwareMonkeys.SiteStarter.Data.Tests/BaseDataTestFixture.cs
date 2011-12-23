using SoftwareMonkeys.SiteStarter.Tests;
using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Entities.Tests;
using SoftwareMonkeys.SiteStarter.Configuration.Tests;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	/// <summary>
	/// Provides a base implementation for all data test fixtures and test fixtures that require a mock data tier to function.
	/// </summary>
	public abstract class BaseDataTestFixture : BaseEntityTestFixture
	{
		/// <summary>
		/// Starts a test by initializing the mock environment, registering test entities, and ensuring the testing directory is clear.
		/// </summary>
		[SetUp]
		public override void Start()
		{
			base.Start();
			
			InitializeMockData();
		}
		
		/// <summary>
		/// Ends a test by disposing the mock test environment and deleting mock data.
		/// </summary>
		[TearDown]
		public override void End()
		{
			DisposeMockData();
			
			base.End();
		}
		
		
		/// <summary>
		/// Initializes the mock data system. Should be overridden and data source/provider specific mock data provider should be initialized.
		/// </summary>
		public virtual void InitializeMockData()
		{
			MockDataProviderInitializer.Initialize();
		}
		
		/// <summary>
		/// Disposes the mock data system and deletes all mock data.
		/// </summary>
		public virtual void DisposeMockData()
		{
			DisposeMockData(true);
		}
		
		/// <summary>
		/// Disposes the mock data system and deletes all mock data.
		/// </summary>
		/// <param name="commit"></param>
		public virtual void DisposeMockData(bool commit)
		{
			if (StateAccess.IsInitialized && DataAccess.IsInitialized && DataAccess.Data != null)
			{	
				DataAccess.Dispose(true, commit);
			}
		}
	}
}
