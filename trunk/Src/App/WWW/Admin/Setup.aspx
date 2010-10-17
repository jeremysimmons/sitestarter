<%@ Page language="c#" AutoEventWireup="true" MasterPageFile="~/Site.master" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Data" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Projections" %>
<%@ Import namespace="System.IO" %>
<script language="C#" runat="server">


private void Page_Load(object sender, EventArgs e)
{
	using (LogGroup logGroup = AppLogger.StartGroup("Running setup script", NLog.LogLevel.Info))
	{
		Setup();
		//	Response.Redirect("Restore.aspx");
	}
}

private void Setup()
{

		ApplicationInstaller installer = new ApplicationInstaller();
		
			
		installer.ApplicationPath = Request.ApplicationPath;
		installer.FileMapper = new FileMapper();
		installer.PathVariation = WebUtilities.GetLocationVariation(Request.Url);
		installer.DataProviderInitializer = new DataProviderInitializer();
		installer.AdministratorRoleName = Resources.Language.Administrator;
		
		
		if (installer.IsInstalled)
			Response.Redirect(Request.ApplicationPath);
		
		installer.Administrator = CreateAdministrator();
		
		installer.Setup();
		
		InitializeWeb();
		
		if (!installer.UseLegacyData)
			FormsAuthentication.SetAuthCookie(installer.Administrator.Username, true);
}

	private void InitializeWeb()
	{
		new ProjectionsInitializer().Initialize();
	}


		/// <summary>
		/// Creates the default administrator.
		/// </summary>
		/// <returns>The default administrator.</returns>
		private User CreateAdministrator()
		{
			User user = null;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Creating the default administrator user.", NLog.LogLevel.Debug))
			{
				user = new User();
				user.ID = Guid.NewGuid();
				user.FirstName = "System";
				user.LastName = "Administrator";
				user.Username = "admin";
				user.Password = SoftwareMonkeys.SiteStarter.Business.Crypter.EncryptPassword("pass");
				user.IsApproved = true;
				user.IsLockedOut = false;
				user.Email = "default@softwaremonkeys.net";
				
				AppLogger.Debug("Administrator name: " + user.Name);
			}
			
			return user;
		}

</script>
<asp:Content runat="server" ContentPlaceHolderID="Body">
<h1>Setup</h1>
<p>
Setup complete.</p>
<p>Test user "Default Administrator" was created with username "admin" and password "pass".
</p>
		</asp:Content>
