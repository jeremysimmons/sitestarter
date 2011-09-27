using System;
using System.IO;
using System.Web;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.IO;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	public class ProjectionMapper
	{
		private string applicationPath = String.Empty;
		public string ApplicationPath
		{
			get {
				if (applicationPath == String.Empty)
					applicationPath = StateAccess.State.ApplicationPath;
				return applicationPath; }
			set { applicationPath = value; }
		}
		
		private UrlConverter converter;
		public UrlConverter Converter
		{
			get {
				if (converter == null)
					converter = new UrlConverter(ApplicationPath);
				return converter; }
			set { converter = value; }
			
		}
		
		private FileExistenceChecker fileExistenceChecker;
		public FileExistenceChecker FileExistenceChecker
		{
			get {
				if (fileExistenceChecker == null)
					fileExistenceChecker = new FileExistenceChecker();
				return fileExistenceChecker; }
			set { fileExistenceChecker = value; }
		}
		
		private IFileMapper fileMapper;
		public IFileMapper FileMapper
		{
			get {
				if (fileMapper == null)
					fileMapper = new FileMapper(ApplicationPath);
				return fileMapper; }
			set { fileMapper = value; }
		}
		
		public ProjectionMapper ()
		{
		}
		
		public string GetInternalPath(string originalPath)
		{
			string path = String.Empty;
			using (LogGroup logGroup = LogGroup.StartDebug("Generating the internal path."))
			{
				if (!Skip(originalPath))
				{
					string shortPath = GetShortPath(originalPath);
					
					LogWriter.Debug("Short path: " + shortPath);
					
					string fileName = Path.GetFileName(shortPath);
					
					LogWriter.Debug("File name: " + fileName);
					
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
					
					LogWriter.Debug("File name without extension: " + fileNameWithoutExtension);
					
					string command = GetCommandName(originalPath);
					
					LogWriter.Debug("Command name: " + command);
					
					ProjectionFormat format = GetFormat(fileName);
					
					LogWriter.Debug("Format: "  + format.ToString());
					
					// If the command name has a dash - in it then it's in the format of [Action]-[TypeName]
					if (command.IndexOf("-") > -1)
					{
						path = GetInternalPathFromCommand(originalPath, command, format);
					}
					// Otherwise it's the name of the projection
					else if (ProjectionState.Projections.Contains(command))
					{
						string projectionName = command;
						
						path = GetInternalPathFromName(originalPath, projectionName, format);
					}
					
					
					
					LogWriter.Debug("Path: " + path);
				}
				else
					LogWriter.Debug("Skipping rewrite.");
			}
			return path;
		}
		
		public string GetAction(string command)
		{
			if (command.IndexOf('-') > -1)
				return new CommandInfo(command).Action;
			
			return String.Empty;
		}
		
		public string GetTypeName(string command)
		{
			if (command.IndexOf('-') > -1)
				return new CommandInfo(command).TypeName;
			
			return String.Empty;
		}
		
		public string GetInternalPathFromName(string originalPath, string projectionName, ProjectionFormat format)
		{
			string newPath = String.Empty;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving the internal path corresponding with original path '" + originalPath + "' and projection name '" + projectionName + "'."))
			{
				LogWriter.Debug("Found projection by name");
				
				newPath = String.Format("{0}/{1}?n={2}&f={3}",
				                        ApplicationPath,
					                    GetRealPageName(originalPath),
				                        UrlEncode(projectionName),
				                        format);
				
				newPath = AddQueryStrings(originalPath, newPath);
				
				LogWriter.Debug("Internal path: " + newPath);
			}
			return newPath;
		}
		
		public string GetInternalPathFromCommand(string originalPath, string command, ProjectionFormat format)
		{
			string internalPath = String.Empty;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving the internal path corresponding with original path '" + originalPath + "' and command '" + command + "'."))
			{
				string[] parts = command.Split('-');
				
				if (parts.Length == 2)
				{
					string typeName = String.Empty;
					string action = String.Empty;
					
					if (EntityState.IsType(parts[0]))
					{
						LogWriter.Debug("First file name part matches an entity.");
						
						typeName = parts[0];
						action = parts[1];
					}
					else if (EntityState.IsType(parts[1]))
					{
						LogWriter.Debug("Second file name part matches an entity.");
						
						typeName = parts[1];
						action = parts[0];
					}
					else
						throw new Exception("Cannot find a type '" + parts[0] + "' or '" + parts[1] + "'.");
					
					internalPath = GetInternalPathFromActionAndType(originalPath, action, typeName, format);
				}
				else
					internalPath = GetInternalPathFromName(originalPath, command, format);
				
				LogWriter.Debug("Internal path: " + internalPath);
			}
			return internalPath;
		}
		
		
		public string GetInternalPathFromActionAndType(string originalPath, string action, string typeName, ProjectionFormat format)
		{
			string newPath = string.Empty;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving the internal path corresponding with original path '" + originalPath + "', action '" + action + "', and type '" + typeName + "'."))
			{
				
				LogWriter.Debug("Action: " + action);
				LogWriter.Debug("Type name: " + typeName);
				
				string shortPath = GetShortPath(originalPath);
				
				// If a projection is found with the action and type name
				if (ProjectionState.Projections.Contains(action, typeName))
				{
					string projectionName = ProjectionState.Projections[action, typeName].Name;
					
					LogWriter.Debug("Found projection by action and type name.");
					
					newPath = String.Format("{0}/{1}?a={2}&t={3}&f={4}",
					                        ApplicationPath,
					                        GetRealPageName(originalPath),
					                        UrlEncode(action),
					                        UrlEncode(typeName),
					                        format);
					
					newPath = AddQueryStrings(originalPath, newPath, action, typeName);
				}
				else
					LogWriter.Debug("No projection found with action '" + action + "' and type '" + typeName + "'.");
				
				LogWriter.Debug("New path: " + newPath);
				
			}
			return newPath;
		}
		
		public Guid GetDynamicID(string originalPath)
		{
			Guid id = Guid.Empty;
			
			string relativePath = Converter.ToRelative(originalPath);
			
			string[] parts = relativePath.Trim('/').Split('/');
			
			if (parts.Length >= 2)
			{
				string idString = parts[1];
				
				if (GuidValidator.IsValidGuid(idString))
				{
					id = GuidValidator.ParseGuid(idString);
				}
			}
			return id;
		}
		
		/// <summary>
		/// Extracts the command name from the provided URL path. Either a combination of [Action]-[Type] or just [ProjectionName].
		/// </summary>
		/// <param name="originalPath"></param>
		/// <returns></returns>
		public string GetCommandName(string originalPath)
		{
			string commandName = String.Empty;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving command name from path: " + originalPath))
			{
				string path = Converter.ToRelative(originalPath);
				
				LogWriter.Debug("Relative path: " + path);
				
				path = path.TrimStart('/');
				
				string x = path;
				
				// Remove the query strings
				if (x.IndexOf('?') > -1)
				{
					x = x.Substring(0, path.IndexOf('?'));
					
					LogWriter.Debug("x (after removing query strings): " + x);
				}
				
				// Get the first section
				if (x.IndexOf('/') > -1)
				{
					x = path.Substring(0, path.IndexOf('/'));
					
					LogWriter.Debug("x (after shortening): " + x);
				}
				
				// Remove the extension if there is one
				x = Path.GetFileNameWithoutExtension(x);
				
				commandName = x;
				
				LogWriter.Debug("Command: " + commandName);
			}
			return commandName;
		}
		
		/// <summary>
		/// Checks whether rewriting for the provided path should be skipped.
		/// </summary>
		/// <param name="originalPath"></param>
		/// <returns></returns>
		public bool Skip(string originalPath)
		{
			bool output = false;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Checking whether to skip rewriting of the URL: " + originalPath))
			{
				originalPath = Converter.ToRelative(originalPath);
				
				LogWriter.Debug("Original path: " + originalPath);
				
				originalPath = originalPath.TrimStart('/');
				
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalPath);
				string fileName = originalPath;
				if (fileName.IndexOf('?') > -1)
					fileName = fileName.Substring(0, fileName.IndexOf('?'));
				if (fileName.IndexOf('/') > -1)
					fileName = fileName.Substring(fileName.LastIndexOf('/'),
					                              fileName.Length-fileName.LastIndexOf('/'));
				
				LogWriter.Debug("File name: " + fileName);
				LogWriter.Debug("File name (without extension): " + fileNameWithoutExtension);
				
				string ext = Path.GetExtension(originalPath);
				
				ext = ext.Trim('.').ToLower();
				
				if (ext.IndexOf("?") > -1)
					ext = ext.Substring(0, ext.IndexOf("?"));
				
				LogWriter.Debug("Ext: " + ext);
				
				if (ext == "js" // javascript file
				    || ext == "script" // script file
				    || ext == "css" // stylesheet
				    || ext == "axd" // WebResource.axd file
				    || fileName.ToLower() == "quicksetup.aspx" // quick setup page
				    || fileName.ToLower() == "setup.aspx" // setup page
				    || fileName.ToLower() == "projector.aspx"
				    || fileName == String.Empty
				    || IsRealFile(originalPath)) // no file name specified
					output = true;
				
				LogWriter.Debug("Output: " + output);
			}
			return output;
		}

		/// <summary>
		/// Checks whether the provided URL is an actual file.
		/// </summary>
		/// <param name="url">The URL of the resource to check.</param>
		/// <returns>A boolean value indicating whether the URL refers to an actual file.</returns>
		public bool IsRealFile(string url)
		{
			bool isReal = false;
			
			using (LogGroup logGroup = LogGroup.Start("Checking whether the specified URL points to a real file.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Provided URL: " + url);
				
				string shortUrl = GetShortPath(url);
				
				LogWriter.Debug("Short URL: " + shortUrl);
				
				//string physicalFile = FileMapper.MapPath(shortUrl);
				
				//LogWriter.Debug("Physical file: " + physicalFile);
				
				isReal = FileExistenceChecker.Exists(shortUrl);
				
				LogWriter.Debug("Is real? " + isReal.ToString());
			}
			return isReal;
		}
		
		public string GetShortPath(string originalPath)
		{
			string shortPath = String.Empty;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving short path from: " + originalPath))
			{
				shortPath = originalPath;
				
				LogWriter.Debug("Original path: " + originalPath);
				
				if (originalPath.IndexOf("?") > -1)
					shortPath = originalPath.Substring(0, originalPath.IndexOf("?"));
				
				string applicationPath = Converter.ToAbsolute(StateAccess.State.ApplicationPath);
				
				LogWriter.Debug("Application path: " + applicationPath);
				
				if (originalPath.IndexOf(applicationPath) > -1)
					shortPath = shortPath.Replace(applicationPath, "");
				
				LogWriter.Debug("Short path: " + shortPath);
			}
			return shortPath;
		}
		
		public ProjectionFormat GetFormat(string fileName)
		{
			string ext = Path.GetExtension(fileName).Trim('.');
			
			if (ext == "aspx")
				return ProjectionFormat.Html;
			
			throw new NotSupportedException("Extension not yet supported: " + ext);
		}
		
		/// <summary>
		/// Extracts entity ID query string data from the provided part and adds it to the dictionary.
		/// </summary>
		/// <param name="friendlyPath"></param>
		/// <param name="queryStrings"></param>
		public void ExtractGuidQueryString(string friendlyPath, Dictionary<string, string> queryStrings)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Extracting the entity ID section of the path to add to the query string."))
			{
				LogWriter.Debug("Friendly path: " + friendlyPath);
				
				Guid id = GetDynamicID(friendlyPath);
				
				string typeName = GetTypeName(GetCommandName(friendlyPath));
				
				if (id != Guid.Empty)
				{
					LogWriter.Debug("Adding '" + typeName + "-ID' query string.");
					
					queryStrings.Add(typeName + "-ID", id.ToString());
				}
				else
					LogWriter.Debug("No ID found in path. Not adding query string.");
			}
		}
		
		/// <summary>
		/// Extracts a dynamic query string from the provided part and adds it to the dictionary.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="part">The part/section of the URL to extract the query string from.</param>
		/// <param name="queryStrings"></param>
		public void ExtractDynamicQueryString(string typeName, string part, Dictionary<string, string> queryStrings)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Extracting query string data from the provided part of a path."))
			{
				// Replace the "--" with "|" then split it
				string[] subParts = part.Replace("--", "|").Split('|');
				
				LogWriter.Debug("Type name: " + typeName);
				LogWriter.Debug("Part: " + part);
				
				if (subParts.Length == 2)
				{
					LogWriter.Debug("Part is correct format.");
					
					string propertyName = subParts[0];
					string value = subParts[1];
					
					LogWriter.Debug("Property name: " + propertyName);
					LogWriter.Debug("Value: " + value);
					
					// If the property name is "K" it's short for "UniqueKey"
					if (propertyName == "K")
						propertyName = typeName + "-UniqueKey";
					
					// If it's not an ignored property then add it
					if (propertyName != "I")
					{
						LogWriter.Debug("Adding '" + propertyName + "=" + value + "' query string.");
						queryStrings.Add(propertyName, value);
					}
				}
				else
					LogWriter.Debug("Not the correct format. Skipping.");
			}
		}
		
		/// <summary>
		/// Extracts the page format from teh provided URL and adds it to the query strings dictionary.
		/// </summary>
		/// <param name="friendlyUrl"></param>
		/// <param name="queryStrings"></param>
		/// <returns></returns>
		public void ExtractFormatQueryString(string friendlyUrl, Dictionary<string, string> queryStrings)
		{
			ProjectionFormat format = GetFormat(GetExtension(friendlyUrl));
			
			queryStrings.Add("f", format.ToString());
		}
		
		public string AddQueryStrings(string originalPath, string newPath)
		{
			string output = String.Empty;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Adding query strings to path: " + newPath))
			{
				LogWriter.Debug("Original path: " + originalPath);
				
				string[] parts = GetShortPath(originalPath).Trim('/').Split('/');

				if (parts != null)
					LogWriter.Debug("# parts: " + parts.Length);

				string lastPart = parts[parts.Length-1];

				if (parts.Length > 0 && lastPart.IndexOf(".") > -1)
					parts[parts.Length - 1] = lastPart.Substring(0, lastPart.IndexOf("."));
				
				Dictionary<string, string> queryStrings = new Dictionary<string, string>();
				
				ExtractFormatQueryString(originalPath, queryStrings);
				
				ExtractOriginalQueryStrings(originalPath, queryStrings);
				
				output = newPath;
				
				output = AddQueryStrings(output, queryStrings);
				
				LogWriter.Debug("Output: " + output);
			}
			
			return output;
		}
		
		public string AddQueryStrings(string path, Dictionary<string, string> queryStrings)
		{
			string output = string.Empty;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Adding query strings to the path: " + path))
			{
				output = path;
				
				UrlCreator urlCreator = new UrlCreator(ApplicationPath, path);
				
				foreach (string key in queryStrings.Keys)
				{
					string separator = "?";
					if (path.IndexOf("?") > -1)
						separator = "&";
					
					LogWriter.Debug("Separator: " + separator);
					
					if (output.IndexOf("?" + key + "=") == -1
					    && output.IndexOf("&" + key + "=") == -1)
					{
						LogWriter.Debug("Adding query string \"" + key + "=" + queryStrings[key] + "\"");
						
						output = output + separator + urlCreator.PrepareForUrl(key)
							+ "=" + urlCreator.PrepareForUrl(queryStrings[key]);
						
						LogWriter.Debug("New path: " + output);
					}
					else
						LogWriter.Debug("Query string key '" + key + "' found in path. Skipping append.");
				}
				
				LogWriter.Debug("Output: " + output);
				
			}
			return output;
		}
		
		public string AddQueryStrings(string originalPath, string newPath, string action, string typeName)
		{
			string output = String.Empty;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Adding query strings to path: " + newPath))
			{
				
				LogWriter.Debug("Original path: " + originalPath);
				
				Dictionary<string, string> queryStrings = new Dictionary<string, string>();
				
				ExtractQueryStringsFromFriendlyPath(originalPath, queryStrings);
				
				output = AddQueryStrings(newPath, queryStrings);
				
				LogWriter.Debug("Output: " + output);
			}
			
			return output;
		}
		
		public void ExtractQueryStringsFromFriendlyPath(string friendlyPath, Dictionary<string, string> queryStrings)
		{
			ExtractFormatQueryString(friendlyPath, queryStrings);
			
			ExtractOriginalQueryStrings(friendlyPath, queryStrings);
			
			ExtractGuidQueryString(friendlyPath, queryStrings);
			
			ExtractDynamicQueryStrings(friendlyPath, queryStrings);
			
		}
		
		public void ExtractDynamicQueryStrings(string friendlyPath, Dictionary<string, string> queryStrings)
		{
			string typeName = GetTypeName(GetCommandName(friendlyPath));
			
			string[] parts = GetShortPath(friendlyPath).Trim('/').Split('/');

			if (parts != null)
				LogWriter.Debug("# parts: " + parts.Length);

			string lastPart = parts[parts.Length-1];

			if (parts.Length > 0 && lastPart.IndexOf(".") > -1)
				parts[parts.Length - 1] = lastPart.Substring(0, lastPart.IndexOf("."));
			
			for (int i = 1; i < parts.Length; i++)
			{
				ExtractDynamicQueryString(typeName, parts[i], queryStrings);
			}
		}
		
		public string UrlEncode(string original)
		{
			if (HttpContext.Current != null)
				original = HttpContext.Current.Server.UrlEncode(original);
			
			return original;
		}
		
		/// <summary>
		/// Retrieves the file extension from the provided friendly URL.
		/// </summary>
		/// <param name="friendlyUrl"></param>
		/// <returns></returns>
		public string GetExtension(string friendlyUrl)
		{
			string originalFileName = Path.GetFileName(GetShortPath(friendlyUrl));
			
			int pos = originalFileName.IndexOf(".");

			string ext = originalFileName.Substring(pos, originalFileName.Length - pos);
			
			return ext;
		}
		
		public string GetRealPageName(string friendlyUrl)
		{
			string ext = GetExtension(friendlyUrl);

			ProjectionFormat format = GetFormat(ext);
			
			string realPageName = "Projector.aspx";
			
			if (format == ProjectionFormat.Xslt
			    || format == ProjectionFormat.Xml)
				realPageName = "XmlProjector.aspx";
			
			return realPageName;
		}
		
		public void ExtractOriginalQueryStrings(string originalPath, Dictionary<string, string> parameters)
		{
			if (originalPath.IndexOf("?") > -1)
			{
				string query = originalPath.Substring(originalPath.IndexOf("?"), originalPath.Length-originalPath.IndexOf("?"));
				query = query.Trim('?');
				
				string[] queryParts = query.Split('&');
				
				foreach (string part in queryParts)
				{
					string[] subParts = part.Split('=');
					
					string key = subParts[0];
					string value = subParts[1];
					
					if (!parameters.ContainsKey(key))
						parameters.Add(key, value);
				}
			}
		}
	}
}

