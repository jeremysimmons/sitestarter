using System;
using System.IO;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	/// <summary>
	/// Used to load projections (a type of user control) for display on a page.
	/// </summary>
	public class ProjectionLoader
	{
		private ProjectionFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create file paths for projections.
		/// </summary>
		public ProjectionFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new ProjectionFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		private string projectionsDirectoryPath;
		/// <summary>
		/// Gets/sets the path to the directory containing the projection files.
		/// </summary>
		public string ProjectionsDirectoryPath
		{
			get {
				if (projectionsDirectoryPath == null || projectionsDirectoryPath == String.Empty)
				{
					if (FileNamer == null)
						throw new InvalidOperationException("FileNamer is not set.");
					
					projectionsDirectoryPath = FileNamer.ProjectionsDirectoryPath;
				}
				return projectionsDirectoryPath; }
			set { projectionsDirectoryPath = value; }
		}
		
		private string projectionsInfoDirectoryPath;
		/// <summary>
		/// Gets/sets the path to the directory containing the projection info files.
		/// </summary>
		public string ProjectionsInfoDirectoryPath
		{
			get {
				if (projectionsInfoDirectoryPath == null || projectionsInfoDirectoryPath == String.Empty)
				{
					if (FileNamer == null)
						throw new InvalidOperationException("FileNamer is not set.");
					
					projectionsInfoDirectoryPath = FileNamer.ProjectionsInfoDirectoryPath;
				}
				return projectionsInfoDirectoryPath; }
			set { projectionsInfoDirectoryPath = value; }
		}
		
		public ProjectionLoader()
		{
		}
		
		/// <summary>
		/// Loads all the projections found in the projections directory.
		/// </summary>
		/// <returns>An array of the the projections found in the directory.</returns>
		public ProjectionInfo[] LoadFromDirectory()
		{
			List<ProjectionInfo> projections = new List<ProjectionInfo>();
			
			using (LogGroup logGroup = AppLogger.StartGroup("Loading the projections from the XML files.", NLog.LogLevel.Debug))
			{
				foreach (string file in Directory.GetFiles(ProjectionsDirectoryPath))
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
		public ProjectionInfo[] LoadInfoFromDirectory()
		{
			List<ProjectionInfo> projections = new List<ProjectionInfo>();
			
			using (LogGroup logGroup = AppLogger.StartGroup("Loading the projections info from the XML files.", NLog.LogLevel.Debug))
			{
				foreach (string file in Directory.GetFiles(ProjectionsInfoDirectoryPath))
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
		public ProjectionInfo LoadFromFile(string projectionPath)
		{
			ProjectionInfo info = null;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Loading the projection from the specified path.", NLog.LogLevel.Debug))
			{
				if (!File.Exists(projectionPath))
					throw new ArgumentException("The specified file does not exist.");
				
				AppLogger.Debug("Path: " + projectionPath);
				
				
				using (StreamReader reader = new StreamReader(File.OpenRead(projectionPath)))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(ProjectionInfo));
					
					info = (ProjectionInfo)serializer.Deserialize(reader);
					
					reader.Close();
				}
			}
			
			return info;
		}
	}
}
