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
					if (ControllersDirectoryPath != null && ControllersDirectoryPath != String.Empty)
						saver.ControllersDirectoryPath = ControllersDirectoryPath;
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
					if (ControllersDirectoryPath != null && ControllersDirectoryPath != String.Empty)
						loader.ControllersDirectoryPath = ControllersDirectoryPath;
				}
				return loader; }
			set { loader = value; }
		}
		
		/// <summary>
		/// Gets a value indicating whether the controllers have been mapped yet.
		/// </summary>
		public bool IsMapped
		{
			get {
				bool isMapped = ControllerMappingsExist();
				return isMapped; }
		}
		
		/// <summary>
		/// Gets the full path to the directory containing controller mappings.
		/// </summary>
		public string ControllersDirectoryPath
		{
			get { return FileNamer.ControllersDirectoryPath; }
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
			using (LogGroup logGroup = LogGroup.Start("Initializing the business controllers.", NLog.LogLevel.Debug))
			{
				if (StateAccess.IsInitialized)
				{
					ControllerInfo[] controllers = new ControllerInfo[]{};
					if (IsMapped)
					{
						LogWriter.Debug("Is mapped. Loading from XML.");
						
						controllers = LoadControllers();
					}
					else
					{
						LogWriter.Debug("Is not mapped. Scanning from type attributes.");
						
						controllers = FindControllers();
						SaveInfoToFile(controllers);
					}
					
					Initialize(controllers);
				}
			}
		}
		
		/// <summary>
		/// Saves the info for the provided controllers to controllers info directory.
		/// </summary>
		/// <param name="controllers">The controllers to save to file.</param>
		public void SaveInfoToFile(ControllerInfo[] controllers)
		{
			using (LogGroup logGroup = LogGroup.Start("Saving the provided controllers to XML.", NLog.LogLevel.Debug))
			{
				foreach (ControllerInfo controller in controllers)
				{
					Saver.SaveInfoToFile(controller);
				}
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
		/// <returns>An array of the available controllers.</returns>
		public ControllerInfo[] FindControllers()
		{
			List<ControllerInfo> controllers = new List<ControllerInfo>();
			
			using (LogGroup logGroup = LogGroup.Start("Finding controllers.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("# of scanners: " + Scanners.Length);
				
				foreach (BaseControllerScanner scanner in Scanners)
				{
					foreach (ControllerInfo controller in scanner.FindControllers())
					{
						controllers.Add(controller);
					}
				}
			}
			return controllers.ToArray();
		}
		
		/// <summary>
		/// Checks whether the controller mappings have been created and saved to file.
		/// </summary>
		/// <returns>A value indicating whether the controller mappings directory was found.</returns>
		public bool ControllerMappingsExist()
		{
			string directory = FileNamer.ControllersInfoDirectoryPath;
			
			return (Directory.Exists(directory) && Directory.GetFiles(directory).Length > 0);
		}
	}
	
}
