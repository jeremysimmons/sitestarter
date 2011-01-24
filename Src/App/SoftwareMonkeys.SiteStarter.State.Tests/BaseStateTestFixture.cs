using System;
using SoftwareMonkeys.SiteStarter.Tests;
using NUnit.Framework;

namespace SoftwareMonkeys.SiteStarter.State.Tests
{
	/// <summary>
	/// Provides a base implementation for all state and state dependent test fixtures.
	/// </summary>
	public class BaseStateTestFixture : BaseTestFixture
	{
		
		/// <summary>
		/// Starts a test by initializing the mock environment, registering test entities, and ensuring the testing directory is clear.
		/// </summary>
		[SetUp]
		public void Start()
		{
			TestUtilities.ClearTestingDirectory(this);
			InitializeMockState();
		}
		
		/// <summary>
		/// Ends a test by disposing the mock test environment and deleting mock data.
		/// </summary>
		[TearDown]
		public void End()
		{
			DisposeMockState();
			TestUtilities.ClearTestingDirectory(this);
		}
	
		
		/// <summary>
		/// Initializes a mock state environment for use during testing.
		/// </summary>
		public void InitializeMockState()
		{
			StateAccess.State = CreateMockState();
		}
		
		/// <summary>
		/// Disposes the mock state environment.
		/// </summary>
		public void DisposeMockState()
		{
			StateAccess.State = null;
		}
		
		/// <summary>
		/// Creates a mock state provider for using during testing.
		/// </summary>
		/// <returns>The mock StateProvider ready for use.</returns>
		public BaseStateProvider CreateMockState()
        {
        	BaseStateProvider config = new MockStateProvider(this);
        	
        	return config;
        }
	}
}
