using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web;
using System.IO;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// Contains utility functions for the web site related parts of the application
	/// </summary>
	[Obsolete("Use UrlConverter")]
	static public class WebUtilities
	{
		[Obsolete("Use UrlConverter.ResolveUrl(string)")]
		static public string ResolveUrl(string originalUrl)
		{
			return new UrlConverter().ResolveUrl(originalUrl);

		}

		[Obsolete]
		static public string ConvertApplicationRelativeUrlToAbsoluteUrl(string relativeUrl)
		{
			return new UrlConverter().ToAbsolute(relativeUrl);
		}
		
		[Obsolete]
		static public string ConvertAbsoluteUrlToApplicationRelativeUrl(string originalUrl)
		{
			return ConvertAbsoluteUrlToRelativeUrl(originalUrl, HttpContext.Current.Request.ApplicationPath);
		}

		[Obsolete("Use UrlConverter.ToRelative")]
		static public string ConvertAbsoluteUrlToRelativeUrl(string originalUrl, string relatedPath)
		{
			return new UrlConverter(relatedPath).ToRelative(originalUrl);
		}
		
		/// <summary>
		/// Converts the provided relative URL to an absolute URL.
		/// </summary>
		/// <param name="relativeUrl">The relative URL to convert.</param>
		/// <returns>An absolute version of the provided URL.</returns>
		[Obsolete("Use UrlConverter.ToRelative")]
		static public string ConvertRelativeUrlToAbsoluteUrl(string relativeUrl)
		{
			return new UrlConverter(StateAccess.State.ApplicationPath).ToAbsolute(relativeUrl);
		}
		
		// TODO: Move to another class
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
		
		// TODO: Move to another class
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
