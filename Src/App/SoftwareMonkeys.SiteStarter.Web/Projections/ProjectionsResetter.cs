using System;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	/// <summary>
	/// Used to reset the projections so they can be re-scanned and re-intialized.
	/// </summary>
	public class ProjectionsResetter
	{
		public ProjectionsResetter()
		{
		}
		
		public void Reset()
		{
			// Dispose the in-memory state
			new ProjectionsDisposer().Dispose();
			
			string path = new ProjectionFileNamer().ProjectionsInfoDirectoryPath;
			
			// Delete projection info (not the actual projections)
			foreach (string file in Directory.GetFiles(path))
			{
				File.Delete(file);
			}
			
			// Now the projections can be re-scanned and re-initialized
		}
	}
}
