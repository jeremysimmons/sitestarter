using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Tests
{
	/// <summary>
	/// Provides a base implementation for all test fixtures.
	/// </summary>
	public abstract class BaseTestFixture
	{
		[SetUp]
		public void Start()
		{
			TestUtilities.ClearTestingDirectory(this);
			// Create a new ID for the current test
			testID = Guid.NewGuid();
		}
		
		[TearDown]
		public void End()
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
