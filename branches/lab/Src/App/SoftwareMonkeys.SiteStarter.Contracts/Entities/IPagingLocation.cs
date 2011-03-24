using System;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// 
	/// </summary>
	public interface IPagingLocation
	{
		int PageSize {get;set;}
		int PageIndex {get;set;}
		int AbsoluteTotal {get;set;}
	}
}
