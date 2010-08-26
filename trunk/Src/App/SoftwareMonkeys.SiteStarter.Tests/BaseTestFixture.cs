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
			// Create a new ID for the current test
			testID = Guid.NewGuid();
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
