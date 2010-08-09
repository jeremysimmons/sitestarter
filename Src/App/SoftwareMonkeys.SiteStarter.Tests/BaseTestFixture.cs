using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Tests.State;

namespace SoftwareMonkeys.SiteStarter.Tests
{
	/// <summary>
	/// Provides a base implementation for all test fixtures and initializes a mock state environment.
	/// </summary>
	public abstract class BaseTestFixture
	{			
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
		public StateProvider CreateMockState()
        {
        	StateProvider config = new MockStateProvider();
        	
        	
        	return config;
        }
	}
}
