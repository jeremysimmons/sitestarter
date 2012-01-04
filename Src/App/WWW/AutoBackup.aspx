<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web" %>
<script runat="server">
private void Page_Load(object sender, EventArgs e)
{
	new AutoBackupExecutor().Initialize();
}
</script>
<html>
<body>
<script type="text/javascript">
window.location.href='Default.aspx';
</script>
</body>
</html>