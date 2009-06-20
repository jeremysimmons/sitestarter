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

            config.EnableVirtualServer = true;
            config.EnableVirtualServerRegistration = true;
            config.AutoApproveVirtualServerRegistration = true;
            config.PrimaryAdministratorID = user.ID;
            config.Settings["VirtualServerWelcomeEmailSubject"] = Resources.Language.DefaultVirtualServerWelcomeEmailSubject;
            config.Settings["VirtualServerWelcomeEmail"] = Resources.Language.DefaultVirtualServerWelcomeEmail;
            config.Settings["VirtualServerRegistrationAlertSubject"] = Resources.Language.DefaultVirtualServerRegistrationAlertSubject;
            config.Settings["VirtualServerRegistrationAlert"] = Resources.Language.DefaultVirtualServerRegistrationAlert;
	
			//SoftwareMonkeys.SiteStarter.Web.Config.Current = config;

			//config.Save();

            //Config.Application = config;
            

            ConfigFactory.SaveConfig(Request.MapPath(Request.ApplicationPath + "/App_Data"), (IConfig)config, WebUtilities.GetLocationVariation(Request.Url));
            
            SetupMappings();

            // Initialize everything now that the default config has been created
            Config.Initialize(Server.MapPath(Request.ApplicationPath), WebUtilities.GetLocationVariation(HttpContext.Current.Request.Url));
            SoftwareMonkeys.SiteStarter.Web.Providers.DataProviderManager.Initialize();


            if (!SoftwareMonkeys.SiteStarter.Business.UserFactory.Current.SaveUser(user))
            {
            	user = (User)UserFactory.Current.GetUserByUsername(user.Username);
            	
            	config.PrimaryAdministratorID = user.ID;
            	
            	ConfigFactory.SaveConfig(Request.MapPath(Request.ApplicationPath + "/App_Data"), (IConfig)config, WebUtilities.GetLocationVariation(Request.Url));
            }


            if(!Roles.RoleExists("Administrator"))
                Roles.CreateRole("Administrator");

            if (!Roles.IsUserInRole(user.Username, "Administrator"))
                Roles.AddUserToRole(user.Username, "Administrator");

	}

           // Response.Redirect("SetupDefaultData.aspx");
}

private void SetupMappings()
{
	MappingConfig config = Config.Mappings;
	if (config == null)
		config = new MappingConfig();
		
	MappingItem userEntityItem = new MappingItem(typeof(IUser));
	userEntityItem.Settings.Add("DataStoreName", "Users");
	
	config.AddItem(userEntityItem);
	
	MappingItem userRoleEntityItem = new MappingItem(typeof(IUserRole));
	userRoleEntityItem.Settings.Add("DataStoreName", "UserRoles");
	
	config.AddItem(userRoleEntityItem);
	
	string path = Server.MapPath(Request.ApplicationPath + "/App_Data");
	
	ConfigFactory.SaveConfig(path, config);
}
</script>
<asp:Content runat="server" ContentPlaceHolderID="Body">
<div class="Heading1">Quick Setup</div>
<p>
Setup complete.</p>
<p>Test user "Joe Bloggs" was created with username "test" and password "pass".
</p>
		</asp:Content>
