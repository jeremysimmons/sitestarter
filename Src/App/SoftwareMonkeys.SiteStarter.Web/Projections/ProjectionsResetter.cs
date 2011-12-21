using System;
using System.IO;
using System.Web.UI;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	/// <summary>
	/// Used to reset the projections so they can be re-scanned and re-intialized.
	/// </summary>
	public class ProjectionsResetter
	{
		public Page Page;
		
		public ProjectionsResetter(Page page)
		{
			Page = page;
		}
		
		public void Reset()
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Resetting projections cache."))
			{
				// Dispose the in-memory state
				new ProjectionsDisposer().Dispose();
				
				// Delete projection info (not the actual projections)
				File.Delete(new ProjectionFileNamer().ProjectionsInfoFilePath);				
			}
		}
	}
}
