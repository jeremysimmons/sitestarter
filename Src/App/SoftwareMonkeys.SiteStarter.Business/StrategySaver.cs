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
		/// Saves the provided strategies to file.
		/// </summary>
		/// <param name="strategies">An array of the strategies to save to file.</param>
		public void SaveToFile(StrategyInfo[] strategies)
		{
			// Logging disabled to boost performance
			//using (LogGroup logGroup = LogGroup.StartDebug("Saving the provided strategies to XML file."))
			//{
			string path = FileNamer.StrategiesInfoFilePath;
			
			//LogWriter.Debug("Path : " + path);
			
			if (!Directory.Exists(Path.GetDirectoryName(path)))
				Directory.CreateDirectory(Path.GetDirectoryName(path));
			
			using (StreamWriter writer = File.CreateText(path))
			{
				XmlSerializer serializer = new XmlSerializer(strategies.GetType());
				serializer.Serialize(writer, strategies);
				writer.Close();
			}
			//}
		}
	}
}
