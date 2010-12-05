using System;
using SoftwareMonkeys.SiteStarter.Data;
using System.IO;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Web.Parts
{
	/// <summary>
	/// Used to initialize the projection state and make projections available for use.
	/// </summary>
	public class PartsInitializer
	{
		private PartFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create projection file names/paths.
		/// </summary>
		public PartFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new PartFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		private PartSaver saver;
		/// <summary>
		/// Gets/sets the projection saver used to save projections to file.
		/// </summary>
		public PartSaver Saver
		{
			get {
				if (saver == null)
				{
					saver = new PartSaver();
					if (PartsDirectoryPath != null && PartsDirectoryPath != String.Empty)
						saver.PartsDirectoryPath = PartsDirectoryPath;
				}
				return saver; }
			set { saver = value; }
		}
		
		private BasePartScanner[] scanners;
		/// <summary>
		/// Gets/sets the projection scanners used to find available projections in the existing assemblies.
		/// </summary>
		public BasePartScanner[] Scanners
		{
			get {
				if (scanners == null)
				{
					scanners = DefaultScanners;
					
					// Make sure each of the default scanners uses the right page
					foreach (BasePartScanner scanner in scanners)
						scanner.Page = Page;
					/*scanners = new BasePartScanner[]
					{
						new PartScanner(Page)
					};*/
				}
				return scanners; }
			set { scanners = value; }
		}
		
		private PartLoader loader;
		/// <summary>
		/// Gets/sets the projection loader used to find available projections in the existing assemblies.
		/// </summary>
		public PartLoader Loader
		{
			get {
				if (loader == null)
				{
					loader = new PartLoader();
					if (PartsDirectoryPath != null && PartsDirectoryPath != String.Empty)
						loader.PartsDirectoryPath = PartsDirectoryPath;
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
				bool isMapped = PartMappingsExist();
				return isMapped; }
		}
		
		public Page Page;
		
		/// <summary>
		/// Gets the full path to the directory containing projection mappings.
		/// </summary>
		public string PartsDirectoryPath
		{
			get { return FileNamer.PartsDirectoryPath; }
		}
		
		static public string DefaultScannersKey = "PartsInitializer.DefaultScanners";
		
		static private BasePartScanner[] defaultScanners;
		/// <summary>
		/// Gets/sets the projection scanners used to find available projections in the existing assemblies.
		/// </summary>
		static public BasePartScanner[] DefaultScanners
		{
			get {
				if (defaultScanners == null)
				{
					if (StateAccess.State.ContainsApplication(DefaultScannersKey))
						defaultScanners = (BasePartScanner[])StateAccess.State.GetApplication(DefaultScannersKey);
					
					defaultScanners = new BasePartScanner[]
					{
						new PartScanner()
					};
				}
				return defaultScanners; }
			set { defaultScanners = value;
				StateAccess.State.SetApplication(DefaultScannersKey, defaultScanners);
			}
		}
		
		
		public PartsInitializer()
		{
		}
		
		public PartsInitializer(Page page)
		{
			Page = page;
			Scanners = new BasePartScanner[]
			{
				new PartScanner(Page)
			};
		}
		
		public PartsInitializer(Page page, BasePartScanner[] scanners)
		{
			Page = page;
			Scanners = scanners;
		}
		
		
		/// <summary>
		/// Initializes the projections and loads all projections to state.
		/// </summary>
		/// <param name="projections">The projections to initialize.</param>
		public void Initialize(PartInfo[] projections)
		{
			PartState.Parts = new PartStateCollection(projections);
		}
		
		/// <summary>
		/// Initializes the projections and loads all projections to state.
		/// </summary>
		public void Initialize()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Initializing the business projections.", NLog.LogLevel.Debug))
			{
				PartInfo[] projections = new PartInfo[]{};
				
				bool pageIsAccessible = Page != null;
				
				// Only scan for projections if the page component is accessible (otherwise they can't be loaded through LoadControl)
				// and when the projections have NOT yet been mapped
				if (pageIsAccessible && !IsMapped)
				{
					AppLogger.Debug("Is not mapped. Scanning from type attributes.");
					
					projections = FindParts();
					
					SaveInfoToFile(projections);
					
					Initialize(projections);
				}
				else if(IsMapped)
				{
					AppLogger.Debug("Is mapped. Loading from XML.");
					
					projections = LoadParts();
					
					Initialize(projections);
				}
				// Otherwise just skip it, as it's likely before setup has run and just needs to wait
				
			}
		}
		
		/// <summary>
		/// Saves the info for the provided projections to projections info directory.
		/// </summary>
		/// <param name="projections">The projections to save to file.</param>
		public void SaveInfoToFile(PartInfo[] projections)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Saving the provided projections to XML.", NLog.LogLevel.Debug))
			{
				foreach (PartInfo projection in projections)
				{
					Saver.SaveInfoToFile(projection);
				}
			}
		}
		
		
		/// <summary>
		/// Loads the available projections from file.
		/// </summary>
		/// <returns>The loaded from the projections mappings directory.</returns>
		public PartInfo[] LoadParts()
		{
			return Loader.LoadInfoFromDirectory();
		}
		
		/// <summary>
		/// Finds all the projections available to the application.
		/// </summary>
		/// <returns>An array of the available projections.</returns>
		public PartInfo[] FindParts()
		{
			List<PartInfo> projections = new List<PartInfo>();
			
			using (LogGroup logGroup = AppLogger.StartGroup("Finding projections.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("# of scanners: " + Scanners.Length);
				
				foreach (BasePartScanner scanner in Scanners)
				{
					foreach (PartInfo projection in scanner.FindParts())
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
		public bool PartMappingsExist()
		{
			string directory = FileNamer.PartsInfoDirectoryPath;
			
			return (Directory.Exists(directory) && Directory.GetFiles(directory).Length > 0);
		}
	}
	
}
