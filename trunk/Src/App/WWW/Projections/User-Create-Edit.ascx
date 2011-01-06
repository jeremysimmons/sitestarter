<%@ Control Language="C#" ClassName="CreateEditProjection" Inherits="SoftwareMonkeys.SiteStarter.Web.Projections.BaseCreateEditProjection" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
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
        Initialize(typeof(User), DataForm, "Username");
        
    }

    #region Main functions
    

    /// <summary>
    /// Displays the form for creating a new user.
    /// </summary>
    public override void Create()
    {
        User user = new User();
        user.ID = Guid.NewGuid();
        
        DataForm.DataSource = user;
         
        WindowTitle = Resources.Language.CreateUser;
         
        Create(user);
    }
    
    /// <summary>
    /// Displays the form for creating a new user.
    /// </summary>
    public override void Edit()
    {
    	 User user = PrepareEdit<User>();
         
         WindowTitle = Resources.Language.EditUser + ": " + user.Name;
         
         Edit(user);
    }
    
    
    /// <summary>
    /// Saves the user from the form.
    /// </summary>
    public override bool Save()
    {
    	AutoNavigate = false;
    	
    	User user = PrepareSave<User>();
    	
    	user.Password = Crypter.EncryptPassword(user.Password);
    	
    	bool success = base.Save(user);
    	    	
    	if (success)
    		NavigateAfterSave();
    	
    	return success;
    }
    
    /// <summary>
    /// Updates the user from the form.
    /// </summary>
    public override bool Update()
    {
    	AutoNavigate = false;
    	
    	// Get the original user data
    	User originalUser = RetrieveStrategy.New<User>().Retrieve<User>("ID", DataForm.EntityID);
    	
    	User user = base.PrepareUpdate<User>();
    	
    	// If the password wasn't added then reset it
    	if (user.Password == null || user.Password == String.Empty)
    		user.Password = originalUser.Password;
    	else
        	user.Password = Crypter.EncryptPassword(user.Password);
    
    	bool success = base.Update(user);
    	
    	// If the current user edited their username then fix their authentication session
    	if (originalUser.Username == AuthenticationState.Username
    		&& user.Username != AuthenticationState.Username)
    		AuthenticationState.Username = user.Username;
    	    	
    	if (success)
    		NavigateAfterSave();
    	
    	return success;
    }
    
    
    public override void NavigateAfterOperation()
    {
    	Navigator.Go("Index", "User");
    }
    #endregion


    protected void UserRolesSelect_DataLoading(object sender, EventArgs e)
    {
        ((EntitySelect)sender).DataSource = IndexStrategy.New<UserRole>().Index();
    }
                    
</script>
                   <h1>
                                <%= OperationManager.CurrentOperation == "CreateUser" ? Resources.Language.CreateUser : Resources.Language.EditUser %>
                            </h1>
                                <cc:Result ID="Result2" runat="server">
                                </cc:Result>
                                <p class="Intro">
                                    <%= OperationManager.CurrentOperation == "CreateUser" ? Resources.Language.CreateUserIntro : Resources.Language.EditUserIntro %></p>  
                            <cc:EntityForm runat="server" DataSource='<%# DataSource %>' id="DataForm" CssClass="Panel" headingtext='<%# OperationManager.CurrentOperation == "CreateUser" ? Resources.Language.NewUserDetails : Resources.Language.UserDetails %>' headingcssclass="Heading2" width="100%">
