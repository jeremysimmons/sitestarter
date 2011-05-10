<%@ Page Language="C#" MasterPageFile="~/Site.master" Title="Untitled Page" %>
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
<%@ Import namespace="ICSharpCode.SharpZipLib.Zip" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<script runat="server">
	
    protected void Page_Load(object sender, EventArgs e)
    {
    	Authorisation.EnsureIsAuthenticated();
    	Authorisation.EnsureIsInRole("Administrator");
    
        if (Request.QueryString["BackupComplete"] == "true")
            Finish();
        else
            Start();
	}
    
    private void Start()
    {
        PageViews.SetActiveView(Step1View);
    }

    private void Finish()
    {
        PageViews.SetActiveView(Step2View);
    }

</script>
<asp:Content ID="Content1" ContentPlaceHolderID="Body" Runat="Server">
<asp:MultiView runat="server" ID="PageViews">
<asp:View runat="server" id="Step1View">
		<h1>Application Update</H1>
			<p>
			    This wizard will guide you through the process of updating the application.
			</p>
			<p>The current application version is: <%= DataAccess.Data.Schema.ApplicationVersion.ToString() %></p>
			<asp:placeholder runat="server" visible="false">
			<p>
			<i>Note: Your data will be backed up before the update begins. The backup zip files can be accessed at:<br/>
			<%= new ApplicationBackup().BackupDirectoryPath %></i>
			</p>
			</asp:placeholder>
				<p>
			    <ol><li>
			    Download the latest release via HTTP or SVN - get it at <a href="http://www.softwaremonkeys.net" target="_blank">www.softwaremonkeys.net</a>.
			    </li>
			    <li>
			    When you are ready... <input type="button" value='Begin &raquo;' onclick="location.href='Backup.aspx?PrepareForUpdate=true'"/><br/>
				(IMPORTANT: clicking "Begin" will take the application offline, display a friendly "Down for Maintenance" message until the update process is complete)
			    </li>
			    <li>
			    Follow the instructions provided on the next page.
			    </li>
			    </ol>
			</p>
</asp:View>
<asp:View runat="server" id="Step2View">
		<h1>Preparation Complete</H1>
			<p>
			    The preparation and backup process is complete.
			</p>
			<p>
			    A zip file of your data can be found in the following folder:<br />
			    <i><%= new ApplicationBackup().BackupDirectoryPath %></i>
			</p>
			<p>You can now upload the files via FTP.</p>
			<p><a href="Admin/Setup.aspx">Click here when you're done.</a></p>
				
</asp:View>
</asp:MultiView>
</asp:Content>

