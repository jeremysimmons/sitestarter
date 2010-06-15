using System;
using System.Collections.Generic;
using System.Web;
using System.Text;

namespace SoftwareMonkeys.SiteStarter.Web
{
    public class QueryStrings
    {
    	
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
        /// The module passed to the page via the query string variable "m".
        /// </summary>
        static public string Module
        {
            get
            {
                if (HttpContext.Current.Request.QueryString["m"] != null)
                    return HttpContext.Current.Request.QueryString["m"];
                else
                    return String.Empty;
            }
        }

        /// <summary>
        /// The control ID passed to the page via the query string variable "cid".
        /// </summary>
        static public string ControlID
        {
            get
            {
                if (HttpContext.Current.Request.QueryString["cid"] != null)
                    return HttpContext.Current.Request.QueryString["cid"];
                else
                    return String.Empty;
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
                		-1;
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