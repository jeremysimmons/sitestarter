s<%@ Page language="c#" AutoEventWireup="true" theme=""%>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Properties" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import namespace="System.IO" %>

<script language="C#" runat="server">
private void Page_Load(object sender, EventArgs e)
{

    //if (!Request.IsAuthenticated)
    //	Response.Redirect("../Members/Login.aspx");
    //else
	//{
	    if (!IsPostBack)
	    {
		PrepareLog();
		//ShowLog();
	    }
    //}
    
}

    private void PrepareLog()
    {
		Response.ContentType = "text/xml";
		
		string helpFileName = Request.QueryString["a"];
	
		string path = Server.MapPath(Request.ApplicationPath + "/Help/" + helpFileName + ".xml");
		if (File.Exists(path))
		{
			string content = String.Empty;
			using (StreamReader reader = new StreamReader(File.OpenRead(path)))
			{
				// Get the contents of the log
				content = reader.ReadToEnd();
	
				reader.Close();
			}
	
				
			// Change the path of the LogTemplate file to suit the web setup
			//logContents = logContents.Replace("../../../LogTemplate.xsl", "../LogTemplate.xsl");
	
			// Add the ending </Log> tag if necessary
			//if (logContents.IndexOf("</Log>") == -1)
			//		logContents = logContents + "</Log>";
	
			// Write the contents of the log to the page
			Response.Write(content);
		}
		else
			Response.Write("No file found: " + path);
    }

</script>