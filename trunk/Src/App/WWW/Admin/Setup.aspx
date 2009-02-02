<%@ Page language="c#" AutoEventWireup="true" MasterPageFile="~/Site.master" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import namespace="System.IO" %>
<script language="C#" runat="server">
private void Page_Load(object sender, EventArgs e)
{
	using (LogGroup logGroup = AppLogger.StartGroup("Running setup script", NLog.LogLevel.Info))
	{
 //   File.Delete(Config.Current.DatabasePath);
    
			User user = new User();
            user.ID = Guid.NewGuid();
            user.FirstName = "Joe";
			user.LastName = "Bloggs";
			user.Username = "test";
            user.Password = SoftwareMonkeys.SiteStarter.Business.Crypter.EncryptPassword("pass");
            user.IsApproved = true;
            user.IsLockedOut = false;
			user.Email = "test@softwaremonkeys.net";

			SoftwareMonkeys.SiteStarter.Configuration.IAppConfig config = new SoftwareMonkeys.SiteStarter.Configuration.AppConfig();
			config.ApplicationPath = Request.ApplicationPath;
			//config.ApplicationUrl = Request.Url.ToString().ToLower().Replace("/setup.aspx", "");
			config.PhysicalPath = Request.PhysicalApplicationPath;
			//config.BackupDirectory = "Backup";
			//config.DataDirectory = "Data";
		//	config.FriendlyDateFormat = "D";
		//	config.HostingDirectory = "Hosted";
		//	config.AttachmentDirectory = "Attachments";
		//	config.PrimaryAdministrator = user;
		//	config.ProjectID = new Guid("2b60aba5-db91-4af8-a5bf-e8ad948d50cd");
		//	config.WorkHubID = new Guid("14bbbd37-51f6-4d82-a8f5-66fa737f4438");
         //   config.WorkHubUrl = "http://www.softwaremonkeys.net/Hub";
		//	config.WorkHubUsername = "WorkHubClient";
		//	config.WorkHubPassword = "d8h3ns6gh";
		//	config.WorkHubHostedUrl = "http://localhost/SoftwareMonkeys/WorkHub/Application/Web";
	
			//SoftwareMonkeys.SiteStarter.Web.Config.Current = config;

			//config.Save();

            //Config.Application = config;

            ConfigFactory.SaveConfig(Request.MapPath(Request.ApplicationPath + "/App_Data"), (IConfig)config, WebUtilities.GetLocationVariation(Request.Url));

            // Initialize everything now that the default config has been created
            Config.Initialize(Server.MapPath(Request.ApplicationPath));
            SoftwareMonkeys.SiteStarter.Web.Providers.DataProviderManager.Initialize();


            SoftwareMonkeys.SiteStarter.Business.UserFactory.SaveUser(user);

	}

           // Response.Redirect("SetupDefaultData.aspx");
}
</script>
<asp:Content runat="server" ContentPlaceHolderID="Body">
<div class="Heading1">Quick Setup</div>
<p>
Setup complete.</p>
<p>Test user "Joe Bloggs" was created with username "test" and password "pass".
</p>
		</asp:Content>
