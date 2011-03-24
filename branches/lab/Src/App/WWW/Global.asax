<%@ Application Language="C#" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.State" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Providers" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Projections" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Controllers" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Data" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="NLog" %>
<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
    
       // using (LogGroup logGroup = LogGroup.Start("Preparing to start application.", LogLevel.Debug))
       // {
            // Attempt to initialize the config
            Initialize();
       // }

    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown
        Dispose();
    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
    	using (LogGroup logGroup = LogGroup.Start("Handling application error.", NLog.LogLevel.Error))
	    {
	        // Code that runs when an unhandled error occurs
		    Exception lastException = Server.GetLastError();
	   	    LogWriter.Error(lastException.ToString());
   	    }
    }

    void Session_Start(object sender, EventArgs e) 
    {
       // using (LogGroup logGroup = LogGroup.Start("Preparing to start session.", LogLevel.Debug))
       // {	        
	        // Code that runs when a new session is started
	        if (!StateAccess.IsInitialized || !Config.IsInitialized || !DataAccess.IsInitialized)
	            Initialize();
		//}

    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }
    
    void Application_BeginRequest(object sender, EventArgs e)
    {
    	using (LogGroup logGroup = LogGroup.Start("Beginning application request.", NLog.LogLevel.Debug))
    	{
            // Initialize the URL rewriter to take care of friendly URLs
            UrlRewriter.Initialize();
        }
    }

    private void Initialize()
    {
        //using (LogGroup logGroup = LogGroup.Start("Initializing the state management, config, modules, and data.", LogLevel.Debug))
        //{
	        if (!StateAccess.IsInitialized || !Config.IsInitialized)
	        {
	        	InitializeState();
                Config.Initialize(Server.MapPath(HttpContext.Current.Request.ApplicationPath), WebUtilities.GetLocationVariation(HttpContext.Current.Request.Url));
                InitializeEntities();
	            new DataProviderInitializer().Initialize();
	        	InitializeBusiness();
	        	InitializeWeb();
	        }
		//}
    }

    private void InitializeEntities()
    {
        if (Config.IsInitialized)
            new EntityInitializer().Initialize();
    }
    
    private void InitializeBusiness()
    {
    	if (Config.IsInitialized)
	    	new StrategyInitializer().Initialize();
    }
    
    private void InitializeWeb()
    {
    	if (Config.IsInitialized)
    	{
    	
    		// These are now taken care of by the Projector control as it's responsible for projections and controllers
    		// The projection scanner needs a Page component to access the LoadControl function and so initializing in the Projector control is an appropriate solution
    		//new ControllersInitializer().Initialize();
    		//new ProjectionsInitializer().Initialize();
    	}
    }

    private void InitializeState()
    {
    	SoftwareMonkeys.SiteStarter.Web.State.StateProviderInitializer.Initialize();
    }
    
    public override void Dispose()
    {
        Config.Dispose();
        DataAccess.Dispose();

        base.Dispose();
    }
   
       
</script>
