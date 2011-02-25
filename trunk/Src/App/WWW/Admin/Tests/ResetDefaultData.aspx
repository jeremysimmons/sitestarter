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
	ResetDefaultData();
}


private void ResetDefaultData()
{ 
	DeleteImportData();
	CopyDefaultImportData();
}

private void DeleteImportData()
{
	string dataDirectory = DataDirectoryPath;

	string importDirectory = dataDirectory + Path.DirectorySeparatorChar + "Import";

	if (Directory.Exists(importDirectory ))
		Directory.Delete(importDirectory, true);
}

private void CopyDefaultImportData()
{
	string dataDirectory = DataDirectoryPath;

	string importDirectory = dataDirectory + Path.DirectorySeparatorChar + "Import";
	string importDefaultDirectory = dataDirectory + Path.DirectorySeparatorChar + "ImportDefault";

	if (Directory.Exists(importDefaultDirectory))
	{
		if (!Directory.Exists(importDirectory))
			Directory.CreateDirectory(importDirectory);

		// Now Create all of the directories
	        foreach (string dirPath in Directory.GetDirectories(importDefaultDirectory, "*", SearchOption.AllDirectories))
		{
        		Directory.CreateDirectory(dirPath.Replace(importDefaultDirectory, importDirectory));
		}

		// Copy all the files
		foreach (string file in Directory.GetFiles(importDefaultDirectory, "*.*", SearchOption.AllDirectories))
		{
                	File.Copy(file, file.Replace(importDefaultDirectory, importDirectory));
		}

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
