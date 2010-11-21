using System;
using SoftwareMonkeys.SiteStarter.Web.Controllers;
using SoftwareMonkeys.SiteStarter.Web;
using NUnit.Framework;
using System.Web.UI.WebControls;
using SoftwareMonkeys.SiteStarter.Web.WebControls;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Data;
using System.Configuration;
using System.Web.UI;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Web.Projections;

namespace SoftwareMonkeys.SiteStarter.Web.Tests.Controllers
{
	[TestFixture]
	public class IndexControllerTests : BaseWebTestFixture
	{
		[Test]
		public void Test_Index_Default()
		{
			TestRecord record1 = new TestRecord();
			record1.ID = Guid.NewGuid();
			record1.Text = "Record 1";
			
			TestRecord record2 = new TestRecord();
			record2.ID = Guid.NewGuid();
			record2.Text = "Record 2";
			
			TestRecord record3 = new TestRecord();
			record3.ID = Guid.NewGuid();
			record3.Text = "Record 3";
			
			TestRecord record4 = new TestRecord();
			record4.ID = Guid.NewGuid();
			record4.Text = "Record 4";
			
			if (DataAccess.Data == null)
				throw new InvalidOperationException("Data provider has not been initialized. Run setup.");
			
			DataAccess.Data.Saver.Save(record1);
			DataAccess.Data.Saver.Save(record2);
			DataAccess.Data.Saver.Save(record3);
			DataAccess.Data.Saver.Save(record4);
			
			// Check that the records do exist
			int count = DataAccess.Data.Indexer.GetEntities<TestRecord>().Length;
			Assert.AreEqual(4, count, "Invalid number of TestRecord objects found.");
			
			BaseIndexProjection page = new BaseIndexProjection();
			
			IndexController controller = IndexController.New(page, typeof(TestRecord), false);
			controller.RequireAuthorisation = false;
			
			
			if (controller == null)
				throw new Exception("Controller is null.");
			
			//controller.CurrentPageIndex = pageIndex;
			//controller.PageSize = 20;
			
			controller.Index();
			
			Assert.IsNotNull(controller.DataSource, "The DataSource property on the controller wasn't set.");
			
			
			Assert.AreEqual(4, controller.DataSource.Length, "Item count mismatch.");
			
			foreach (TestRecord record in DataAccess.Data.Indexer.GetEntities<TestRecord>())
			{
				DataAccess.Data.Deleter.Delete(record);
			}
		}
		
		[Test]
		public void Test_Index_Paged_Page1()
		{
			TestRecord.RegisterType();
			
			List<TestRecord> list = new List<TestRecord>();
			
			for (int i = 0; i < 30; i++)
			{
				TestRecord record = new TestRecord();
				record.ID = Guid.NewGuid();
				record.Text = "Record " + i;
				
				DataAccess.Data.Saver.Save(record);
				
				list.Add(record);
			}
			
			if (DataAccess.Data == null)
				throw new InvalidOperationException("Data provider has not been initialized. Run setup.");
			
			BaseIndexProjection page = new BaseIndexProjection();
			
			
			
			
			IndexController controller = IndexController.New(page, typeof(TestRecord), true);
			controller.RequireAuthorisation = false;
			
			if (controller == null)
				throw new Exception("Controller is null.");
			
			controller.CurrentPageIndex = 0;
			controller.PageSize = 20;
			
			controller.Index();
			
			Assert.IsNotNull(controller.DataSource, "The DataSource of the controller wasn't set.");
			
			
			IEntity[] entities = (IEntity[])controller.DataSource;
			
			
			Assert.AreEqual(20, entities.Length, "DataSource count mismatch.");
						
			
			foreach (TestRecord record in list)
				DataAccess.Data.Deleter.Delete(record);
		}
		
		
		[Test]
		public void Test_Index_Paged_Page2()
		{
			TestRecord.RegisterType();
			
			List<TestRecord> list = new List<TestRecord>();
			
			for (int i = 0; i < 30; i++)
			{
				TestRecord record = new TestRecord();
				record.ID = Guid.NewGuid();
				record.Text = "Record " + i;
				
				DataAccess.Data.Saver.Save(record);
				
				list.Add(record);
			}
			
			if (DataAccess.Data == null)
				throw new InvalidOperationException("Data provider has not been initialized. Run setup.");
			
			BaseIndexProjection page = new BaseIndexProjection();
			
			
			
			IndexController controller = IndexController.New(page, typeof(TestRecord), true);
			controller.RequireAuthorisation = false;
			
			if (controller == null)
				throw new Exception("Controller is null.");
			
			controller.CurrentPageIndex = 1;
			controller.PageSize = 20;
			controller.Index();
			
			Assert.IsNotNull(controller.DataSource, "The DataSource of the controller wasn't set.");
			
			IEntity[] entities = controller.DataSource;
			
			
			Assert.AreEqual(10, entities.Length, "DataSource count mismatch.");
			
			
			
			foreach (TestRecord record in list)
				DataAccess.Data.Deleter.Delete(record);
		}
		
		
		[Test]
		public void Test_Index_PrepareIndex()
		{
			List<TestRecord> list = new List<TestRecord>();
			
			for (int i = 0; i < 30; i++)
			{
				TestRecord record = new TestRecord();
				record.ID = Guid.NewGuid();
				record.Text = "Record " + i;
				
				DataAccess.Data.Saver.Save(record);
				
				list.Add(record);
			}
			
			if (DataAccess.Data == null)
				throw new InvalidOperationException("Data provider has not been initialized. Run setup.");
			
			BaseIndexProjection page = new BaseIndexProjection();
			
			IndexController controller = IndexController.New(page, typeof(TestRecord), true);
			controller.RequireAuthorisation = false;
			
			if (controller == null)
				throw new Exception("Controller is null.");
			
			controller.CurrentPageIndex = 0;
			controller.PageSize = 20;
			controller.SortExpression = "NameAscending";
			
			IEntity[] entities = controller.PrepareIndex();
			
			Assert.IsNotNull(entities, "entities == null");
			
			Assert.AreEqual(20, entities.Length, "Entity count mismatch.");
			
			Assert.AreEqual(30, controller.AbsoluteTotal, "Absolute count mismatch.");
			
			foreach (TestRecord record in list)
				DataAccess.Data.Deleter.Delete(record);
		}
		
		/*public IndexController CreateIndexController()
		{
			MultiView multiView = new MultiView();

			View indexView = new View();

			IndexGrid grid = new IndexGrid();
			grid.ID = "IndexGrid";
			
			BoundColumn column = new BoundColumn();
			column.DataField = "Text";
			column.HeaderText = "Text";
			
			grid.Columns.Add(column);
			
			multiView.Views.Add(indexView);
			
			indexView.Controls.Add(grid);
			
			IndexController controller = new IndexController(indexView,grid);
			
			return controller;
		}*/
	}
}
