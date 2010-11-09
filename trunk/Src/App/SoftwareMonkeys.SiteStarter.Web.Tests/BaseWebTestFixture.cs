using System;
using SoftwareMonkeys.SiteStarter.Web.Controllers;
using SoftwareMonkeys.SiteStarter.Business.Tests;
using NUnit.Framework;

namespace SoftwareMonkeys.SiteStarter.Web.Tests
{
	public class BaseWebTestFixture : BaseBusinessTestFixture
	{
		[SetUp]
		public void Initialize()
		{
			new ControllersInitializer().Initialize();
		}
		
		public BaseWebTestFixture()
		{
		}
	}
}
