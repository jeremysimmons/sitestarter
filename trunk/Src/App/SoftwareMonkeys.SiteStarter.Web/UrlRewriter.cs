using System;
using System.Web;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.IO;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.IO;
using System.Configuration;
using SoftwareMonkeys.SiteStarter.State;

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
				using (LogGroup logGroup = AppLogger.StartGroup("Initializes the URL cloaking.", NLog.LogLevel.Debug))
				{
					UrlRewriter rewriter = new UrlRewriter();
					
					if (Config.IsInitialized)
					{
						if (rewriter.EnableFriendlyUrls)
						{
							AppLogger.Debug("Url cloaking is enabled.");

							string uri = HttpContext.Current.Request.Url.ToString();

							AppLogger.Debug("Url: " + uri);

							if (!rewriter.IsRealFile(uri) && !rewriter.IsExcluded(uri))
							{
								AppLogger.Debug("Application path: " + HttpContext.Current.Request.ApplicationPath);
								
								rewriter.RewriteUrl(uri, HttpContext.Current.Request.ApplicationPath, true);
							}
						}
						else
							AppLogger.Debug("Url cloaking is disabled.");
					}
					else
						AppLogger.Debug("Application is not initialized. Skipping cloaking.");
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
			using (LogGroup logGroup = AppLogger.StartGroup("Checking whether the specified URL points to a real file.", NLog.LogLevel.Debug))
			{
				/*if (url.ToLower().IndexOf("//") > -1)
				{
					AppLogger.Debug("Provided URL: " + url);
					
					AppLogger.Debug("URL is absolute. Converting to relative.");
					
					Uri uri = new Uri(url);
					
					string host = uri.Host;
					bool isSecure = uri.Scheme == Uri.UriSchemeHttps;
					
					url = WebUtilities.ConvertAbsoluteUrlToRelativeUrl(url,
					                                                   WebUtilities.ConvertRelativeUrlToAbsoluteUrl(ApplicationPath, host, isSecure));
					
				}*/

				AppLogger.Debug("Provided URL: " + url);
				
				string shortUrl = GetShortUrl(url);
				
				AppLogger.Debug("Short URL: " + shortUrl);
				
				string physicalFile = MapPath(shortUrl);
				
				AppLogger.Debug("Physical file: " + physicalFile);
				
				isReal = (File.Exists(physicalFile));
				
				AppLogger.Debug("Is real? " + isReal.ToString());
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
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the short URL to the current page.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Url: " + url);
				
				if (url.IndexOf("//") >-1)
				{
					Uri uri = new Uri(url);
					
					string host = uri.Host;
					bool isSecure = uri.Scheme == Uri.UriSchemeHttps;
					
					url = WebUtilities.ConvertAbsoluteUrlToRelativeUrl(url,
					                                                   WebUtilities.ConvertRelativeUrlToAbsoluteUrl(ApplicationPath, host, isSecure));
					
					AppLogger.Debug("Relative url: " + url);
				}
				
				int pos = url.IndexOf("?");
				
				AppLogger.Debug("Position of ? character: " + pos.ToString());
				
				string filePart = url;
				
				if (pos > -1)
				{
					AppLogger.Debug("Query string (ie. ? character) found. Stripping from url.");
					
					filePart = filePart.Substring(0, pos);
					
					AppLogger.Debug("Part used: " + filePart);
				}

				shortUrl = filePart;//WebUtilities.ConvertAbsoluteUrlToRelativeUrl(filePart, HttpContext.Current.Request.ApplicationPath);
				
				AppLogger.Debug("Short URL: " + shortUrl);
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
			using (LogGroup logGroup = AppLogger.StartGroup("Attempting to interpret and rewrite the provided cloaked URL.", NLog.LogLevel.Debug))
			{
				if (friendlyUrl == null)
					throw new ArgumentNullException("friendlyUrl");

				if (applicationPath == null)
					throw new ArgumentNullException("applicationPath");

				AppLogger.Debug("Original path: " + friendlyUrl);
				AppLogger.Debug("Application path: " + applicationPath);

				//string applicationUrl = applicationPath;
				//if (applicationUrl.ToLower().IndexOf("http://") == -1)
				//	applicationUrl = WebUtilities.ConvertRelativeUrlToAbsoluteUrl(applicationUrl);

				string shortPath = GetShortUrl(friendlyUrl);//.Replace(applicationUrl, String.Empty);
				shortPath = shortPath.Trim('/');

				if (shortPath == null)
					throw new Exception("shortPath == null");

				AppLogger.Debug("Short path: " + shortPath);

				string[] parts = shortPath.Split('/');

				if (parts != null)
					AppLogger.Debug("# parts: " + parts.Length);

				string lastPart = parts[parts.Length-1];

				if (parts.Length > 0)
					parts[parts.Length - 1] = lastPart.Substring(0, lastPart.IndexOf("."));

				// If there aren't enough parts then it's not cloaked and can't be rewritten.
				if (parts != null && parts.Length > 1)
				{
					newUrl = RewriteToEntityAction(friendlyUrl, applicationPath, parts, commit);
				}
				
				string existingQueryString = String.Empty;
				int pos = friendlyUrl.IndexOf("?");
				
				AppLogger.Debug("? pos: " + pos);
				
				if (pos > -1)
				{
					existingQueryString = friendlyUrl.Substring(pos+1, friendlyUrl.Length-pos-1);
					
					AppLogger.Debug("Existing query string: " + existingQueryString);
					
					string separator = "?";
					if (friendlyUrl.IndexOf("?") > -1)
						separator = "&";
					
					AppLogger.Debug("Separator: " + separator);
					
					newUrl = newUrl + separator + existingQueryString;
				}
				AppLogger.Debug("New URL: " + newUrl);
				
				if (commit)
					CommitRewrite(newUrl);
			}


			return newUrl;
		}
		
		/// <summary>
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

			using (LogGroup logGroup = AppLogger.StartGroup("Rewriting path to an entity action.", NLog.LogLevel.Debug))
			{
				if (parts == null)
					throw new ArgumentNullException("parts");

				if (parts.Length < 2)
					throw new InvalidOperationException("Not enough parts. Expected 2. Was " + parts.Length);

				//AppLogger.Debug("Original path: " + friendlyUrl);
				//AppLogger.Debug("Application path: " + applicationPath);
				
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
				
				//AppLogger.Debug("Original file name: " + originalFileName);
				
				int pos = originalFileName.IndexOf(".");

				string ext = originalFileName.Substring(pos, originalFileName.Length - pos);
				
				//AppLogger.Debug("Extension: " + ext);
				
				if (ext.ToLower().Trim('.') == "xml.aspx")
					pageType = "Xml";
				else if (ext.ToLower().Trim('.') == "xslt.aspx")
					pageType = "Xslt";
				
				//AppLogger.Debug("Page type: " + pageType.ToString());


				//AppLogger.Debug("Action: " + action);
				//AppLogger.Debug("Type: " + type);
				//AppLogger.Debug("Property name: " + propertyName);
				//AppLogger.Debug("Data: " + data);

				string realPageName = "Projector.aspx";
				if (pageType == "Xml"
				    || pageType == "Xslt")
					realPageName = "XmlProjector.aspx";
				
				//AppLogger.Debug("Real page name: " + realPageName);

				newUrl = applicationPath + "/" + realPageName
					+ "?a=" + action
					+ "&t=" + type
					+ "&f=" + pageType;
				
				//AppLogger.Debug("New url: " + newUrl);

				// TODO: Remove comment
				//if (propertyName.ToLower() == "ID".ToLower())
				propertyName = type + "-" + propertyName;
				
				//AppLogger.Debug("Property name: " + propertyName);

				if (propertyName != String.Empty
				    && data != String.Empty)
				{
					string qs = "&" + propertyName + "=" + data;
					
				//	AppLogger.Debug("Adding query string: " + qs);
					
					newUrl = newUrl + qs;
					
				//	AppLogger.Debug("New url: " + newUrl);
				}
			}


			return newUrl;
		}
		
		/// <summary>
		/// Commits the URL rewrite for the current request. Uses HttpContext.Current.RewritePath(..., ...)
		/// </summary>
		/// <param name="url">The URL to rewrite to.</param>
		public void CommitRewrite(string url)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Committing rewrite to new URL.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Url: " + url);

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
			if (ConfigurationSettings.AppSettings != null)
				return Convert.ToBoolean(ConfigurationSettings.AppSettings["FriendlyUrls.Enabled"]);
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
