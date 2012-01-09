<%@ Page Language="C#" autoeventwireup="true" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.State" %>
<script runat="server">


private void Page_Load(object sender, EventArgs e)
{
	ResetAutoBackup();
}


private void ResetAutoBackup()
{
	if (Config.IsInitialized && Config.Application != null)
	{
		StateAccess.State.SetApplication("LastAutoBackup", DateTime.Now.Subtract(new TimeSpan(100, 0, 0)));
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
