using System;
using System.IO;
using System.Web;
using SoftwareMonkeys.SiteStarter.Data;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	/// <summary>
	/// Used to save projection components to file.
	/// </summary>
	public class ProjectionSaver
	{
		private string projectionsDirectoryPath;
		/// <summary>
		/// Gets the full path to the directory containing projection mappings.
		/// </summary>
		public string ProjectionsDirectoryPath
		{
			get {
				if (projectionsDirectoryPath == null || projectionsDirectoryPath == String.Empty)
				{
					if (FileNamer != null)
						projectionsDirectoryPath = FileNamer.ProjectionsDirectoryPath;
					
				}
				return projectionsDirectoryPath;
			}
			set { projectionsDirectoryPath = value; }
		}
		
		private ProjectionFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create file names/paths.
		/// </summary>
		public ProjectionFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new ProjectionFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		public ProjectionSaver()
		{
		}
		
		/// <summary>
		/// Saves the provided projection to the specified location.
		/// </summary>
		/// <param name="newFilePath">The new path to the projection file.</param>
		/// <param name="content">The content of the projection file.</param>
		public void SaveToFile(string newFilePath, string content)
		{
			SaveToFile(String.Empty, newFilePath, content);
		}
		
		/// <summary>
		/// Saves the provided projection to the specified location.
		/// </summary>
		/// <param name="originalFilePath">The original path to the projection file.</param>
		/// <param name="newFilePath">The new path to the projection file.</param>
		/// <param name="content">The content of the projection file.</param>
		public bool SaveToFile(string originalFilePath, string newFilePath, string content)
		{
			bool alreadyExists = false;

			using (LogGroup logGroup = LogGroup.StartDebug("Saving the provided projection to file."))
			{
				string fullOriginalFilePath = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath + "/" + originalFilePath);
				string fullNewFilePath = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath + "/" + newFilePath);
				
				LogWriter.Debug("Original path: " + fullOriginalFilePath);
				LogWriter.Debug("Path: " + fullNewFilePath);
				
				// If the original file path was specified
				if (originalFilePath != String.Empty
				    && File.Exists(fullOriginalFilePath))
				{
					File.Move(fullOriginalFilePath, fullNewFilePath);
					
					LogWriter.Debug("Moving file.");
				}
				else
				{
					// If the projection already exists but is not being edited (ie. no original file is specified)
					if (File.Exists(fullNewFilePath))
					{
						alreadyExists = true;
						
						LogWriter.Debug("Already exists: " + fullNewFilePath);
					}
				}
				
				LogWriter.Debug("Path : " + fullNewFilePath);
				
				if (!alreadyExists)
				{
					if (!Directory.Exists(Path.GetDirectoryName(fullNewFilePath)))
						Directory.CreateDirectory(Path.GetDirectoryName(fullNewFilePath));
					
					LogWriter.Debug("Saved projection to file.");
					
					using (StreamWriter writer = File.CreateText(fullNewFilePath))
					{
						writer.Write(content);
						writer.Close();
					}
				}
				else
					LogWriter.Debug("Projection name is in use. Skipping save.");
			}
			
			return alreadyExists;
		}
		
		
		/// <summary>
		/// Saves the provided projection info to the projections info directory.
		/// </summary>
		/// <param name="projection">The projection info to save to file.</param>
		public void SaveInfoToFile(ProjectionInfo projection)
		{
			// Logging disabled to boost performance
			//using (LogGroup logGroup = LogGroup.Start("Saving the provided projection to file.", NLog.LogLevel.Debug))
			//{
			string path = FileNamer.CreateInfoFilePath(projection);
			
			//LogWriter.Debug("Path : " + path);
			
			if (!Directory.Exists(Path.GetDirectoryName(path)))
				Directory.CreateDirectory(Path.GetDirectoryName(path));
			
			using (StreamWriter writer = File.CreateText(path))
			{
				XmlSerializer serializer = new XmlSerializer(projection.GetType());
				serializer.Serialize(writer, projection);
				writer.Close();
			}
			//}
		}
		
		/// <summary>
		/// Saves the provided projections info to file.
		/// </summary>
		/// <param name="projections">An array of the projections to save to file.</param>
		public void SaveInfoToFile(ProjectionInfo[] projections)
		{
			// Logging disabled to boost performance
			//using (LogGroup logGroup = LogGroup.Start("Saving the provided projections to XML files.", NLog.LogLevel.Debug))
			//{
			foreach (ProjectionInfo projection in projections)
			{
				SaveInfoToFile(projection);
			}
			//}
		}
	}
}
