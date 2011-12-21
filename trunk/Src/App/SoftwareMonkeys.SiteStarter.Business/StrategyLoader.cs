using System;
using System.IO;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Business.Security;
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
		
		public StrategyInfo[] Strategies;
		
		public StrategyLoader()
		{
		}
		
		/// <summary>
		/// Loads all the strategies found in the strategies directory.
		/// </summary>
		/// <returns>An array of the the strategies found in the directory.</returns>
		public StrategyInfo[] LoadInfoFromDirectory()
		{
			return LoadInfoFromDirectory(false);
		}
		
		/// <summary>
		/// Loads all the strategies found in the strategies directory.
		/// </summary>
		/// <param name="includeDisabled"></param>
		/// <returns>An array of the the strategies found in the directory.</returns>
		public StrategyInfo[] LoadInfoFromDirectory(bool includeDisabled)
		{
			// Logging disabled to boost performance
			//using (LogGroup logGroup = LogGroup.StartDebug("Loading the strategies from the XML file."))
			//{
			if (Strategies == null)
			{
				List<StrategyInfo> validStrategies = new List<StrategyInfo>();
				
				StrategyInfo[] strategies = new StrategyInfo[]{};
				
				using (StreamReader reader = new StreamReader(File.OpenRead(FileNamer.StrategiesInfoFilePath)))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(StrategyInfo[]));
					strategies = (StrategyInfo[])serializer.Deserialize(reader);
				}
				
				foreach (StrategyInfo strategy in strategies)
					if (strategy.Enabled || includeDisabled)
						validStrategies.Add(strategy);
				
				Strategies = validStrategies.ToArray();
			}
			//}
			return Strategies;
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
			
			Type type = typeof(StrategyInfo);
			
			string fileName  =Path.GetFileName(strategyPath);
			
			// If the info file name contains the '.ar' sub extension then it's an authorise reference strategy
			if (fileName.IndexOf(".ar.strategy") > -1)
				type = typeof(AuthoriseReferenceStrategyInfo);
			
			using (StreamReader reader = new StreamReader(File.OpenRead(strategyPath)))
			{
				XmlSerializer serializer = new XmlSerializer(type);
				
				info = (StrategyInfo)serializer.Deserialize(reader);
				
				reader.Close();
			}
			//}
			
			return info;
		}
	}
}
