using System;
using SoftwareMonkeys.SiteStarter.Business.Tests;
using SoftwareMonkeys.SiteStarter.Tests;

namespace SoftwareMonkeys.SiteStarter.Web.Tests
{
	/// <summary>
	/// 
	/// </summary>
	public class MockFileExistenceChecker : FileExistenceChecker
	{
		public bool DoesExist = false;
				
		public MockFileExistenceChecker(BaseTestFixture fixture, bool doesExist)
		{
			DoesExist = doesExist;
			FileMapper = new MockFileMapper(fixture);
		}
		
		public override bool Exists(string relativeFilePath)
		{
			return DoesExist;
		}
	}
}
