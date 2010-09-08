using System;
using SoftwareMonkeys.SiteStarter.Data.Tests;
using NUnit.Framework;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	public class BaseBusinessTestFixture : BaseDataTestFixture
	{
		[SetUp]
		public void Initialize()
		{
			new StrategyInitializer().Initialize();
		}
		
		public BaseBusinessTestFixture()
		{
			
		}
		
		
		public override void InitializeMockData()
		{
			new MockDb4oDataProviderInitializer(this).Initialize();
		}
	}
}
