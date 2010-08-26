using System;
using SoftwareMonkeys.SiteStarter.Data.Tests;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// Description of BaseBusinessTestFixture.
	/// </summary>
	public class BaseBusinessTestFixture : BaseDataTestFixture
	{
		public BaseBusinessTestFixture()
		{
			
		}
		
		
		public override void InitializeMockData()
		{
			new MockDb4oDataProviderInitializer(this).Initialize();
		}
	}
}
