<%@ Page
	Language           = "C#"
	AutoEventWireup    = "true"
	ValidateRequest    = "false"
	EnableSessionState = "false"
%>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<script runat="server">
private void Page_Load(object sender, EventArgs e)
{
	MappingConfig config = new MappingConfig();
	config.Settings.Add("Test", "Value");
	
	string path = Server.MapPath(Request.ApplicationPath + "/App_Data");
	
	Response.Write("Path: " + path);
	
	ConfigFactory.SaveConfig(path, config);
}
</script>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>TestMappings</title>

		<meta http-equiv="content-type" content="text/html; charset=utf-8" />
		<meta http-equiv="CACHE-CONTROL" content="NO-CACHE" />
		<meta http-equiv="PRAGMA" content="NO-CACHE" />

		
	</head>
	<body>
		<form id="Form_TestMappings" method="post" runat="server">

			

		</form>
	</body>
</html>
