<%@ Page language="c#" AutoEventWireup="true" MasterPageFile="~/Site.master" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Properties" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Projections" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Controllers" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.State" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Data" %>
<%@ Import namespace="System.Xml.Serialization" %>
<%@ Import namespace="System.Collections.Specialized" %>
<%@ Import namespace="System.Collections.Generic" %>
<%@ Import namespace="System.Xml" %>
<%@ Register TagPrefix="cc" Assembly="SoftwareMonkeys.SiteStarter.Web" Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import namespace="System.IO" %>

<script language="C#" runat="server">


    private void Page_Init(object sender, EventArgs e)
    {
        using (LogGroup logGroup = LogGroup.Start("Initializing the import page.", NLog.LogLevel.Debug))
        {
            if (!IsPostBack)
            {
				using (TimeoutExtender extender = TimeoutExtender.NewMinutes(30))
				{
					Import();
				}
            }
        }
        
    }

    private void Import()
    {
        using (LogGroup logGroup = LogGroup.Start("Starting the import.", NLog.LogLevel.Debug))
        {
        	ExecuteSetup();
        	
            ExecuteImport();

            PageViews.SetActiveView(SetupCompleteView);
            
        }
    }

    private void ExecuteSetup()
    {
    
        string dataDirectoryPath = Server.MapPath(Request.ApplicationPath) + Path.DirectorySeparatorChar + "App_Data";
            
		ApplicationInstaller installer = new ApplicationInstaller();
		installer.UseLegacyData = true;
		installer.ApplicationPath = Request.ApplicationPath;
		installer.FileMapper = new FileMapper();
		installer.PathVariation = WebUtilities.GetLocationVariation(Request.Url);
		installer.DataProviderInitializer = new DataProviderInitializer();
		
		installer.Setup();
		
		InitializeWeb();
    }
    
	private void InitializeWeb()
	{
		new ControllersInitializer().Initialize();
		new ProjectionsInitializer(this).Initialize();
	}
    
    private void ExecuteImport()
    {
		using (LogGroup logGroup = LogGroup.Start("Running the import process.", NLog.LogLevel.Debug))
        {
            string dataDirectoryPath = Server.MapPath(Request.ApplicationPath) + Path.DirectorySeparatorChar + "App_Data";
            
            ApplicationRestorer restorer = new ApplicationRestorer(new FileMapper());
            restorer.LegacyDirectoryPath = dataDirectoryPath + Path.DirectorySeparatorChar + "Import";
            
            restorer.PersonalizationDirectoryPath = dataDirectoryPath + Path.DirectorySeparatorChar + "Personalization_Data";
            
            restorer.Restore();
            
		}
    }
	
</script>
<asp:Content ContentPlaceHolderID="Body" runat="Server" ID="Body">
<asp:MultiView runat="server" ID="PageViews">
<asp:View runat="server" ID="SetupCompleteView">

<div class="Heading1"><%= Resources.Language.ImportComplete%></div>
<p><%= Resources.Language.ImportCompleteMessage %></p>
<ul><li><a href='../User-SignIn.aspx'><%= Resources.Language.SignIn %></a></li></ul>

</asp:View>
</asp:MultiView>
</asp:Content>
