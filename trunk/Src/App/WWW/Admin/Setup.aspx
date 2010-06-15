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
            user.FirstName = "System";
			user.LastName = "Administrator";
			user.Username = "admin";
            user.Password = SoftwareMonkeys.SiteStarter.Business.Crypter.EncryptPassword("pass");
            user.IsApproved = true;
            user.IsLockedOut = false;
			user.Email = "default@softwaremonkeys.net";

            AppConfig config = ConfigFactory<AppConfig>.NewConfig("Application");
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
	
            config.Settings["ApplicationVersion"] = Utilities.GetVersion();

			//SoftwareMonkeys.SiteStarter.Web.Config.Current = config;

			//config.Save();

            //Config.Application = config;
            

            ConfigFactory<AppConfig>.SaveConfig(Request.MapPath(Request.ApplicationPath + "/App_Data"), config, WebUtilities.GetLocationVariation(Request.Url));
            
            SetupMappings();

            // Initialize everything now that the default config has been created
            Config.Initialize(Server.MapPath(Request.ApplicationPath), WebUtilities.GetLocationVariation(HttpContext.Current.Request.Url));
            SoftwareMonkeys.SiteStarter.Web.Providers.DataProviderManager.Initialize();

            UserRole administratorRole = new UserRole();
            administratorRole.ID = Guid.NewGuid();
            administratorRole.Name = Resources.Language.Administrator;
            //administratorRole.Users = new User[] { user };
            
            user.Roles = new UserRole[] {administratorRole};

            if (SoftwareMonkeys.SiteStarter.Business.UserFactory<User>.Current.SaveUser(user))
            {
                UserRoleFactory.Current.SaveUserRole(administratorRole);
                
            	//user = (User)UserFactory<User>.Current.GetUserByUsername(user.Username);
            	
            	config.PrimaryAdministratorID = user.ID;
            	
            	ConfigFactory<AppConfig>.SaveConfig(Request.MapPath(Request.ApplicationPath + "/App_Data"), config, WebUtilities.GetLocationVariation(Request.Url));
            }


           //if(!Roles.RoleExists("Administrator"))
           //     Roles.CreateRole("Administrator");

           // if (!Roles.IsUserInRole(user.Username, "Administrator"))
           //     Roles.AddUserToRole(user.Username, "Administrator");

	}

           // Response.Redirect("SetupDefaultData.aspx");
}

private void SetupMappings()
{
			if (Config.Mappings == null)
				Config.Mappings = ConfigFactory<MappingConfig>.NewConfig("Mappings");
	
	SoftwareMonkeys.SiteStarter.Entities.User.RegisterType();
    UserRole.RegisterType();
    Keyword.RegisterType();
    VirtualServer.RegisterType();
/*	MappingItem userEntityItem = new MappingItem("IUser");
	userEntityItem.Settings.Add("DataStoreName", "Users");
	
	config.AddItem(userEntityItem);
	
	MappingItem userRoleEntityItem = new MappingItem("IUserRole");
	userRoleEntityItem.Settings.Add("DataStoreName", "UserRoles");
	
	config.AddItem(userRoleEntityItem);
	
	MappingItem keywordItem = new MappingItem("Keyword");
	keywordItem.Settings.Add("DataStoreName", "Keywords");
	
	config.AddItem(keywordItem);
	*/
	
	string path = Server.MapPath(Request.ApplicationPath + "/App_Data");
	
	ConfigFactory<MappingConfig>.SaveConfig(path, (MappingConfig)Config.Mappings, WebUtilities.GetLocationVariation(Request.Url));
}
</script>
<asp:Content runat="server" ContentPlaceHolderID="Body">
<div class="Heading1">Quick Setup</div>
<p>
Setup complete.</p>
<p>Test user "Default Administrator" was created with username "admin" and password "pass".
</p>
		</asp:Content>
