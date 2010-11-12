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
using SoftwareMonkeys.SiteStarter.Business.Security;

namespace SoftwareMonkeys.SiteStarter.Web.Security
{
    /// <summary>
    /// Contains functions to authorise specific actions of users.
    /// </summary>
    public class Authorisation
    {
        public static bool UserCan(string action, Type type)
        {
        	if (!AuthenticationState.IsAuthenticated)
        		return false;
        	
        	string internalAction = GetInternalAction(action);
        	
        	IAuthoriseStrategy strategy = StrategyState.Strategies.Creator.New<IAuthoriseStrategy>("Authorise" + internalAction, type.Name);
        	
        	return strategy.Authorise(type.Name);
        	
        }

        public static bool UserCan(string action, IEntity entity)
        {
        	
        	if (!AuthenticationState.IsAuthenticated)
        		return false;
        	
        	string internalAction = GetInternalAction(action);
        	
        	IAuthoriseStrategy strategy = StrategyState.Strategies.Creator.New<IAuthoriseStrategy>("Authorise" + internalAction, entity.GetType().Name);
        	
        	return strategy.Authorise(entity.GetType().Name);
        	
        }

        public static bool UserCan(string action, IEntity[] entities)
        {
        	if (entities == null)
        		return true;
        	
        	if (!AuthenticationState.IsAuthenticated)
        		return false;
        	
        	string internalAction = GetInternalAction(action);
        	
        	Type type = entities.GetType().GetElementType();
        	
        	IAuthoriseStrategy strategy = StrategyState.Strategies.Creator.New<IAuthoriseStrategy>("Authorise" + internalAction, type.Name);
        	
        	return strategy.Authorise(type.Name);
        
        }
        
        private static string GetInternalAction(string action)
        {
        	switch (action)
        	{
        		case "View":
        			return "Retrieve";
        		case "Manage":
        			return "Index";
        		case "Edit":
        			return "Update";
        		case "Create":
        			return "Save";
        	}
        	
        	return action;
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
            if (HttpContext.Current != null && !AuthenticationState.IsAuthenticated)
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

            if (AuthenticationState.User == null)
                return false;
            else
            {
            	ActivateStrategy.New<User>().Activate(AuthenticationState.User, "Roles");

                foreach (UserRole role in AuthenticationState.User.Roles)
                {
                    if (role.Name == roleName)
                        return true;
                }

                return false;
            }
        }
    }
}