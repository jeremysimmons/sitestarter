<%@ Page Language="C#" MasterPageFile="~/Site.master" Title="Backup" %>
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
<script runat="server">

    XmlBackupManager manager;

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
        manager = new XmlBackupManager(Server.MapPath(Request.ApplicationPath),
            "App_Data",
            "Backups",
            WebUtilities.GetLocationVariation(Request.Url),
            false);
    }
	
    protected void Page_Load(object sender, EventArgs e)
    {
		if (!Request.IsAuthenticated && Request.QueryString["Auto"] != "true")
			Response.Redirect("../Members/Login.aspx");
	
		
		//if (Request.QueryString["Auto"] == "true")
		//{
		//	Step2();
		//}
        //else
        if (PrepareForUpdate)
        {
            Step2();
        }
        else
        {
            if (Request.QueryString["a"] == "ClearBackups")
            {
                ClearBackups();
            }
            else
                Start();
        }
	}

    private void ClearBackups()
    {
        manager.ClearBackups();
        
        Result.Display("The backups have been cleared.");

        PageViews.SetActiveView(BackupsClearedView);
    }
    
    
    private void Start()
    {
        PageViews.SetActiveView(Step1View);
    }


    protected void Step2()
    {
        manager.ExecuteBackup();

        Result.Display(Resources.Language.BackupCompleted);

        if (PrepareForUpdate)
        {
            Response.Redirect("Update.aspx?BackupComplete=true");
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
    

		
		public string ConvertRelativeUrlToAbsoluteUrl(string relativeUrl)
		{
			if (Page.Request.IsSecureConnection)
				return string.Format("https://{0}{1}", Page.Request.Url.Host, Page.ResolveUrl(relativeUrl));
			else
				return string.Format("http://{0}{1}", Page.Request.Url.Host, Page.ResolveUrl(relativeUrl));
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
<ss:Result runat="server"></ss:Result><br />
<%= Resources.Language.Found %>: <%= manager.TotalRecordsFound %><br/>
<%= Resources.Language.Exported %>: <%= manager.TotalRecordsExported %><br/>
<%= Resources.Language.Zipped %>: <%= manager.TotalRecordsZipped %><br/>
<asp:PlaceHolder runat="server" Visible="false">
<a href='<%= manager.GetBackupZipWebPath(Request.ApplicationPath) %>' target="_blank"><%= Resources.Language.DownloadBackup %></a>
<br/><br/>

<a href='?a=ClearBackups' target="_blank"><%= Resources.Language.DeleteBackups %></a> - <%= Resources.Language.DeleteBackupsInfo %>
</asp:PlaceHolder>
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