<cc:EntityFormTextBoxItem runat="server" PropertyName="FirstName" TextBox-Width="400" FieldControlID="FirstName" IsRequired="true" text='<%# Resources.Language.FirstName + ":" %>' RequiredErrorMessage='<%# Resources.Language.FirstNameRequired %>'></cc:EntityFormTextBoxItem>
<cc:EntityFormTextBoxItem runat="server" PropertyName="LastName" TextBox-Width="400" FieldControlID="LastName" IsRequired="true" text='<%# Resources.Language.LastName + ":" %>' RequiredErrorMessage='<%# Resources.Language.LastNameRequired %>'></cc:EntityFormTextBoxItem>
   <cc:EntityFormTextBoxItem runat="server" PropertyName="Email" TextBox-Width="400" FieldControlID="Email" IsRequired="true" text='<%# Resources.Language.Email + ":" %>' RequiredErrorMessage='<%# Resources.Language.EmailRequired %>'></cc:EntityFormTextBoxItem>
                                    <cc:EntityFormTextBoxItem runat="server" PropertyName="Username" TextBox-Width="400" FieldControlID="Username" IsRequired="true" text='<%# Resources.Language.Username + ":" %>' RequiredErrorMessage='<%# Resources.Language.UsernameRequired %>'></cc:EntityFormTextBoxItem>
                                    <cc:EntityFormPasswordItem runat="server" PropertyName="Password" TextBox-Width="400" FieldControlID="Password" IsRequired='<%# OperationManager.CurrentOperation == "CreateUser" %>' text='<%# Resources.Language.Password + ":" %>' RequiredErrorMessage='<%# Resources.Language.PasswordRequired %>'></cc:EntityFormPasswordItem>
                                     <cc:EntityFormPasswordConfirmItem runat="server" PropertyName="Password" AutoBind="false" TextBox-Width="400" FieldControlID="PasswordConfirm" IsRequired='<%# OperationManager.CurrentOperation == "CreateUser" %>' text='<%# Resources.Language.PasswordConfirm + ":" %>' CompareTo="Password" CompareToErrorMessage='<%# Resources.Language.PasswordsDontMatch %>'></cc:EntityFormPasswordConfirmItem>
                                     <cc:EntityFormCheckBoxItem runat="server" PropertyName="IsApproved" Text='<%# Resources.Language.IsApproved + ":" %>' FieldControlID="IsApproved" TextBox-Text='<%# Resources.Language.IsApprovedNote %>'></cc:EntityFormCheckBoxItem>
                                      <cc:EntityFormCheckBoxItem runat="server" PropertyName="IsLockedOut" Text='<%# Resources.Language.IsLockedOut + ":" %>' FieldControlID="IsLockedOut" TextBox-Text='<%# Resources.Language.IsLockedOutNote %>'></cc:EntityFormCheckBoxItem>
                                      <cc:EntityFormCheckBoxItem runat="server" PropertyName="EnableNotifications" Text='<%# Resources.Language.EnableNotifications + ":" %>' FieldControlID="EnableNotifications" TextBox-Text='<%# Resources.Language.EnableNotifications %>'></cc:EntityFormCheckBoxItem>
                                      <cc:EntityFormItem runat="server" PropertyName="Roles" FieldControlID="UserRoles" ControlValuePropertyName="SelectedEntities"
                              text='<%# Resources.Language.Roles + ":" %>'>
                              <FieldTemplate>
                                  <cc:EntitySelect width="400" EntityType="SoftwareMonkeys.SiteStarter.Entities.UserRole, SoftwareMonkeys.SiteStarter.Entities" runat="server"
                                      ValuePropertyName='Name' id="UserRoles" DisplayMode="Multiple" SelectionMode="Multiple"
                                      NoDataText='<%# "-- " + Resources.Language.NoRoles + " --" %>' OnDataLoading='UserRolesSelect_DataLoading'>
                                  </cc:EntitySelect>
                                    </FieldTemplate>
                                    </cc:EntityFormItem>
                                   <cc:EntityFormButtonsItem ID="EntityFormButtonsItem1" runat="server"><FieldTemplate><asp:Button ID="SaveButton" runat="server" CausesValidation="True" CommandName="Save"
                                                    Text='<%# Resources.Language.Save %>' Visible='<%# OperationManager.CurrentOperation == "CreateUser" %>'></asp:Button>
                                                    <asp:Button ID="UpdateButton" runat="server" CausesValidation="True" CommandName="Update"
                                                    Text='<%# Resources.Language.Update %>' Visible='<%# OperationManager.CurrentOperation == "EditUser" %>'></asp:Button>
                                                </FieldTemplate></cc:EntityFormButtonsItem>
						</cc:EntityForm> 
               