<%@ Page language="c#" AutoEventWireup="true" theme="" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Properties" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import namespace="System.IO" %>

<script language="C#" runat="server">
private void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
	PrepareLog();
	//ShowLog();
    }
    
}

    private void PrepareLog()
    {
	Response.ContentType = "text/xml";

	string logPath = Server.MapPath(Request.ApplicationPath + "/App_Data/Logs/" + DateTime.Now.ToString("yyyy-MM-dd") + "/Log.xml");
	if (File.Exists(logPath))
	{
		string logContents = String.Empty;
		using (StreamReader reader = new StreamReader(File.OpenRead(logPath)))
		{
			// Get the contents of the log
			logContents = reader.ReadToEnd();

			reader.Close();
		}

			
		// Change the path of the LogTemplate file to suit the web setup
		logContents = logContents.Replace("../../../LogTemplate.xsl", "../LogTemplate.xsl");

		// Add the ending </Log> tag if necessary
		if (logContents.IndexOf("</Log>") == -1)
				logContents = logContents + "</Log>";

		// Write the contents of the log to the page
		Response.Write(logContents);
	}
	else
		Response.Write("No log found: " + logPath);
    }

</script>