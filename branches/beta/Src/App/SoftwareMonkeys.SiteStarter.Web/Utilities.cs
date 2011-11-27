using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Web
{
    /// <summary>
    /// Provides some utility functions.
    /// </summary>
    public class Utilities
    {
        /// <summary>
        /// Whether or not to hide the template.
        /// </summary>
        static public bool HideTemplate
        {
            get
            {
                if (HttpContext.Current.Request.QueryString["HideTemplate"] != null)
                    return Convert.ToBoolean(HttpContext.Current.Request.QueryString["HideTemplate"]);
                else
                    return false;
            }
        }

        /// <summary>
        /// Checks the provided value and returns "N/A" if the value is empty.
        /// </summary>
        /// <param name="val">The value to check.</param>
        /// <param name="secondaryText">The secondary text to use if the optional text is empty.</param>
        /// <returns>The value if it is not empty, otherwise "N/A"</returns>
        static public string OptionalText(object val, string secondaryText)
        {
            if (val != null && val.ToString().Trim().Length > 0)
                return val.ToString().Trim();
            else
                return secondaryText;
        }

        /// <summary>
        /// Retrieves the specified Guid from the query string.
        /// </summary>
        /// <param name="name">The name of the query string data.</param>
        /// <returns>The Guid retrieved from the query string.</returns>
        [Obsolete("Use QueryStrings.GetID(...)")]
        static public Guid GetQueryStringID(string name)
        {
        	return QueryStrings.GetID(name);
        }

        /// <summary>
        /// Formats the provided unit value ready for use in styles.
        /// </summary>
        /// <param name="value">The unit value to format.</param>
        /// <returns>The provided unit value ready for use in styles.</returns>
        static public string FormatUnit(Unit value)
        {
            string ext = String.Empty;
            if (value.Type == UnitType.Pixel)
                ext = "px";
            else if (value.Type == UnitType.Percentage)
                ext = "%";
            return value.Value + ext;
        }

        /// <summary>
        /// Summarizes the provided text to the specified number of charactes.
        /// </summary>
        /// <param name="text">The text to summarize.</param>
        /// <param name="characters">The maximum number of characters to keep from the text.</param>
        /// <returns>A summary of the provided text.</returns>
        static public string Summarize(string text, int characters)
        {
            if (text == null)
                return String.Empty;
            text = StripHTML(text);
            if (text.Length > characters)
            {
                text = text.Substring(0, characters) + "...";
            }
            return text;
        }

        /// <summary>
        /// Strips all the HTML from the provided text.
        /// </summary>
        /// <param name="val">The text to strip HTML from.</param>
        /// <returns>The provided text without any HTML tags.</returns>
        static public string StripHTML(string val)
        {
            return Regex.Replace(val, "<[^>]*>", "");
        }

        /// <summary>
        /// Whether or not the current page is the setup page.
        /// </summary>
        static public bool IsSetupPage
        {
            get
            {
                return HttpUtility.UrlEncode(HttpContext.Current.Request.Path.ToLower()).IndexOf(HttpUtility.UrlEncode("setup.aspx")) >= 0;
            }
        }

        /// <summary>
        /// Creates an output friendly version of the provided status.
        /// </summary>
        /// <param name="enabled">Whether or not to indicate enabled.</param>
        /// <returns>A text/html version of the provided status.</returns>
        static public string GetEnabled(bool enabled)
        {
            if (enabled)
                return "<font color='green'>Enabled</font>";
            else
                return "<font color='red'>Disabled</font>";
        }

        /// <summary>
        /// Adds the provided additional ID to the provided ID array and returns the combined array.
        /// </summary>
        /// <param name="ids">The original array of ids to append the addition to.</param>
        /// <param name="addition">The additional ID to append to the array.</param>
        /// <returns>The IDs with the additional one added.</returns>
        static public Guid[] AddID(Guid[] ids, Guid addition)
        {
            if (ids == null)
                return new Guid[] { addition };

            List<Guid> list = new List<Guid>(ids);
            if (!list.Contains(addition) && addition != Guid.Empty)
                list.Add(addition);

            return (Guid[])list.ToArray();
        }
        
        static public string GetVersion()
		{
			string version = String.Empty;
			string versionFile = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath + "/Version.Number");
			if (File.Exists(versionFile))
			{
				using (StreamReader reader = new StreamReader(File.OpenRead(versionFile)))
				{
					version = reader.ReadToEnd();
				}
			}
			
			return version;
		}
    }
}