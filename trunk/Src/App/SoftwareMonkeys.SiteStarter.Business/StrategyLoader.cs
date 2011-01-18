﻿using System;
using System.IO;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to load strategies from file.
	/// </summary>
	public class StrategyLoader
	{
		private StrategyFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create file paths for strategies.
		/// </summary>
		public StrategyFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new StrategyFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		private string strategiesDirectoryPath;
		/// <summary>
		/// Gets/sets the path to the directory containing the strategy files.
		/// </summary>
		public string StrategiesDirectoryPath
		{
			get {
				if (strategiesDirectoryPath == null || strategiesDirectoryPath == String.Empty)
				{
					if (FileNamer == null)
						throw new InvalidOperationException("FileNamer is not set.");
					
					strategiesDirectoryPath = FileNamer.StrategiesDirectoryPath;
				}
				return strategiesDirectoryPath; }
			set { strategiesDirectoryPath = value; }
		}
		
		public StrategyLoader()
		{
		}
		
		/// <summary>
		/// Loads all the strategies found in the strategies directory.
		/// </summary>
		/// <returns>An array of the the strategies found in the directory.</returns>
		public StrategyInfo[] LoadFromDirectory()
		{
			List<StrategyInfo> strategies = new List<StrategyInfo>();
			
			using (LogGroup logGroup = AppLogger.StartGroup("Loading the strategies from the XML files.", NLog.LogLevel.Debug))
			{
				foreach (string file in Directory.GetFiles(StrategiesDirectoryPath))
				{
					AppLogger.Debug("File: " + file);
					
					strategies.Add(LoadFromFile(file));
				}
			}
			
			return strategies.ToArray();
		}
		
		/// <summary>
		/// Loads the strategy from the specified path.
		/// </summary>
		/// <param name="strategyPath">The full path to the strategy to load.</param>
		/// <returns>The strategy deserialized from the specified file path.</returns>
		public StrategyInfo LoadFromFile(string strategyPath)
		{
			StrategyInfo info = null;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Loading the strategy from the specified path.", NLog.LogLevel.Debug))
			{
				if (!File.Exists(strategyPath))
					throw new ArgumentException("The specified file does not exist.");
				
				AppLogger.Debug("Path: " + strategyPath);
				
				
				using (StreamReader reader = new StreamReader(File.OpenRead(strategyPath)))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(StrategyInfo));
					
					info = (StrategyInfo)serializer.Deserialize(reader);
					
					reader.Close();
				}
			}
			
			return info;
		}
	}
}