using System;
using System.Web.UI;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	/// <summary>
	/// Used to remove/dispose projections from the system.
	/// </summary>
	public class ProjectionsDisposer
	{
		
		private ProjectionFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create projection file names/paths.
		/// </summary>
		public ProjectionFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new ProjectionFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		public Page Page;
		
		public ProjectionsDisposer()
		{
		}
		
		public ProjectionsDisposer(Page page)
		{
			Page = page;
		}
		
		/// <summary>
		/// Disposes the projections found by the scanner.
		/// </summary>
		public void Dispose()
		{
			using (LogGroup logGroup = LogGroup.Start("Disposing the projections.", NLog.LogLevel.Debug))
			{
				if (ProjectionState.IsInitialized)
				{					
					Dispose(ProjectionState.GetProjections(false));
				}
			}
		}
		
		/// <summary>
		/// Disposes the provided projections.
		/// </summary>
		public void Dispose(ProjectionStateCollection projections)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Disposing the projections."))
			{
				if (ProjectionState.IsInitialized)
				{
					foreach (ProjectionInfo projection in projections)
					{
						projections.Remove(projection);
						
						// Delete the info file
						DeleteInfo(projection);
					}
				}
			}
		}
		
		/// <summary>
		/// Deletes the projection info file.
		/// </summary>
		/// <param name="info"></param>
		public void DeleteInfo(ProjectionInfo info)
		{
			string path = FileNamer.CreateInfoFilePath(info);
			
			File.Delete(path);
		}
	}
}
