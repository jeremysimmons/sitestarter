﻿using System;
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
					if (StrategiesDirectoryPath != null && StrategiesDirectoryPath != String.Empty)
						saver.StrategiesDirectoryPath = StrategiesDirectoryPath;
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
					if (StrategiesDirectoryPath != null && StrategiesDirectoryPath != String.Empty)
						loader.StrategiesDirectoryPath = StrategiesDirectoryPath;
				}
				return loader; }
			set { loader = value; }
		}
		
		/// <summary>
		/// Gets a value indicating whether the strategies have been mapped yet.
		/// </summary>
		public bool IsMapped
		{
			get {
				bool isMapped = StrategyMappingsExist();
				return isMapped; }
		}
		
		/// <summary>
		/// Gets the full path to the directory containing strategy mappings.
		/// </summary>
		public string StrategiesDirectoryPath
		{
			get { return FileNamer.StrategiesDirectoryPath; }
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
			using (LogGroup logGroup = AppLogger.StartGroup("Initializing the business strategies.", NLog.LogLevel.Debug))
			{
				StrategyInfo[] strategies = new StrategyInfo[]{};
				if (IsMapped)
				{
					AppLogger.Debug("Is mapped. Loading from XML.");
					
					strategies = LoadStrategies();
				}
				else
				{
					AppLogger.Debug("Is not mapped. Scanning from type attributes.");
					
					strategies = FindStrategies();
					SaveToFile(strategies);
				}
				
				Initialize(strategies);
			}
		}
		
		/// <summary>
		/// Saves the mappings for the provided strategies to strategies mappings directory.
		/// </summary>
		/// <param name="strategies">The strategies to save to file.</param>
		public void SaveToFile(StrategyInfo[] strategies)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Saving the provided strategies to XML.", NLog.LogLevel.Debug))
			{
				foreach (StrategyInfo strategy in strategies)
				{
					Saver.SaveToFile(strategy);
				}
			}
		}
		
		
		/// <summary>
		/// Loads the available strategies from file.
		/// </summary>
		/// <returns>The loaded from the strategies mappings directory.</returns>
		public StrategyInfo[] LoadStrategies()
		{
			return Loader.LoadFromDirectory();
		}
		
		/// <summary>
		/// Finds all the strategies available to the application.
		/// </summary>
		/// <returns>An array of the available strategies.</returns>
		public StrategyInfo[] FindStrategies()
		{
			return Scanner.FindStrategies();
		}
		
		/// <summary>
		/// Checks whether the strategy mappings have been created and saved to file.
		/// </summary>
		/// <returns>A value indicating whether the strategy mappings directory was found.</returns>
		public bool StrategyMappingsExist()
		{
			string directory = StrategiesDirectoryPath;
			
			return (Directory.Exists(directory) && Directory.GetFiles(directory).Length > 0);
		}
	}
	
}