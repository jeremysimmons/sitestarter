using System;
using System.IO;
using SoftwareMonkeys.SiteStarter.Data;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Parts
{
	/// <summary>
	/// Used to save part components to file.
	/// </summary>
	public class PartSaver
	{
		private string partsDirectoryPath;
		/// <summary>
		/// Gets the full path to the directory containing part mappings.
		/// </summary>
		public string PartsDirectoryPath
		{
			get {
				if (partsDirectoryPath == null || partsDirectoryPath == String.Empty)
				{
					if (FileNamer != null)
						partsDirectoryPath = FileNamer.PartsDirectoryPath;
					
				}
				return partsDirectoryPath;
			}
			set { partsDirectoryPath = value; }
		}
		
		private PartFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create file names/paths.
		/// </summary>
		public PartFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new PartFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		public PartSaver()
		{
		}
		
		/// <summary>
		/// Saves the provided part info to the parts info directory.
		/// </summary>
		/// <param name="part">The part info to save to file.</param>
		public void SaveInfoToFile(PartInfo part)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Saving the provided part to file.", NLog.LogLevel.Debug))
			{
				string path = FileNamer.CreateInfoFilePath(part);
				
				AppLogger.Debug("Path : " + path);
				
				if (!Directory.Exists(Path.GetDirectoryName(path)))
					Directory.CreateDirectory(Path.GetDirectoryName(path));
				
				using (StreamWriter writer = File.CreateText(path))
				{
					XmlSerializer serializer = new XmlSerializer(part.GetType());
					serializer.Serialize(writer, part);
					writer.Close();
				}
			}
		}
		
		/// <summary>
		/// Saves the provided parts info to file.
		/// </summary>
		/// <param name="parts">An array of the parts to save to file.</param>
		public void SaveInfoToFile(PartInfo[] parts)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Saving the provided parts to XML files.", NLog.LogLevel.Debug))
			{
				foreach (PartInfo part in parts)
				{
					SaveInfoToFile(part);
				}
			}
		}
	}
}
