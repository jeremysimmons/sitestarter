<%@ Page Language="C#" autoeventwireup="true" Title="Untitled Page" StylesheetTheme="Default" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import namespace="NLog" %>
<%@ Import namespace="NLog.Config" %>
<%@ Import namespace="NLog.Targets" %>
<script runat="server">
	void Page_Load(object sender, EventArgs e)
	{



















		//log4net.ILog log = log4net.LogManager.GetLogger("FileAppender");
		//log.Info("test");
// Logger logger = LogManager.GetCurrentClassLogger();
//logger.Debug("testlogging");

//System.Diagnostics.Trace.WriteLine("sdf");
//Response.Write("done 2");

	    //if (!SoftwareMonkeys.SiteStarter.Configuration.Config.IsInitialized)
	   //{
        using (LogGroup group = AppLogger.StartGroup("Does something cool", LogLevel.Info))
        {
        }
	   // }
	}

	//    protected void Testing1_Load(object sender, EventArgs e)
	//    {
	
	//    }
</script>
<html>
<head runat="Server">
</head>
<body>
<div class="Heading1"></div>
</body>
</html>

