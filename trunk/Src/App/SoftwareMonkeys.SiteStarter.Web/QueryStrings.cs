using System;
using System.Collections.Generic;
using System.Web;
using System.Text;
using SoftwareMonkeys.SiteStarter.Web.Projections;

namespace SoftwareMonkeys.SiteStarter.Web
{
    public class QueryStrings
    {
    	
        /// <summary>
        /// Gets a flag indicating whether the query strings are available. Note: This is determined by checking whther HttpContext.Current.Request is available.
        /// </summary>
        static public bool Available
        {
            get
            {
                return HttpContext.Current != null
                	&& HttpContext.Current.Request != null;
            }
        }
    	
    	
        /// <summary>
        /// The type passed to the page via the query string variable "Sort".
        /// </summary>
        static public string Sort
        {
            get
            {
                if (HttpContext.Current.Request.QueryString["Sort"] != null)
                    return HttpContext.Current.Request.QueryString["Sort"];
                else
                    return String.Empty;
            }
        }
    	
        /// <summary>
        /// The type passed to the page via the query string variable "t".
        /// </summary>
        static public string Type
        {
            get
            {
                if (HttpContext.Current.Request.QueryString["t"] != null)
                    return HttpContext.Current.Request.QueryString["t"];
                else
                    return String.Empty;
            }
        }
        
        /// <summary>
        /// The action passed to the page via the query string variable "a".
        /// </summary>
        static public string Action
        {
            get
            {
                if (HttpContext.Current.Request.QueryString["a"] != null)
                    return HttpContext.Current.Request.QueryString["a"];
                else
                    return String.Empty;
            }
        }
        
        /// <summary>
        /// The action passed to the page via the query string variable "a".
        /// </summary>
        static public ProjectionFormat Format
        {
            get
            {
                if (HttpContext.Current.Request.QueryString["f"] != null)
                	return (ProjectionFormat)Enum.Parse(typeof(ProjectionFormat), HttpContext.Current.Request.QueryString["f"]);
                else
                    return ProjectionFormat.NotSet;
            }
        }
        
        /// <summary>
        /// The index of the current page.
        /// Note: This is adjusted from page number back to page index by decrementing by 1.
        /// </summary>
        static public int PageIndex
        {
            get
            {
                if (HttpContext.Current.Request.QueryString["Page"] != null)
                	return Convert.ToInt32(HttpContext.Current.Request.QueryString["Page"])
                		-1; // Decrement by one to convert page number into 0 based page index
                else
                    return 0;
            }
        }
        
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
        
        
		static public string GetUniqueKey(string typeName)
		{
			if (HttpContext.Current.Request.QueryString[typeName + "-UniqueKey"] != null)
				return HttpContext.Current.Request.QueryString[typeName + "-UniqueKey"];
			else if (HttpContext.Current.Request.QueryString[typeName + "UniqueKey"] != null)
				return HttpContext.Current.Request.QueryString[typeName + "UniqueKey"];
			else if (HttpContext.Current.Request.QueryString["UniqueKey"] != null)
				return HttpContext.Current.Request.QueryString["UniqueKey"];
			else
				return String.Empty;
		}
		
		
		static public Guid GetID(string typeName)
		{
			string value = String.Empty;
			
			if (HttpContext.Current.Request.QueryString[typeName + "-ID"] != null)
				value = HttpContext.Current.Request.QueryString[typeName + "-ID"];
			else if (HttpContext.Current.Request.QueryString[typeName + "ID"] != null)
				value = HttpContext.Current.Request.QueryString[typeName + "ID"];
			else if (HttpContext.Current.Request.QueryString["ID"] != null)
				value = HttpContext.Current.Request.QueryString["ID"];
			else
				value = String.Empty;
			
			if (value != String.Empty)
				return new Guid(value);
			else
				return Guid.Empty;
		}
        
    }
}