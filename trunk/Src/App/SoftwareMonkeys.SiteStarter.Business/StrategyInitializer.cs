using System;
using SoftwareMonkeys.SiteStarter.Data;
using System.IO;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to initialize the strategy state and make strategies available for use.
	/// </summary>
	public class StrategyInitializer
	{
		private StrategyFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create strategy file names/paths.
		/// </summary>
		public StrategyFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new StrategyFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		private StrategySaver saver;
		/// <summary>
		/// Gets/sets the strategy saver used to save strategies to file.
		/// </summary>
		public StrategySaver Saver
		{
			get {
				if (saver == null)
				{
					saver = new StrategySaver();
				}
				return saver; }
			set { saver = value; }
		}
		
		private StrategyScanner scanner;
		/// <summary>
		/// Gets/sets the strategy scanner used to find available strategies in the existing assemblies.
		/// </summary>
		public StrategyScanner Scanner
		{
			get {
				if (scanner == null)
				{
					scanner = new StrategyScanner();
				}
				return scanner; }
			set { scanner = value; }
		}
		
		private StrategyLoader loader;
		/// <summary>
		/// Gets/sets the strategy loader used to find available strategies in the existing assemblies.
		/// </summary>
		public StrategyLoader Loader
		{
			get {
				if (loader == null)
				{
					loader = new StrategyLoader();
				}
				return loader; }
			set { loader = value; }
		}
		
		/// <summary>
		/// Gets a value indicating whether the strategies info has been cached.
		/// </summary>
		public bool IsCached
		{
			get {
				bool isCached = StrategiesInfoExists();
				return isCached; }
		}
		
		public StrategyInitializer()
		{
		}
		
		/// <summary>
		/// Initializes the strategies and loads all strategies to state.
		/// </summary>
		/// <param name="strategies">The strategies to initialize.</param>
		public void Initialize(StrategyInfo[] strategies)
		{
			StrategyState.Strategies = new StrategyStateNameValueCollection(strategies);
		}
		
		/// <summary>
		/// Initializes the strategies and loads all strategies to state.
		/// </summary>
		public void Initialize()
		{
			Initialize(false);
		}
		
		/// <summary>
		/// Initializes the strategies and loads all strategies to state. Note: Skips initialization if already initialized.
		/// </summary>
		public void Initialize(bool includeTestStrategies)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Initializing the business strategies."))
			{
				if (!StrategyState.IsInitialized)
				{
					StrategyInfo[] strategies = new StrategyInfo[]{};
					if (IsCached)
					{
						LogWriter.Debug("Is cached. Loading from XML.");
						
						strategies = LoadStrategies();
					}
					else
					{
						LogWriter.Debug("Is not cached. Scanning from type attributes.");
						
						strategies = FindStrategies(includeTestStrategies);
						Saver.SaveToFile(strategies);
					}
					
					Initialize(strategies);
				}
				else
					LogWriter.Debug("Already initialized.");
			}
		}		
		
		/// <summary>
		/// Loads the available strategies from file.
		/// </summary>
		/// <returns>The loaded from the strategies mappings directory.</returns>
		public StrategyInfo[] LoadStrategies()
		{
			return Loader.LoadInfoFromDirectory();
		}
		
		/// <summary>
		/// Finds all the strategies available to the application.
		/// </summary>
		/// <returns>An array of the available strategies.</returns>
		public StrategyInfo[] FindStrategies()
		{
			return FindStrategies(false);
		}
		
		/// <summary>
		/// Finds all the strategies available to the application.
		/// </summary>
		/// <param name="includeTestStrategies"></param>
		/// <returns>An array of the available strategies.</returns>
		public StrategyInfo[] FindStrategies(bool includeTestStrategies)
		{
			return Scanner.FindStrategies(includeTestStrategies);
		}
		
		/// <summary>
		/// Checks whether the strategies info file has been created.
		/// </summary>
		/// <returns>A value indicating whether the strategies info file was found</returns>
		public bool StrategiesInfoExists()
		{
			string path = FileNamer.StrategiesInfoFilePath;
			
			return File.Exists(path);
		}
	}
	
}
