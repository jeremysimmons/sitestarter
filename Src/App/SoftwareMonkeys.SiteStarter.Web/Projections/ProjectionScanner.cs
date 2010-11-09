﻿using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	/// <summary>
	/// Used for scanning assemblies to look for usable business projections.
	/// </summary>
	public class ProjectionScanner : BaseProjectionScanner
	{
		// TODO: Tidy up
		
		/*private string projectionsDirectoryName;
		/// <summary>
		/// Gets/sets the name of the directory containing the projections.
		/// </summary>
		public string ProjectionsDirectoryName
		{
			get { return projectionsDirectoryName; }
			set { projectionsDirectoryName = value; }
		}
		
		/// <summary>
		/// Gets/sets the full path of the directory containing the projections.
		/// </summary>
		public string ProjectionsDirectoryPath
		{
			get { return projectionsDirectoryName; }
			set { projectionsDirectoryName = value; }
		}*/
		
		private ProjectionFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used for generating projection file names and paths.
		/// </summary>
		public ProjectionFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new ProjectionFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		public ProjectionScanner()
		{
		}
		
		/// <summary>
		/// Finds all the projections in the available assemblies.
		/// </summary>
		/// <returns>An array of info about the projections found.</returns>
		public override ProjectionInfo[] FindProjections()
		{
			List<ProjectionInfo> projections = new List<ProjectionInfo>();
			
			using (LogGroup logGroup = AppLogger.StartGroup("Finding projections by scanning the attributes of the available type.", NLog.LogLevel.Debug))
			{
				foreach (string file in Directory.GetFiles(FileNamer.ProjectionsDirectoryPath))
				{
					if (IsProjection(file))
					{
						foreach (ProjectionInfo info in ExtractProjectionInfo(file))
						{
							projections.Add(info);
						}
					}
				}
			}
			
			return projections.ToArray();
		}
		
		/// <summary>
		/// Extracts the projection infos from the provided file path.
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public ProjectionInfo[] ExtractProjectionInfo(string filePath)
		{
			string shortName = Path.GetFileNameWithoutExtension(filePath);
			string extension = Path.GetExtension(filePath).Trim('.');
			
			string[] actions = ExtractActions(shortName);
			
			string typeName = ExtractType(shortName);
			
			List<ProjectionInfo> projections = new List<ProjectionInfo>();
			
			string relativeFilePath = filePath.Replace(Configuration.Config.Application.PhysicalApplicationPath, "")
				.Replace(@"\", "/")
				.Trim('/');
			
			
			foreach (string action in actions)
			{
				ProjectionInfo info = new ProjectionInfo();
				info.Action = action;
				info.TypeName = typeName;
				info.ProjectionFilePath = relativeFilePath;
				info.Format = GetFormatFromFileName(filePath);
				//info.Format = ConvertSubExtensionToFormat(subExtension);
				
				projections.Add(info);
			}
			
			return projections.ToArray();
		}
		
		/// <summary>
		/// Retrieves the format of the projection with the provided file name.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public ProjectionFormat GetFormatFromFileName(string fileName)
		{
			string shortFileName = Path.GetFileName(fileName);
			string extension = shortFileName.Substring
				(
					shortFileName.IndexOf('.'),
					shortFileName.Length - shortFileName.IndexOf('.')
				);
			
			extension = extension.Trim('.');
			
			string subExtension = extension;
			
			if (extension.IndexOf(".") > -1)
			{
				subExtension = extension.Substring(0, extension.IndexOf("."));
			}
			
			return ConvertSubExtensionToFormat(subExtension);
		}
		
		/// <summary>
		/// Converts a sub extension such as 'xml' from '*.xml.ascx' into a projection format such as ProjectionFormat.Xml.
		/// </summary>
		/// <param name="subExtension"></param>
		/// <returns></returns>
		private ProjectionFormat ConvertSubExtensionToFormat(string subExtension)
		{
			switch (subExtension.Trim('.').ToLower())
			{
				case "xml":
					return ProjectionFormat.Xml;
				case "xslt":
					return ProjectionFormat.Xslt;
				case "html":
				default:
					return ProjectionFormat.Html;
			}
		}
		
		/// <summary>
		/// Extracts the actions from the provide file name.
		/// </summary>
		/// <param name="shortFileName">The name of the file to extract the actions from.</param>
		/// <returns>The actions extracted from the file name.</returns>
		private string[] ExtractActions(string shortFileName)
		{
			if (shortFileName.IndexOf(".") > -1 || shortFileName.IndexOf(":") > -1)
				shortFileName = Path.GetFileNameWithoutExtension(shortFileName);
			
			string[] parts = shortFileName.Split('-');
			
			if (parts.Length <= 1)
				throw new ArgumentException("The provided short file name is invalid: " + shortFileName);
			
			List<string> actions = new List<string>();
			
			for (int i = 1; i < parts.Length; i++)
			{
				actions.Add(parts[i]);
			}
			
			return actions.ToArray();
		}
		
		/// <summary>
		/// Extracts the type name from the provided file name.
		/// </summary>
		/// <param name="shortFileName">The name of the file to extract the name from.</param>
		/// <returns>The action extracted from the file name.</returns>
		private string ExtractType(string shortFileName)
		{
			if (shortFileName.IndexOf(".") > -1 || shortFileName.IndexOf(":") > -1)
				shortFileName = Path.GetFileNameWithoutExtension(shortFileName);
			
			string[] parts = shortFileName.Split('-');
			
			if (parts.Length <= 1)
				throw new ArgumentException("The provided short file name is invalid: " + shortFileName);
			
			return parts[0];
		}
		
		/// <summary>
		/// Checks whether the file at the specified location is a projection file.
		/// </summary>
		/// <param name="fileName">The full name and path of the file to check.</param>
		/// <returns></returns>
		public bool IsProjection(string fileName)
		{
			string ext = Path.GetExtension(fileName);
			string shortFileName = Path.GetFileNameWithoutExtension(fileName);
			
			// - in file name
			if (shortFileName.IndexOf('-') == -1)
				return false;
			
			// File extension
			if (ext.ToLower() == ".ascx")
				return true;
			
			// File extension
			if (ext.ToLower() == ".xslt")
				return true;
			
			// File extension
			if (ext.ToLower() == ".html")
				return true;
			
			
			return true;
		}
	}
}
