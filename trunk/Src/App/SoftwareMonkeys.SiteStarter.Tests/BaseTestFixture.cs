using System;
using System.Reflection;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Tests
{
	/// <summary>
	/// Provides a base implementation for all test fixtures.
	/// </summary>
	public abstract class BaseTestFixture
	{
		public string TestName = String.Empty;
		
		[SetUp]
		public virtual void Start()
		{
			TestName = TestContext.CurrentContext.Test.Name;
			
			TestUtilities.ClearTestingDirectory(this);
			
			// Create a new ID for the current test
			testID = Guid.NewGuid();
			
		}
		
		[TearDown]
		public virtual void End()
		{
			TestUtilities.ClearTestingDirectory(this);
		}
		
		private Guid testID;
		/// <summary>
		/// Gets a random ID specific to the current test.
		/// </summary>
		public Guid TestID
		{
			get {
				return testID;
			}
		}
	}
}
