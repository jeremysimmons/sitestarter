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
		
		
		/*static private BaseProjectionScanner[] defaultScanners;
		/// <summary>
		/// Gets/sets the projection scanners used to find available projections in the existing assemblies.
		/// </summary>
		static public BaseProjectionScanner[] DefaultScanners
		{
			get {
				if (defaultScanners == null)
				{
					if (StateAccess.State.ContainsApplication(DefaultScannersKey))
						defaultScanners = (BaseProjectionScanner[])StateAccess.State.GetApplication(DefaultScannersKey);
					
					defaultScanners = new BaseProjectionScanner[]
					{
						new ProjectionScanner()
					};
				}
				return defaultScanners; }
			set { defaultScanners = value;
				StateAccess.State.SetApplication(DefaultScannersKey, defaultScanners);
			}
		}*/
		
		
		private BaseProjectionScanner[] scanners;
		/// <summary>
		/// Gets/sets the projection scanners used to find available projections in the existing assemblies.
		/// </summary>
		public BaseProjectionScanner[] Scanners
		{
			get {
				if (scanners == null)
				{
					scanners = ProjectionsInitializer.DefaultScanners;
					
					// Make sure each of the default scanners uses the right page
					foreach (BaseProjectionScanner scanner in scanners)
						scanner.Page = Page;
					/*scanners = new BaseProjectionScanner[]
					{
						new ProjectionScanner(Page)
					};*/
				}
				return scanners; }
			set { scanners = value; }
		}
		
		public Page Page;
		
		public ProjectionsDisposer()
		{
		}
		
		public ProjectionsDisposer(Page page)
		{
			Page = page;
			Scanners = new BaseProjectionScanner[]
			{
				new ProjectionScanner(Page)
			};
		}
		
		public ProjectionsDisposer(Page page, params BaseProjectionScanner[] scanners)
		{
			Page = page;
			Scanners = scanners;
		}
		
		/// <summary>
		/// Disposes the projections found by the scanner.
		/// </summary>
		public void Dispose()
		{
			using (LogGroup logGroup = LogGroup.Start("Disposing the projections.", NLog.LogLevel.Debug))
			{
				ProjectionInfo[] projections = new ProjectionInfo[]{};
				
				foreach (ProjectionScanner scanner in Scanners)
				{
					Dispose(scanner.FindProjections());
				}
				
			}
		}
		
		/// <summary>
		/// Disposes the provided projections.
		/// </summary>
		public void Dispose(ProjectionInfo[] projections)
		{
			using (LogGroup logGroup = LogGroup.Start("Disposing the projections.", NLog.LogLevel.Debug))
			{
				foreach (ProjectionInfo projection in projections)
				{
					ProjectionState.Projections.Remove(
						ProjectionState.Projections[projection.Key]
					);
					
					DeleteInfo(projection);
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
