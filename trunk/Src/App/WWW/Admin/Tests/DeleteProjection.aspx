<%@ Page Language="C#" autoeventwireup="true" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Projections" %>
<%@ Import namespace="System.IO" %>
<script runat="server">

public string OutputText = "Done";

private void Page_Load(object sender, EventArgs e)
{
	string projectionName = Request.QueryString["Projection"];
	if (projectionName != null && projectionName != String.Empty)
	{
		ProjectionInfo info = ProjectionState.Projections[projectionName];
	
		if (info == null)
		{
			info = new ProjectionInfo();
			info.Name = projectionName;
			info.ProjectionFilePath = new ProjectionFileNamer().CreateRelativeProjectionFilePath(projectionName);
		}

		string filePath = new ProjectionFileNamer().CreateProjectionFilePath(info);
		string infoPath = new ProjectionFileNamer().CreateInfoFilePath(info);
		
		File.Delete(filePath);
		File.Delete(infoPath);
		
		ProjectionState.Projections.Remove(info);
		
	}
	else
		throw new Exception("No projection specified.");
}

</script>
<html>
<head runat="server">
</head>
<body>
<form runat="server">
<%= OutputText %>
</form>
</body>
</html>
