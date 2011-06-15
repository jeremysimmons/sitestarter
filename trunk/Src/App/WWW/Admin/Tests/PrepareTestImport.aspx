<%@ Page Language="C#" Title="Prepare Test Import" %>
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
public int TotalEntitiesDeleted = 0;
public int TotalReferencesDeleted = 0;
public int TotalStoresDeleted = 0;

public string DataDirectoryPath
{
	get { return StateAccess.State.PhysicalApplicationPath + Path.DirectorySeparatorChar + "App_Data"; }
}

protected override void OnLoad(EventArgs e)
{
	using (LogGroup logGroup = LogGroup.Start("Executing the test reset, to clear the test environment ready for a new test.", NLog.LogLevel.Debug))
	{
		using (TimeoutExtender extender = TimeoutExtender.NewMinutes(30)) // 30 minutes
		{
			CreateMockData();
			
			ExportData();
			
			MoveExportedToImportable();
		}
	}
}

protected void CreateMockData()
{
	for (int i = 0; i < 500; i++)
	{
		CreateMockUser(i);
	}
}

protected void CreateMockUser(int i)
{
	User user = new User();
	user.Username = "Test User " + i;
	user.FirstName = "First Name " + i;
	user.LastName = "Last Name " + i;
	user.Email = "test" + i + "@softwaremonkeys.net";
	
	SaveStrategy.New(user).Save(user);
}

protected void ExportData()
{
	DataAccess.Data.Exporter.ExportToXml();
}

protected void MoveExportedToImportable()
{
	string exportPath = DataAccess.Data.Exporter.ExportDirectoryPath;
	string importablePath = DataAccess.Data.DataDirectoryPath + Path.DirectorySeparatorChar + "Import";
	
	Directory.Move(exportPath, importablePath);
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