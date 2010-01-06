using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// Contains utility functions for the web site related parts of the application
	/// </summary>
	static public class WebUtilities
	{
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
