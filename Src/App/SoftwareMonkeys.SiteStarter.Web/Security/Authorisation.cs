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
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Web.Security
{
	/// <summary>
	/// Contains functions to authorise specific actions of users.
	/// </summary>
	public class Authorisation
	{
		public static bool UserCan(string action, string typeName)
		{
			bool isAuthorised = false;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Checking whether the user can perform the action '" + action + "' with the entity type '" + typeName + "'."))
			{
				string internalAction = GetInternalAction(action);
				
				IAuthoriseStrategy strategy = StrategyState.Strategies.Creator.New<IAuthoriseStrategy>("Authorise" + internalAction, typeName);
				
				isAuthorised = strategy.Authorise(typeName);
			}
			
			return isAuthorised;
			
		}
		
		public static bool UserCan(string action, Type type)
		{
			bool isAuthorised = false;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Checking whether the user can perform the action '" + action + "' with the entity type '" + type.Name + "'."))
			{
				string internalAction = GetInternalAction(action);
				
				IAuthoriseStrategy strategy = StrategyState.Strategies.Creator.New<IAuthoriseStrategy>("Authorise" + internalAction, type.Name);
				
				isAuthorised = strategy.Authorise(type.Name);
			}
			
			return isAuthorised;
		}

		public static bool UserCan(string action, IEntity entity)
		{
			bool can = false;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Checking whether the user can perform the action '" + action + "' with the entity type '" + entity.ShortTypeName + "'."))
			{
				string internalAction = GetInternalAction(action);
				
				LogWriter.Debug("Internal action: " + internalAction);
				
				IAuthoriseStrategy strategy = StrategyState.Strategies.Creator.New<IAuthoriseStrategy>("Authorise" + internalAction, entity.GetType().Name);
				
				can = strategy.Authorise(entity);
			}
			
			return can;
			
		}

		public static bool UserCan(string action, IEntity[] entities)
		{
			bool can = false;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Checking whether the user can perform the action '" + action + "'."))
			{
				if (entities == null || entities.Length == 0)
				{
					can = true;
				}
				else
				{
					string internalAction = GetInternalAction(action);
					
					LogWriter.Debug("Internal action: " + internalAction);
					
					string shortTypeName = entities[0].ShortTypeName;
					
					List<IEntity> matching = new List<IEntity>();
					
					foreach (IEntity entity in entities)
					{
						if (UserCan(action, entity))
							matching.Add(entity);
					}
					
					entities = matching.ToArray();
					
					can = UserCan(action, shortTypeName);
				}
			}
			
			return can;
			
		}
		
		private static string GetInternalAction(string action)
		{
			string internalAction = action;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Getting the internal action that corresponds with the one provided."))
			{
				LogWriter.Debug("Action: " + action);
				
				switch (action)
				{
					case "View":
						internalAction = "Retrieve";
						break;
					case "Manage":
						internalAction = "Index";
						break;
					case "Edit":
						internalAction = "Update";
						break;
					case "Create":
						internalAction = "Save";
						break;
				}
			}
			
			return internalAction;
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
			LogWriter.Debug("Invalid permissions. Redirecting.");
			
			Result.DisplayError(Language.Unauthorised);
			
			// TODO: This shouldn't be hard coded.
			HttpContext.Current.Response.Redirect("~/User-SignIn.aspx?ReturnUrl=" + Authentication.GetUrl());
		}

		public static void EnsureIsAuthenticated()
		{
			LogWriter.Debug("Not authenticated. Redirecting.");
			
			Authentication.EnsureIsAuthenticated();
		}

		public static void EnsureIsInRole(string role)
		{
			if (!IsInRole(role))
			{
				Result.DisplayError(Language.InvalidRole);
				if (HttpContext.Current != null)
					HttpContext.Current.Response.Redirect(HttpContext.Current.Request.ApplicationPath + "/User-SignIn.aspx");
			}
		}

		static public bool IsInRole(string roleName)
		{
			bool isInRole = false;
			using (LogGroup logGroup = LogGroup.StartDebug("Checking whether the current user is in the specified role."))
			{
				if (!AuthenticationState.IsAuthenticated)
					isInRole = false;
				else
				{
					isInRole = AuthenticationState.UserIsInRole(roleName);
				}
			}
			return isInRole;
		}
	}
}