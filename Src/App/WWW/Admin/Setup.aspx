<%@ Page language="c#" AutoEventWireup="true" MasterPageFile="~/Site.master" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import namespace="System.IO" %>
<script language="C#" runat="server">


private void Page_Load(object sender, EventArgs e)
{
	using (LogGroup logGroup = AppLogger.StartGroup("Running setup script", NLog.LogLevel.Info))
	{
 //   File.Delete(Config.Current.DatabasePath);
			User user = new User();
			
    		if (!SetupUtilities.UseExistingData)
	    	{
	            user = CreateAdministrator();
	            
	    		AppLogger.Debug("No legacy data. Created new administrator.");
			}

			AppConfig config = CreateConfig(user);
			
			config.Save(); // Needs to be saved here but will be updated again during setup
			
			AppLogger.Debug("Created application configuration object.");

            SetupMappings();
            
            AppLogger.Debug("Finished setting up core mappings.");

			Initialize();
			
			AppLogger.Debug("Initialized config and data.");

			if (!SetupUtilities.UseExistingData)
			{			
	            UserRole role = CreateAdministratorRole(user);
	            
	            AppLogger.Debug("Created administrator role.");
	            
	            Save(user, role, config);
	            
	            AppLogger.Debug("Saved administrator user and administrator role.");
	            
				DataUtilities.InitializeDataVersion();
            }
            else
            {
            	AppLogger.Debug("Legacy data exists. Redirecting to import page.");
            	Response.Redirect("Import.aspx");
            }


           //if(!Roles.RoleExists("Administrator"))
           //     Roles.CreateRole("Administrator");

           // if (!Roles.IsUserInRole(user.Username, "Administrator"))
           //     Roles.AddUserToRole(user.Username, "Administrator");

	}

           // Response.Redirect("SetupDefaultData.aspx");
}

private User CreateAdministrator()
{
		User user = new User();
		user.ID = Guid.NewGuid();
        user.FirstName = "System";
		user.LastName = "Administrator";
		user.Username = "admin";
        user.Password = SoftwareMonkeys.SiteStarter.Business.Crypter.EncryptPassword("pass");
        user.IsApproved = true;
        user.IsLockedOut = false;
		user.Email = "default@softwaremonkeys.net";
		
		return user;
}

private UserRole CreateAdministratorRole(User user)
{
	UserRole administratorRole = new UserRole();
    administratorRole.ID = Guid.NewGuid();
    administratorRole.Name = Resources.Language.Administrator;
    administratorRole.Users = new User[] { user };
    
    
    return administratorRole;
}

private void Save(User user, UserRole administratorRole, AppConfig config)
{

    if (SoftwareMonkeys.SiteStarter.Business.UserFactory<User>.Current.SaveUser(user))
    {
    
        UserRoleFactory.Current.SaveUserRole(administratorRole);
        
    	//user = (User)UserFactory<User>.Current.GetUserByUsername(user.Username);
    	
    	config.PrimaryAdministratorID = user.ID;
    	
    	config.Save();
    }
}

private AppConfig CreateConfig(User user)
{

            AppConfig config = ConfigFactory<AppConfig>.NewConfig(Server.MapPath(Request.ApplicationPath + "/App_Data"), "Application", WebUtilities.GetLocationVariation(Request.Url));
			config.ApplicationPath = Request.ApplicationPath;
			config.PhysicalApplicationPath = Request.PhysicalApplicationPath;

            config.EnableVirtualServer = true;
            config.EnableVirtualServerRegistration = true;
            config.AutoApproveVirtualServerRegistration = true;
            
            if (!SetupUtilities.UseExistingData)
            	config.PrimaryAdministratorID = user.ID;
            	
            	// TODO: Remove if not needed
            	/*
            config.Settings["VirtualServerWelcomeEmailSubject"] = Resources.Language.DefaultVirtualServerWelcomeEmailSubject;
            config.Settings["VirtualServerWelcomeEmail"] = Resources.Language.DefaultVirtualServerWelcomeEmail;
            config.Settings["VirtualServerRegistrationAlertSubject"] = Resources.Language.DefaultVirtualServerRegistrationAlertSubject;
            config.Settings["VirtualServerRegistrationAlert"] = Resources.Language.DefaultVirtualServerRegistrationAlert;
	*/
            config.Settings["ApplicationVersion"] = Utilities.GetVersion();
            
            return config;
            
}

private void Initialize()
{

            // Initialize everything now that the default config has been created
            Config.Initialize(Server.MapPath(Request.ApplicationPath), WebUtilities.GetLocationVariation(HttpContext.Current.Request.Url));
            SoftwareMonkeys.SiteStarter.Web.Providers.DataProviderManager.Initialize();
}

private void SetupMappings()
{
	if (Config.Mappings == null)
		Config.Mappings = ConfigFactory<MappingConfig>.NewConfig(Server.MapPath(Request.ApplicationPath + "/App_Data"), "Mappings", WebUtilities.GetLocationVariation(Request.Url));

	SoftwareMonkeys.SiteStarter.Entities.User.RegisterType();
    UserRole.RegisterType();
    Keyword.RegisterType();
    VirtualServer.RegisterType();

	Config.Mappings.Save();
}
</script>
<asp:Content runat="server" ContentPlaceHolderID="Body">
<div class="Heading1">Quick Setup</div>
<p>
Setup complete.</p>
<p>Test user "Default Administrator" was created with username "admin" and password "pass".
</p>
		</asp:Content>
