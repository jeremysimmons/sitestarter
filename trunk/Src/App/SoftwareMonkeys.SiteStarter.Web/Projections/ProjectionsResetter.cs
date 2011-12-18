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
				
				string path = new ProjectionFileNamer().ProjectionsInfoDirectoryPath;
				
				LogWriter.Debug("Projections info path: " + path);
				
				int i = 0;
				
				// Delete projection info (not the actual projections)
				foreach (string file in Directory.GetFiles(path))
				{
					File.Delete(file);
					i++;
				}
				
				LogWriter.Debug("Total info files cleared: " + i.ToString());
				
			}
		}
	}
}
