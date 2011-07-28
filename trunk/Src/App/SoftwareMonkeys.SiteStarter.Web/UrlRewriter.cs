using System;
using System.Web;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.IO;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.IO;
using System.Configuration;
using SoftwareMonkeys.SiteStarter.State;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// Rewrites friendly URLs to the raw URL used behind the scenes.
	/// </summary>
	public class UrlRewriter
	{
		/// <summary>
		/// Initializes the automatic URL rewriting.
		/// </summary>
		static public void Initialize()
		{
			if (StateAccess.IsInitialized)
			{
				using (LogGroup logGroup = LogGroup.Start("Initializes the URL cloaking.", NLog.LogLevel.Debug))
				{
					UrlRewriter rewriter = new UrlRewriter();
					
					if (Config.IsInitialized)
					{
						if (rewriter.EnableFriendlyUrls)
						{
							LogWriter.Debug("Url cloaking is enabled.");

							string uri = HttpContext.Current.Request.Url.ToString();

							LogWriter.Debug("Url: " + uri);

							if (!rewriter.IsRealFile(uri) && !rewriter.IsExcluded(uri))
							{
								LogWriter.Debug("Application path: " + HttpContext.Current.Request.ApplicationPath);
								
								rewriter.RewriteUrl(uri, HttpContext.Current.Request.ApplicationPath, true);
							}
						}
						else
							LogWriter.Debug("Url cloaking is disabled.");
					}
					else
						LogWriter.Debug("Application is not initialized. Skipping cloaking.");
				}
			}
		}
		
		private bool enableFriendlyUrls;
		/// <summary>
		/// Gest/sets a value indicating whether friendly URLs are enabled. (Using URL rewriting behind the scenes.)
		/// </summary>
		public bool EnableFriendlyUrls
		{
			get { return enableFriendlyUrls; }
			set { enableFriendlyUrls = value; }
		}
		
		private string applicationPath = String.Empty;
		/// <summary>
		/// Gets/sets the relative path to the root of the application.
		/// </summary>
		public string ApplicationPath
		{
			get {
				if (applicationPath == String.Empty && HttpContext.Current != null)
					applicationPath = HttpContext.Current.Request.ApplicationPath;
				return applicationPath; }
			set { applicationPath = value; }
		}
		
		private IFileMapper fileMapper;
		/// <summary>
		/// Gets/sets the file mapper used to map relative paths to physical paths.
		/// </summary>
		public IFileMapper FileMapper
		{
			get {
				if (fileMapper == null && HttpContext.Current != null)
					fileMapper = new FileMapper();
				return fileMapper;
			}
			set { fileMapper = value; }
		}
		
		/// <summary>
		/// Empty constructor.
		/// </summary>
		public UrlRewriter()
		{
			EnableFriendlyUrls = GetEnableFriendlyUrlsSetting();
		}
		
		/// <summary>
		/// Sets the application path and the file mapper used by the rewriter.
		/// </summary>
		/// <param name="applicationPath">The relative application path.</param>
		/// <param name="fileMapper">The file mapper used to map file paths.</param>
		public UrlRewriter(string applicationPath, IFileMapper fileMapper)
		{
			ApplicationPath = applicationPath;
			FileMapper = fileMapper;
			EnableFriendlyUrls = GetEnableFriendlyUrlsSetting();
		}
		
		/// <summary>
		/// Maps the provided path to the physical path of the resource.
		/// </summary>
		/// <param name="relativePath">The relative path of the resource being mapped.</param>
		/// <returns>The full physical path to the specified resource.</returns>
		public virtual string MapPath(string relativePath)
		{
			if (FileMapper == null)
				throw new InvalidOperationException("The FileMapper property has not been initialized.");
			
			return FileMapper.MapApplicationRelativePath(relativePath);
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
				
				string shortUrl = GetShortUrl(url);
				
				LogWriter.Debug("Short URL: " + shortUrl);
				
				string physicalFile = MapPath(shortUrl);
				
				LogWriter.Debug("Physical file: " + physicalFile);
				
				isReal = (File.Exists(physicalFile));
				
				LogWriter.Debug("Is real? " + isReal.ToString());
			}
			return isReal;
		}
		
		/// <summary>
		/// Retrieves the short version of the provided URL.
		/// </summary>
		/// <param name="url">The URL to shorten.</param>
		/// <returns>The shortened version of the provided URL.</returns>
		public string GetShortUrl(string url)
		{
			string shortUrl = String.Empty;
			using (LogGroup logGroup = LogGroup.Start("Retrieving the short URL to the current page.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Url: " + url);
				
				if (url.IndexOf("//") >-1)
				{
					Uri uri = new Uri(url);
					
					string host = uri.Host;
					bool isSecure = uri.Scheme == Uri.UriSchemeHttps;
					
					url = WebUtilities.ConvertAbsoluteUrlToRelativeUrl(url,
					                                                   WebUtilities.ConvertRelativeUrlToAbsoluteUrl(ApplicationPath, host, isSecure));
					
					LogWriter.Debug("Relative url: " + url);
				}
				
				int pos = url.IndexOf("?");
				
				LogWriter.Debug("Position of ? character: " + pos.ToString());
				
				string filePart = url;
				
				if (pos > -1)
				{
					LogWriter.Debug("Query string (ie. ? character) found. Stripping from url.");
					
					filePart = filePart.Substring(0, pos);
					
					LogWriter.Debug("Part used: " + filePart);
				}

				shortUrl = filePart;
				
				LogWriter.Debug("Short URL: " + shortUrl);
			}
			
			return shortUrl;
		}
		
		/// <summary>
		/// Rewrites the provided friendly URL to a raw URL used behind the scenes.
		/// </summary>
		/// <param name="friendlyUrl">The friendly URL to rewrite.</param>
		/// <param name="applicationPath">The relative path to the root of the application.</param>
		/// <returns>The rewritten raw URL for use behind the scenes.</returns>
		public string RewriteUrl(string friendlyUrl, string applicationPath)
		{
			return RewriteUrl(friendlyUrl, applicationPath, true);
		}
		
		/// <summary>
		/// Rewrites the provided friendly URL to a raw URL used behind the scenes.
		/// </summary>
		/// <param name="friendlyUrl">The friendly URL to rewrite.</param>
		/// <param name="applicationPath">The relative path to the root of the application.</param>
		/// <param name="commit">A boolean value indicating whether to commit the rewrite (ie. actually direct the user request to the rewritten URL).</param>
		/// <returns>The rewritten raw URL for use behind the scenes.</returns>
		public string RewriteUrl(string friendlyUrl, string applicationPath, bool commit)
		{
			string newUrl = String.Empty;
			using (LogGroup logGroup = LogGroup.Start("Attempting to interpret and rewrite the provided cloaked URL.", NLog.LogLevel.Debug))
			{
				if (friendlyUrl == null)
					throw new ArgumentNullException("friendlyUrl");

				if (applicationPath == null)
					throw new ArgumentNullException("applicationPath");

				LogWriter.Debug("Original path: " + friendlyUrl);
				LogWriter.Debug("Application path: " + applicationPath);

				//string applicationUrl = applicationPath;
				//if (applicationUrl.ToLower().IndexOf("http://") == -1)
				//	applicationUrl = WebUtilities.ConvertRelativeUrlToAbsoluteUrl(applicationUrl);

				string shortPath = GetShortUrl(friendlyUrl);//.Replace(applicationUrl, String.Empty);
				shortPath = shortPath.Trim('/');
				
				if (shortPath == null)
					throw new Exception("shortPath == null");

				LogWriter.Debug("Short path: " + shortPath);

				string[] parts = shortPath.Split('/');

				if (parts != null)
					LogWriter.Debug("# parts: " + parts.Length);

				string lastPart = parts[parts.Length-1];

				if (parts.Length > 0)
					parts[parts.Length - 1] = lastPart.Substring(0, lastPart.IndexOf("."));
				
				// If there aren't enough parts then it's not cloaked and can't be rewritten.
				if (parts != null && parts.Length >= 1)
				{
					newUrl = SmartRewrite(friendlyUrl, applicationPath, parts, commit);
				}
				
				// If a new URL was created
				if (newUrl != String.Empty)
				{
					string existingQueryString = String.Empty;
					int pos = friendlyUrl.IndexOf("?");
					
					LogWriter.Debug("? pos: " + pos);
					
					if (pos > -1)
					{
						existingQueryString = friendlyUrl.Substring(pos+1, friendlyUrl.Length-pos-1);
						
						LogWriter.Debug("Existing query string: " + existingQueryString);
						
						string separator = "?";
						if (friendlyUrl.IndexOf("?") > -1)
							separator = "&";
						
						LogWriter.Debug("Separator: " + separator);
						
						newUrl = newUrl + separator + existingQueryString;
					}
					LogWriter.Debug("New URL: " + newUrl);
					
					if (commit)
						CommitRewrite(newUrl);
				}
				// Otherwise skip the rewrite
			}


			return newUrl;
		}
		
		/*/// <summary>
		/// Rewrites the friendly URL to the corresponding raw URL. Used for entity action URLs.
		/// </summary>
		/// <param name="friendlyUrl">The friendly URL to rewrite.</param>
		/// <param name="applicationPath">The relative path to the root of the application.</param>
		/// <param name="parts">The parts of the URL.</param>
		/// <param name="commit">A boolean value indicating whether to commit the rewrite (ie. actually redirect the request to the rewritten path).</param>
		/// <returns>The rewritten URL that corresponds with the friendly URL provided.</returns>
		public string RewriteToEntityAction(string friendlyUrl, string applicationPath, string[] parts, bool commit)
		{
			string newUrl = String.Empty;

			using (LogGroup logGroup = LogGroup.Start("Rewriting path to an entity action.", NLog.LogLevel.Debug))
			{
				if (parts == null)
					throw new ArgumentNullException("parts");

				if (parts.Length < 2)
					throw new InvalidOperationException("Not enough parts. Expected 2. Was " + parts.Length);

				//LogWriter.Debug("Original path: " + friendlyUrl);
				//LogWriter.Debug("Application path: " + applicationPath);
				
				string type = parts[0];
				string action = parts[1];
				string propertyName = String.Empty;
				string data = String.Empty;

				// If the property name is specified
				if (parts.Length > 3)
				{
					data = parts[3];
					propertyName = parts[2];
				}
				// Otherwise
				else if (parts.Length > 2)
				{
					data = parts[2];
					if (GuidValidator.IsValidGuid(data))
						propertyName = "ID";
					else
						propertyName = "UniqueKey";
					
					
				}

				string pageType = "Html";
				string originalFileName = Path.GetFileName(GetShortUrl(friendlyUrl));
				
				//LogWriter.Debug("Original file name: " + originalFileName);
				
				int pos = originalFileName.IndexOf(".");

				string ext = originalFileName.Substring(pos, originalFileName.Length - pos);
				
				//LogWriter.Debug("Extension: " + ext);
				
				if (ext.ToLower().Trim('.') == "xml.aspx")
					pageType = "Xml";
				else if (ext.ToLower().Trim('.') == "xslt.aspx")
					pageType = "Xslt";
				
				//LogWriter.Debug("Page type: " + pageType.ToString());


				//LogWriter.Debug("Action: " + action);
				//LogWriter.Debug("Type: " + type);
				//LogWriter.Debug("Property name: " + propertyName);
				//LogWriter.Debug("Data: " + data);

				string realPageName = "Projector.aspx";
				if (pageType == "Xml"
				    || pageType == "Xslt")
					realPageName = "XmlProjector.aspx";
				
				//LogWriter.Debug("Real page name: " + realPageName);

				newUrl = applicationPath + "/" + realPageName
					+ "?a=" + action
					+ "&t=" + type
					+ "&f=" + pageType;
				
				//LogWriter.Debug("New url: " + newUrl);

				// TODO: Remove comment
				//if (propertyName.ToLower() == "ID".ToLower())
				propertyName = type + "-" + propertyName;
				
				//LogWriter.Debug("Property name: " + propertyName);

				if (propertyName != String.Empty
				    && data != String.Empty)
				{
					string qs = "&" + propertyName + "=" + data;
					
					//	LogWriter.Debug("Adding query string: " + qs);
					
					newUrl = newUrl + qs;
					
					//	LogWriter.Debug("New url: " + newUrl);
					
				}
			}


			return newUrl;
		}*/
			
			/// <summary>
			/// Rewrites the friendly URL to the corresponding raw URL. Used for entity action URLs.
			/// </summary>
			/// <param name="friendlyUrl">The friendly URL to rewrite.</param>
			/// <param name="applicationPath">The relative path to the root of the application.</param>
			/// <param name="parts">The parts of the URL.</param>
			/// <param name="commit">A boolean value indicating whether to commit the rewrite (ie. actually redirect the request to the rewritten path).</param>
			/// <returns>The rewritten URL that corresponds with the friendly URL provided.</returns>
			public string SmartRewrite(string friendlyUrl, string applicationPath, string[] parts, bool commit)
		{
			string command = parts[0];
			
			string rewrittenUrl = String.Empty;
			
			string action = GetAction(friendlyUrl);
			string typeName = GetTypeName(friendlyUrl);
			
			if (action != String.Empty && typeName != String.Empty)
			{
				Dictionary<string, string> queryStrings = new Dictionary<string, string>();
				
				for (int i = 1; i < parts.Length; i++)
				{
					ExtractQueryString(typeName, parts[i], queryStrings);
				}
				
				ExtractFormatQueryString(friendlyUrl, queryStrings);
				
				string realPageName = GetRealPageName(friendlyUrl);
				
				rewrittenUrl = applicationPath + "/" + realPageName + "?a=" + action + "&t=" + typeName;
				
				UrlCreator urlCreator = new UrlCreator(ApplicationPath, friendlyUrl);
				
				foreach (string key in queryStrings.Keys)
				{
					rewrittenUrl = rewrittenUrl + "&" +  urlCreator.PrepareForUrl(key)
						+ "=" + urlCreator.PrepareForUrl(queryStrings[key]);
				}
			}
			
			return rewrittenUrl;
		}
		
		/// <summary>
		/// Extracts the command string from the provided friendly URL.
		/// </summary>
		/// <param name="friendlyUrl"></param>
		/// <returns></returns>
		public string GetCommand(string friendlyUrl)
		{
			string shortUrl = GetShortUrl(friendlyUrl);
			
			string[] parts = shortUrl.Trim('/').Split('/');
			
			string command = parts[0];
			
			if (command.IndexOf(".") > -1)
				command = command.Substring(0, command.IndexOf("."));
			
			return command;
		}
		
		/// <summary>
		/// Extracts the action from the provided friendly URL.
		/// </summary>
		/// <param name="friendlyUrl"></param>
		/// <returns></returns>
		public string GetAction(string friendlyUrl)
		{
			string commandString = GetCommand(friendlyUrl);
			
			string[] commandParts = commandString.Trim('/').Split('-');
			
			string action = String.Empty;
			
			// If there are two parts to the command string
			if (commandParts.Length == 2)
			{
				// If the first part is an entity type name
				if (EntityState.IsType(commandParts[0]))
					// then the action is the second part
					action = commandParts[1];
				// Otherwise
				else
					// the action is the first part
					action = commandParts[0];
			}
			
			return action;
		}
		
		/// <summary>
		/// Extracts the type name from the provided friendly URL.
		/// </summary>
		/// <param name="friendlyUrl"></param>
		/// <returns></returns>
		public string GetTypeName(string friendlyUrl)
		{
			string commandString = GetCommand(friendlyUrl);
			
			string[] commandParts = commandString.Trim('/').Split('-');
			
			string typeName = String.Empty;
			
			// If there are two parts to the command string
			if (commandParts.Length == 2)
			{
				if (EntityState.IsType(commandParts[0]))
					typeName = commandParts[0];
				else
					typeName = commandParts[1];
			}
			
			return typeName;
		}
		
		/// <summary>
		/// Extracts the page format from teh provided URL and adds it to the query strings dictionary.
		/// </summary>
		/// <param name="friendlyUrl"></param>
		/// <param name="queryStrings"></param>
		/// <returns></returns>
		public void ExtractFormatQueryString(string friendlyUrl, Dictionary<string, string> queryStrings)
		{
			string format = GetPageFormat(GetExtension(friendlyUrl));
			
			queryStrings.Add("f", format);
		}
		
		public string GetPageFormat(string extension)
		{
			string pageFormat = "Html";
			
			if (extension.ToLower().Trim('.') == "xml.aspx")
				pageFormat = "Xml";
			else if (extension.ToLower().Trim('.') == "xslt.aspx")
				pageFormat = "Xslt";
			
			return pageFormat;
		}
		
		public string GetRealPageName(string friendlyUrl)
		{

			string ext = GetExtension(friendlyUrl);

			string pageType = GetPageFormat(ext);
			
			string realPageName = "Projector.aspx";
			if (pageType == "Xml"
			    || pageType == "Xslt")
				realPageName = "XmlProjector.aspx";
			
			return realPageName;
		}
		
		/// <summary>
		/// Retrieves the file extension from the provided friendly URL.
		/// </summary>
		/// <param name="friendlyUrl"></param>
		/// <returns></returns>
		public string GetExtension(string friendlyUrl)
		{
			string originalFileName = Path.GetFileName(GetShortUrl(friendlyUrl));
			
			//LogWriter.Debug("Original file name: " + originalFileName);
			
			int pos = originalFileName.IndexOf(".");

			string ext = originalFileName.Substring(pos, originalFileName.Length - pos);
			
			return ext;
		}
		
		/// <summary>
		/// Extracts the query string from the provided part and adds it to the dictionary.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="part">The part/section of the URL to extract the query string from.</param>
		/// <param name="queryStrings"></param>
		public void ExtractQueryString(string typeName, string part, Dictionary<string, string> queryStrings)
		{
			// If it's a GUID then it's the ID of an entity
			if (GuidValidator.IsValidGuid(part))
				ExtractGuidQueryString(typeName, part, queryStrings);
			// Otherwise it's a dynamic query string
			else
				ExtractDynamicQueryString(typeName, part, queryStrings);
		}
		
		/// <summary>
		/// Extracts a GUID query string from the provided part and adds it to the dictionary.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="part">The part/section of the URL to extract the query string from.</param>
		/// <param name="queryStrings"></param>
		public void ExtractGuidQueryString(string typeName, string part, Dictionary<string, string> queryStrings)
		{
			// If it's a GUID then it's the ID of an entity
			if (GuidValidator.IsValidGuid(part))
			{
				queryStrings.Add(typeName + "-ID", part);
			}
			else
				throw new ArgumentException("The provided part is not a valid GUID.");
		}
		
		/// <summary>
		/// Extracts a dynamic query string from the provided part and adds it to the dictionary.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="part">The part/section of the URL to extract the query string from.</param>
		/// <param name="queryStrings"></param>
		public void ExtractDynamicQueryString(string typeName, string part, Dictionary<string, string> queryStrings)
		{
			// Replace the "--" with "|" then split it
			string[] subParts = part.Replace("--", "|").Split('|');
			
			if (subParts.Length < 2)
				throw new ArgumentException("The provided part '" + part + "' is not in the correct format of [PropertyName]-[Value].", "part");
			
			string propertyName = subParts[0];
			string value = subParts[1];
			
			// If the property name is "K" it's short for "UniqueKey"
			if (propertyName == "K")
				propertyName = typeName + "-UniqueKey";
			
			// If it's not an ignored property then add it
			if (propertyName != "I")
				queryStrings.Add(propertyName, value);
		}
		
		/// <summary>
		/// Commits the URL rewrite for the current request. Uses HttpContext.Current.RewritePath(..., ...)
		/// </summary>
		/// <param name="url">The URL to rewrite to.</param>
		public void CommitRewrite(string url)
		{
			using (LogGroup logGroup = LogGroup.Start("Committing rewrite to new URL.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Url: " + url);

				if (HttpContext.Current.Items != null)
					HttpContext.Current.Items["RewrittenUrl"] = url;

				HttpContext.Current.RewritePath(url, false);
				//HttpContext.Current.Server.Transfer(url, false);
			}
		}
		
		/// <summary>
		/// Retrieves the current EnableFriendlyURLs setting from the Web.config file.
		/// </summary>
		/// <returns>The value of the EnableFriendlyURLs setting in the Web.config file.</returns>
		public bool GetEnableFriendlyUrlsSetting()
		{
			if (ConfigurationManager.AppSettings != null)
				return Convert.ToBoolean(ConfigurationManager.AppSettings["FriendlyUrls.Enabled"]);
			else
				return false;
		}
		
		/// <summary>
		/// Checks whether the provided URL is to be excluded from rewriting.
		/// </summary>
		/// <param name="url">The URL to check.</param>
		/// <returns>A boolean value indicating whether the specified URL is excluded from rewriting.</returns>
		public bool IsExcluded(string url)
		{
			// WebResource.axd request
			if (url.ToLower().IndexOf("webresource.axd") > -1)
				return true;
			
			
			return false;
		}
	}
}
