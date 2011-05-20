<%@ Page Language="C#" Title="Test Reset" %>
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

protected override void OnLoad(EventArgs e)
{
	using (LogGroup logGroup = LogGroup.Start("Executing the test reset, to clear the test environment ready for a new test.", NLog.LogLevel.Debug))
	{
		DeleteEntities();
		
		//DeleteMenuFile();
		
		// Leave caches because it takes too long to recreate them each time, and the cache should be exactly the same for each test so there's no chance of conflicts
		// TODO: Ensure there are no conflicts and that the above premise is indeed correct.
		//DeleteCaches();
		
		//DeleteVersionFile();
		
		if (Request.QueryString["Log"] != null && Request.QueryString["Log"].ToLower() == "true")
			DeleteLogs();
		
		//DeleteConfigurationFile();
		
		if (StateAccess.IsInitialized && AuthenticationState.IsAuthenticated)
			Authentication.SignOut();
		
		//Response.Redirect(Request.ApplicationPath + "/Admin/QuickSetup.aspx");
		
		// Restart the asp.net application
		//System.Web.HttpRuntime.UnloadAppDomain();
	}
}

private void DeleteEntities()
{
	using (LogGroup logGroup = LogGroup.Start("Deleting all entities in the system.", NLog.LogLevel.Debug))
	{
		if (DataAccess.IsInitialized)
		{
			using (Batch batch = BatchState.StartBatch())
			{
				IEntity[] entities = DataAccess.Data.Indexer.GetEntities();
				
				foreach (IEntity entity in entities)
				{
					//if (!CanSkipDelete(entity))
						DataAccess.Data.Deleter.Delete(entity);
						
					TotalEntitiesDeleted++;
				}		
			}
		}
	}
}

private bool CanSkipDelete(IEntity entity)
{
	if (entity.ID == Config.Application.PrimaryAdministratorID)
		return true;
	else
		return false;
}

private void DeleteConfigurationFile()
{
	if (Config.Application != null)
	{
		string path = Config.Application.FilePath;
		
		if (File.Exists(path))
			File.Delete(path);
			
		Config.Application = null;
	}
}

private void DeleteMenuFile()
{
	string path = SoftwareMonkeys.SiteStarter.Web.SiteMap.DefaultFilePath;
	
	if (File.Exists(path))
		File.Delete(path);
}

private void DeleteVersionFile()
{
	string path = Server.MapPath(Request.ApplicationPath + "/App_Data/" + VersionUtilities.GetVersionFileName(WebUtilities.GetLocationVariation(Request.Url)));
	
	if (File.Exists(path))
		File.Delete(path);
}


private void DeleteCaches()
{
	string[] cacheDirectories = new string[]
	{
		"Controllers",
		"Parts",
		"Projections",
		"Entities",
		"Strategies"
	};
	
	foreach (string dir in cacheDirectories)
	{
		string path = Server.MapPath(Request.ApplicationPath + "/App_Data/" + dir);
		
		if (Directory.Exists(path))
			Directory.Delete(path, true);
	}
}

private void DeleteLogs()
{
	string path = Server.MapPath(Request.ApplicationPath + "/App_Data/Logs");
	
	if (Directory.Exists(path))
		Directory.Delete(path, true);
}


</script>
<html>
<head runat="server">
</head>
<body>
<form runat="server">
Done...<br/>
Entities deleted: <%= TotalEntitiesDeleted %>
</form>
</body>
</html>