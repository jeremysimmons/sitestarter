using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Web.WebControls;

namespace SoftwareMonkeys.SiteStarter.Web.Tests.WebControls
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class IndexGridTests : BaseWebTestFixture
	{
		
		[Test]
		public void Test_GetValidPageIndex_ZeroItems_Keep()
		{
			IndexGrid grid = new IndexGrid();
			grid.CurrentPageIndex = 0;
			grid.PageSize = 10;
			grid.VirtualItemCount = 0;
			
			int pageIndex = grid.GetValidPageIndex();
			
			Assert.AreEqual(0, pageIndex, "Invalid page index.");
			
		}
		
		[Test]
		public void Test_GetValidPageIndex_ZeroItems_Adjust()
		{
			IndexGrid grid = new IndexGrid();
			grid.CurrentPageIndex = 1;
			grid.PageSize = 10;
			grid.VirtualItemCount = 0;
			
			int pageIndex = grid.GetValidPageIndex();
			
			Assert.AreEqual(0, pageIndex, "Invalid page index.");
			
		}
		
		[Test]
		public void Test_GetValidPageIndex_Adjust()
		{
			IndexGrid grid = new IndexGrid();
			grid.CurrentPageIndex = 2;
			grid.PageSize = 10;
			grid.VirtualItemCount = 20;
			
			int pageIndex = grid.GetValidPageIndex();
			
			// Check that the page index was moved from 1 (an invalid index) to 0 (which is valid)
			Assert.AreEqual(1, pageIndex, "The page index wasn't adjusted according to the maximum number of pages.");
			
		}
		
		[Test]
		public void Test_VerifyPageIndex_Adjust_Fraction()
		{
			IndexGrid grid = new IndexGrid();
			grid.CurrentPageIndex = 2;
			grid.PageSize = 10;
			grid.VirtualItemCount = 11;
			
			int pageIndex = grid.GetValidPageIndex();
			
			// Check that the page index was moved from 1 (an invalid index) to 0 (which is valid)
			Assert.AreEqual(1, pageIndex, "The page index wasn't adjusted according to the maximum number of pages.");
			
		}
		
		[Test]
		public void Test_VerifyPageIndex_Keep()
		{
			IndexGrid grid = new IndexGrid();
			grid.CurrentPageIndex = 1;
			grid.PageSize = 10;
			grid.VirtualItemCount = 20;
			
			int pageIndex = grid.GetValidPageIndex();
			
			Assert.AreEqual(1, pageIndex, "The valid page index wasn't kept.");
			
		}
		
		[Test]
		public void Test_VerifyPageIndex_Keep_Fraction()
		{
			IndexGrid grid = new IndexGrid();
			grid.CurrentPageIndex = 1;
			grid.PageSize = 10;
			grid.VirtualItemCount = 19;
			
			int pageIndex = grid.GetValidPageIndex();
			
			Assert.AreEqual(1, pageIndex, "The valid page index wasn't kept.");
			
		}
	}
}
