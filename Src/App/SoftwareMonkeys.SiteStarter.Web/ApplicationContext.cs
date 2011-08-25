using System;
using System.Web.Routing;
using System.Web;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Web;
using SoftwareMonkeys.SiteStarter.Web.Data;
using SoftwareMonkeys.SiteStarter.Web.Projections;
using SoftwareMonkeys.SiteStarter.Web.Controllers;
using SoftwareMonkeys.SiteStarter.Web.Parts;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// The base class for the Global.asax file.
	/// </summary>
	public class ApplicationContext : System.Web.HttpApplication
	{
		
		void Application_Start(object sender, EventArgs e)
		{	
			// Initialze the core state management and diagnostics
			InitializeCore();
			
			using (LogGroup logGroup = LogGroup.StartDebug("Starting application."))
			{
				LogWriter.Debug("${Application.Start}");
				
				// Initialze the entire application
				Initialize();
				
			}
		}
		
		void Application_End(object sender, EventArgs e)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Ending application."))
			{
				LogWriter.Debug("${Application.End}");
			}

			//  Dispose outside the log group because all logging needs to be finished
			Dispose();
		}
		
		void Application_Error(object sender, EventArgs e)
		{
			using (LogGroup logGroup = LogGroup.Start("Application error.", NLog.LogLevel.Error))
			{
				LogWriter.Debug("${Application.Error}");
				
				Exception lastException = Server.GetLastError();
				
				ExceptionHandler handler = new ExceptionHandler();
				handler.Handle(lastException);
			}
		}

		void Session_Start(object sender, EventArgs e)
		{
			InitializeCore();
			
			// Create a log group for the session
			HttpContext.Current.Session["Session_Start.LogGroup"] = LogGroup.StartDebug("Starting session.");
			
			LogWriter.Debug("${Session.Start}");
			
			// Attempt to initialize the config
			Initialize();
		}

		void Session_End(object sender, EventArgs e)
		{
			LogWriter.Debug("${Session.End}");
			
			HttpContext.Current.Session["Session_Start.LogGroup"] = null;
		}
		
		void Application_BeginRequest(object sender, EventArgs e)
		{
			InitializeCore();
			
			// Create a log group for the request
			HttpContext.Current.Items["Application_BeginRequest.LogGroup"] = LogGroup.StartDebug("Beginning request: " + HttpContext.Current.Request.Url.ToString());

			LogWriter.Debug("${Application.BeginRequest}");
			
			Initialize();
		}
		
		
		void Application_EndRequest(object sender, EventArgs e)
		{
			LogWriter.Debug("${Application.EndRequest}");
			
			new AutoBackupInitializer().Initialize();
			
			HttpContext.Current.Items["Application_BeginRequest.LogGroup"] = null;
		}

		private void InitializeCore()
		{
			InitializeState();
		}

		private void Initialize()
		{
			using (LogGroup logGroup = LogGroup.Start("Initializing the config, entities, data, business, and web components", LogLevel.Debug))
			{
				Config.Initialize(Server.MapPath(HttpContext.Current.Request.ApplicationPath), WebUtilities.GetLocationVariation(HttpContext.Current.Request.Url));
				InitializeEntities();
				InitializeData();
				InitializeBusiness();
				InitializeWeb();
			}
		}

		private void InitializeEntities()
		{
			if (Config.IsInitialized)
				new EntityInitializer().Initialize();
		}
		
		private void InitializeData()
		{
			new DataProviderInitializer().Initialize();
		}
		
		private void InitializeBusiness()
		{
			if (Config.IsInitialized)
			{
				new StrategyInitializer().Initialize();
				new ReactionInitializer().Initialize();
			}
		}
		
		private void InitializeWeb()
		{
			if (Config.IsInitialized)
			{
				new ControllersInitializer().Initialize();
				new ProjectionsInitializer().Initialize();
				new PartsInitializer().Initialize();
				
				RegisterRoutes (RouteTable.Routes);
			}
		}

		public static void RegisterRoutes (RouteCollection routes)
		{
			routes.RouteExistingFiles = true;
			if (routes["Projections"] == null)
				routes.Add("Projections", new Route("{name}.aspx", new ProjectionRouteHandler()));

		}
		
		private void InitializeState()
		{
			SoftwareMonkeys.SiteStarter.Web.State.StateProviderInitializer.Initialize();
		}
		
		public override void Dispose()
		{
			DataAccess.Dispose(true);
			Config.Dispose();
			DiagnosticState.Dispose();

			base.Dispose();
		}
		
	}
}
