using System;
using SoftwareMonkeys.SiteStarter.Data;
using System.IO;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	/// <summary>
	/// Used to initialize the projection state and make projections available for use.
	/// </summary>
	public class ProjectionsInitializer
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
		
		private ProjectionSaver saver;
		/// <summary>
		/// Gets/sets the projection saver used to save projections to file.
		/// </summary>
		public ProjectionSaver Saver
		{
			get {
				if (saver == null)
				{
					saver = new ProjectionSaver();
					if (ProjectionsDirectoryPath != null && ProjectionsDirectoryPath != String.Empty)
						saver.ProjectionsDirectoryPath = ProjectionsDirectoryPath;
				}
				return saver; }
			set { saver = value; }
		}
		
		private BaseProjectionScanner[] scanners;
		/// <summary>
		/// Gets/sets the projection scanners used to find available projections in the existing assemblies.
		/// </summary>
		public BaseProjectionScanner[] Scanners
		{
			get {
				if (scanners == null)
				{
					scanners = DefaultScanners;
					
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
		
		private ProjectionLoader loader;
		/// <summary>
		/// Gets/sets the projection loader used to find available projections in the existing assemblies.
		/// </summary>
		public ProjectionLoader Loader
		{
			get {
				if (loader == null)
				{
					loader = new ProjectionLoader();
					if (ProjectionsDirectoryPath != null && ProjectionsDirectoryPath != String.Empty)
						loader.ProjectionsDirectoryPath = ProjectionsDirectoryPath;
				}
				return loader; }
			set { loader = value; }
		}
		
		/// <summary>
		/// Gets a value indicating whether the projections have been mapped yet.
		/// </summary>
		public bool IsMapped
		{
			get {
				bool isMapped = ProjectionMappingsExist();
				return isMapped; }
		}
		
		public Page Page;
		
		/// <summary>
		/// Gets the full path to the directory containing projection mappings.
		/// </summary>
		public string ProjectionsDirectoryPath
		{
			get { return FileNamer.ProjectionsDirectoryPath; }
		}
		
		static public string DefaultScannersKey = "ProjectionsInitializer.DefaultScanners";
		
		static private BaseProjectionScanner[] defaultScanners;
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
		}
		
		
		public ProjectionsInitializer()
		{
		}
		
		public ProjectionsInitializer(Page page)
		{
			Page = page;
			Scanners = new BaseProjectionScanner[]
			{
				new ProjectionScanner(Page)
			};
		}
		
		public ProjectionsInitializer(Page page, params BaseProjectionScanner[] scanners)
		{
			Page = page;
			Scanners = scanners;
		}
		
		
		/// <summary>
		/// Initializes the projections and loads all projections to state.
		/// </summary>
		/// <param name="projections">The projections to initialize.</param>
		public void Initialize(ProjectionInfo[] projections)
		{
			ProjectionState.Projections = new ProjectionStateCollection(projections);
		}
		
		/// <summary>
		/// Initializes the projections and loads all projections to state.
		/// </summary>
		public void Initialize()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Initializing the business projections.", NLog.LogLevel.Debug))
			{
				ProjectionInfo[] projections = new ProjectionInfo[]{};
				
				bool pageIsAccessible = Page != null;
				
				// Only scan for projections if the page component is accessible (otherwise they can't be loaded through LoadControl)
				// and when the projections have NOT yet been mapped
				if (pageIsAccessible && !IsMapped)
				{
					AppLogger.Debug("Is not mapped. Scanning from type attributes.");
					
					projections = FindProjections();
					
					SaveInfoToFile(projections);
					
					Initialize(projections);
				}
				else if(IsMapped)
				{
					AppLogger.Debug("Is mapped. Loading from XML.");
					
					projections = LoadProjections();
					
					Initialize(projections);
				}
				// Otherwise just skip it, as it's likely before setup has run and just needs to wait
				
			}
		}
		
		/// <summary>
		/// Saves the info for the provided projections to projections info directory.
		/// </summary>
		/// <param name="projections">The projections to save to file.</param>
		public void SaveInfoToFile(ProjectionInfo[] projections)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Saving the provided projections to XML.", NLog.LogLevel.Debug))
			{
				foreach (ProjectionInfo projection in projections)
				{
					Saver.SaveInfoToFile(projection);
				}
			}
		}
		
		
		/// <summary>
		/// Loads the available projections from file.
		/// </summary>
		/// <returns>The loaded from the projections mappings directory.</returns>
		public ProjectionInfo[] LoadProjections()
		{
			return Loader.LoadInfoFromDirectory();
		}
		
		/// <summary>
		/// Finds all the projections available to the application.
		/// </summary>
		/// <returns>An array of the available projections.</returns>
		public ProjectionInfo[] FindProjections()
		{
			List<ProjectionInfo> projections = new List<ProjectionInfo>();
			
			using (LogGroup logGroup = AppLogger.StartGroup("Finding projections.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("# of scanners: " + Scanners.Length);
				
				foreach (BaseProjectionScanner scanner in Scanners)
				{
					foreach (ProjectionInfo projection in scanner.FindProjections())
					{
						projections.Add(projection);
					}
				}
			}
			return projections.ToArray();
		}
		
		/// <summary>
		/// Checks whether the projection mappings have been created and saved to file.
		/// </summary>
		/// <returns>A value indicating whether the projection mappings directory was found.</returns>
		public bool ProjectionMappingsExist()
		{
			string directory = FileNamer.ProjectionsInfoDirectoryPath;
			
			return (Directory.Exists(directory) && Directory.GetFiles(directory).Length > 0);
		}
	}
	
}
