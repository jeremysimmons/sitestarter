using System;
using System.Web;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// Utilities available in relation to style sheets.
	/// </summary>
	static public class StyleUtilities
	{
		/// <summary>
		/// Retrieves the code that references the specified style sheet name.
		/// </summary>
		/// <param name="styleSheetName"></param>
		/// <returns></returns>
		static public string GetStyleSheet(string styleSheetName)
		{
			// Add the application path to the beginning of the style sheet name
			styleSheetName = HttpContext.Current.Request.ApplicationPath + "/Styles/" + styleSheetName;
			
			// If the style sheet name doesn't include .css then add it.
			if (styleSheetName.ToLower().IndexOf(".css") == -1)
				styleSheetName = styleSheetName + ".css";
			
			return String.Format("<link href='{0}' type='text/css' rel='stylesheet' />", styleSheetName);
		}
	}
}
