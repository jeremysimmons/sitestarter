<%@ Page Language="C#" MasterPageFile="~/Site.master" Title="Register" AutoEventWireup="true" %>
<%@ Register TagPrefix="cc" Assembly="SoftwareMonkeys.SiteStarter.Web" Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.State" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.State" %>
<%@ Import Namespace="System.Web.Security" %>
<script runat="server">
    private void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Perform the appropriate action based on the query string (parameter: "a")
            switch (QueryStrings.Action)
            {
                case "Register":
                default:
                    Register();
                    break;
            }
        }
    }

    #region Main functions
    /// <summary>
    /// Displays the form for registration.
    /// </summary>
    private void Register()
    {
        FormsAuthentication.SignOut();
        
        OperationManager.StartOperation("Register", CreateView);

        VirtualServer server = new VirtualServer();
        server.ID = Guid.NewGuid();
        server.DateCreated = DateTime.Now;
        server.IsApproved = Config.Application.AutoApproveVirtualServerRegistration; // TODO: Check if servers should automatically be approved
        
        User user = new User();
        user.ID = Guid.NewGuid();
//        user.IsAdministrator = true;
        user.IsApproved = true;
        
        VirtualServerDataForm.DataSource = server;
        AdministratorDataForm.DataSource = user;

        CreateView.DataBind();
    }
    
        /// <summary>
    /// Displays the form for registration.
    /// </summary>
    private void WaitForApproval()
    {
        
        OperationManager.StartOperation("WaitForApproval", WaitForApprovalView);

    }

    /// <summary>
    /// Submits the newly registered server to be saved.
    /// </summary>
    private void SubmitRegister()
    {
    	using (LogGroup logGroup = AppLogger.StartGroup("Submitting the registration to create a new virtual server.", NLog.LogLevel.Debug))
    	{
	    	VirtualServer server = (VirtualServer)VirtualServerDataForm.DataSource;
	    	User administrator = (User)AdministratorDataForm.DataSource;
	    	
	    	RetrieveUserStrategy administratorRetriever = new RetrieveUserStrategy();
	    	administratorRetriever.VirtualServerID = String.Empty;
	    	
	    	User systemAdministrator = administratorRetriever.Retrieve(Config.Application.PrimaryAdministratorID);
	    	
	    	server.PrimaryAdministrator = administrator;
	        
	        VirtualServerDataForm.ReverseBind(server);
	        AdministratorDataForm.ReverseBind(administrator);
	        
	        administrator.Password = Crypter.EncryptPassword(administrator.Password);
	        
	        server.Keywords = Config.Application.DefaultVirtualServerKeywords;
	        
	        // Reset the current server state temporarily
	        if (VirtualServerFactory.Current.SaveVirtualServer(server))
	        {
		            if (Config.Application.AutoApproveVirtualServerRegistration)
		            {                    
			            VirtualServerState.Switch(server.Name, server.ID);
	
	
	                    if (!Roles.RoleExists("Administrator"))
	                        Roles.CreateRole("Administrator");
	
	                    administrator.Roles = new UserRole[] { UserRoleFactory.Current.GetUserRoleByName("Administrator") };
	                    
			            UserFactory.Current.SaveUser(administrator);
				            
			            FormsAuthentication.SetAuthCookie(administrator.Username, false);
			            My.User = administrator;
	                    
			            server.PrimaryAdministrator = administrator;
	
	                    VirtualServerFactory.Current.SendWelcomeEmail(server, TextParser.ParseSetting("VirtualServerWelcomeEmailSubject"), TextParser.ParseSetting("VirtualServerWelcomeEmail"), systemAdministrator);
	                    VirtualServerFactory.Current.SendRegistrationAlert(server, TextParser.ParseSetting("VirtualServerRegistrationAlertSubject"), TextParser.ParseSetting("VirtualServerRegistrationAlert"), systemAdministrator);
	                    
		    	        // Display the result to the server
			            Result.Display(Resources.Language.VirtualServerCreated);
			            
			            Response.Redirect("Admin/Users.aspx");
		
			            //Response.Redirect(Request.ApplicationPath + "/Admin/SetupVS.aspx");
			        }
			        else
			        	WaitForApproval();
	        }
	        else
	            Result.DisplayError(Resources.Language.VirtualServerNameTaken);
        }
    }
    #endregion

    #region Event handlers
    private void DataForm_EntityCommand(object sender, EntityFormEventArgs e)
    {
        if (e.CommandName == "Register")
        {
            SubmitRegister();
        }
    }
    #endregion
