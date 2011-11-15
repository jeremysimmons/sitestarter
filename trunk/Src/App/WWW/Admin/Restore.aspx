<%@ Page language="c#" AutoEventWireup="true" MasterPageFile="~/Site.master" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Properties" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.State" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Data" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Projections" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Controllers" %>
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
		        
        	EnsureRequiresRestore();
        
            if (!IsPostBack)
            {
            	// Set a long timeout because the import process can take a long time especially if there's a lot of data
				using (TimeoutExtender extender = TimeoutExtender.NewMinutes(120))
				{
					Restore();
				}
            }
        }
        
    }
    
    private void EnsureRequiresRestore()
    {
			SetupChecker setupChecker = new SetupChecker();
			if (!setupChecker.RequiresRestore())
				Response.Redirect(Request.ApplicationPath);
    }

    private void Restore()
    {
        using (LogGroup logGroup = LogGroup.Start("Starting the import.", NLog.LogLevel.Debug))
        {
        	ExecuteSetup();
        
            ExecuteRestore();
        }
    }
    
    private void ExecuteSetup()
    {
    
		ApplicationInstaller installer = new ApplicationInstaller();
		installer.UseLegacyData = true;
		installer.ApplicationPath = Request.ApplicationPath;
		installer.FileMapper = new FileMapper();
		installer.PathVariation = WebUtilities.GetLocationVariation(Request.Url);
		installer.DataProviderInitializer = new DataProviderInitializer();
		installer.AdministratorRoleName = Resources.Language.Administrator;
		
		installer.Setup();
		
		InitializeWeb();
    }
    
	private void InitializeWeb()
	{
		new ControllersInitializer().Initialize();
		new ProjectionsInitializer(this).Initialize();
	}


    private void ExecuteRestore()
    {
		using (LogGroup logGroup = LogGroup.Start("Running the import process.", NLog.LogLevel.Debug))
        {
            
            LogWriter.Debug("Converting and importing core data.");

           
            string dataDirectoryPath = Server.MapPath(Request.ApplicationPath) + Path.DirectorySeparatorChar + "App_Data";
            
            ApplicationRestorer restorer = new ApplicationRestorer(new FileMapper());
            restorer.LegacyDirectoryPath = dataDirectoryPath + Path.DirectorySeparatorChar + "Legacy";
            restorer.PersonalizationDirectoryPath = dataDirectoryPath + Path.DirectorySeparatorChar + "Personalization_Data";
            
            if (restorer.RequiresRestore)
	            restorer.Restore();
	        else
	        	Response.Redirect(Request.ApplicationPath + "/Default.aspx");



            
		}
    }
	
</script>
<asp:Content ContentPlaceHolderID="Body" runat="Server" ID="Body">
<div class="Heading1"><%= Resources.Language.UpdateComplete%></div>
<p><%= Resources.Language.UpdateCompleteMessage %></p>
<p><%= Resources.Language.PreviousVersion %>: <%= DataAccess.Data.Schema.LegacyVersion %></p>
<p><%= Resources.Language.CurrentVersion %>: <%= DataAccess.Data.Schema.ApplicationVersion %></p>
<p>
<a href='<%= Request.ApplicationPath %>'><%= Resources.Language.Continue %> &raquo;</a>
</p>
</asp:Content>
