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
	/// Used to initialize the part state and make parts available for use.
	/// </summary>
	public class PartsInitializer
	{
		private PartFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create part file names/paths.
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
		/// Gets/sets the part saver used to save parts to file.
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
		/// Gets/sets the part scanners used to find available parts in the existing assemblies.
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
		/// Gets/sets the part loader used to find available parts in the existing assemblies.
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
		/// Gets a value indicating whether the parts info has been cached yet.
		/// </summary>
		public bool IsCached
		{
			get {
				bool isCached = PartsCacheExists();
				return isCached; }
		}
		
		public Page Page;
		
		/// <summary>
		/// Gets the full path to the directory containing part mappings.
		/// </summary>
		public string PartsDirectoryPath
		{
			get { return FileNamer.PartsDirectoryPath; }
		}
		
		static public string DefaultScannersKey = "PartsInitializer.DefaultScanners";
		
		static private BasePartScanner[] defaultScanners;
		/// <summary>
		/// Gets/sets the part scanners used to find available parts in the existing assemblies.
		/// </summary>
		static public BasePartScanner[] DefaultScanners
		{
			get {
				if (defaultScanners == null)
				{
					if (StateAccess.State.ContainsApplication(DefaultScannersKey))
						defaultScanners = (BasePartScanner[])StateAccess.State.GetApplication(DefaultScannersKey);
					
					if (defaultScanners == null)
					{
						defaultScanners = new BasePartScanner[]
						{
							new PartScanner()
						};
					}
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
		}
		
		public PartsInitializer(Page page, params BasePartScanner[] scanners)
		{
			Page = page;
			Scanners = scanners;
		}
		
		
		/// <summary>
		/// Initializes the parts and loads all parts to state.
		/// </summary>
		/// <param name="parts">The parts to initialize.</param>
		public void Initialize(PartInfo[] parts)
		{
			PartState.Parts = new PartStateCollection(parts);
		}
		
		/// <summary>
		/// Initializes the parts and loads all parts to state.
		/// </summary>
		public void Initialize()
		{
			using (LogGroup logGroup = LogGroup.Start("Initializing the web parts.", NLog.LogLevel.Debug))
			{
				if (StateAccess.IsInitialized && !PartState.IsInitialized)
				{
					PartInfo[] parts = new PartInfo[]{};
					
					bool pageIsAccessible = Page != null;
					
					// Only scan for parts if the page component is accessible (otherwise they can't be loaded through LoadControl)
					// and when the parts have NOT yet been mapped
					if (pageIsAccessible && !IsCached)
					{
						LogWriter.Debug("Is not cached. Scanning from type attributes.");
						
						parts = FindParts();
						
						Saver.SaveInfoToFile(parts);
						
						Initialize(parts);
					}
					else if(IsCached)
					{
						LogWriter.Debug("Is cached. Loading from XML.");
						
						parts = LoadParts();
						
						Initialize(parts);
					}
					
				}
				else
					LogWriter.Debug("State is not initialized. Skipping.");
			}
		}
		
		/// <summary>
		/// Loads the available parts from file.
		/// </summary>
		/// <returns>The loaded from the parts cache file.</returns>
		public PartInfo[] LoadParts()
		{
			return Loader.LoadInfoFromFile();
		}
		
		/// <summary>
		/// Finds all the parts available to the application.
		/// </summary>
		/// <returns>An array of the available parts.</returns>
		public PartInfo[] FindParts()
		{
			List<PartInfo> parts = new List<PartInfo>();
			
			using (LogGroup logGroup = LogGroup.Start("Finding parts.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("# of scanners: " + Scanners.Length);
				
				foreach (BasePartScanner scanner in Scanners)
				{
					foreach (PartInfo part in scanner.FindParts())
					{
						parts.Add(part);
					}
				}
			}
			return parts.ToArray();
		}
		
		/// <summary>
		/// Checks whether the parts info file exists.
		/// </summary>
		/// <returns>A value indicating whether the parts info files was found.</returns>
		public bool PartsCacheExists()
		{
			string file = FileNamer.PartsInfoFilePath;
			
			return File.Exists(file);
		}
	}
	
}
