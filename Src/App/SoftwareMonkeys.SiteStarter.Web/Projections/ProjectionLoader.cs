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
		
		protected ProjectionInfo[] Projections;
		
		public ProjectionLoader()
		{
		}
		
		/// <summary>
		/// Loads all the enabled projections found in the projections file.
		/// </summary>
		/// <returns>An array of the the projections found.</returns>
		public ProjectionInfo[] LoadInfoFromFile()
		{
			return LoadInfoFromFile(false);
		}
		
		/// <summary>
		/// Loads all the projections found in the projections file.
		/// </summary>
		/// <param name="includeDisabled">A value indicating whether or not to include disabled projections.</param>
		/// <returns>An array of the the projections found.</returns>
		public ProjectionInfo[] LoadInfoFromFile(bool includeDisabled)
		{
			// Logging disabled to boost performance
			//using (LogGroup logGroup = LogGroup.StartDebug("Loading the projections from the XML file."))
			//{
			if (Projections == null)
			{
				List<ProjectionInfo> validProjections = new List<ProjectionInfo>();
				
				ProjectionInfo[] projections = new ProjectionInfo[]{};
				
				using (StreamReader reader = new StreamReader(File.OpenRead(FileNamer.ProjectionsInfoFilePath)))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(ProjectionInfo[]));
					projections = (ProjectionInfo[])serializer.Deserialize(reader);
				}
				
				foreach (ProjectionInfo projection in projections)
					if (projection.Enabled || includeDisabled)
						validProjections.Add(projection);
				
				Projections = validProjections.ToArray();
			}
			//}
			return Projections;
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
