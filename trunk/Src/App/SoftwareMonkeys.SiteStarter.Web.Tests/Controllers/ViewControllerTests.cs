using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Tests;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Web.Controllers;
using SoftwareMonkeys.SiteStarter.Web.Properties;
using SoftwareMonkeys.SiteStarter.Web.Projections;

namespace SoftwareMonkeys.SiteStarter.Web.Tests.Controllers
{
	[TestFixture]
	public class ViewControllerTests : BaseWebTestFixture
	{
		[Test]
		public void Test_View_EntityProvided()
		{
			TestRecord.RegisterType();
			
			TestRecord record1 = new TestRecord();
			record1.ID = Guid.NewGuid();
			record1.Text = "Record 1";
			
			//DataAccess.Data.Saver.Save(record1);
			
			// Check that the records do exist
			//int count = DataAccess.Data.Indexer.GetEntities<TestRecord>().Length;
			//Assert.AreEqual(1, count, "Invalid number of TestRecord objects found.");
			
			BaseViewProjection page = new BaseViewProjection();

			
			ViewController controller = ViewController.New(page, typeof(TestRecord));
			
			
			if (controller == null)
				throw new Exception("Controller is null.");
			
			
			controller.View(record1);
			
			Assert.IsNotNull(controller.DataSource, "The DataSource property on the controller wasn't set.");
		}
	}
}
