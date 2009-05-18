<%@ Page Language="C#" MasterPageFile="~/Site.master" Title="Register" AutoEventWireup="true" %>
<%@ Register TagPrefix="cc" Assembly="SoftwareMonkeys.SiteStarter.Web" Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.State" %>
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
        server.IsApproved = Config.Application.AutoApproveVirtualServerRegistration; // TODO: Check if servers should automatically be approved
        
        DataForm.DataSource = server;

        DataBind();
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
        DataForm.ReverseBind();
           
        if (VirtualServerFactory.SaveVirtualServer((VirtualServer)DataForm.DataSource))
        {
            // Display the result to the server
            Result.Display(Resources.Language.VirtualServerCreated);

            // Go to the account page
            VirtualServerState.VirtualServerName = ((VirtualServer)DataForm.DataSource).Name;
            VirtualServerState.VirtualServerID = ((VirtualServer)DataForm.DataSource).ID.ToString();
            
            if (Config.Application.AutoApproveVirtualServerRegistration)
	            Response.Redirect(Request.ApplicationPath + "/Admin/SetupVS.aspx");
	        else
	        	WaitForApproval();
        }
        else
            Result.DisplayError(Resources.Language.VirtualServerNameTaken);
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
<asp:MultiView runat="server" id="PageViews">
<asp:View runat="server" id="CreateView">
<div class="Heading1"><%# Resources.Language.Register %></div>
<p>
<cc:Result runat="Server"></cc:Result>
<%# Resources.Language.RegisterIntro %></p>
    <cc:EntityForm runat="server" id="DataForm" OnEntityCommand="DataForm_EntityCommand" CssClass="Panel" headingtext='<%# OperationManager.CurrentOperation == "CreateVirtualServer" ? Resources.Language.NewVirtualServerDetails : Resources.Language.VirtualServerDetails %>' headingcssclass="Heading2" width="100%">
<cc:EntityFormTextBoxItem runat="server" PropertyName="Name" TextBox-Width="400" FieldControlID="Name" IsRequired="true" text='<%# Resources.Language.Name + ":" %>' RequiredErrorMessage='<%# Resources.Language.VirtualServerNameRequired %>'></cc:EntityFormTextBoxItem>
                                      <cc:EntityFormButtonsItem runat="server"><FieldTemplate><asp:Button ID="RegisterButton" runat="server" CausesValidation="True" CommandName="Register"
                                                    Text='<%# Resources.Language.Register %>'></asp:Button>
                                                   </FieldTemplate></cc:EntityFormButtonsItem>
</cc:EntityForm> 
</asp:View>
<asp:View runat="Server" id="WaitForApprovalView"><div class="Heading1"><%# Resources.Language.ApprovalRequired %></div>
<p>
<cc:Result runat="Server"></cc:Result>
<%# Resources.Language.ApprovalRequiredIntro %></p>
   
</asp:View>
</asp:MultiView>
</asp:Content>
