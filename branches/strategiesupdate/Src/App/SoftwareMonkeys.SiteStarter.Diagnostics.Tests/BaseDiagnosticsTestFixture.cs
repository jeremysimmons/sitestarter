using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Tests;
using SoftwareMonkeys.SiteStarter.State.Tests;

namespace SoftwareMonkeys.SiteStarter.Diagnostics.Tests
{
	/// <summary>
	/// Provides a base implementation for all diagnostics test fixtures.
	/// </summary>
	public class BaseDiagnosticsTestFixture : BaseStateTestFixture
	{
		[SetUp]
		public void Start()
		{
			TestUtilities.ClearTestingDirectory(this);
			InitializeMockDiagnostics();
		}
		
		/// <summary>
		/// Ends a test by disposing the mock test environment and deleting mock data.
		/// </summary>
		[TearDown]
		public void End()
		{
			DisposeMockDiagnostics();
			TestUtilities.ClearTestingDirectory(this);
		}
		
		
		/// <summary>
		/// Initializes the mock diagnostic system.
		/// </summary>
		public virtual void InitializeMockDiagnostics()
		{
			
		}
		
		/// <summary>
		/// Disposes the mock diagnostic system.
		/// </summary>
		public virtual void DisposeMockDiagnostics()
		{
			
		}
	}
}
