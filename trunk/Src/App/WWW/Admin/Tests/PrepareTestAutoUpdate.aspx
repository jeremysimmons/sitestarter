<%@ Page Language="C#" Title="Prepare Test Import" autoeventwireup="true" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business.Security" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import Namespace="System.Reflection" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Data" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import namespace="System.IO" %>
<%@ Import namespace="System.Xml" %>
<%@ Import namespace="System.Xml.Serialization" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.State" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<script runat="server">


protected override void OnLoad(EventArgs e)
{
	using (LogGroup logGroup = LogGroup.StartDebug("Preparing to test the auto update feature."))
	{
			SetAllowConfig();
	}
	base.OnLoad(e);
}

private void SetAllowConfig()
{
	string filePath = Server.MapPath(Request.ApplicationPath + "/AllowAutoUpdate.config");
	using (StreamWriter writer = File.CreateText(filePath))
	{
		writer.Write(true.ToString());
	}
	
}

</script>
<html>
<head runat="server">
</head>
<body>
<form runat="server">
Done...<br/>
</form>
</body>
</html>