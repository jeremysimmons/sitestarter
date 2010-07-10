<%@ Page language="c#" AutoEventWireup="true" theme="" masterpagefile="~/Site.master" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Properties" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import namespace="System.IO" %>
<%@ Import namespace="System.Xml" %>

<script language="C#" runat="server">

	protected string LogsDir
	{
		get {
			return Server.MapPath(Request.ApplicationPath + "/App_Data/Logs");
		}
	}


	private void Page_Load(object sender, EventArgs e)
	{
        Authorisation.EnsureIsAuthenticated();

        Authorisation.EnsureIsInRole("Administrator");
        
	    if (!IsPostBack)
	    {
			if (Request.QueryString["LogDate"] == null || Request.QueryString["LogDate"] == String.Empty)
			{
				ListLogs();	
	
			}
			else if (Request.QueryString["LogThread"] == null || Request.QueryString["LogThread"] == String.Empty)
			{
				ListThreads();	
	
			}
			else
			{
				PrepareLogContents();
			}
		//ShowLog();
	    }
	    
	}
	
	private void ListLogs()
	{
		if (Directory.Exists(LogsDir))
		{
			foreach (string directory in Directory.GetDirectories(LogsDir))
			{
				HyperLink link = new HyperLink();
				link.Text = Path.GetFileName(directory);
				string urlStart = Request.Url.ToString();
				if (urlStart.IndexOf("?") > -1)
					urlStart += "&";
				else
					urlStart += "?";
				link.NavigateUrl = urlStart + "LogDate=" + Path.GetFileName(directory);
				OutputHolder.Controls.Add(link);
				OutputHolder.Controls.Add(new LiteralControl("<br/>"));
			}
		}
	}


	
	private void ListThreads()
	{
		string logRoot = LogsDir + "\\" + Request.QueryString["LogDate"];

		string indexFile = logRoot + @"\Detail\Index.xml";

		LogUtilities.AnalyzeLog(logRoot);

		XmlDocument indexDoc = new XmlDocument();
		indexDoc.Load(indexFile);

		foreach (XmlNode node in indexDoc.DocumentElement)
		{
				Guid id = new Guid(node.Attributes["ID"].Value);
				string title = node.Attributes["Title"].Value;

				HyperLink link = new HyperLink();
				link.Text = title;
				link.NavigateUrl = Request.Url + "&LogThread=" + id;
				OutputHolder.Controls.Add(link);
				OutputHolder.Controls.Add(new LiteralControl("<br/>"));
		}

		/*string[] fileNames = Directory.GetFiles(logRoot + @"\Detail");

		// Now read the creation time for each file
		DateTime[] creationTimes = new DateTime[fileNames.Length];
		for (int i=0; i < fileNames.Length; i++)
		creationTimes[i] = new FileInfo(fileNames[i]).CreationTime;
		
		// sort it
		Array.Sort(creationTimes,fileNames);

			foreach (string file in fileNames)
			{
				HyperLink link = new HyperLink();
				link.Text = Path.GetFileNameWithoutExtension(file);
				link.NavigateUrl = Request.Url + "&LogThread=" + LogUtilities.PrepareFileName(Path.GetFileNameWithoutExtension(file));
				OutputHolder.Controls.Add(link);
				OutputHolder.Controls.Add(new LiteralControl("<br/>"));
			}*/
	}

    private void PrepareLogContents()
    {
	//Response.ContentType = "text/xml";

	string logDate = Request.QueryString["LogDate"];
	string logThread = Request.QueryString["LogThread"];

	string logPath = Server.MapPath(Request.ApplicationPath + "/App_Data/Logs/" + logDate + "/Detail/" + logThread + ".xml");

	if (File.Exists(logPath))
	{
		string output = String.Empty;
		string logContents = String.Empty;

	//	NameTable nt = new NameTable();
	//	    XmlNamespaceManager nsmgr = new XmlNamespaceManager(nt);
	//	    nsmgr.AddNamespace("SS", "urn:SiteStarter");
		
	//	    //Create the XmlParserContext.
	//	    XmlParserContext context = new XmlParserContext(null, nsmgr, null, XmlSpace.None);

    		/*using (XmlTextReader xmlReader = new XmlTextReader(logPath))//, context))
		{
			logContents = String.Empty; // Clearing from memory

			while(xmlReader.Read()) 
			{
				if (xmlReader.ElementType
	
				output += CreateLogItem(xmlReader);
			}
		}*/

				XmlDocument doc = new XmlDocument();
				doc.Load(logPath);

				XmlNodeList nodeList;
				XmlNode root = doc.DocumentElement;

				nodeList=root.SelectNodes("Entry");
				
				XmlNode threadRoot = null;
				
				XmlDocument threadDoc = null;
				
				
				string threadTitle = string.Empty;
				string threadFile = string.Empty;

				int i = 0;
				
				foreach (XmlNode node in nodeList)
				{
					
					int indent = Convert.ToInt32(node.SelectSingleNode("Indent").InnerText);
					
					

						OutputHolder.Controls.Add(CreateLogItem(node, i));
					

					i++;
				}

	}
	else
		throw new Exception("No log found: " + logPath);
    }

	private LiteralControl CreateLogItem(XmlNode node, int id)
	{
						string componentName = node.SelectSingleNode("Component").InnerText;
						string methodName = node.SelectSingleNode("Method").InnerText;
						string stackTrace = node.SelectSingleNode("StackTrace").InnerText;
						string timestamp = node.SelectSingleNode("Timestamp").InnerText;
						int indent = Int32.Parse(node.SelectSingleNode("Indent").InnerText);
						string data = node.SelectSingleNode("Data").InnerText;
						string src = componentName + "." + methodName;

		string output = "<div>";
		output += "<table><tr><td valign=top>";
		output += "<a id='" + id + "_Toggle' href=\"javascript:ToggleExpansion('" + id + "')\">&raquo;</a>";
		output += "</td><td style='padding-left: " + (indent*10) + "px;'>";
		//output += "<br/>";
		//output += HttpUtility.HtmlEncode(timestamp);
		//output += "<br/>";
		output += HttpUtility.HtmlEncode(data);
		output += "<div style='font-size:x-small; color:gray;'>";
		
		output += HttpUtility.HtmlEncode(src);
		
		output += "</div>";

		output += "<div style='display:none;' id='" + id + "_Expand' class='LogBox'>";

		//output += "<div>";
		output += HttpUtility.HtmlEncode(timestamp);
		//output += "</div>";

		output += "<hr/>";

		//output += "<div>";
		output += "<div class='LogSubTitle'>Stack Trace</div>";
		output += HttpUtility.HtmlEncode(stackTrace).Replace("\n\n", "<br/>");
		//output += "</div>";

		output += "</div>";

		output += "</td></tr></table>";
		output += "</div>";
		return new LiteralControl(output);
	}

	private string CreatePartItem(string part)
	{
		string output = "<p>";
		output += HttpUtility.HtmlEncode(part);
		output += "</p>";
		return output;
	}

	private LiteralControl CreateIndent(int indent)
	{
		string output = String.Empty;
		for (int i = 0; i < indent; i++)
		{
			output += "&nbsp;&nbsp;";
		}
		return new LiteralControl(output);
	}

</script>

    <asp:Content runat="server" ContentPlaceHolderID="Body">
<style>
	.LogBox
	{
		/*font-family: courier new;*/
		padding: 3px; 5px; 3px; 5px;
		border: 1px solid #33E6EE;
		font-size: 11px;
		margin: 3px 0px 3px 0px;
		background-color: #BAFAFD;
	}

	.LogBox HR
	{
		border: 1px dashed #33E6EE;
	}

	.LogSubTitle
	{
		font-family: verdana;
		font-size: 11px;
		font-weight: bold;
		color: black;
		margin: 6px 0px 3px 0px;
	}
</style>
	<script language="Javascript">
	function ToggleExpansion(id)
	{
		var expand = document.getElementById(id + "_Expand");
		var toggle = document.getElementById(id + "_Toggle");

		if (expand)
		{
			if (expand.style.display == "none")
			{
				expand.style.display = "";
				toggle.innerHTML = '&laquo;';
			}
			else
			{
				expand.style.display = "none";
				toggle.innerHTML = "&raquo;";
			}
		}
	}
	</script>
	<div class="Heading1">Log</div>
<asp:Panel runat="server" id="OutputHolder"></asp:Panel>
	
	</asp:Content>