using System;
using System.IO;
using System.Web;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	public class ProjectionMapper
	{
		private string applicationPath = String.Empty;
		public string ApplicationPath
		{
			get {
				if (applicationPath == String.Empty)
					applicationPath = HttpContext.Current.Request.ApplicationPath;
				return applicationPath; }
			set { applicationPath = value; }
		}
		
		public ProjectionMapper ()
		{
		}
		
		public string GetInternalPath(string originalPath)
		{
			string path = String.Empty;
			using (LogGroup logGroup = LogGroup.StartDebug("Generating the internal path."))
			{
				string shortPath = GetShortPath(originalPath);
				
				LogWriter.Debug("Short path: " + shortPath);
				
				string fileName = Path.GetFileName(shortPath);
				
				LogWriter.Debug("File name: " + fileName);
				
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
				
				LogWriter.Debug("File name without extension: " + fileNameWithoutExtension);
				
				string[] parts = fileNameWithoutExtension.Split('-');
				
				ProjectionFormat format = GetFormat(fileName);
				
				LogWriter.Debug("Format: "  + format.ToString());
				
				string projectionName = String.Empty;
				
				// If the projection name has a dash - in it then it's in the format of [Action]-[TypeName]
				if (parts.Length == 2)
				{
					string typeName = String.Empty;
					string action = String.Empty;
					
					if (EntityState.IsType(parts[0]))
					{
						typeName = parts[0];
						action = parts[1];
					}
					else if (EntityState.IsType(parts[1]))
					{
						typeName = parts[1];
						action = parts[0];
					}
					
					LogWriter.Debug("Action: " + action);
					LogWriter.Debug("Type name: " + typeName);
					
					if (ProjectionState.Projections.Contains(action, typeName))
					{
						projectionName = ProjectionState.Projections[action, typeName].Name;
						
						LogWriter.Debug("Found projection by action and type name.");
						
						path = String.Format("{0}/Projector.aspx?a={1}&t={2}&f={3}",
					                     ApplicationPath,
					                     HttpContext.Current.Server.UrlEncode(action),
					                     HttpContext.Current.Server.UrlEncode(typeName),
					                     format);
					}
				}
				// Otherwise it's the name of the projection
				else if (ProjectionState.Projections.Contains(fileNameWithoutExtension))
				{					
					LogWriter.Debug("Found projection by name");
					
					projectionName = HttpContext.Current.Server.UrlEncode(fileNameWithoutExtension);
					
					path = String.Format("{0}/Projector.aspx?n={1}&f={2}",
					                     ApplicationPath,
					                     HttpContext.Current.Server.UrlEncode(projectionName),
					                     format);
				}
				
				LogWriter.Debug("Projection name: " + projectionName);
				
				LogWriter.Debug("Path: " + path);
			}
			return path;
		}
		
		public string GetShortPath(string originalPath)
		{
			string shortPath = originalPath;
			
			if (originalPath.IndexOf("?") > -1)
				shortPath = originalPath.Substring(0, originalPath.IndexOf("?"));
			
			return shortPath;
		}
		
		public ProjectionFormat GetFormat(string fileName)
		{
			string ext = Path.GetExtension(fileName).Trim('.');
			
			if (ext == "aspx")
				return ProjectionFormat.Html;
			
			throw new NotSupportedException("Extension not yet supported: " + ext);
		}
	}
}

