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
	using (LogGroup logGroup = LogGroup.Start("Test group #1", NLog.LogLevel.Info))
	{
		LogWriter.Info("Test entry #1");
		
		DoSomething();
	}
}

private void DoSomething()
{
	LogGroup breakingGroup = null;
	
	using (LogGroup logGroup = LogGroup.Start("Test group #2", NLog.LogLevel.Info))
	{
		LogWriter.Info("Test entry #2");
		
		breakingGroup = LogGroup.Start("Test group #3", NLog.LogLevel.Info);
		
	}
	
	LogGroup stillAlive = breakingGroup;
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
