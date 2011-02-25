<%@ Page Language="C#" autoeventwireup="true" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="ss" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import Namespace="System.Reflection" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import namespace="System.IO" %>
<%@ Import namespace="System.Xml" %>
<%@ Import namespace="System.Xml.Serialization" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.State" %>
<script runat="server">

public string DataDirectoryPath
{
	get { return StateAccess.State.PhysicalApplicationPath + Path.DirectorySeparatorChar + "App_Data"; }
}

private void Page_Init(object sender, EventArgs e)
{
}

private void Page_Load(object sender, EventArgs e)
{
	DeleteLogs();
	DeleteYapFiles();
	DeleteAppConfig();
	DeleteSiteMap();
	DeleteVersion();
	DeleteCaches();
	DeleteSuspended();
	DeleteImport();
	DeleteImported();
	DeletePersonalization();
}


private void DeleteLogs()
{
	string dataDirectory = DataDirectoryPath;

	string logsDirectory = dataDirectory + Path.DirectorySeparatorChar + "Logs";

	if (Directory.Exists(logsDirectory))
		Directory.Delete(logsDirectory, true);
}

private void DeleteSuspended()
{
	string dataDirectory = DataDirectoryPath;

	string suspendedDirectory = dataDirectory + Path.DirectorySeparatorChar + "Suspended";

	if (Directory.Exists(suspendedDirectory ))
		Directory.Delete(suspendedDirectory , true);
}


private void DeleteExport()
{
	string dataDirectory = DataDirectoryPath;

	string exportDirectory = dataDirectory + Path.DirectorySeparatorChar + "Export";

	if (Directory.Exists(exportDirectory))
		Directory.Delete(exportDirectory, true);
}

private void DeleteImport()
{
	string dataDirectory = DataDirectoryPath;

	string importDirectory = dataDirectory + Path.DirectorySeparatorChar + "Import";

	if (Directory.Exists(importDirectory))
		Directory.Delete(importDirectory, true);
}

private void DeleteImported()
{
	string dataDirectory = DataDirectoryPath;

	string importedDirectory = dataDirectory + Path.DirectorySeparatorChar + "Imported";

	if (Directory.Exists(importedDirectory))
		Directory.Delete(importedDirectory, true);
}

private void DeletePersonalization()
{
	string dataDirectory = DataDirectoryPath;

	string personalizationDirectory = dataDirectory + Path.DirectorySeparatorChar + "Personalization_Data";

	if (Directory.Exists(personalizationDirectory))
		Directory.Delete(personalizationDirectory, true);
}

private void DeleteCaches()
{
	string[] caches = new string[]
	{
		"Strategies",
		"Entities",
		"Controllers",
		"Projections",
		"Parts"
	};

	string dataDirectory = DataDirectoryPath;

	foreach (string cache in caches)
	{
		string cacheDirectory = dataDirectory + Path.DirectorySeparatorChar + cache;

		if (Directory.Exists(cacheDirectory))
			Directory.Delete(cacheDirectory, true);
	}
}


private void DeleteYapFiles()
{
	if (DataAccess.IsInitialized)
		DataAccess.Dispose();

	string dataDirectory = DataDirectoryPath;

	foreach (string file in Directory.GetFiles(dataDirectory, "*.yap"))
	{
		File.Delete(file);
	}
}

private void DeleteAppConfig()
{
	if (Config.IsInitialized)
	{
		string configPath = Config.Application.FilePath;

		if (File.Exists(configPath))
			File.Delete(configPath);
	}
}

private void DeleteSiteMap()
{
	string dataDirectory = DataDirectoryPath;

	foreach (string file in Directory.GetFiles(dataDirectory, "*.sitemap"))
	{
		if (file.ToLower().IndexOf("default.sitemap") == -1)
		{
			File.Delete(file);
		}
	}
}

private void DeleteVersion()
{
	string dataDirectory = DataDirectoryPath;

	foreach (string file in Directory.GetFiles(dataDirectory, "*.number"))
	{
		File.Delete(file);
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
