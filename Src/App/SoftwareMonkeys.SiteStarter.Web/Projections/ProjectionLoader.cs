using System;
using System.IO;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Xml;

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
			
			// Logging disabled to boost performance
			//using (LogGroup logGroup = LogGroup.Start("Loading the projections from the XML files.", NLog.LogLevel.Debug))
			//{
				foreach (string file in Directory.GetFiles(ProjectionsDirectoryPath))
				{
			//		LogWriter.Debug("File: " + file);
					
					projections.Add(LoadInfoFromFile(file));
				}
			//}
			
			return projections.ToArray();
		}
		
		/// <summary>
		/// Loads all the enabled projections found in the projections directory.
		/// </summary>
		/// <returns>An array of the the projections found in the directory.</returns>
		public ProjectionInfo[] LoadInfoFromDirectory()
		{
			return LoadInfoFromDirectory(false);
		}
		
		/// <summary>
		/// Loads all the projections found in the projections directory.
		/// </summary>
		/// <param name="includeDisabled">A value indicating whether or not to include disabled projections.</param>
		/// <returns>An array of the the projections found in the directory.</returns>
		public ProjectionInfo[] LoadInfoFromDirectory(bool includeDisabled)
		{
			List<ProjectionInfo> projections = new List<ProjectionInfo>();
			
			// Logging disabled to boost performance
			//using (LogGroup logGroup = LogGroup.Start("Loading the projections info from the XML files.", NLog.LogLevel.Debug))
			//{
				foreach (string file in Directory.GetFiles(ProjectionsInfoDirectoryPath))
				{
			//		LogWriter.Debug("File: " + file);
					
					ProjectionInfo projection = LoadInfoFromFile(file);
					if (projection.Enabled || includeDisabled)
						projections.Add(projection);
				}
			//}
			
			return projections.ToArray();
		}
		
		/// <summary>
		/// Loads the projection info from the specified path.
		/// </summary>
		/// <param name="projectionPath">The full path to the projection to load.</param>
		/// <returns>The projection deserialized from the specified file path.</returns>
		public ProjectionInfo LoadInfoFromFile(string projectionPath)
		{
			ProjectionInfo info = null;
			
			// Disabled logging to boost performance
			//using (LogGroup logGroup = LogGroup.Start("Loading the projection from the specified path.", NLog.LogLevel.Debug))
			//{
				if (!File.Exists(projectionPath))
					throw new ArgumentException("The specified file does not exist.");
				
			//	LogWriter.Debug("Path: " + projectionPath);
				
				
				using (StreamReader reader = new StreamReader(File.OpenRead(projectionPath)))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(ProjectionInfo));
					
					try
					{
						info = (ProjectionInfo)serializer.Deserialize(reader);
					}
					catch (XmlException ex)
					{
						throw new Exception("Can't load projection at '" + projectionPath + "'.", ex);
					}
					
					reader.Close();
				}
			//}
			
			return info;
		}
		
		/// <summary>
		/// Loads the projection content from the specified path.
		/// </summary>
		/// <param name="projectionPath">The full path to the projection to load.</param>
		/// <returns>The projection content from the specified file path.</returns>
		public string LoadContentFromFile(string projectionPath)
		{
			string content = String.Empty;
			
			// Disabled logging to boost performance
			//using (LogGroup logGroup = LogGroup.Start("Loading the projection from the specified path.", NLog.LogLevel.Debug))
			//{
				if (!File.Exists(projectionPath))
					throw new ArgumentException("The specified file does not exist: " + projectionPath);
				
			//	LogWriter.Debug("Path: " + projectionPath);
				
				
				using (StreamReader reader = new StreamReader(File.OpenRead(projectionPath)))
				{
					content = reader.ReadToEnd();
					reader.Close();
				}
			//}
			
			return content;
		}
	}
}
