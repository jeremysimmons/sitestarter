﻿
using System;
using System.Web;
using SoftwareMonkeys.SiteStarter.Web;
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
		
		public Control Parent;
		
		public Navigator(Control parent)
		{
			Parent = parent;
		}
		
		public string GetLink(string action, string type, string uniqueKey)
		{
			return GetLink(action, type, "UniqueKey", uniqueKey);
		}
		
		public string GetLink(string action, string type, string propertyName, string dataKey)
		{
			return UrlCreator.CreateUrl(action, type, propertyName, dataKey);
		}
		
		public string GetLink(string action, string type)
		{
			return UrlCreator.CreateUrl(action, type);
		}
		
		public string GetExternalLink(string action, string type)
		{
			return UrlCreator.CreateExternalUrl(action, type);
		}
			
		public string GetExternalLink(string action, string type, string propertyName, string value)
		{
			return UrlCreator.CreateExternalUrl(action, type, propertyName, value);
		}
		
		
		public string GetStandardLink(string action, string type)
		{
			
			return UrlCreator.CreateStandardUrl(action, type);
		}
		
		
		public virtual void Go(string action, string type, string uniqueKey)
		{
			
			string link = GetLink(action, type, uniqueKey);
			
			Redirect(link);
		}
		
		public virtual void Go(string action, string type, string propertyName, string dataKey)
		{
			
			string link = GetLink(action, type, propertyName, dataKey);
			
			Redirect(link);
		}
		
		public virtual void Go(string action, string type)
		{
			
			string link = GetLink(action, type);
			
			Redirect(link);
		}
		
		public virtual void Go(string action)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Redirecting to carry out the specified action.", NLog.LogLevel.Debug))
			{
				string type = GetEntityType();
				string link = GetLink(action, type);
				
				AppLogger.Debug("Type: " + type);
				AppLogger.Debug("Link: " + link);
				
				Redirect(link);
			}
			
		}
		
		public virtual void Redirect(string link)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Redirecting to the specified link.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Link: " + link);
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
	}
}
