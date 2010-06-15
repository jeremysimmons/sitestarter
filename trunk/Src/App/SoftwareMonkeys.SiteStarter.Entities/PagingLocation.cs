using System;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// 
	/// </summary>
	public class PagingLocation : IPagingLocation
	{
		private int pageIndex;
		public int PageIndex	
		{
			get { return pageIndex; }
			set { pageIndex = value; }
		}
		
		private int pageSize;
		public int PageSize
		{
			get { return pageSize; }
			set { pageSize = value; }
		}
		
		private int absoluteTotal;
		public int AbsoluteTotal
		{
			get { return absoluteTotal; }
			set { absoluteTotal = value; }
		}
		
		public PagingLocation()
		{
		}
		
		public PagingLocation(int pageIndex, int pageSize)
		{
			PageIndex = pageIndex;
			PageSize = pageSize;
		}
	}
}
