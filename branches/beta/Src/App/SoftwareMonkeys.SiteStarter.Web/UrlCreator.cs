using System;
using System.Web.UI;
using System.Collections;
using System.Web;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.State;
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
		// TODO: Check if standard URLs are needed. They should be obsolete and are no longer entirely supported
		
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
		protected string CurrentUrl
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
		
		private UrlConverter converter = null;
		public UrlConverter Converter
		{
			get {
				if (converter == null)
					converter = new UrlConverter(ApplicationPath);
				return converter; }
			set { converter  = value; }
		
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
		
		/// <summary>
		/// Sets the provided application path.
		/// </summary>
		/// <param name="applicationPath">The relative path to the root of the application.</param>
		/// <param name="enableFriendlyUrls"></param>
		/// <param name="currentUrl">The URL of the current request.</param>
		public UrlCreator(string applicationPath, bool enableFriendlyUrls, string currentUrl)
		{
			Initialize(applicationPath,
			           enableFriendlyUrls,
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
		
		#region Friendly URL functions
		
		/// <summary>
		/// Creates a friendly URL to the page corresponding with the specified action and specified type.
		/// </summary>
		/// <param name="action">The action to be performed at the target page.</param>
		/// <param name="typeName">The name of the type being acted upon at the target page.</param>
		/// <returns>The URL to the page handling the provided action in relation to the provided type.</returns>
		public string CreateFriendlyUrl(string action, string typeName)
		{
			string link = String.Empty;
			using (LogGroup logGroup = LogGroup.Start("Creating a friendly URL for the specified action and type.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Action: " + action);
				LogWriter.Debug("Type name: " + typeName);
				
				link = ApplicationPath + "/" + PrepareForUrl(action) + "-" + PrepareForUrl(typeName) + ".aspx";
				link = AddResult(link);
				
				LogWriter.Debug("Link: " + link);
			}
			return link;
		}
		
		/// <summary>
		/// Creates a friendly URL to the page corresponding with the specified action and specified type.
		/// </summary>
		/// <param name="action">The action to be performed at the target page.</param>
		/// <param name="typeName">The name of the type being acted upon at the target page.</param>
		/// <param name="propertyName">The name of the property to filter the type by.</param>
		/// <param name="value">The value of the property to filter the key by.</param>
		/// <returns>The URL to the page handling the provided action in relation to the provided type.</returns>
		public string CreateFriendlyUrl(string action, string typeName, string propertyName, string value)
		{
			string link = String.Empty;
			using (LogGroup logGroup = LogGroup.Start("Creating a friendly URL for the specified action and type, including the specified property name and value.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Action: " + action);
				LogWriter.Debug("Type name: " + typeName);
				
				LogWriter.Debug("Property name: " + propertyName);
				LogWriter.Debug("Value: " + value);
				
				link = ApplicationPath + "/" + PrepareForUrl(action) + "-" + PrepareForUrl(typeName);
			
				link = link + "/";
				
				if (propertyName == "UniqueKey")
					link = link + "K";
				else
					link = link + PrepareForUrl(propertyName);
				
				link = link + "--" + PrepareForUrl(value) + ".aspx";
								
				link = AddResult(link);
				
				LogWriter.Debug("Link: " + link);
			}
			return link;
		}
		
		/// <summary>
		/// Creates a friendly URL to the page corresponding with the specified action and specified type.
		/// </summary>
		/// <param name="action">The action to be performed at the target page.</param>
		/// <param name="typeName">The name of the type being acted upon at the target page.</param>
		/// <param name="propertyName">The name of the property to filter the type by.</param>
		/// <param name="entity">The entity to link to.</param>
		/// <returns>The URL to the page handling the provided action in relation to the provided type.</returns>
		public string CreateFriendlyUrl(string action, IEntity entity)
		{
			string link = String.Empty;
			using (LogGroup logGroup = LogGroup.Start("Creating a friendly URL for the specified action and provided entity type.", NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				LogWriter.Debug("Action: " + action);
				LogWriter.Debug("Entity type: " + entity.ShortTypeName);
				
				link = ApplicationPath
				+ "/" + PrepareForUrl(action)
				+ "-" + PrepareForUrl(entity.ShortTypeName);
			
				link = link + "/" + entity.ID.ToString();
				
				if (entity.ToString() != entity.GetType().FullName)
					link = link + "/" + PrepareForUrl(Utilities.Summarize(entity.ToString(), 80));
				
				link = link + ".aspx";
				
				link = AddResult(link);
				
				LogWriter.Debug("Link: " + link);
			}
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
			string link = String.Empty;
			using (LogGroup logGroup = LogGroup.Start("Creating a standard URL for the specified action and type, with the specified format.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Action: " + action);
				LogWriter.Debug("Type name: " + typeName);
				
				LogWriter.Debug("Format: " + format);
				
				link = ApplicationPath + "/Projector.aspx?a=" + PrepareForUrl(action) + "&t=" + PrepareForUrl(typeName) + "&f=" + PrepareForUrl(format.ToString());
				link = AddResult(link);
				
				LogWriter.Debug("Link: " + link);
			}
			return link;
		}
		
		public string CreateStandardUrl(string action, string typeName)
		{
			string link = String.Empty;
			using (LogGroup logGroup = LogGroup.Start("Creating a standard URL for the specified action and type.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Action: " + action);
				LogWriter.Debug("Type name: " + typeName);
				
				link = CreateStandardUrl(action, typeName, ProjectionFormat.Html);
				
				LogWriter.Debug("Link: " + link);
			}
			return link;
		}
		
		/// <summary>
		/// Creates a raw/standard URL to the page corresponding with the specified action and entity.
		/// </summary>
		/// <param name="action">The action to be performed at the target page.</param>
		/// <param name="entity">The entity involved in the action.</param>
		/// <returns>The URL to the page handling the provided action in relation to the provided type.</returns>
		public string CreateStandardUrl(string action, IEntity entity)
		{
			string link = String.Empty;
			using (LogGroup logGroup = LogGroup.Start("Creating a standard URL for the specified action and provided entity type.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Action: " + action);
				LogWriter.Debug("Entity type: " + entity.ShortTypeName);
				
				link = CreateStandardUrl(action, entity, ProjectionFormat.Html);
				
				LogWriter.Debug("Link: " + link);
			}
			return link;
		}
		
		/// <summary>
		/// Creates a raw/standard URL to the page corresponding with the specified action and entity.
		/// </summary>
		/// <param name="action">The action to be performed at the target page.</param>
		/// <param name="entity">The entity involved in the action.</param>
		/// <param name="format">The format of the target projection.</param>
		/// <returns>The URL to the page handling the provided action in relation to the provided type.</returns>
		public string CreateStandardUrl(string action, IEntity entity, ProjectionFormat format)
		{
			string link = String.Empty;
			using (LogGroup logGroup = LogGroup.Start("Creating a standard URL for the specified action and provided entity type, with the specified format.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Action: " + action);
				LogWriter.Debug("Entity type: " + entity.ShortTypeName);
				LogWriter.Debug("Format: " + format.ToString());
				
				link = ApplicationPath + "/Projector.aspx?a=" + PrepareForUrl(action) + "&t=" + PrepareForUrl(entity.ShortTypeName) + "&f=" + PrepareForUrl(format.ToString()) + "&" + PrepareForUrl(entity.ShortTypeName) + "-ID=" + PrepareForUrl(entity.ID.ToString());
				link = AddResult(link);
				
				LogWriter.Debug("Link: " + link);
			}
			return link;
		}
		
		
		/// <summary>
		/// Creates a raw/standard URL to the page corresponding with the specified action and specified type.
		/// </summary>
		/// <param name="action">The action to be performed at the target page.</param>
		/// <param name="typeName">The name of the type being acted upon at the target page.</param>
		/// <param name="propertyName">The name of the property to filter the specified type by.</param>
		/// <param name="value">The value of the property to filter the specified type by.</param>
		/// <param name="format">The format of the target projection.</param>
		/// <returns>The URL to the page handling the provided action in relation to the provided type.</returns>
		public string CreateStandardUrl(string action, string typeName, string propertyName, string value, ProjectionFormat format)
		{
			string link = String.Empty;
			using (LogGroup logGroup = LogGroup.Start("Creating a standard URL for the specified action and provided entity type, with the specified property name and value, as well as the specified format.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Action: " + action);
				LogWriter.Debug("Type name: " + typeName);
				
				LogWriter.Debug("Property name: " + propertyName);
				LogWriter.Debug("Value: " + value);
				
				LogWriter.Debug("Format: " + format.ToString());
				
				link = ApplicationPath + "/Projector.aspx?a=" + PrepareForUrl(action) + "&t=" + PrepareForUrl(typeName) + "&f=" + PrepareForUrl(format.ToString()) + "&" + PrepareForUrl(typeName) + "-" + PrepareForUrl(propertyName) + "=" + PrepareForUrl(value);
				link = AddResult(link);
				
				LogWriter.Debug("Link: " + link);
			}
			return link;
		}
		
		public string CreateStandardUrl(string action, string typeName, string propertyName, string value)
		{
			string link = String.Empty;
			using (LogGroup logGroup = LogGroup.Start("Creating a standard URL for the specified action and provided entity type, with the specified property name and value.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Action: " + action);
				LogWriter.Debug("Type name: " + typeName);
				
				LogWriter.Debug("Property name: " + propertyName);
				LogWriter.Debug("Value: " + value);
								
				link = CreateStandardUrl(action, typeName, propertyName, value, ProjectionFormat.Html);
				
				LogWriter.Debug("Link: " + link);
			}
			return link;
		}
		
		#endregion
		
		
		#region General URL functions
		
		/// <summary>
		/// Creates a URL to the specified projection.
		/// </summary>
		/// <param name="projection">The projection to create a URL to.</param>
		/// <returns>The URL to the specified projection.</returns>
		public virtual string CreateUrl(ProjectionInfo projection)
		{
			if (projection.TypeName != null && projection.TypeName != String.Empty
			    && projection.Action != null && projection.Action != String.Empty)
				return CreateUrl(projection.Action, projection.TypeName);
			else
				return CreateUrl(projection.Name);
		}
		
		/// <summary>
		/// Creates a URL to the specified projection.
		/// </summary>
		/// <param name="projectionName">The name of the projection.</param>
		/// <returns>The URL to the specified projection.</returns>
		public virtual string CreateUrl(string projectionName)
		{
			return StateAccess.State.ApplicationPath + "/" + projectionName + ".aspx";
		}
		
		/// <summary>
		/// Creates a URL to the specified action and type.
		/// </summary>
		/// <param name="action">The action to be performed by following the link.</param>
		/// <param name="type">The type that the action is dealing with.</param>
		/// <returns>The URL to the specified action and type.</returns>
		public virtual string CreateUrl(string action, Type type)
		{
			return CreateUrl(action, type.Name);
		}
		
		/// <summary>
		/// Creates a URL to the specified action and entity.
		/// </summary>
		/// <param name="action">The action to be performed by following the link.</param>
		/// <param name="entity">The entity involved in the action.</param>
		/// <returns>The URL to the specified action and entity.</returns>
		public virtual string CreateUrl(string action, IEntity entity)
		{
			if (EnableFriendlyUrls)
				return CreateFriendlyUrl(action, entity);
			else
				return CreateStandardUrl(action, entity);
		}
		
		/// <summary>
		/// Creates a URL to the specified action and type matching the specified property and value.
		/// </summary>
		/// <param name="action">The action to be performed by following the link.</param>
		/// <param name="typeName">The name of the type that the action is dealing with.</param>
		/// <returns>The URL to the specified action and entity.</returns>
		public virtual string CreateUrl(string action, string typeName)
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
		/// <returns>The URL to the specified action and entity..</returns>
		public virtual string CreateUrl(string action, Type type, Guid entityID)
		{
			return CreateUrl(action, type.Name, entityID);
		}

		/// <summary>
		/// Creates a URL to the specified action and type matching the specified property and value.
		/// </summary>
		/// <param name="action">The action to be performed by following the link.</param>
		/// <param name="typeName">The name of the type that the action is dealing with.</param>
		/// <param name="entityID">The value of the ID property to filter the type by.</param>
		/// <returns>The URL to the specified action and entity.</returns>
		public virtual string CreateUrl(string action, string typeName, Guid entityID)
		{
			return CreateUrl(action, typeName, "ID", entityID.ToString());
		}
		
		/// <summary>
		/// Creates a URL to the specified action and type matching the specified property and value.
		/// </summary>
		/// <param name="action">The action to be performed by following the link.</param>
		/// <param name="type">The type that the action is dealing with.</param>
		/// <param name="dataKey">The value of the UniqueKey property to filter the type by.</param>
		/// <returns>The URL to the specified action and entity.</returns>
		public virtual string CreateUrl(string action, Type type, string dataKey)
		{
			return CreateUrl(action, type.Name, dataKey);
		}

		/// <summary>
		/// Creates a URL to the specified action and type matching the specified property and value.
		/// </summary>
		/// <param name="action">The action to be performed by following the link.</param>
		/// <param name="typeName">The name of the type that the action is dealing with.</param>
		/// <param name="dataKey">The value of the UniqueKey property to filter the type by.</param>
		/// <returns>The URL to the specified action and entity.</returns>
		public virtual string CreateUrl(string action, string typeName, string dataKey)
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
		/// <returns>The URL to the specified action and entity.</returns>
		public virtual string CreateUrl(string action, string typeName, string propertyName, string dataKey)
		{
			string link = String.Empty;
			using (LogGroup logGroup = LogGroup.Start("Creating a link.", NLog.LogLevel.Debug))
			{
				if (EnableFriendlyUrls)
					link = CreateFriendlyUrl(action, typeName, propertyName, dataKey);
				else
					link = CreateStandardUrl(action, typeName, propertyName, dataKey);

				link = AddResult(link);
				
				LogWriter.Debug("Link: " + link);
			}

			return link;
		}
		#endregion
		
		#region XSLT URL functions
		/// <summary>
		/// Creates a URL to the specified XSLT file.
		/// </summary>
		/// <param name="action">The action being performed and rendering the XML.</param>
		/// <param name="typeName">The short type name of the entity involved in action.</param>
		/// <returns>The URL to the specified XSLT file.</returns>
		public string CreateXsltUrl(string action, string typeName)
		{
			string link = ApplicationPath + "/" + PrepareForUrl(action) + "-" + PrepareForUrl(typeName) + ".xslt.aspx";
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
			
			url = Converter.ToAbsolute(url);
			
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
				return ApplicationPath + "/" + PrepareForUrl(action) + "-" + PrepareForUrl(type) + ".xml.aspx";
			else
			{
				return ApplicationPath + "/XmlProjector.aspx?a=" + PrepareForUrl(action) + "&t=" + PrepareForUrl(type) + "&f=" + ProjectionFormat.Xml;
			}
		}

		#endregion
		
		#region Current URL functions
		
		public string CreateUrl()
		{
			string url = string.Empty;
			
			using (LogGroup logGroup = LogGroup.Start("Creating a URL.", NLog.LogLevel.Debug))
			{
				string action = QueryStrings.Action;
				string typeName = QueryStrings.Type;
				string uniqueKey = QueryStrings.GetUniqueKey(typeName);
				Guid id = QueryStrings.GetID(typeName);
				
				LogWriter.Debug("Action: " + action);
				LogWriter.Debug("Type name: " + typeName);
				LogWriter.Debug("Unique key: " + uniqueKey);
				LogWriter.Debug("ID: " + id.ToString());
				
				if (action == String.Empty ||
					typeName == String.Empty)
				{
					url = WebUtilities.ConvertAbsoluteUrlToApplicationRelativeUrl(HttpContext.Current.Request.Url.ToString());
				}
				else if (uniqueKey != String.Empty)
				{
					url = CreateUrl(action, typeName, "UniqueKey", uniqueKey);
				}
				else if (id != Guid.Empty)
				{
					url = CreateUrl(action, typeName, "ID", id.ToString());
				}
				else
					url = CreateUrl(action, typeName);
					
				LogWriter.Debug("URL: " + url);
			}
			return url;
		}
		#endregion
		
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
		/// Adds the current result message to the provided url.
		/// </summary>
		/// <param name="originalUrl">The url to add the result message to.</param>
		/// <returns>The full URL including the result message.</returns>
		public string AddResult(string originalUrl)
		{
			if (StateAccess.IsInitialized && Result.Text != String.Empty)
				return AddResult(originalUrl, Result.Text, Result.IsError);
			else
				return originalUrl;
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
			
			using (LogGroup logGroup = LogGroup.Start("Adding the result text to the link.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Link before: " + originalUrl);
				
				LogWriter.Debug("Result text: " + resultText);
				LogWriter.Debug("Result is error: " + resultIsError.ToString());
				
				// Don't add it if it's already found
				if (!ResultAlreadyExists(resultText))
				{
					LogWriter.Debug("Result doesn't yet exist. Adding.");
					
					if (resultText != String.Empty)
					{
						string separator = "?";
						if (originalUrl.IndexOf('?') > -1)
							separator = "&";
						
						LogWriter.Debug("Separator: " + separator);
						
						newUrl = newUrl + separator + "Result=" + PrepareForUrl(resultText) + "&ResultIsError=" + resultIsError.ToString();
					}
					else{
						LogWriter.Debug("Result text is String.Empty. Skipping add.");
					}
				}
				else
					LogWriter.Debug("Result already exists. Skipping add.");
				
				LogWriter.Debug("Link after: " + newUrl);
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
			using (LogGroup logGroup = LogGroup.Start("Checking whether the result already exists in the URL.", NLog.LogLevel.Debug))
			{
				Uri uri = new Uri(CurrentUrl);
				
				NameValueCollection qs = HttpUtility.ParseQueryString(uri.Query);
				
				LogWriter.Debug("Checking for result: " + resultText);
				
				LogWriter.Debug("Result found in query string: " + qs["Result"]);
				
				bool existsInQueryString = qs["Result"] != null
					&& qs["Result"] != String.Empty
					&& qs["Result"] == resultText;
				
				
				LogWriter.Debug("Result found in result control: " + Result.Text);
				
				bool existsInResultControl = Result.Text == resultText;
				
				LogWriter.Debug("Found result in existing URL query string: " + existsInQueryString.ToString());
				LogWriter.Debug("Found result in existing result control: " + existsInResultControl.ToString());
				
				exists = (existsInQueryString || existsInResultControl);
				
				LogWriter.Debug("Result exists: " + exists.ToString());
				
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
			text = EntitiesUtilities.FormatUniqueKey(text);
			return HttpUtility.UrlEncode(text);
		}
		
		/// <summary>
		/// Prepares the provided text for use in a query string.
		/// </summary>
		/// <param name="text">The text to prepare for use in a query string.</param>
		/// <returns>The prepared version of the provided text.</returns>
		public string PrepareForQueryString(string text)
		{
			// TODO: Check if any formatting needs to be applied.
			
			return text;
		}
	}
}
