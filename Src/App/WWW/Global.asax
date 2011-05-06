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
    	InitializeCore();
    	
        using (LogGroup logGroup = LogGroup.Start("Starting application.", LogLevel.Debug))
        {
        	LogWriter.Debug("${Application.Start}");
        
            // Attempt to initialize the config
            Initialize();
        }

    }
    
    void Application_End(object sender, EventArgs e) 
    {
    
        using (LogGroup logGroup = LogGroup.Start("Ending application.", LogLevel.Debug))
        {
        	LogWriter.Debug("${Application.End}");
        }
        
        //  Code that runs on application shutdown
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
    	
        using (LogGroup logGroup = LogGroup.Start("Starting session.", LogLevel.Debug))
        {
        	LogWriter.Debug("${Session.Start}");
        
            // Attempt to initialize the config
            Initialize();
        }

    }

    void Session_End(object sender, EventArgs e) 
    {
        LogWriter.Debug("${Session.End}");
        	
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }
    
    void Application_BeginRequest(object sender, EventArgs e)
    {
    	using (LogGroup logGroup = LogGroup.Start("Beginning application request: " + DateTime.Now.ToString(), NLog.LogLevel.Debug))
    	{
        	LogWriter.Debug("${Application.BeginRequest}");
        	
            // Initialize the URL rewriter to take care of friendly URLs
            UrlRewriter.Initialize();
        }
    }
    
    
    void Application_EndRequest(object sender, EventArgs e)
    {
    	using (LogGroup logGroup = LogGroup.Start("Ending application request: " + DateTime.Now.ToString(), NLog.LogLevel.Debug))
    	{
        	LogWriter.Debug("${Application.EndRequest}");
        	
        }
    }

	private void InitializeCore()
	{
	        if (!StateAccess.IsInitialized || !Config.IsInitialized)
	        {
	        	InitializeState();
	        }
	}

    private void Initialize()
    {
        //using (LogGroup logGroup = LogGroup.Start("Initializing the state management, config, modules, and data.", LogLevel.Debug))
        //{
	        if (!Config.IsInitialized)
	        {
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
        DataAccess.Dispose(true);

        base.Dispose();
    }
   
       
</script>
