using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web;
using System.IO;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// Contains utility functions for the web site related parts of the application
	/// </summary>
	static public class WebUtilities
	{
		static public string ResolveUrl(string originalUrl)
		{

			if (!string.IsNullOrEmpty(originalUrl) && '~' == originalUrl[0])
			{
				int index = originalUrl.IndexOf('?');
				string queryString = (-1 == index) ? null : originalUrl.Substring(index);
				if (-1 != index) originalUrl = originalUrl.Substring(0, index);
				originalUrl = VirtualPathUtility.ToAbsolute(originalUrl) + queryString;
			}

			return originalUrl;

		}

		static public string ConvertAbsoluteUrlToApplicationRelativeUrl(string originalUrl)
		{
			return ConvertAbsoluteUrlToRelativeUrl(originalUrl, HttpContext.Current.Request.ApplicationPath);
		}

		static public string ConvertAbsoluteUrlToRelativeUrl(string originalUrl, string relatedPath)
		{
			string newPath = String.Empty;
			using (LogGroup logGroup = AppLogger.StartGroup("Converting the provided absolute URL to one that's relative to the one provided.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Original URL: " + originalUrl);
				AppLogger.Debug("Related path: " + relatedPath);
				
				if (relatedPath.ToLower().IndexOf("http://") == -1
				    && relatedPath.ToLower().IndexOf("https://") == -1)
				{
					relatedPath = ConvertRelativeUrlToAbsoluteUrl(relatedPath);
					
					AppLogger.Debug("Related path changed to absolute: " + relatedPath);
				}
				
				if (originalUrl.ToLower().IndexOf("http://") == -1
				    && originalUrl.ToLower().IndexOf("https://") == -1)
				{
					originalUrl = ConvertRelativeUrlToAbsoluteUrl(originalUrl);
					
					AppLogger.Debug("Original url changed to absolute: " + originalUrl);
				}
				
				string[] originalParts = originalUrl.Trim('/').Split('/');
				string[] relatedParts = relatedPath.Trim('/').Split('/');
				
				List<string> keptItems = new List<string>();
				
				for (int i = 0; i < originalParts.Length; i++)
				{
					if (i >= relatedParts.Length)
						keptItems.Add(originalParts[i]);
				}
				
				newPath = "/" + String.Join("/", keptItems.ToArray());
				/*
				Uri uri = null;
				try
				{
					uri = new Uri(relatedPath.TrimEnd('/'));
				}
				catch (UriFormatException ex)
				{
					throw new ArgumentException("Invalid relatedPath: " + relatedPath);
				}
				
				Uri uri2 = null;
				try
				{
					uri2 = new Uri(originalUrl);
				}
				catch (UriFormatException ex)
				{
					throw new ArgumentException("Invalid original url: " + originalUrl);
				}
				newPath = "/" + uri.MakeRelativeUri(uri2).ToString().TrimStart('/');//.Replace(ResolveUrl(HttpContext.Current.Request.ApplicationPath), "");
				*/
				/*string start = String.Empty;
				if (HttpContext.Current.Request.IsSecureConnection)
					start = "https://";
				else
					start = "http://";
				
				AppLogger.Debug("Start section: " + start);
				
				AppLogger.Debug("Port: " + HttpContext.Current.Request.Url.Port.ToString());

				string removable = start + HttpContext.Current.Request.Url.Host.TrimEnd('/');
				
				// If there are two instances of ":" character in the URL then a port must be specified
				// To detect if there are two instances we can compare the first and last instance. If one instance then it returns the same position. If two it returns two different positions.
				if (HttpContext.Current.Request.Url.ToString().IndexOf(":") != HttpContext.Current.Request.Url.ToString().LastIndexOf(":"))
					removable = removable + ":" + HttpContext.Current.Request.Url.Port;
				
				// TODO: Remove and clean up
				//.Port != 0
				   //&& HttpContext.Current.Request.Url.Port != 80)
				   
				removable = removable + relatedPath.TrimEnd('/') + "/";
				
				AppLogger.Debug("Removable: " + removable);
				
				newPath = "/" + absoluteUrl.Replace(removable, String.Empty).Trim('/');
				 */
				AppLogger.Debug("New path: " + newPath);
				
			}
			return newPath;
		}
		
		static public string ConvertRelativeUrlToAbsoluteUrl(string relativeUrl)
		{
			if (HttpContext.Current.Request.IsSecureConnection)
				return string.Format("https://{0}{1}", HttpContext.Current.Request.Url.Host, "/" + ResolveUrl(relativeUrl).Trim('/'));
			else
				return string.Format("http://{0}{1}", HttpContext.Current.Request.Url.Host, "/" + ResolveUrl(relativeUrl).Trim('/'));
		}
		
		/// <summary>
		/// Gets the config file name variation based on the provided URI.
		/// </summary>
		/// <param name="uri">The URI of the application.</param>
		/// <returns>The config file name variation.</returns>
		static public string GetLocationVariation(Uri uri)
		{
			// Declare the variation variable
			string variation = String.Empty;
			
			// If running on a local machine the variation is "local"
			if (uri.Host == "localhost" || uri.Host == "127.0.0.1")
				variation = "local";
			// If running on a staging site the variation is "staging"
			else if (uri.ToString().ToLower().IndexOf("staging") > -1)
				variation = "staging";
			// Otherwise
			// Leave the variation as String.Empty

			// Return the variation
			return variation;

		}
		
		public static string EncodeJsString(string s)
		{
			if (s == null)
				return String.Empty;
			
			StringBuilder sb = new StringBuilder();
			//sb.Append("\"");
			foreach (char c in s)
			{
				switch (c)
				{
					case '\"':
						sb.Append("\\\"");
						break;
					case '\\':
						sb.Append("\\\\");
						break;
					case '\b':
						sb.Append("\\b");
						break;
					case '\f':
						sb.Append("\\f");
						break;
					case '\n':
						sb.Append("\\n");
						break;
					case '\r':
						sb.Append("\\r");
						break;
					case '\t':
						sb.Append("\\t");
						break;
					default:
						int i = (int)c;
						if (i < 32 || i > 127)
						{
							sb.AppendFormat("\\u{0:X04}", i);
						}
						else
						{
							sb.Append(c);
						}
						break;
				}
			}
			//sb.Append("\"");

			return sb.ToString();
		}
	}
}
