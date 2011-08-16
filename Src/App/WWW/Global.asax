<%@ Application Language="C#" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.State" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Providers" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Projections" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Controllers" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Parts" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Data" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
    	InitializeCore();
        
		// Start a log group for the application
		HttpContext.Current.Application["Application_Start.LogGroup"] = LogGroup.StartDebug("Starting application.");
		
		LogWriter.Debug("${Application.Start}");
	
	    // Attempt to initialize the config
	    Initialize();
    }
    
    void Application_End(object sender, EventArgs e) 
    {
        LogWriter.Debug("${Application.End}");
        	
        HttpContext.Current.Application["Application_Start.LogGroup"] = null;
        
        //  Dispose outside the log group
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
    	//InitializeCore();
    	
    	// Create a log group for the request
		HttpContext.Current.Items["Application_BeginRequest.LogGroup"] = LogGroup.StartDebug("Beginning request: " + HttpContext.Current.Request.Url.ToString());

        LogWriter.Debug("${Application.BeginRequest}");
        	
       // Initialize();
        	
        // Initialize the URL rewriter to take care of friendly URLs
        UrlRewriter.Initialize();
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
    	}
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
    
</script>
