<%@ Page Language="C#" Title="InitializeCache" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import Namespace="System.Reflection" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import namespace="System.IO" %>
<%@ Import namespace="System.Xml" %>
<%@ Import namespace="System.Xml.Serialization" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.State" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Projections" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Controllers" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Parts" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.State" %>
<script runat="server">

private void Page_Load(object sender, EventArgs e)
{
	using (TimeoutExtender extender = TimeoutExtender.NewMinutes(30))
	{
		StateProviderInitializer.Initialize();
		
		new EntityInitializer().Initialize();
		new StrategyInitializer().Initialize();
		new ReactionInitializer().Initialize();
		new PartsInitializer(this).Initialize();
		new ControllersInitializer().Initialize();

		// Skip projection initialization to work around issue. Initialization instead occurs during setup.
		//new ProjectionsInitializer(this).Initialize();
	}
}
   
</script>
<html>
<head runat="server"></head>
<body>
Cache has been initialized.
</body>
</html>








