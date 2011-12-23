using System;
using SoftwareMonkeys.SiteStarter.Data;
using System.IO;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// Used to initialize the controller state and make controllers available for use.
	/// </summary>
	public class ControllersInitializer
	{
		private ControllerFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create controller file names/paths.
		/// </summary>
		public ControllerFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new ControllerFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		private ControllerSaver saver;
		/// <summary>
		/// Gets/sets the controller saver used to save controllers to file.
		/// </summary>
		public ControllerSaver Saver
		{
			get {
				if (saver == null)
				{
					saver = new ControllerSaver();
				}
				return saver; }
			set { saver = value; }
		}
		
		private BaseControllerScanner[] scanners;
		/// <summary>
		/// Gets/sets the controller scanners used to find available controllers in the existing assemblies.
		/// </summary>
		public BaseControllerScanner[] Scanners
		{
			get {
				if (scanners == null)
				{
					scanners = new BaseControllerScanner[]
					{
						new ControllerScanner()
					};
				}
				return scanners; }
			set { scanners = value; }
		}
		
		private ControllerLoader loader;
		/// <summary>
		/// Gets/sets the controller loader used to find available controllers in the existing assemblies.
		/// </summary>
		public ControllerLoader Loader
		{
			get {
				if (loader == null)
				{
					loader = new ControllerLoader();
				}
				return loader; }
			set { loader = value; }
		}
		
		/// <summary>
		/// Gets a value indicating whether the controllers info has been cached yet.
		/// </summary>
		public bool IsCached
		{
			get {
				bool isCached = ControllerInfoExists();
				return isCached; }
		}
		
		public ControllersInitializer()
		{
		}
		
		public ControllersInitializer(BaseControllerScanner[] scanners)
		{
			Scanners = scanners;
		}
		
		
		/// <summary>
		/// Initializes the controllers and loads all controllers to state.
		/// </summary>
		/// <param name="controllers">The controllers to initialize.</param>
		public void Initialize(ControllerInfo[] controllers)
		{
			ControllerState.Controllers = new ControllerStateCollection(controllers);
		}
		
		/// <summary>
		/// Initializes the controllers and loads all controllers to state.
		/// </summary>
		public void Initialize()
		{
			Initialize(false);
		}
		
		/// <summary>
		/// Initializes the controllers and loads all controllers to state.
		/// </summary>
		/// <param name="includeTestControllers"></param>
		public void Initialize(bool includeTestControllers)
		{
			using (LogGroup logGroup = LogGroup.Start("Initializing the web controllers.", NLog.LogLevel.Debug))
			{
				if (StateAccess.IsInitialized)
				{
					if (!ControllerState.IsInitialized)
					{
						ControllerInfo[] controllers = new ControllerInfo[]{};
						if (IsCached)
						{
							LogWriter.Debug("Is cached. Loading from XML.");
							
							controllers = LoadControllers();
						}
						else
						{
							LogWriter.Debug("Is not cached. Scanning from type attributes.");
							
							controllers = FindControllers(includeTestControllers);
							Saver.SaveInfoToFile(controllers);
						}
						
						Initialize(controllers);
					}
					else
						LogWriter.Debug("Already initialized.");
				}
				else
					LogWriter.Debug("State is not initialized. Skipping.");
			}
		}	
		
		/// <summary>
		/// Loads the available controllers from file.
		/// </summary>
		/// <returns>The loaded from the controllers mappings directory.</returns>
		public ControllerInfo[] LoadControllers()
		{
			return Loader.LoadInfoFromDirectory();
		}
		
		/// <summary>
		/// Finds all the controllers available to the application.
		/// </summary>
		/// <param name="includeTestControllers"></param>
		/// <returns>An array of the available controllers.</returns>
		public ControllerInfo[] FindControllers(bool includeTestControllers)
		{
			List<ControllerInfo> controllers = new List<ControllerInfo>();
			
			using (LogGroup logGroup = LogGroup.Start("Finding controllers.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("# of scanners: " + Scanners.Length);
				
				foreach (BaseControllerScanner scanner in Scanners)
				{
					foreach (ControllerInfo controller in scanner.FindControllers(includeTestControllers))
					{
						controllers.Add(controller);
					}
				}
			}
			return controllers.ToArray();
		}
		
		/// <summary>
		/// Checks whether the controller info has been created and saved to file.
		/// </summary>
		/// <returns>A value indicating whether the controllers info file was found.</returns>
		public bool ControllerInfoExists()
		{
			string file = FileNamer.ControllersInfoFilePath;
			
			return File.Exists(file);
		}
	}
	
}
