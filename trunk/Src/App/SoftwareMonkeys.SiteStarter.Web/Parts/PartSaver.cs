using System;
using System.IO;
using SoftwareMonkeys.SiteStarter.Data;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Parts
{
	/// <summary>
	/// Used to save projection components to file.
	/// </summary>
	public class PartSaver
	{
		private string projectionsDirectoryPath;
		/// <summary>
		/// Gets the full path to the directory containing projection mappings.
		/// </summary>
		public string PartsDirectoryPath
		{
			get {
				if (projectionsDirectoryPath == null || projectionsDirectoryPath == String.Empty)
				{
					if (FileNamer != null)
						projectionsDirectoryPath = FileNamer.PartsDirectoryPath;
					
				}
				return projectionsDirectoryPath;
			}
			set { projectionsDirectoryPath = value; }
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
		/// Saves the provided projection info to the projections info directory.
		/// </summary>
		/// <param name="projection">The projection info to save to file.</param>
		public void SaveInfoToFile(PartInfo projection)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Saving the provided projection to file.", NLog.LogLevel.Debug))
			{
				string path = FileNamer.CreateInfoFilePath(projection);
				
				AppLogger.Debug("Path : " + path);
				
				if (!Directory.Exists(Path.GetDirectoryName(path)))
					Directory.CreateDirectory(Path.GetDirectoryName(path));
				
				using (StreamWriter writer = File.CreateText(path))
				{
					XmlSerializer serializer = new XmlSerializer(projection.GetType());
					serializer.Serialize(writer, projection);
					writer.Close();
				}
			}
		}
		
		/// <summary>
		/// Saves the provided projections info to file.
		/// </summary>
		/// <param name="projections">An array of the projections to save to file.</param>
		public void SaveInfoToFile(PartInfo[] projections)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Saving the provided projections to XML files.", NLog.LogLevel.Debug))
			{
				foreach (PartInfo projection in projections)
				{
					SaveInfoToFile(projection);
				}
			}
		}
	}
}