</script>
<asp:Content ID="Body" ContentPlaceHolderID="Body" runat="Server">
<asp:MultiView runat="server" id="PageViews">
<asp:View runat="server" id="CreateView">
<div class="Heading1"><%# Resources.Language.CreateVirtualServer %></div>
<p>
<cc:Result runat="Server"></cc:Result>
<%# Resources.Language.CreateVirtualServerIntro %></p>
    <cc:EntityForm runat="server" id="VirtualServerDataForm" OnEntityCommand="DataForm_EntityCommand" CssClass="Panel" headingtext='<%# OperationManager.CurrentOperation == "CreateVirtualServer" ? Resources.Language.NewVirtualServerDetails : Resources.Language.VirtualServerDetails %>' headingcssclass="Heading2" width="100%">
<cc:EntityFormTextBoxItem runat="server" PropertyName="Name" TextBox-Width="400" FieldControlID="Name" IsRequired="true" text='<%# Resources.Language.Name + ":" %>' RequiredErrorMessage='<%# Resources.Language.VirtualServerNameRequired %>'></cc:EntityFormTextBoxItem>
                                      
</cc:EntityForm> 

      <cc:EntityForm runat="server" id="AdministratorDataForm" OnEntityCommand="DataForm_EntityCommand" CssClass="Panel" HeadingText='<%# Resources.Language.AdministratorDetails %>' headingcssclass="Heading2" width="100%">
			
                           <cc:EntityFormTextBoxItem runat="server" PropertyName="Username" TextBox-Width="400" FieldControlID="Username" IsRequired="true" text='<%# Resources.Language.Username + ":" %>' RequiredErrorMessage='<%# Resources.Language.UsernameRequired %>'></cc:EntityFormTextBoxItem>
                        
                           <cc:EntityFormTextBoxItem runat="server" PropertyName="Password" TextBox-Width="200" FieldControlID="Password" IsRequired='true' text='<%# Resources.Language.Password + ":" %>' RequiredErrorMessage='<%# Resources.Language.PasswordRequired %>' TextBox-TextMode='Password'></cc:EntityFormTextBoxItem>
                        
                           <cc:EntityFormTextBoxItem runat="server" PropertyName="Password" TextBox-Width="200" FieldControlID="PasswordConfirm" IsRequired='true' text='<%# Resources.Language.PasswordConfirm + ":" %>' RequiredErrorMessage='<%# Resources.Language.PasswordRequired %>' TextBox-TextMode='Password'></cc:EntityFormTextBoxItem>
                        
                           <cc:EntityFormTextBoxItem runat="server" PropertyName="FirstName" TextBox-Width="400" FieldControlID="FirstName" IsRequired="true" text='<%# Resources.Language.FirstName + ":" %>' RequiredErrorMessage='<%# Resources.Language.FirstNameRequired %>'></cc:EntityFormTextBoxItem>
                        
                           <cc:EntityFormTextBoxItem runat="server" PropertyName="LastName" TextBox-Width="400" FieldControlID="LastName" IsRequired="true" text='<%# Resources.Language.LastName + ":" %>' RequiredErrorMessage='<%# Resources.Language.LastNameRequired %>'></cc:EntityFormTextBoxItem>
                        
                           <cc:EntityFormTextBoxItem runat="server" PropertyName="Email" TextBox-Width="400" FieldControlID="Email" IsRequired="true" text='<%# Resources.Language.Email + ":" %>' RequiredErrorMessage='<%# Resources.Language.EmailRequired %>'></cc:EntityFormTextBoxItem>
                        
                      <cc:EntityFormButtonsItem runat="server"><FieldTemplate><asp:Button ID="RegisterButton" runat="server" CausesValidation="True" CommandName="Register"
                                                    Text='<%# Resources.Language.Register %>'></asp:Button>
                                                   </FieldTemplate></cc:EntityFormButtonsItem>
</cc:EntityForm> 
</asp:View>
<asp:View runat="Server" id="WaitForApprovalView"><div class="Heading1"><%# Resources.Language.ApplicationSubmitted %></div>
<p>
<cc:Result runat="Server"></cc:Result>
<%# Resources.Language.ApprovalRequiredIntro %></p>
   
</asp:View>
</asp:MultiView>
</asp:Content>
