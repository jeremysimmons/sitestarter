<%@ Page language="c#" AutoEventWireup="true" MasterPageFile="~/Site.master" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.State" %>
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

            SoftwareMonkeys.SiteStarter.Configuration.IVirtualServerConfig config = new SoftwareMonkeys.SiteStarter.Configuration.VirtualServerConfig();
			
            ConfigFactory.SaveConfig(Request.MapPath(Request.ApplicationPath + "/App_Data/VS/" + VirtualServerState.VirtualServerID), (IConfig)config, WebUtilities.GetLocationVariation(Request.Url));

            // Initialize everything now that the default config has been created
            //Config.Initialize(Server.MapPath(Request.ApplicationPath));
            //SoftwareMonkeys.SiteStarter.Web.Providers.DataProviderManager.Initialize();


            SoftwareMonkeys.SiteStarter.Business.UserFactory.SaveUser(user);

            if(!Roles.RoleExists("Administrator"))
                Roles.CreateRole("Administrator");

            if (!Roles.IsUserInRole(user.Username, "Administrator"))
                Roles.AddUserToRole(user.Username, "Administrator");

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
