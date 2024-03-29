<%@ Page Language="C#" MasterPageFile="~/Site.master" Title="Backup" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="ss" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import Namespace="System.Reflection" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import namespace="System.IO" %>
<%@ Import namespace="System.Xml" %>
<%@ Import namespace="System.Xml.Serialization" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<script runat="server">

	ApplicationBackup appBackup;

    protected bool PrepareForUpdate
    {
        get
        {
            if (Request.QueryString["PrepareForUpdate"] != null)
                return Convert.ToBoolean(Request.QueryString["PrepareForUpdate"]);
            else
                return false;
        }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
    	appBackup = new ApplicationBackup();
    	
    	try
    	{
    		appBackup.PrepareForUpdate = Convert.ToBoolean(Request.QueryString["PrepareForUpdate"]);
    	}
    	catch (FormatException ex)
    	{
    		LogWriter.Error(ex.ToString());
    		// Don't throw exception. An error here can be ignored and a standard backup will be performed
    	}
    }
	
    protected void Page_Load(object sender, EventArgs e)
    {
		Authorisation.EnsureIsAuthenticated();
		Authorisation.EnsureIsInRole("Administrator");
	
        if (PrepareForUpdate)
        {
            Step2();
        }
        else
        {
            Start();
        }
	}
    
    private void Start()
    {
        PageViews.SetActiveView(Step1View);
    }


    protected void Step2()
    {
        appBackup.Backup();

        Result.Display(Resources.Language.BackupCompleted);

        if (PrepareForUpdate)
        {
            Response.Redirect("UpdateReady.html");
        }
        else
        {

            PageViews.SetActiveView(Step2View);
        }
    }
    
    protected void NextButton_Click(object sender, EventArgs e)
    {
        Step2();
    }
   
</script>
<asp:Content ID="Content1" ContentPlaceHolderID="Body" Runat="Server">
<asp:MultiView runat="server" ID="PageViews">
<asp:View runat="server" id="Step1View">
		<div class="Heading1"><%= Resources.Language.DataBackup %></div>
			
		<p class="Intro"><%= Resources.Language.DataBackupIntro %></p>
				<P>
			
					<asp:Button id="NextButton" onclick="NextButton_Click" Runat="server" Text="Start"></asp:Button><BR/>
				</P>
</asp:View>
<asp:View runat="server" ID="Step2View">
<div class="Heading1"><%= Resources.Language.BackupComplete %></div>
<ss:Result runat="server"></ss:Result>
<p>
<%= Resources.Language.TotalFilesBackedUp %>: <%= appBackup.TotalFilesZipped %>
</p>

<asp:Panel id="OutputPanel" Runat="server"></asp:Panel>
		</asp:View>
		<asp:View runat="server" ID="Step3View">

</asp:View>
<asp:View runat="server" ID="BackupsClearedView">
<h1>Backups Cleared</H1>
<ss:Result runat="server"></ss:Result>


</asp:View>
</asp:MultiView>
</asp:Content>

