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
    }
    
    void Application_BeginRequest(object sender, EventArgs e)
    {
    	InitializeCore();
    	
    	using (LogGroup logGroup = LogGroup.Start("Beginning application request: " + DateTime.Now.ToString(), NLog.LogLevel.Debug))
    	{
        	LogWriter.Debug("${Application.BeginRequest}");
        	
        	Initialize();
        	
            // Initialize the URL rewriter to take care of friendly URLs
            UrlRewriter.Initialize();
        }
    }
    
    
    void Application_EndRequest(object sender, EventArgs e)
    {
    	using (LogGroup logGroup = LogGroup.Start("Ending application request: " + DateTime.Now.ToString(), NLog.LogLevel.Debug))
    	{
        	LogWriter.Debug("${Application.EndRequest}");
       	
       		new AutoBackupInitializer().Initialize();
        }
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
