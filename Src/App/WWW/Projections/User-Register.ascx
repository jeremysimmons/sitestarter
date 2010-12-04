<%@ Control Language="C#" ClassName="RegisterEditProjection" Inherits="SoftwareMonkeys.SiteStarter.Web.Projections.BaseCreateEditProjection" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Properties" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business.Security" %>
<%@ Import Namespace="System.Collections.Generic" %>
<script runat="server">
    private void Page_Init(object sender, EventArgs e)
    {
        RequireAuthorisation = false;
        
        Initialize(typeof(User), DataForm); 
        
        CreateController.EntitySavedLanguageKey = "UserRegistered";
    }

    #region Main functions
    

    /// <summary>
    /// Displays the form for registering a new user.
    /// </summary>
    public override void Create()
    {
    	PageViews.SetActiveView(FormView);
    
    	bool isApproved = false;
    
    	RequireAuthorisation = false;
    
        User user = new User();
        user.ID = Guid.NewGuid();
        user.IsApproved = Config.Application.Settings.GetBool("AutoApproveNewUsers");
        
        DataForm.DataSource = user;
         
        WindowTitle = Resources.Language.Register;
         
        Create(user);
    }
    
    
    /// <summary>
    /// Saves the user from the form.
    /// </summary>
    public override bool Save()
    {
    	AutoNavigate = false;
    	
    	bool success = base.Save();
    	
    	User user = (User)DataSource;
    	
    	if (Config.Application.Settings.GetBool("AutoApproveNewUsers"))
    	{
    		Authentication.SetAuthenticatedUsername(user.Username);

    		NavigateAfterSave();
    	}
    	else
    	{
    		PageViews.SetActiveView(PendingView);
    	}
    	
    	return success;
    }
    
    
    
    public override void NavigateAfterOperation()
    {
    	Response.Redirect(Request.ApplicationPath);
    }
    #endregion


    protected void UserRolesSelect_DataLoading(object sender, EventArgs e)
    {
        ((EntitySelect)sender).DataSource = IndexStrategy.New<UserRole>().Index();
    }
                    
</script>
	<asp:MultiView runat="server" id="PageViews">
	<asp:View runat="server" id="FormView">
                   <h1>
                                <%= Resources.Language.Register %>
                   </h1>
                                <cc:Result ID="Result2" runat="server">
                                </cc:Result>
                                <p class="Intro">
                                    <%= Resources.Language.RegisterIntro %></p>  
                            <cc:EntityForm runat="server" DataSource='<%# DataSource %>' id="DataForm" CssClass="Panel" headingtext='<%# Resources.Language.NewUserDetails %>' headingcssclass="Heading2" width="100%">
<cc:EntityFormTextBoxItem runat="server" PropertyName="FirstName" TextBox-Width="400" FieldControlID="FirstName" IsRequired="true" text='<%# Resources.Language.FirstName + ":" %>' RequiredErrorMessage='<%# Resources.Language.FirstNameRequired %>'></cc:EntityFormTextBoxItem>
<cc:EntityFormTextBoxItem runat="server" PropertyName="LastName" TextBox-Width="400" FieldControlID="LastName" IsRequired="true" text='<%# Resources.Language.LastName + ":" %>' RequiredErrorMessage='<%# Resources.Language.LastNameRequired %>'></cc:EntityFormTextBoxItem>
   <cc:EntityFormTextBoxItem runat="server" PropertyName="Email" TextBox-Width="400" FieldControlID="Email" IsRequired="true" text='<%# Resources.Language.Email + ":" %>' RequiredErrorMessage='<%# Resources.Language.EmailRequired %>'></cc:EntityFormTextBoxItem>
                                    <cc:EntityFormTextBoxItem runat="server" PropertyName="Username" TextBox-Width="400" FieldControlID="Username" IsRequired="true" text='<%# Resources.Language.Username + ":" %>' RequiredErrorMessage='<%# Resources.Language.UsernameRequired %>'></cc:EntityFormTextBoxItem>
                                    <cc:EntityFormPasswordItem runat="server" PropertyName="Password" TextBox-Width="400" FieldControlID="Password" IsRequired='<%# OperationManager.CurrentOperation == "Register" %>' text='<%# Resources.Language.Password + ":" %>' RequiredErrorMessage='<%# Resources.Language.PasswordRequired %>'></cc:EntityFormPasswordItem>
                                     <cc:EntityFormPasswordConfirmItem runat="server" PropertyName="Password" AutoBind="false" TextBox-Width="400" FieldControlID="PasswordConfirm" IsRequired='<%# OperationManager.CurrentOperation == "Register" %>' text='<%# Resources.Language.PasswordConfirm + ":" %>' CompareTo="Password" CompareToErrorMessage='<%# Resources.Language.PasswordsDontMatch %>'></cc:EntityFormPasswordConfirmItem>
                                      <cc:EntityFormCheckBoxItem runat="server" PropertyName="EnableNotifications" Text='<%# Resources.Language.EnableNotifications + ":" %>' FieldControlID="EnableNotifications" TextBox-Text='<%# Resources.Language.EnableNotifications %>'></cc:EntityFormCheckBoxItem>
                                                        <cc:EntityFormButtonsItem ID="EntityFormButtonsItem1" runat="server"><FieldTemplate><asp:Button ID="RegisterButton" runat="server" CausesValidation="True" CommandName="Save"
                                                    Text='<%# Resources.Language.Register %>'></asp:Button>
                                                </FieldTemplate></cc:EntityFormButtonsItem>
						</cc:EntityForm> 
    </asp:View>
    <asp:View runat="server" id="PendingView">
    	<h1>
                                <%= Resources.Language.PendingApproval %>
        </h1>
        <cc:Result runat="server" visible="false"/>
        <p><%= Resources.Language.RegistrationPendingApprovalMessage %></p>
    </asp:View>
</asp:MultiView>