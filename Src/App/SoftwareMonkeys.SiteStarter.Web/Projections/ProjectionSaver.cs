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
		/// <param name="extractor">A projection info extractor (used to extract cache info).</param>
		public void SaveToFile(string newFilePath, string content, ProjectionInfoExtractor extractor)
		{
			SaveToFile(String.Empty, newFilePath, content, extractor);
		}
		
		/// <summary>
		/// Saves the provided projection to the specified location.
		/// </summary>
		/// <param name="originalFilePath">The original path to the projection file.</param>
		/// <param name="newFilePath">The new path to the projection file.</param>
		/// <param name="content">The content of the projection file.</param>
		/// <param name="extractor">A projection info extractor (used to extract cache info).</param>
		public bool SaveToFile(string originalFilePath, string newFilePath, string content, ProjectionInfoExtractor extractor)
		{
			bool alreadyExists = false;

			using (LogGroup logGroup = LogGroup.StartDebug("Saving the provided projection to file."))
			{
				string fullOriginalFilePath = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath + "/" + originalFilePath);
				string fullNewFilePath = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath + "/" + newFilePath);
				
				LogWriter.Debug("Original path: " + fullOriginalFilePath);
				LogWriter.Debug("Path: " + fullNewFilePath);
				
				// If the original file path was specified and it exists then move the old file
				// to the new location
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
					
					SaveCache(originalFilePath, newFilePath, extractor);
				}
				else
					LogWriter.Debug("Projection name is in use. Skipping save.");
			}
			
			return alreadyExists;
		}
		
		/// <summary>
		/// Saves the provided projections info to file.
		/// </summary>
		/// <param name="projections">An array of the projections to save to file.</param>
		public void SaveInfoToFile(ProjectionInfo[] projections)
		{
			// Logging disabled to boost performance
			//using (LogGroup logGroup = LogGroup.StartDebug("Saving the provided projections to XML file."))
			//{
			string path = FileNamer.ProjectionsInfoFilePath;
			
			//LogWriter.Debug("Path : " + path);
			
			if (!Directory.Exists(Path.GetDirectoryName(path)))
				Directory.CreateDirectory(Path.GetDirectoryName(path));
			
			using (StreamWriter writer = File.CreateText(path))
			{
				XmlSerializer serializer = new XmlSerializer(projections.GetType());
				serializer.Serialize(writer, projections);
				writer.Close();
			}
			//}
		}
		
		/// <summary>
		/// Saves/updates the cache for the specified file.
		/// </summary>
		/// <param name="originalFilePath"></param>
		/// <param name="newFilePath"></param>
		/// <param name="extractor"></param>
		internal void SaveCache(string originalFilePath, string newFilePath, ProjectionInfoExtractor extractor)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Saving projection cache."))
			{
				string fullOriginalFilePath = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath + "/" + originalFilePath);
				string fullNewFilePath = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath + "/" + newFilePath);
				
				LogWriter.Debug("Original path: " + fullOriginalFilePath);
				LogWriter.Debug("Path: " + fullNewFilePath);
				
				// If the original file path was specified
				if (originalFilePath != String.Empty
				    && File.Exists(fullOriginalFilePath))
				{
					LogWriter.Debug("Removing old cache info.");
					
					ProjectionInfo[] oldInfos = extractor.ExtractProjectionInfo(fullOriginalFilePath);
					
					// Remove old state
					foreach (ProjectionInfo oldInfo in oldInfos)
					{
						ProjectionState.Projections.Remove(oldInfo);
					}
				}
				
				ProjectionInfo[] infos = extractor.ExtractProjectionInfo(fullNewFilePath);
				
				// Add the new info to state
				foreach (ProjectionInfo info in infos)
				{
					using (LogGroup logGroup3 = LogGroup.StartDebug("Adding cache info for: " + info.Name + " - " + info.ProjectionFilePath))
					{
						ProjectionState.Projections.Add(info);
					}
				}
				
				SaveInfoToFile(ProjectionState.Projections.ToArray());
			}
		}
	}
}
