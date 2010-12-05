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
        appBackup.PrepareForUpdate = false;
    }
	
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Config.IsInitialized)
            AutoBackup();
        
	}

    private void AutoBackup()
    {
		if (Config.IsInitialized)
		{
        	appBackup.Backup();

        	//Result.Display(Resources.Language.BackupComplete);

            Config.Application.Settings["LastAutoBackup"] = DateTime.Now;

			Config.Application.Save();
			
            //ConfigFactory<AppConfig>.SaveConfig(
            //    Server.MapPath(Request.ApplicationPath + "/App_Data"),
            //    (AppConfig)Config.Application,
        	//Config.Application.PathVariation);
        }
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
<asp:panel runat="server" visible="false">
<div style="padding: 5px; margin:0px;" class="AutoBackup">
<span style="color:Green;"><%= Resources.Language.BackupComplete %></span><br />
<%= DateTime.Now.ToString() %>
</div>
</asp:panel>
</asp:Content>

