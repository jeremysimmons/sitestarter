using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.State;
using System.Web;

namespace SoftwareMonkeys.SiteStarter.Web.State
{
    /// <summary>
    /// Provides easy access to information relating to the projects module.
    /// </summary>
    public class VirtualServerState
    {
        /// <summary>
        /// Gets/sets a boolean value indicating whether the projects module is enabled.
        /// </summary>
        static public bool IsEnabled
        {
            get { return SiteStarter.Configuration.Config.Application != null && SiteStarter.Configuration.Config.Application.EnableVirtualServer; }
        }

        /// <summary>
        /// Gets/sets a boolean value indicating whether a current project has been selected.
        /// </summary>
        static public bool VirtualServerSelected
        {
            get { return VirtualServerName != String.Empty && VirtualServerName.ToLower() != String.Empty; }
        }

        /// <summary>
        /// Gets/sets the name of the current server.
        /// </summary>
        static public string VirtualServerName
        {
            get { return (string)HttpContext.Current.Session["VirtualServerName"]; }
            set { HttpContext.Current.Session["VirtualServerName"] = value; }
        }

        /// <summary>
        /// Gets/sets the ID of the current server.
        /// </summary>
        static public string VirtualServerID
        {
            get { return (string)HttpContext.Current.Session["VirtualServerID"]; }
            set { HttpContext.Current.Session["VirtualServerID"] = value; }
        }
    }
}
