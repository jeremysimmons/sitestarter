using System;
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
					
					strategiesDirectoryPath = FileNamer.StrategiesInfoDirectoryPath;
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
			
			// Logging disabled to boost performance
			//using (LogGroup logGroup = LogGroup.StartDebug("Loading the strategies from the XML files."))
			//{
				foreach (string file in Directory.GetFiles(StrategiesDirectoryPath))
				{
			//		LogWriter.Debug("File: " + file);
					
					strategies.Add(LoadFromFile(file));
				}
			//}
			
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
			
			// Logging disabled to boost performance
			//using (LogGroup logGroup = LogGroup.StartDebug("Loading the strategy from the specified path."))
			//{
				if (!File.Exists(strategyPath))
					throw new ArgumentException("The specified file does not exist.");
				
			//	LogWriter.Debug("Path: " + strategyPath);
				
				
				using (StreamReader reader = new StreamReader(File.OpenRead(strategyPath)))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(StrategyInfo));
					
					info = (StrategyInfo)serializer.Deserialize(reader);
					
					reader.Close();
				}
			//}
			
			return info;
		}
	}
}
