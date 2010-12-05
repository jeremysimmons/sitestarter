using System;
using System.IO;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Parts
{
	/// <summary>
	/// Used to load projections (a type of user control) for display on a page.
	/// </summary>
	public class PartLoader
	{
		private PartFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create file paths for projections.
		/// </summary>
		public PartFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new PartFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		private string projectionsDirectoryPath;
		/// <summary>
		/// Gets/sets the path to the directory containing the projection files.
		/// </summary>
		public string PartsDirectoryPath
		{
			get {
				if (projectionsDirectoryPath == null || projectionsDirectoryPath == String.Empty)
				{
					if (FileNamer == null)
						throw new InvalidOperationException("FileNamer is not set.");
					
					projectionsDirectoryPath = FileNamer.PartsDirectoryPath;
				}
				return projectionsDirectoryPath; }
			set { projectionsDirectoryPath = value; }
		}
		
		private string projectionsInfoDirectoryPath;
		/// <summary>
		/// Gets/sets the path to the directory containing the projection info files.
		/// </summary>
		public string PartsInfoDirectoryPath
		{
			get {
				if (projectionsInfoDirectoryPath == null || projectionsInfoDirectoryPath == String.Empty)
				{
					if (FileNamer == null)
						throw new InvalidOperationException("FileNamer is not set.");
					
					projectionsInfoDirectoryPath = FileNamer.PartsInfoDirectoryPath;
				}
				return projectionsInfoDirectoryPath; }
			set { projectionsInfoDirectoryPath = value; }
		}
		
		public PartLoader()
		{
		}
		
		/// <summary>
		/// Loads all the projections found in the projections directory.
		/// </summary>
		/// <returns>An array of the the projections found in the directory.</returns>
		public PartInfo[] LoadFromDirectory()
		{
			List<PartInfo> projections = new List<PartInfo>();
			
			using (LogGroup logGroup = AppLogger.StartGroup("Loading the projections from the XML files.", NLog.LogLevel.Debug))
			{
				foreach (string file in Directory.GetFiles(PartsDirectoryPath))
				{
					AppLogger.Debug("File: " + file);
					
					projections.Add(LoadFromFile(file));
				}
			}
			
			return projections.ToArray();
		}
		
		/// <summary>
		/// Loads all the projections found in the projections directory.
		/// </summary>
		/// <returns>An array of the the projections found in the directory.</returns>
		public PartInfo[] LoadInfoFromDirectory()
		{
			List<PartInfo> projections = new List<PartInfo>();
			
			using (LogGroup logGroup = AppLogger.StartGroup("Loading the projections info from the XML files.", NLog.LogLevel.Debug))
			{
				foreach (string file in Directory.GetFiles(PartsInfoDirectoryPath))
				{
					AppLogger.Debug("File: " + file);
					
					projections.Add(LoadFromFile(file));
				}
			}
			
			return projections.ToArray();
		}
		
		/// <summary>
		/// Loads the projection from the specified path.
		/// </summary>
		/// <param name="projectionPath">The full path to the projection to load.</param>
		/// <returns>The projection deserialized from the specified file path.</returns>
		public PartInfo LoadFromFile(string projectionPath)
		{
			PartInfo info = null;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Loading the projection from the specified path.", NLog.LogLevel.Debug))
			{
				if (!File.Exists(projectionPath))
					throw new ArgumentException("The specified file does not exist.");
				
				AppLogger.Debug("Path: " + projectionPath);
				
				
				using (StreamReader reader = new StreamReader(File.OpenRead(projectionPath)))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(PartInfo));
					
					info = (PartInfo)serializer.Deserialize(reader);
					
					reader.Close();
				}
			}
			
			return info;
		}
	}
}
