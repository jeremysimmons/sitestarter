using System;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Holds the coordinates to a specific page on an index.
	/// </summary>
	public class PagingLocation : IPagingLocation
	{
		private int pageIndex;
		/// <summary>
		/// Gets/sets the current page index.
		/// Note: This is 0 based so PageIndex=(PageNumber-1)
		/// </summary>
		public int PageIndex
		{
			get { return pageIndex; }
			set { pageIndex = value; }
		}
		
		private int pageSize;
		/// <summary>
		/// Gets/sets the size of each page.
		/// </summary>
		public int PageSize
		{
			get { return pageSize; }
			set { pageSize = value; }
		}
		
		private int absoluteTotal;
		/// <summary>
		/// Gets/sets the absolute total count of all matching items, including those on ALL pages, not just the specified one.
		/// </summary>
		public int AbsoluteTotal
		{
			get { return absoluteTotal; }
			set { absoluteTotal = value; }
		}
		
		public int FirstPageIndex = 0;
		public int LastPageIndex = 0;
		
		/// <summary>
		/// Sets the page index and page size of the current location.
		/// </summary>
		/// <param name="pageIndex">The index of the current page.</param>
		/// <param name="pageSize">The size of each page.</param>
		public PagingLocation(int pageIndex, int pageSize)
		{
			//using (LogGroup logGroup = LogGroup.StartDebug("Constructing PagingLocation"))
			//{
				PageIndex = pageIndex;
				PageSize = pageSize;
				
			//	LogWriter.Debug("Position (i): " + i.ToString());
			//	LogWriter.Debug("Page index: " + pageIndex);
			//	LogWriter.Debug("Page size: " + pageSize);
				
				// Calculate the position of the first item on the page
				FirstPageIndex = (pageIndex * pageSize); // 0 based
				
				// Calculate the position of the last item on the page
				LastPageIndex = (FirstPageIndex + pageSize) -1; // -1 to make it the last of the page, instead of first item on next page
				
			//	LogWriter.Debug("First position: " + first.ToString());
			//	LogWriter.Debug("Last position: " + last.ToString());
			//}
		}
		
		
		/// <summary>
		/// Checks whether the specified position is within the specified page.
		/// </summary>
		/// <param name="i">The 0 based index of item to check.</param>
		/// <returns>A bool value indicating whether the specified index position is within the specified page.</returns>
		public bool IsInPage(int i)
		{
			// Create the return flag
			bool isInPage = false;
			
			//using (LogGroup logGroup = LogGroup.StartDebug("Checking whether the specified position is within the specified page."))
			//{
				// The position is in the current page if it is between or equal
				// to the first and last items on the page
				isInPage = i >= FirstPageIndex
					&& i <= LastPageIndex;
				
			//	LogWriter.Debug("Is in page? " + isInPage.ToString());
			//}
			
			return isInPage;
		}
	}
}
