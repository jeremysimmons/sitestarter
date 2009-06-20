<%@ Application Language="C#" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.State" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Providers" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="NLog" %>
<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        // TODO: Clean up
	//SoftwareMonkeys.SiteStarter.Diagnostics.Tracer.Initialize();
	//logger = LogManager.GetLogger("Test");

	//logger.Info("test worked");
	////System.Diagnostics.Trace.Listeners.Add(new LogWriter());

        using (LogGroup logGroup = AppLogger.StartGroup("Initializes the state management, config, modules, and data.", LogLevel.Info))
        {
            //log4net.Config.XmlConfigurator.Configure();
            // Attempt to initialize the config
            Initialize();
        }

    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown
        Dispose();
    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code that runs when an unhandled error occurs
	    Exception lastException = Server.GetLastError();
   	    AppLogger.Error(lastException.ToString());
    }

    void Session_Start(object sender, EventArgs e) 
    {
        if (Request.QueryString["VS"] != null && Request.QueryString["VS"] != String.Empty)
        {
            VirtualServer server = VirtualServerFactory.GetVirtualServerByName(Request.QueryString["VS"]);

            if (server != null)
                VirtualServerState.Switch(server.Name, server.ID);
            
        }
        
        // Code that runs when a new session is started
        if (!StateAccess.IsInitialized || !Config.IsInitialized || !DataAccess.IsInitialized)
            Initialize();

    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }

    private void Initialize()
    {
        if (!StateAccess.IsInitialized || !Config.IsInitialized)
        {
            SoftwareMonkeys.SiteStarter.Web.State.StateProviderManager.Initialize();   
            Config.Initialize(Server.MapPath(HttpContext.Current.Request.ApplicationPath), WebUtilities.GetLocationVariation(HttpContext.Current.Request.Url));
            DataProviderManager.Initialize();
        }
    }

    public override void Dispose()
    {
        Config.Dispose();
        DataAccess.Dispose();

        base.Dispose();
    }
       
</script>
