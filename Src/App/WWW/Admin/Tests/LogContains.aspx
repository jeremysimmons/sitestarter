<%@ Page Language="C#" Title="LogContains" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="ss" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import Namespace="System.Reflection" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import namespace="System.IO" %>
<%@ Import namespace="System.Xml" %>
<%@ Import namespace="System.Xml.Serialization" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import namespace="System.Collections.Generic" %>
<script runat="server">

private void Page_Load(object sender, EventArgs e)
{
	if (!IsPostBack)
	{
		string query = Request.QueryString["Query"];
		
		if (query == null || query == String.Empty)
			throw new Exception("A query must be provided via the 'Query' query string.");
		
		string logContents = LoadLog();
		
		string output = "LogContains=";
		
		output += LogContains(query, logContents).ToString();
		
		string foundTotal = "FoundTotal=";
		
		foundTotal += Count(query, LoadLog()).ToString();
		
		OutputHolder.Controls.Add(new LiteralControl("<div>"));
		OutputHolder.Controls.Add(new LiteralControl(output));
		OutputHolder.Controls.Add(new LiteralControl("</div>"));
		OutputHolder.Controls.Add(new LiteralControl("<div>"));
		OutputHolder.Controls.Add(new LiteralControl(foundTotal));
		OutputHolder.Controls.Add(new LiteralControl("</div>"));
	}
}

private bool LogContains(string query, string logContents)
{	
	if (logContents == String.Empty)
		return false;
	
	return logContents.IndexOf(query) > -1;
}

private string LoadLog()
{
	string path = Request.ApplicationPath + "/App_Data/Logs/" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.ToString("dd") + "/Log.xml";
	path = Server.MapPath(path);
	
	string content = String.Empty;
	
	LogWriter.Dispose();
	
	using (StreamReader reader = new StreamReader(File.OpenRead(path)))
	{
		content = reader.ReadToEnd();
	}
	
	return content;
}

private int Count(string query, string searchable)
{
	System.Text.RegularExpressions.MatchCollection wordColl = System.Text.RegularExpressions.Regex.Matches(searchable, Regex.Escape(query));
    return wordColl.Count;

}
</script>
<html>
<head runat="server"></head>
<body>
<form runat="server">
<asp:placeholder runat="server" id="OutputHolder">
</asp:placeholder>
</form>
</body>
</html>