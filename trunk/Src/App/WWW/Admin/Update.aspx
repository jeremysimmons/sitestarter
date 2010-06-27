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
<script runat="server">
	
    protected void Page_Load(object sender, EventArgs e)
    {
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

    protected void NextButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("Backup.aspx?PrepareForUpdate=true");
    }
    

</script>
<asp:Content ID="Content1" ContentPlaceHolderID="Body" Runat="Server">
<asp:MultiView runat="server" ID="PageViews">
<asp:View runat="server" id="Step1View">
		<h1>Application Update</H1>
			<p>
			    This page prepares the application for an update.
			</p>
			<p>
			    The following procedure ensures data is backed up and prepared so that it works in the new version.
			    <ol><li>
			    Download a project release via HTTP or SVN (see <a href="http://www.softwaremonkeys.net">www.softwaremonkeys.net</a>).
			    </li>
			    <li>
			    Click the button below to prepare the installation and backup your data.
			    </li>
			    <li>
			    Upload the new release via FTP.
			    </li>
			    <li>
			    Launch the project via the browser to complete the process.
			    </li>
			    </ol>
			</p>
			<p>When you're ready for step 2...</p>
				<P>
			
					<asp:Button id="NextButton" onclick="NextButton_Click" Runat="server" Text="Start Preparation"></asp:Button><BR/>
				</P>
</asp:View>
<asp:View runat="server" id="Step2View">
		<h1>Preparation Complete</H1>
			<p>
			    The preparation and backup process is complete.
			</p>
			<p>
			    A zip file of your data can be found in the following folder:<br />
			    <i><%= Server.MapPath(Request.ApplicationPath + Path.DirectorySeparatorChar + ConfigurationSettings.AppSettings["BackupDirectory"]) %></i>
			</p>
			<p>You can now upload the files via FTP.</p>
			<p><a href="Admin/Setup.aspx">Click here when you're done.</a></p>
				
</asp:View>
</asp:MultiView>
</asp:Content>

