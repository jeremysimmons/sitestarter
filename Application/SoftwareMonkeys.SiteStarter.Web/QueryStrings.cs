using System;
using System.Collections.Generic;
using System.Web;
using System.Text;

namespace SoftwareMonkeys.SiteStarter.Web
{
    public class QueryStrings
    {
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
    }
}
