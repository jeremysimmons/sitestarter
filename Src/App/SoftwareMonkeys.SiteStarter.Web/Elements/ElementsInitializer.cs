using System;
using SoftwareMonkeys.SiteStarter.Data;
using System.IO;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Web.Elements
{
	/// <summary>
	/// Used to initialize the element state and make elements available for use.
	/// </summary>
	public class ElementsInitializer
	{
		private ElementFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create element file names/paths.
		/// </summary>
		public ElementFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new ElementFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		private ElementSaver saver;
		/// <summary>
		/// Gets/sets the element saver used to save elements to file.
		/// </summary>
		public ElementSaver Saver
		{
			get {
				if (saver == null)
				{
					saver = new ElementSaver();
				}
				return saver; }
			set { saver = value; }
		}
		
		private BaseElementScanner[] scanners;
		/// <summary>
		/// Gets/sets the element scanners used to find available elements in the existing assemblies.
		/// </summary>
		public BaseElementScanner[] Scanners
		{
			get {
				if (scanners == null)
				{
					scanners = new BaseElementScanner[]
					{
						new ElementScanner()
					};
				}
				return scanners; }
			set { scanners = value; }
		}
		
		private ElementLoader loader;
		/// <summary>
		/// Gets/sets the element loader used to find available elements in the existing assemblies.
		/// </summary>
		public ElementLoader Loader
		{
			get {
				if (loader == null)
				{
					loader = new ElementLoader();
				}
				return loader; }
			set { loader = value; }
		}
		
		/// <summary>
		/// Gets a value indicating whether the elements info has been cached.
		/// </summary>
		public bool IsCached
		{
			get {
				bool isCached = ElementsInfoExists();
				return isCached; }
		}
		
		public ElementsInitializer()
		{
		}
		
		public ElementsInitializer(BaseElementScanner[] scanners)
		{
			Scanners = scanners;
		}
		
		
		/// <summary>
		/// Initializes the elements and loads all elements to state.
		/// </summary>
		/// <param name="elements">The elements to initialize.</param>
		public void Initialize(ElementInfo[] elements)
		{
			ElementState.Elements = new ElementStateCollection(elements);
		}
		
		/// <summary>
		/// Initializes the elements and loads all elements to state.
		/// </summary>
		public void Initialize()
		{
			Initialize(false);
		}
		
		/// <summary>
		/// Initializes the elements and loads all elements to state.
		/// </summary>
		/// <param name="includeTestElements"></param>
		public void Initialize(bool includeTestElements)
		{
			using (LogGroup logGroup = LogGroup.Start("Initializing the web elements.", NLog.LogLevel.Debug))
			{
				if (StateAccess.IsInitialized)
				{
					if (!ElementState.IsInitialized)
					{
						ElementInfo[] elements = new ElementInfo[]{};
						if (IsCached)
						{
							LogWriter.Debug("Is cached. Loading from XML.");
							
							elements = LoadElements();
						}
						else
						{
							LogWriter.Debug("Is not cached. Scanning from type attributes.");
							
							elements = FindElements(includeTestElements);
							Saver.SaveInfoToFile(elements);
						}
						
						Initialize(elements);
					}
					else
						LogWriter.Debug("Already initialized.");
				}
				else
					LogWriter.Debug("State is not initialized. Skipping.");
			}
		}		
		
		/// <summary>
		/// Loads the available elements from file.
		/// </summary>
		/// <returns>The loaded elements info.</returns>
		public ElementInfo[] LoadElements()
		{
			return Loader.LoadInfoFromFile();
		}
		
		/// <summary>
		/// Finds all the elements available to the application.
		/// </summary>
		/// <param name="includeTestElements"></param>
		/// <returns>An array of the available elements.</returns>
		public ElementInfo[] FindElements(bool includeTestElements)
		{
			List<ElementInfo> elements = new List<ElementInfo>();
			
			using (LogGroup logGroup = LogGroup.Start("Finding elements.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("# of scanners: " + Scanners.Length);
				
				foreach (BaseElementScanner scanner in Scanners)
				{
					foreach (ElementInfo element in scanner.FindElements(includeTestElements))
					{
						elements.Add(element);
					}
				}
			}
			return elements.ToArray();
		}
		
		/// <summary>
		/// Checks whether the elements info has been saved to file.
		/// </summary>
		/// <returns>A value indicating whether the elements info file was found.</returns>
		public bool ElementsInfoExists()
		{
			return File.Exists(FileNamer.ElementsInfoFilePath);
		}
	}
	
}
