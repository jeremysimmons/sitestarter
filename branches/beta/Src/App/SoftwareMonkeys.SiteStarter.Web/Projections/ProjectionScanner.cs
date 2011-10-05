using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Web.UI;
using System.Web;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	/// <summary>
	/// Used for scanning assemblies to look for usable business projections.
	/// </summary>
	public class ProjectionScanner : BaseProjectionScanner
	{
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
		
		private ControlLoader controlLoader;
		/// <summary>
		/// Gets/sets component used to load user controls.
		/// </summary>
		public ControlLoader ControlLoader
		{
			get {
				if (controlLoader == null)
				{
					if (Page != null)
						controlLoader = new ControlLoader(Page);
					else
						throw new InvalidOperationException("ControlLoader has not been initialized, and cannot initialize automatically because the Page property is null.");
				}
				return controlLoader; }
			set { controlLoader = value; }
		}
		
		public ProjectionScanner()
		{
		}
		
		public ProjectionScanner(ControlLoader controlLoader)
		{
			ControlLoader = controlLoader;
		}
		
		/// <summary>
		/// Finds all the projections in the available assemblies.
		/// </summary>
		/// <param name="page"></param>
		/// <returns>An array of info about the projections found.</returns>
		public override ProjectionInfo[] FindProjections()
		{
			List<ProjectionInfo> projections = new List<ProjectionInfo>();
			
			using (LogGroup logGroup = LogGroup.Start("Finding projections by scanning the attributes of the available type.", NLog.LogLevel.Debug))
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
		/// <param name="filePath">The full path to the projection (.ascx) file.</param>
		/// <returns></returns>
		public ProjectionInfo[] ExtractProjectionInfo(string filePath)
		{
			return new ProjectionInfoExtractor(ControlLoader).ExtractProjectionInfo(filePath);
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
			
			// File extension
			if (ext.ToLower() == ".ascx")
				return true;
			
			// File extension
			if (ext.ToLower() == ".xslt")
				return true;
			
			// File extension
			if (ext.ToLower() == ".html")
				return true;
			
			
			return false;
		}
		
	}
}
