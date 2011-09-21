
using System;
using System.Web;
using SoftwareMonkeys.SiteStarter.Web;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Web.UI;

namespace SoftwareMonkeys.SiteStarter.Web.Navigation
{
	/// <summary>
	/// Description of Navigator.
	/// </summary>
	public class Navigator
	{
		private UrlCreator urlCreator;
		/// <summary>
		/// Gets the URL creator used to create URLs.
		/// </summary>
		public UrlCreator UrlCreator
		{
			get {
				if (urlCreator == null)
					urlCreator = new UrlCreator();
				return urlCreator; }
			set { urlCreator = value; }
		}
		
		static private Navigator current;
		static public Navigator Current
		{
			get {
				if (current == null)
					current = new Navigator();
				return current; }
		}
		
		public Control Parent;
		
		public Navigator(Control parent)
		{
			Parent = parent;
		}
		
		public Navigator()
		{
		}
		
		public string GetCurrentLink()
		{
			string url = String.Empty;
			
			if (HttpContext.Current.Items.Contains("FriendlyUrl"))
				url = (string)HttpContext.Current.Items["FriendlyUrl"];
			else
				url = HttpContext.Current.Request.Url.ToString();
			
			return url;
		}
		
		public string GetLink()
		{
			return UrlCreator.CreateUrl();
		}
		
		public string GetLink(string action, string type, string uniqueKey)
		{
			return GetLink(action, type, "UniqueKey", uniqueKey);
		}
		
		public string GetLink(string action, IEntity entity)
		{
			return UrlCreator.CreateUrl(action, entity);
		}
		
		public string GetLink(string action, string type, string propertyName, string dataKey)
		{
			return UrlCreator.CreateUrl(action, type, propertyName, dataKey);
		}
		
		public string GetLink(string action, string type)
		{
			return UrlCreator.CreateUrl(action, type);
		}
		
		public string GetStandardLink(string action, string type)
		{
			
			return UrlCreator.CreateStandardUrl(action, type);
		}
		
		public virtual void Go(string projectionName)
		{
			using (LogGroup logGroup = LogGroup.Start("Navigating.", NLog.LogLevel.Debug))
			{
				string link = new UrlCreator().CreateUrl(projectionName);
				
				LogWriter.Debug("Link: " + link);
				
				Redirect(link);
			}
		}
		
		public virtual void Go(string action, string type, string uniqueKey)
		{
			
			using (LogGroup logGroup = LogGroup.Start("Navigating.", NLog.LogLevel.Debug))
			{
				string link = GetLink(action, type, uniqueKey);
				
				LogWriter.Debug("Link: " + link);
				
				Redirect(link);
			}
		}
		
		public virtual void Go(string action, string type, string propertyName, string dataKey)
		{
			using (LogGroup logGroup = LogGroup.Start("Navigating.", NLog.LogLevel.Debug))
			{
				
				string link = GetLink(action, type, propertyName, dataKey);
				
				LogWriter.Debug("Link: " + link);
				
				Redirect(link);
			}
		}
		
		public virtual void Go(string action, string type)
		{
			using (LogGroup logGroup = LogGroup.Start("Navigating.", NLog.LogLevel.Debug))
			{
				string link = GetLink(action, type);
				
				LogWriter.Debug("Link: " + link);
				
				Redirect(link);
			}
		}
		
		public virtual void Go(string action, IEntity entity)
		{
			
			using (LogGroup logGroup = LogGroup.Start("Navigating.", NLog.LogLevel.Debug))
			{
				string link = GetLink(action, entity);
				
				LogWriter.Debug("Link: " + link);
				
				Redirect(link);
			}
		}

		public virtual void Redirect(string link)
		{
			using (LogGroup logGroup = LogGroup.Start("Redirecting to the specified link.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Link: " + link);
				//string rewrittenUrl = CloakHandler.RewriteUrl(link, HttpContext.Current.Request.ApplicationPath);
				//HttpContext.Current.Server.Transfer(link, false);
				
				//Parent.Page.Response.Redirect(link, false);
				HttpContext.Current.Response.Redirect(link, true);
			}
		}
		
		public virtual string GetEntityType()
		{
			return HttpContext.Current.Request.QueryString["t"];
		}
		
		/// <summary>
		/// Actions that display multiple entities.
		/// </summary>
		public string[] IndexActions = new String[]{
			"Index"
		};
		
		public bool IsIndexAction(string action)
		{
			return Array.IndexOf(IndexActions, action) > -1;
		}
		
		public void NavigateAfterOperation(CommandInfo command, IEntity entity)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Navigating after a save operation."))
			{
				if (command == null)
					throw new ArgumentNullException("command");
				
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				LogWriter.Debug("Action: " + command.Action);
				LogWriter.Debug("Command type name: " + command.TypeName);
				LogWriter.Debug("Entity type name: " + entity.ShortTypeName);
				
				// If the type matches the current entity type then pass the actual entity to the navigate function
				// so that the ID can be passed through as part of the URL.
				if (command.TypeName == entity.ShortTypeName
				    && !IsIndexAction(command.Action)) // And if the action is NOT an index action (which doesn't need the entity ID in the URL)
				{
					LogWriter.Debug("Entity specific command.");
					NavigateAfterOperation(command.Action, entity);
				}
				else
				{
					LogWriter.Debug("Index command.");
					NavigateAfterOperation(command.Action, command.TypeName);
				}
			}
		}
		
		public void NavigateAfterOperation(string action, IEntity entity)
		{
			string url = String.Empty;
			
			using (LogGroup logGroup = LogGroup.Start("Navigating after performing an operation.", NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				if (action == null)
					throw new ArgumentNullException("action");
				
				if (action == string.Empty)
					throw new ArgumentException("An action must be provided.");
				
				LogWriter.Debug("Action: " + action);
				
				if (IsIndexAction(action))
				{
					url = UrlCreator.Current.CreateUrl(action, entity.ShortTypeName);
				}
				else
				{
					url = UrlCreator.Current.CreateUrl(action, entity);
				}
				
				LogWriter.Debug("URL: " + url);
			}
			
			// Redirect (outside the log group)
			HttpContext.Current.Response.Redirect(url);
		}
		
		public void NavigateAfterOperation(string action, string typeName)
		{
			string url = String.Empty;
			
			using (LogGroup logGroup = LogGroup.Start("Navigating after performing an operation.", NLog.LogLevel.Debug))
			{
				if (typeName == null)
					throw new ArgumentNullException("typeName");
				
				if (action == null)
					throw new ArgumentNullException("action");
				
				if (action == string.Empty)
					throw new ArgumentException("An action must be provided.");
				
				LogWriter.Debug("Action: " + action);
				
				url = UrlCreator.Current.CreateUrl(action, typeName);
				
				LogWriter.Debug("URL: " + url);
			}
			
			// Redirect (outside the log group)
			HttpContext.Current.Response.Redirect(url);
		}
	}
}
