using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Web.Security
{
    /// <summary>
    /// Contains functions to authorise specific actions of users.
    /// </summary>
    public class Authorisation
    {
           public static bool UserCan(string action, Type type)
        {
            // TODO: Add security
            if (HttpContext.Current.Request.IsAuthenticated
		|| action == "View")
		return true;
		else
		return false;
        }

        public static bool UserCan(string action, BaseEntity entity)
        {
            // TODO: Add security
            if (HttpContext.Current.Request.IsAuthenticated
		|| action == "View")
		return true;
		else
		return false;
        }

        public static bool UserCan(string action, BaseEntity[] entities)
        {
            // TODO: Add security
            if (HttpContext.Current.Request.IsAuthenticated
		|| action == "View")
		return true;
		else
		return false;
        }

        public static void EnsureUserCan(string action, Type type)
        {
            if (!UserCan(action, type))
		InvalidPermissionsRedirect();
//                throw new Exception("Invalid permissions.");
            // TODO: Add redirect to friendly page
        }

        public static void EnsureUserCan(string action, BaseEntity entity)
        {
            if (!UserCan(action, entity))
		InvalidPermissionsRedirect();
            //    throw new Exception("Invalid permissions.");
            // TODO: Add redirect to friendly page
        }

        public static void EnsureUserCan(string action, BaseEntity[] entities)
        {
            if (!UserCan(action, entities))
		InvalidPermissionsRedirect();
            //    throw new Exception("Invalid permissions.");
            // TODO: Add redirect to friendly page
        }

		public static void InvalidPermissionsRedirect()
		{
			// TODO: This shouldn't be hard coded
			HttpContext.Current.Response.Redirect("~/Members/Login.aspx");
		}
    }
}