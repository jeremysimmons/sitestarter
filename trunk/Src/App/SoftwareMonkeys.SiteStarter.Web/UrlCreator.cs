using System;
using System.Web.UI;
using System.Collections;
using System.Web;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Web.WebControls;
using System.Configuration;
using System.Collections.Specialized;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Web.State;
using SoftwareMonkeys.SiteStarter.Web.Projections;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// Creates URLs for navigating around the application.
	/// </summary>
	public class UrlCreator
	{
		static private UrlCreator current;
		/// <summary>
		/// Gets a default URL creator.
		/// </summary>
		static public UrlCreator Current
		{
			get
			{
				if (current == null)
					current = new UrlCreator();
				return current;
			}
		}

		private string applicationPath;
		/// <summary>
		/// Gets/sets the relative path to the root of the application.
		/// </summary>
		public string ApplicationPath
		{
			get { return applicationPath; }
			set { applicationPath = value; }
		}
		
		private string currentUrl;
		/// <summary>
		/// Gets/sets the URL of the current request.
		/// </summary>
		public string CurrentUrl
		{
			get { return currentUrl; }
			set { currentUrl = value; }
		}
		
		private bool enableFriendlyUrls = true;
		/// <summary>
		/// Gest/sets a value indicating whether friendly URLs are enabled. (Using URL rewriting behind the scenes.)
		/// </summary>
		public bool EnableFriendlyUrls
		{
			get { return enableFriendlyUrls; }
			set { enableFriendlyUrls = value; }
		}
		
		public UrlCreator() : this(true)
		{
		}
		
		/// <summary>
		/// Initializes the URL creator.
		/// </summary>
		public UrlCreator(bool initialize)
		{
			if (initialize)
				Initialize();
		}
		
		/// <summary>
		/// Sets the provided application path.
		/// </summary>
		/// <param name="applicationPath">The relative path to the root of the application.</param>
		/// <param name="currentUrl">The URL of the current request.</param>
		public UrlCreator(string applicationPath, string currentUrl)
		{
			Initialize(applicationPath,
			           GetEnableFriendlyUrlsSetting(),
			           currentUrl);
		}
		
		public virtual void Initialize()
		{
			Initialize(
				HttpContext.Current.Request.ApplicationPath,
				GetEnableFriendlyUrlsSetting(),
				HttpContext.Current.Request.Url.ToString()
			);
		}
		
		public virtual void Initialize(string applicationPath, bool enableFriendlyUrls, string currentUrl)
		{
			ApplicationPath = applicationPath;
			EnableFriendlyUrls = enableFriendlyUrls;
			CurrentUrl = currentUrl;
		}
		
		// TODO: Remove if not needed
		/*/// <summary>
		/// Creates a URL to the page corresponding with the specified action and specified type.
		/// </summary>
		/// <param name="action">The action to be performed at the target page.</param>
		/// <param name="typeName">The name of the type being acted upon at the target page.</param>
		/// <returns>The URL to the page handling the provided action in relation to the provided type.</returns>
		public string CreateActionUrl(string action, string typeName)
		{
			if (enableFriendlyUrls)
				return CreateFriendlyActionUrl(action, typeName);
			else
				return CreateStandardActionUrl(action, typeName);
		}*/
			
			#region Friendly URL functions
			
			/// <summary>
			/// Creates a friendly URL to the page corresponding with the specified action and specified type.
			/// </summary>
			/// <param name="action">The action to be performed at the target page.</param>
			/// <param name="typeName">The name of the type being acted upon at the target page.</param>
			/// <returns>The URL to the page handling the provided action in relation to the provided type.</returns>
			public string CreateFriendlyUrl(string action, string typeName)
		{
			string link = ApplicationPath + "/" + PrepareForUrl(typeName) + "/" + PrepareForUrl(action) + ".aspx";
			link = AddResult(link);
			return link;
		}
		
		/// <summary>
		/// Creates a friendly URL to the page corresponding with the specified action and specified type.
		/// </summary>
		/// <param name="action">The action to be performed at the target page.</param>
		/// <param name="typeName">The name of the type being acted upon at the target page.</param>
		/// <param name="propertyName">The name of the property to filter the type by.</param>
		/// <param name="dataKey">The value of the property to filter the key by.</param>
		/// <returns>The URL to the page handling the provided action in relation to the provided type.</returns>
		public string CreateFriendlyUrl(string action, string typeName, string propertyName, string dataKey)
		{
			string link = ApplicationPath + "/" + PrepareForUrl(typeName) + "/" + PrepareForUrl(action);
			
			if (propertyName != "UniqueKey")
				link = link + "/" + PrepareForUrl(propertyName);
			
			link = link + "/" + PrepareForUrl(dataKey) + ".aspx";
			
			link = AddResult(link);
			return link;
		}
		
		#endregion
		
		#region Standard URL functions
		/// <summary>
		/// Creates a raw/standard URL to the page corresponding with the specified action and specified type.
		/// </summary>
		/// <param name="action">The action to be performed at the target page.</param>
		/// <param name="typeName">The name of the type being acted upon at the target page.</param>
		/// <param name="format">The format of the target projection.</param>
		/// <returns>The URL to the page handling the provided action in relation to the provided type.</returns>
		public string CreateStandardUrl(string action, string typeName, ProjectionFormat format)
		{
			
			string link = ApplicationPath + "/Projector.aspx?a=" + PrepareForUrl(action) + "&t=" + PrepareForUrl(typeName) + "&f=" + PrepareForUrl(format.ToString());
			link = AddResult(link);
			return link;
		}
		
		public string CreateStandardUrl(string action, string typeName)
		{
			return CreateStandardUrl(action, typeName, ProjectionFormat.Html);
		}
		
		
		/// <summary>
		/// Creates a raw/standard URL to the page corresponding with the specified action and specified type.
		/// </summary>
		/// <param name="action">The action to be performed at the target page.</param>
		/// <param name="typeName">The name of the type being acted upon at the target page.</param>
		/// <param name="propertyName">The name of the property to filter the specified type by.</param>
		/// <param name="dataKey">The value of the property to filter the specified type by.</param>
		/// <param name="format">The format of the target projection.</param>
		/// <returns>The URL to the page handling the provided action in relation to the provided type.</returns>
		public string CreateStandardUrl(string action, string typeName, string propertyName, ProjectionFormat format, string dataKey)
		{
			string link = ApplicationPath + "/Projector.aspx?a=" + PrepareForUrl(action) + "&t=" + PrepareForUrl(typeName) + "&f=" + PrepareForUrl(format.ToString()) + "&" + PrepareForUrl(typeName) + "-" + PrepareForUrl(propertyName) + "=" + PrepareForUrl(dataKey);
			link = AddResult(link);
			return link;
		}
		
		public string CreateStandardUrl(string action, string typeName, string propertyName, string dataKey)
		{
			return CreateStandardUrl(action, typeName, propertyName, ProjectionFormat.Html, dataKey);
		}
		
		#endregion
		
		
		#region General URL functions
		/// <summary>
		/// Creates a URL to the specified action and type matching the specified property and value.
		/// </summary>
		/// <param name="action">The action to be performed by following the link.</param>
		/// <param name="type">The type that the action is dealing with.</param>
		/// <returns>The rewritten raw URL to be used behind the scenes.</returns>
		public string CreateUrl(string action, Type type)
		{
			return CreateUrl(action, type.Name);
		}
		
		/// <summary>
		/// Creates a URL to the specified action and type matching the specified property and value.
		/// </summary>
		/// <param name="action">The action to be performed by following the link.</param>
		/// <param name="typeName">The name of the type that the action is dealing with.</param>
		/// <returns>The rewritten raw URL to be used behind the scenes.</returns>
		public string CreateUrl(string action, string typeName)
		{
			if (EnableFriendlyUrls)
				return CreateFriendlyUrl(action, typeName);
			else
				return CreateStandardUrl(action, typeName);
		}
		
		/// <summary>
		/// Creates a URL to the specified action and type matching the specified property and value.
		/// </summary>
		/// <param name="action">The action to be performed by following the link.</param>
		/// <param name="type">The type that the action is dealing with.</param>
		/// <param name="entityID">The value of the ID property to filter the type by.</param>
		/// <returns>The rewritten raw URL to be used behind the scenes.</returns>
		public string CreateUrl(string action, Type type, Guid entityID)
		{
			return CreateUrl(action, type.Name, entityID);
		}

		/// <summary>
		/// Creates a URL to the specified action and type matching the specified property and value.
		/// </summary>
		/// <param name="action">The action to be performed by following the link.</param>
		/// <param name="typeName">The name of the type that the action is dealing with.</param>
		/// <param name="entityID">The value of the ID property to filter the type by.</param>
		/// <returns>The rewritten raw URL to be used behind the scenes.</returns>
		public string CreateUrl(string action, string typeName, Guid entityID)
		{
			return CreateUrl(action, typeName, "ID", entityID.ToString());
		}
		
		/// <summary>
		/// Creates a URL to the specified action and type matching the specified property and value.
		/// </summary>
		/// <param name="action">The action to be performed by following the link.</param>
		/// <param name="type">The type that the action is dealing with.</param>
		/// <param name="dataKey">The value of the UniqueKey property to filter the type by.</param>
		/// <returns>The rewritten raw URL to be used behind the scenes.</returns>
		public string CreateUrl(string action, Type type, string dataKey)
		{
			return CreateUrl(action, type.Name, dataKey);
		}

		/// <summary>
		/// Creates a URL to the specified action and type matching the specified property and value.
		/// </summary>
		/// <param name="action">The action to be performed by following the link.</param>
		/// <param name="typeName">The name of the type that the action is dealing with.</param>
		/// <param name="dataKey">The value of the UniqueKey property to filter the type by.</param>
		/// <returns>The rewritten raw URL to be used behind the scenes.</returns>
		public string CreateUrl(string action, string typeName, string dataKey)
		{
			return CreateUrl(action, typeName, "UniqueKey", dataKey);
		}

		/// <summary>
		/// Creates a URL to the specified action and type matching the specified property and value.
		/// </summary>
		/// <param name="action">The action to be performed by following the link.</param>
		/// <param name="typeName">The name of the type that the action is dealing with.</param>
		/// <param name="propertyName">The name of the property to filter the type by.</param>
		/// <param name="dataKey">The value of the property to filter the type by.</param>
		/// <returns>The rewritten raw URL to be used behind the scenes.</returns>
		public virtual string CreateUrl(string action, string typeName, string propertyName, string dataKey)
		{
			string link = String.Empty;
			using (LogGroup logGroup = AppLogger.StartGroup("Creating a link to a module.", NLog.LogLevel.Debug))
			{
				if (EnableFriendlyUrls)
					link = CreateFriendlyUrl(action, typeName, propertyName, dataKey);
				else
					link = CreateStandardUrl(action, typeName, propertyName, dataKey);

				link = AddResult(link);
				
				AppLogger.Debug("Link: " + link);
			}

			return link;
		}
		#endregion
		
		#region XSLT URL functions
		/// <summary>
		/// Creates a URL to the specified XSLT file in the specified module.
		/// </summary>
		/// <param name="action">The action being performed and rendering the XML.</param>
		/// <param name="typeName">The short type name of the entity involved in action.</param>
		/// <returns>The URL to the specified XSLT file.</returns>
		public string CreateXsltUrl(string action, string typeName)
		{
			string link = ApplicationPath + "/" + PrepareForUrl(typeName) + "/" + PrepareForUrl(action) + ".xslt.aspx";
			link = AddResult(link);
			return link;
		}
		#endregion
		
		#region XML URL functions
		/// <summary>
		/// Creates an external URL to the XML page that handles the specified action and type.
		/// </summary>
		/// <param name="action">The action that is handled by the target page.</param>
		/// <param name="type">The type that the specified action deals with.</param>
		/// <returns>The external URL to the XML file.</returns>
		public string CreateExternalXmlUrl(string action, string type)
		{
			string url = CreateXmlUrl(action, type);
			
			Uri uri = new Uri(CurrentUrl);
			string host = uri.Host;
			bool isSecure = uri.Scheme == Uri.UriSchemeHttps;
			
			url = WebUtilities.ConvertRelativeUrlToAbsoluteUrl(url, host, isSecure);
			
			string separator = "?";
			if (url.IndexOf("?") > -1)
				separator = "&";
			
			//if (ProjectsState.IsEnabled
			//    && ProjectsState.ProjectSelected
			//    && type != "Project") // If the type is project then there's no need for the project ID
			//{
			//	url = url + separator + "ProjectID=" + ProjectsState.ProjectID.ToString();
			//}
			
			return url;
		}

		/// <summary>
		/// Creates a URL to the XML page that handles the specified action and type.
		/// </summary>
		/// <param name="action">The action that is handled by the target page.</param>
		/// <param name="type">The type that the specified action deals with.</param>
		/// <returns>The external URL to the XML file.</returns>
		public string CreateXmlUrl(string action, string type)
		{
			if (EnableFriendlyUrls)
				return ApplicationPath + "/" + PrepareForUrl(type) + "/" + PrepareForUrl(action) + ".xml.aspx";
			else
			{
				/*string moduleID = ModuleState.GetModuleID(action, type);
				string controlID = ModuleState.GetControlID(action, type);
				
				return ApplicationPath + "/XmlProjector.aspx?m=" + moduleID + "&cid=" + controlID + "&a=" + PrepareForUrl(action) + "&Type=" + type;*/
				
				return ApplicationPath + "/XmlProjector.aspx?a=" + PrepareForUrl(action) + "&t=" + PrepareForUrl(type) + "&f=" + ProjectionFormat.Xml;
			}
		}

		#endregion
		
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
		/// Adds the current result message to the provided url.
		/// </summary>
		/// <param name="originalUrl">The url to add the result message to.</param>
		/// <returns>The full URL including the result message.</returns>
		public string AddResult(string originalUrl)
		{
			return AddResult(originalUrl, Result.Text, Result.IsError);
		}
		
		/// <summary>
		/// Adds the current result message to the provided url.
		/// </summary>
		/// <param name="originalUrl">The url to add the result message to.</param>
		/// <param name="resultText">The result text to add to the URL.</param>
		/// <param name="resultIsError">A boolean value indicating whether the result is an error.</param>
		/// <returns>The full URL including the result message.</returns>
		public string AddResult(string originalUrl, string resultText, bool resultIsError)
		{
			string newUrl = originalUrl;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Adding the result text to the link.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Link before: " + originalUrl);
				
				AppLogger.Debug("Result text: " + resultText);
				AppLogger.Debug("Result is error: " + resultIsError.ToString());
				
				// Don't add it if it's already found
				if (!ResultAlreadyExists(resultText))
				{
					AppLogger.Debug("Result doesn't yet exist. Adding.");
					
					if (resultText != String.Empty)
					{
						string separator = "?";
						if (originalUrl.IndexOf('?') > -1)
							separator = "&";
						
						AppLogger.Debug("Separator: " + separator);
						
						newUrl = newUrl + separator + "Result=" + PrepareForUrl(resultText) + "&ResultIsError=" + resultIsError.ToString();
					}
					else{
						AppLogger.Debug("Result text is String.Empty. Skipping add.");
					}
				}
				else
					AppLogger.Debug("Result already exists. Skipping add.");
				
				AppLogger.Debug("Link after: " + newUrl);
			}
			return newUrl;
		}
		
		/// <summary>
		/// Checks whether the provided result message already exists in the current URL.
		/// </summary>
		/// <param name="resultText">The result text to check for.</param>
		/// <returns>A boolean value indicating whether the result text has already been used.</returns>
		public bool ResultAlreadyExists(string resultText)
		{
			bool exists = false;
			using (LogGroup logGroup = AppLogger.StartGroup("Checking whether the result already exists in the URL.", NLog.LogLevel.Debug))
			{
				Uri uri = new Uri(CurrentUrl);
				
				NameValueCollection qs = HttpUtility.ParseQueryString(uri.Query);
				
				AppLogger.Debug("Checking for result: " + resultText);
				
				AppLogger.Debug("Result found in query string: " + qs["Result"]);
				
				bool existsInQueryString = qs["Result"] != null
					&& qs["Result"] != String.Empty
					&& qs["Result"] == resultText;
				
				
				AppLogger.Debug("Result found in result control: " + Result.Text);
				
				bool existsInResultControl = Result.Text == resultText;
				
				AppLogger.Debug("Found result in existing URL query string: " + existsInQueryString.ToString());
				AppLogger.Debug("Found result in existing result control: " + existsInResultControl.ToString());
				
				exists = (existsInQueryString || existsInResultControl);
				
				AppLogger.Debug("Result exists: " + exists.ToString());
				
			}
			return exists;
		}

		/// <summary>
		/// Prepares the provided text for use in a URL.
		/// </summary>
		/// <param name="text">The text to prepare for use in a URL.</param>
		/// <returns>The prepared version of the provided text.</returns>
		public string PrepareForUrl(string text)
		{
			// The FormatUniqueKey takes care of characters that the UrlEncode function misses
			text = EntitiesUtilities.FormatUniqueKey(text);
			
			return HttpUtility.UrlEncode(text);
		}
	}
}
