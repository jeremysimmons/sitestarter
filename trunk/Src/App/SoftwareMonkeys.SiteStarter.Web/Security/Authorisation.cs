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
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Web.WebControls;
using SoftwareMonkeys.SiteStarter.Web.Properties;

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

        public static bool UserCan(string action, IEntity entity)
        {
            // TODO: Add security
            if (HttpContext.Current.Request.IsAuthenticated
        || action == "View")
                return true;
            else
                return false;
        }

        public static bool UserCan(string action, IEntity[] entities)
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

        public static void EnsureUserCan(string action, IEntity entity)
        {
            if (!UserCan(action, entity))
                InvalidPermissionsRedirect();
            //    throw new Exception("Invalid permissions.");
            // TODO: Add redirect to friendly page
        }

        public static void EnsureUserCan(string action, IEntity[] entities)
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

        public static void EnsureIsAuthenticated()
        {
            if (HttpContext.Current != null && !HttpContext.Current.Request.IsAuthenticated)
                HttpContext.Current.Response.Redirect(HttpContext.Current.Request.ApplicationPath + "/Members/Login.aspx");
        }

        public static void EnsureIsInRole(string role)
        {
            if (!IsInRole(role))
            {
                Result.DisplayError(Language.InvalidRole);
                if (HttpContext.Current != null)
                    HttpContext.Current.Response.Redirect(HttpContext.Current.Request.ApplicationPath + "/Members/Default.aspx");
            }
        }

        static public bool IsInRole(string roleName)
        {
            /*foreach (UserRole role in My.User.Roles)
            {
                if (role.Name == roleName)
                    return true;
            }*/
            /*if (HttpContext.Current == null)
                return false;

            if (!HttpContext.Current.Request.IsAuthenticated)
                return false;
            else
                return HttpContext.Current.User.IsInRole(roleName);*/

            //return false;

            if (My.User == null)
                return false;
            else
            {
                UserFactory.Current.Activate(My.User, "Roles");

                foreach (UserRole role in My.User.Roles)
                {
                    if (role.Name == roleName)
                        return true;
                }

                return false;
            }
        }
    }
}