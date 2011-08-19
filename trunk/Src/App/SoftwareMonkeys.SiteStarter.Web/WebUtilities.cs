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
			if (originalUrl == null)
				originalUrl = String.Empty;

			if (!string.IsNullOrEmpty(originalUrl) && '~' == originalUrl[0])
			{
				int index = originalUrl.IndexOf('?');
				string queryString = (-1 == index) ? null : originalUrl.Substring(index);
				if (-1 != index) originalUrl = originalUrl.Substring(0, index);
				originalUrl = VirtualPathUtility.ToAbsolute(originalUrl) + queryString;
			}

			return originalUrl;

		}

		/// <summary>

		/// Converts the provided relative URL to an absolute URL.

		/// </summary>

		/// <param name="relativeUrl">The relative URL to convert.</param>

		/// <param name="host">The base URL host.</param>

		/// <param name="isSecure">A boolean value indicating whether the URL is a secure path.</param>

		/// <returns></returns>

		static public string ConvertRelativeUrlToAbsoluteUrl(string relativeUrl, string host, int port, bool isSecure)

		{

			string protocol = (isSecure ? "https" : "http");

			

			if (port != 80)

				host = host + ":" + port.ToString();

			

			return string.Format("{0}://{1}/{2}", protocol, host, ResolveUrl(relativeUrl).Trim('/'));

		}

		static public string ConvertApplicationRelativeUrlToAbsoluteUrl(string relativeUrl)
		{
			string applicationPath = HttpContext.Current.Request.ApplicationPath.TrimEnd('/');
			return ConvertRelativeUrlToAbsoluteUrl(applicationPath.TrimEnd('/') + "/" + relativeUrl.TrimStart('/'));
		}
		
		static public string ConvertAbsoluteUrlToApplicationRelativeUrl(string originalUrl)
		{
			return ConvertAbsoluteUrlToRelativeUrl(originalUrl, HttpContext.Current.Request.ApplicationPath);
		}

		static public string ConvertAbsoluteUrlToRelativeUrl(string originalUrl, string relatedPath)
		{
			string newPath = String.Empty;
			using (LogGroup logGroup = LogGroup.Start("Converting the provided absolute URL to be relative to the one provided.", NLog.LogLevel.Debug))
			{
				if (originalUrl == null)
					throw new ArgumentNullException("originalUrl");
				
				if (relatedPath == null)
					throw new ArgumentNullException("relatedPath");
				
				LogWriter.Debug("Original URL: " + originalUrl);
				LogWriter.Debug("Related path: " + relatedPath);
				
				if (relatedPath.ToLower().IndexOf("http://") == -1
				    && relatedPath.ToLower().IndexOf("https://") == -1)
				{
					relatedPath = ConvertRelativeUrlToAbsoluteUrl(relatedPath);
					
					LogWriter.Debug("Related path changed to absolute: " + relatedPath);
				}
				
				if (originalUrl.ToLower().IndexOf("http://") == -1
				    && originalUrl.ToLower().IndexOf("https://") == -1)
				{
					originalUrl = ConvertRelativeUrlToAbsoluteUrl(originalUrl);
					
					LogWriter.Debug("Original url changed to absolute: " + originalUrl);
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

				LogWriter.Debug("New path: " + newPath);
				
			}
			return newPath;
		}
		
		/// <summary>
		/// Converts the provided relative URL to an absolute URL.
		/// </summary>
		/// <param name="relativeUrl">The relative URL to convert.</param>
		/// <returns>An absolute version of the provided URL.</returns>
		static public string ConvertRelativeUrlToAbsoluteUrl(string relativeUrl)
		{
			return ConvertRelativeUrlToAbsoluteUrl(relativeUrl, HttpContext.Current.Request.Url.Host, HttpContext.Current.Request.IsSecureConnection);
		}
		
		/// <summary>
		/// Converts the provided relative URL to an absolute URL.
		/// </summary>
		/// <param name="relativeUrl">The relative URL to convert.</param>
		/// <param name="host">The base URL host.</param>
		/// <param name="isSecure">A boolean value indicating whether the URL is a secure path.</param>
		/// <returns></returns>
		static public string ConvertRelativeUrlToAbsoluteUrl(string relativeUrl, string host, bool isSecure)
		{
			string protocol = (isSecure ? "https" : "http");
			
			return string.Format("{0}://{1}/{2}", protocol, host, ResolveUrl(relativeUrl).Trim('/'));
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
			if (uri.Host == "localhost" || uri.Host == "127.0.0.1" || uri.Host == "10.0.0.1")
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
					case '\'':
						sb.Append("\\\'");
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
