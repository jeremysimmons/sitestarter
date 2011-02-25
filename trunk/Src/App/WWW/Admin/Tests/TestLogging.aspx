<%@ Page Language="C#" autoeventwireup="true" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="ss" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import Namespace="System.Reflection" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import namespace="System.IO" %>
<%@ Import namespace="System.Xml" %>
<%@ Import namespace="System.Xml.Serialization" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.State" %>
<script runat="server">

private void Page_Init(object sender, EventArgs e)
{
}

private void Page_Load(object sender, EventArgs e)
{
	
	using (LogGroup logGroup = LogGroup.Start("Testing logging", NLog.LogLevel.Debug))
	{
		LogWriter.Debug("Test debug message");
		
		DoSomething();
	}
}

private void DoSomething()
{
	using (LogGroup logGroup = LogGroup.Start("Doing something", NLog.LogLevel.Debug))
	{
		LogWriter.Debug("The 'Doing something' group should be a sub group of the 'Testing logging' group.");
		
	}
}


    private void Initialize()
    {
        //using (LogGroup logGroup = LogGroup.Start("Initializing the state management, config, modules, and data.", LogLevel.Debug))
        //{
	        if (!StateAccess.IsInitialized || !Config.IsInitialized)
	        {
	        	InitializeState();
                /*Config.Initialize(Server.MapPath(HttpContext.Current.Request.ApplicationPath), WebUtilities.GetLocationVariation(HttpContext.Current.Request.Url));
                InitializeEntities();
	            new DataProviderInitializer().Initialize();
	        	InitializeBusiness();
	        	InitializeWeb();*/
	        }
		//}
    }

    
    private void InitializeState()
    {
    	SoftwareMonkeys.SiteStarter.Web.State.StateProviderInitializer.Initialize();
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

</script>
<html>
<head runat="server">
</head>
<body>
<form runat="server">
Done
</form>
</body>
</html>
