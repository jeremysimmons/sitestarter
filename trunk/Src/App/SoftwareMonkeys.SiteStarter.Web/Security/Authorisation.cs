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
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Security
{
	/// <summary>
	/// Contains functions to authorise specific actions of users.
	/// </summary>
	public class Authorisation
	{
		public static bool UserCan(string action, string typeName)
		{
			string internalAction = GetInternalAction(action);
			
			IAuthoriseStrategy strategy = StrategyState.Strategies.Creator.New<IAuthoriseStrategy>("Authorise" + internalAction, typeName);
			
			return strategy.Authorise(typeName);
			
		}
		
		public static bool UserCan(string action, Type type)
		{
			string internalAction = GetInternalAction(action);
			
			IAuthoriseStrategy strategy = StrategyState.Strategies.Creator.New<IAuthoriseStrategy>("Authorise" + internalAction, type.Name);
			
			return strategy.Authorise(type.Name);
			
		}

		public static bool UserCan(string action, IEntity entity)
		{
			bool can = false;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Checking whether the user can perform the action '" + action + "' with the entity type '" + entity.ShortTypeName + "'."))
			{
				string internalAction = GetInternalAction(action);
				
				AppLogger.Debug("Internal action: " + internalAction);
				
				IAuthoriseStrategy strategy = StrategyState.Strategies.Creator.New<IAuthoriseStrategy>("Authorise" + internalAction, entity.GetType().Name);
				
				can = strategy.Authorise(entity.GetType().Name);
			}
			
			return can;
			
		}

		public static bool UserCan(string action, IEntity[] entities)
		{
			bool can = false;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Checking whether the user can perform the action '" + action + "'."))
			{
				if (entities == null || entities.Length == 0)
				{
					can = true;
				}
				else
				{
					string internalAction = GetInternalAction(action);
					
					AppLogger.Debug("Internal action: " + internalAction);
					
					string shortTypeName = entities[0].ShortTypeName;
					
					IAuthoriseStrategy strategy = StrategyState.Strategies.Creator.New<IAuthoriseStrategy>("Authorise" + internalAction, shortTypeName);
					
					can = strategy.Authorise(shortTypeName);
				}
			}
			
			return can;
			
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

		public static void EnsureUserCan(string action, string typeName)
		{
			if (!UserCan(action, typeName))
				InvalidPermissionsRedirect();
		}

		public static void EnsureUserCan(string action, Type type)
		{
			if (!UserCan(action, type))
				InvalidPermissionsRedirect();
		}

		
		public static void EnsureUserCan(string action, IEntity entity)
		{
			if (!UserCan(action, entity))
				InvalidPermissionsRedirect();
		}

		public static void EnsureUserCan(string action, IEntity[] entities)
		{
			if (!UserCan(action, entities))
				InvalidPermissionsRedirect();
		}
		
		public static void InvalidPermissionsRedirect()
		{
			Result.DisplayError(Language.Unauthorised);
			
			// TODO: This shouldn't be hard coded
			HttpContext.Current.Response.Redirect("~/User/SignIn.aspx");
		}

		public static void EnsureIsAuthenticated()
		{
			if (HttpContext.Current != null && !AuthenticationState.IsAuthenticated)
				HttpContext.Current.Response.Redirect(HttpContext.Current.Request.ApplicationPath + "/User/SignIn.aspx");
		}

		public static void EnsureIsInRole(string role)
		{
			if (!IsInRole(role))
			{
				Result.DisplayError(Language.InvalidRole);
				if (HttpContext.Current != null)
					HttpContext.Current.Response.Redirect(HttpContext.Current.Request.ApplicationPath + "/User/Account.aspx");
			}
		}

		static public bool IsInRole(string roleName)
		{
			if (!AuthenticationState.IsAuthenticated)
				return false;
			else
			{
				return AuthenticationState.UserIsInRole(roleName);
			}
		}
	}
}