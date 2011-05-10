<%@ Page language="c#" AutoEventWireup="true" MasterPageFile="~/Site.master" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Data" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Projections" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Parts" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Controllers" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import namespace="System.Collections.Generic" %>
<%@ Import namespace="System.IO" %>
<script language="C#" runat="server">

	private void Page_Load(object sender, EventArgs e)
	{
		using (LogGroup logGroup = LogGroup.Start("Running setup script", NLog.LogLevel.Info))
		{
	    	SetupChecker checker = new SetupChecker();
	    	if (checker.RequiresImport())
	    		Response.Redirect(Request.ApplicationPath);
		    		
			Setup();
			//	Response.Redirect("Restore.aspx");
		}
	}

	private void Setup()
	{
		using (LogGroup logGroup = LogGroup.Start("Running setup.", NLog.LogLevel.Debug))
		{
			ApplicationInstaller installer = new ApplicationInstaller();
			
				
			installer.ApplicationPath = Request.ApplicationPath;
			installer.FileMapper = new FileMapper();
			installer.PathVariation = WebUtilities.GetLocationVariation(Request.Url);
			installer.DataProviderInitializer = new DataProviderInitializer();
			installer.AdministratorRoleName = Resources.Language.Administrator;
			
			installer.Administrator = CreateAdministrator();
			
			installer.Setup(GetDefaultSettings());
			
			InitializeWeb();
			
			if (!installer.UseLegacyData)
				Authentication.SetAuthenticatedUsername(installer.Administrator.Username);
		}
	}

	private Dictionary<string, object> GetDefaultSettings()
	{
		Dictionary<string, object> settings = new Dictionary<string, object>();
		settings.Add("EnableUserRegistration", true);
		return settings;
	}


	private void InitializeWeb()
	{
		new ControllersInitializer().Initialize();
		new ProjectionsInitializer(this).Initialize();
		new PartsInitializer(this).Initialize();
	}


		/// <summary>
		/// Creates the default administrator.
		/// </summary>
		/// <returns>The default administrator.</returns>
		private User CreateAdministrator()
		{
			User user = null;
			
			using (LogGroup logGroup = LogGroup.Start("Creating the default administrator user.", NLog.LogLevel.Debug))
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
				
				LogWriter.Debug("Administrator name: " + user.Name);
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
