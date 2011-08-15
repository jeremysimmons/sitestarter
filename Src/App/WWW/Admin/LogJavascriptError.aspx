<%@ Page Language="C#" EnableEventValidation="false" ValidateRequest="false" AutoEventWireup="true" %>
<%@ Register tagprefix="cc" namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" assembly="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Projections" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Properties" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import namespace="System.IO" %>
<script runat="server">

	private void Page_Load(object sender, EventArgs e)
	{
		using (LogGroup logGroup = LogGroup.StartDebug("Loading the log javascript error page."))
		{		
			string errorData = Request.Form["ErrorData"];
			string sourcePage = Request.Form["SourcePage"];
			string line = Request.Form["Line"];
			
			string full = "Javascript error: " + errorData + Environment.NewLine
				+ "Location: " + sourcePage + ", line " + line;
			
			LogWriter.Error(full);
			
			Response.Write("Done");
		}
	}

</script>
<html>
<head runat="server">
</head>
<body>
</body>
</html>