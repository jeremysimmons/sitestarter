using System;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// 
	/// </summary>
	public interface IPagingLocation
	{
		int PageSize {get;}
		int PageIndex {get;}
		int AbsoluteTotal {get;}
	}
}
