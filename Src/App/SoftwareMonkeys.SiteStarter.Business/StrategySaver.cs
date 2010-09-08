using System;
using System.IO;
using SoftwareMonkeys.SiteStarter.Data;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to save strategy components to file.
	/// </summary>
	public class StrategySaver
	{
		private string strategiesDirectoryPath;
		/// <summary>
		/// Gets the full path to the directory containing strategy mappings.
		/// </summary>
		public string StrategiesDirectoryPath
		{
			get {
				if (strategiesDirectoryPath == null || strategiesDirectoryPath == String.Empty)
				{
					if (FileNamer != null)
						strategiesDirectoryPath = FileNamer.StrategiesDirectoryPath;
					
				}
				return strategiesDirectoryPath;
			}
			set { strategiesDirectoryPath = value; }
		}
		
		private StrategyFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create file names/paths.
		/// </summary>
		public StrategyFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new StrategyFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		public StrategySaver()
		{
		}
		
		/// <summary>
		/// Saves the provided strategy to the strategies mappings directory.
		/// </summary>
		/// <param name="strategy">The strategy to save to file.</param>
		public void SaveToFile(StrategyInfo strategy)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Saving the provided strategy to file.", NLog.LogLevel.Debug))
			{
				string path = FileNamer.CreateFilePath(strategy);
				
				AppLogger.Debug("Path : " + path);
				
				if (!Directory.Exists(Path.GetDirectoryName(path)))
					Directory.CreateDirectory(Path.GetDirectoryName(path));
				
				using (StreamWriter writer = File.CreateText(path))
				{
					XmlSerializer serializer = new XmlSerializer(strategy.GetType());
					serializer.Serialize(writer, strategy);
					writer.Close();
				}
			}
		}
		
		/// <summary>
		/// Saves the provided strategies to file.
		/// </summary>
		/// <param name="strategies">An array of the strategies to save to file.</param>
		public void SaveToFile(StrategyInfo[] strategies)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Saving the provided strategies to XML files.", NLog.LogLevel.Debug))
			{
				foreach (StrategyInfo strategy in strategies)
				{
					SaveToFile(strategy);
				}
			}
		}
	}
}
