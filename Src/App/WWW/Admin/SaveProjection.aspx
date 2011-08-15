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
		using (LogGroup logGroup = LogGroup.StartDebug("Loading the save projection page."))
		{
			// TODO: Ensure this is secure
					
			Authorisation.EnsureUserCan("Edit", "Projection");
		
			string projectionPath = Request.Form["ProjectionPath"];
			string projectionContent = Request.Form["ProjectionContent"];
			string originalProjectionPath = Request.Form["OriginalProjectionPath"];
			
			LogWriter.Debug("Projection path: " + projectionPath);
			
			ProjectionSaver saver = new ProjectionSaver();
			saver.SaveToFile(originalProjectionPath, projectionPath, projectionContent);
			
			Response.Write("Saved");
		}
	}

</script>
<html>
<head runat="server">
</head>
<body>
</body>
</html>