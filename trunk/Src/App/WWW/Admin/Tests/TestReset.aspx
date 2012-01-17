<%@ Page Language="C#" Title="Test Reset" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.State" %>
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
public int TotalStoresDeleted = 0;
public int TotalEntitiesDeleted = 0;
public int TotalReferencesDeleted = 0;

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
			// TODO: Clean up
	
			//DeleteDb4oFiles();
	
			DeleteEntities();

			DeletePersonalization();
		
			//DeleteMenuFile();
		
			// Leave caches because it takes too long to recreate them each time, and the cache should be exactly the same for each test so there's no chance of conflicts
			// TODO: Ensure there are no conflicts and that the above premise is indeed correct.
			//DeleteCaches();
		
			//DeleteVersionFile();
		
			if (Request.QueryString["Log"] != null && Request.QueryString["Log"].ToLower() == "true")
				DeleteLogs();
		
			if (Request.QueryString["Config"] != null && Request.QueryString["Config"].ToLower() == "true")
				DeleteConfigurationFile();
		
			SuspendAutoBackup();
				
			if (StateAccess.IsInitialized && AuthenticationState.IsAuthenticated)
				Authentication.SignOut();
		}
	}
}

private void DeleteDb4oFiles()
{
	// Dispose the data access layer so the db4o files can be deleted
	if (DataAccess.IsInitialized)
		DataAccess.Dispose(true);

	string dataDirectory = DataDirectoryPath;

	foreach (string file in Directory.GetFiles(dataDirectory, "*.db4o"))
	{
		File.Delete(file);
		
		TotalStoresDeleted++;
	}
	
	// Re-initialize the data access layer
	new DataProviderInitializer().Initialize();
}

private void DeleteEntities()
{
	using (LogGroup logGroup = LogGroup.StartDebug("Deleting all entities in the system."))
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
				
				EntityReferenceCollection references = DataAccess.Data.Referencer.GetReferences();
				
				foreach (EntityReference reference in references)
				{
					DataAccess.Data.Deleter.Delete(reference);
						
					TotalReferencesDeleted++;
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

private void DeletePersonalization()
{
	string path = Server.MapPath(Request.ApplicationPath + "/App_Data/Personalization_Data");
	
	if (Directory.Exists(path))
		Directory.Delete(path, true);
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
	
	try
	{
		if (Directory.Exists(path))
			Directory.Delete(path, true);
	}
	catch (IOException ex)
	{
		LogWriter.Error(ex);
	}
}

private void SuspendAutoBackup()
{
	if (StateAccess.IsInitialized)
	{
		StateAccess.State.SetApplication("LastAutoBackup", DateTime.Now);
	}
}
</script>
<html>
<head runat="server">
</head>
<body>
<form runat="server">
Done...<br/>
Entities cleared: <%= TotalEntitiesDeleted %><br/>
</form>
</body>
</html>