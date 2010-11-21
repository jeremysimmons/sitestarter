<%@ Page Language="C#" MasterPageFile="../Site.master" AutoEventWireup="true"
     Title="Manage UserRoles" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<script runat="server">
    #region Main functions
    /// <summary>
    /// Displays the index for managing roles.
    /// </summary>
    private void ManageUserRoles()
    {
   		using (LogGroup logGroup = AppLogger.StartGroup("Preparing the index of user roles.", NLog.LogLevel.Debug))
   		{
	        Authorisation.EnsureIsAuthenticated();
	
	        Authorisation.EnsureIsInRole("Administrator");
	        
	        OperationManager.StartOperation("ManageUserRoles", IndexView);
	
	        UserRole[] roles = IndexStrategy.New<UserRole>().Index<UserRole>();
	        IndexGrid.DataSource = roles;
	
	        Authorisation.EnsureUserCan("View", roles);        
	        
	        IndexView.DataBind();
        }
    }

    /// <summary>
    /// Displays the form for creating a role.
    /// </summary>
    private void CreateUserRole()
    {
		using (LogGroup logGroup = AppLogger.StartGroup("Preparing the form to create a new user role.", NLog.LogLevel.Debug))
		{
	        Authorisation.EnsureIsAuthenticated();
	
	        Authorisation.EnsureIsInRole("Administrator");
	        
	        OperationManager.StartOperation("CreateUserRole", FormView);
	
	        UserRole role = new UserRole();
	        role.ID = Guid.NewGuid();
	        DataForm.DataSource = role;
	
	        Authorisation.EnsureUserCan("Create", role);        
	
	        FormView.DataBind();
        }
    }

    /// <summary>
    /// Saves the newly created role.
    /// </summary>
    private void SaveUserRole()
    {
        // Save the new role
        DataForm.ReverseBind();
        if (SaveStrategy.New<UserRole>().Save((UserRole)DataForm.DataSource))
        {
            // Display the result to the role
            Result.Display(Resources.Language.UserRoleSaved);

            // Show the index again
            ManageUserRoles();
        }
        else
            Result.DisplayError(Resources.Language.UserRoleNameTaken);
    }

    private void EditUserRole(Guid roleID)
    {
    	using (LogGroup logGroup = AppLogger.StartGroup("Editing the role with the provided ID.", NLog.LogLevel.Debug))
    	{
	        Authorisation.EnsureIsAuthenticated();
	
	        Authorisation.EnsureIsInRole("Administrator");
	        
	        // Start the operation
	        OperationManager.StartOperation("EditUserRole", FormView);
	
	        // Load the specified role
	        UserRole role = null;
	        DataForm.DataSource = role = RetrieveStrategy.New<UserRole>().Retrieve<UserRole>("ID", roleID);
	        ActivateStrategy.New<UserRole>().Activate((UserRole)DataForm.DataSource);
	
	        // Bind the form
	        FormView.DataBind();
        }
    }

    private void UpdateUserRole()
    {
    	using (LogGroup logGroup = AppLogger.StartGroup("Updating the user role on the form.", NLog.LogLevel.Debug))
    	{
	        // Get a fresh copy of the role object
	        UserRole role = (UserRole)RetrieveStrategy.New<UserRole>().Retrieve<UserRole>("ID", DataForm.EntityID);
	        
	        ActivateStrategy.New<UserRole>().Activate(role);
	
	        // Transfer data from the form to the object
	        DataForm.ReverseBind(role);
	        
	        // Update the role
	        if (UpdateStrategy.New<UserRole>().Update(role))
	        {
	        	AppLogger.Debug("Role name not in use. Updating.");
	        
	            // Display the result to the role
	            Result.Display(Resources.Language.UserRoleUpdated);
	
	            // Show the index again
	            ManageUserRoles();
	        }
	        else
	        {
	        	AppLogger.Debug("Role name in use. Showing error.");
	        	
	            Result.DisplayError(Resources.Language.UserRoleNameTaken);
	        }
        }
    }

    /// <summary>
    /// Deletes the role with the provided ID.
    /// </summary>
    /// <param name="roleID">The ID of the role to delete.</param>
    private void DeleteUserRole(Guid roleID)
    {
        Authorisation.EnsureIsAuthenticated();

        Authorisation.EnsureIsInRole("Administrator");

        UserRole role = RetrieveStrategy.New<UserRole>().Retrieve<UserRole>("ID", roleID);

        Authorisation.EnsureUserCan("Delete", role);        
        
        // Delete the specified role
        DeleteStrategy.New<UserRole>().Delete(role);

        // Display the result to the role
        Result.Display(Resources.Language.UserRoleDeleted);

        // Go back to the index
        ManageUserRoles();
    }
    #endregion

    #region Event handlers
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Perform the appropriate action based on the query string (parameter: "a")
            switch (QueryStrings.Action)
            {
                case "ManageUserRoles":
                default:
                    ManageUserRoles();
                    break;
            }
        }
    }

    protected void CreateButton_Click(object sender, EventArgs e)
    {
        // Create a new role
        CreateUserRole();
    }

    private void DataForm_EntityCommand(object sender, EntityFormEventArgs e)
    {
        if (e.CommandName == "Save")
        {
            SaveUserRole();
        }
        else if (e.CommandName == "Update")
        {
            UpdateUserRole();
        }
        else
            ManageUserRoles();
    }

    protected void IndexGrid_ItemCommand(object source, DataGridCommandEventArgs e)
    {
        if (e.CommandName == "Edit")
        {
            EditUserRole(new Guid(IndexGrid.DataKeys[e.Item.ItemIndex].ToString()));
        }
        else if (e.CommandName == "Delete")
        {
            DeleteUserRole(new Guid(IndexGrid.DataKeys[e.Item.ItemIndex].ToString()));
        }
    }

    protected void UsersSelect_DataLoading(object sender, EventArgs e)
    {
        ((EntitySelect)sender).DataSource = IndexStrategy.New<User>().Index<User>();
    }
    #endregion
</script>
<asp:Content ID="Body" ContentPlaceHolderID="Body" runat="Server">
    <asp:MultiView ID="PageView" runat="server">
        <asp:View ID="IndexView" runat="server">
            <h1>
                        <%# Resources.Language.ManageUserRoles %></td>
                </h1>
                        <cc:Result runat="server" ID="IndexResult">
                        </cc:Result>
                        <p>
                            <%# Resources.Language.ManageUserRolesIntro %></p>
                        <p>
                            <asp:Button ID="CreateButton" runat="server" OnClick="CreateButton_Click" Text='<%# Resources.Language.CreateUserRole %>'
                                CommandName="New" />&nbsp;</p>
                        <p>
                            <cc:IndexGrid ID="IndexGrid" runat="server" HeaderText='<%# Resources.Language.Roles %>' AllowPaging="True" HeaderStyle-CssClass="Heading2" AllowSorting="True"
                                AutoGenerateColumns="False" EmptyDataText='<%# Resources.Language.NoUserRolesFound %>'
                                Width="100%"
                                PageSize="20" OnItemCommand="IndexGrid_ItemCommand" DataKeyField="ID">
                                <Columns>
                                    <asp:BoundColumn DataField="Name" HeaderText="Name" SortExpression="Name" />
                                    <asp:TemplateColumn>
                                        <itemtemplate>
<asp:LinkButton ID="LinkButton1" runat="server" text='<%# Resources.Language.Edit %>' ToolTip='<%# Resources.Language.EditUserRoleToolTip %>' CommandName="Edit"></asp:LinkButton>
<asp:LinkButton ID="LinkButton2" ToolTip='<%# Resources.Language.DeleteUserRoleToolTip %>' runat="server" text='<%# Resources.Language.Delete %>' CommandName="Delete" onclientclick='<%# "return confirm(\"" + Resources.Language.ConfirmDeleteRole + "\");" %>'></asp:LinkButton>
                                      
</itemtemplate>
                                    </asp:TemplateColumn>
                                </Columns>
                                <ItemStyle CssClass="ListItem" />
                                <PagerStyle HorizontalAlign="Right" Mode="NumericPages" Position="TopAndBottom" />
                                <HeaderStyle CssClass="Heading2" />
                                <AlternatingItemStyle CssClass="ListItem" />
                            </cc:IndexGrid>
                        </p>
        </asp:View>
        <asp:View ID="FormView" runat="server">
            <h1>
                        <%# OperationManager.CurrentOperation == "CreateUserRole" ? Resources.Language.CreateUserRole : Resources.Language.EditUserRole %>
                    </h1>
                        <p>
                          <cc:Result runat="server"></cc:Result>
                             <%# OperationManager.CurrentOperation == "CreateUserRole" ? Resources.Language.CreateUserRoleIntro : Resources.Language.EditUserRoleIntro %>
                         </p>
                           <cc:EntityForm runat="server" id="DataForm" OnEntityCommand="DataForm_EntityCommand" CssClass="Panel" headingtext='<%# OperationManager.CurrentOperation == "CreateUserRole" ? Resources.Language.NewUserRoleDetails : Resources.Language.UserRoleDetails %>' headingcssclass="Heading2" width="100%">
                             <cc:EntityFormTextBoxItem runat="server" PropertyName="Name" TextBox-Width="400" TextBox-Enabled='<%# ((UserRole)DataForm.DataSource).Name != Resources.Language.Administrator %>' FieldControlID="Name" IsRequired="true" text='<%# Resources.Language.Name + ":" %>' RequiredErrorMessage='<%# Resources.Language.UserRoleNameRequired %>'></cc:EntityFormTextBoxItem>
				<cc:EntityFormItem runat="server" PropertyName="Users" FieldControlID="Users" ControlValuePropertyName="SelectedEntities" text='<%# Resources.Language.Users + ":" %>'><FieldTemplate><cc:EntitySelect width="400px" EntityType="SoftwareMonkeys.SiteStarter.Entities.User, SoftwareMonkeys.SiteStarter.Entities" runat="server" ValuePropertyName='Name' id="Users" displaymode="multiple" selectionmode="multiple" NoDataText='<%# "-- " + Resources.Language.NoUsers + " --" %>' OnDataLoading='UsersSelect_DataLoading'></cc:EntitySelect></FieldTemplate></cc:EntityFormItem>
                                  <cc:EntityFormButtonsItem ID="EntityFormButtonsItem1" runat="server"><FieldTemplate><asp:Button ID="SaveButton" runat="server" CausesValidation="True" CommandName="Save"
                                                    Text='<%# Resources.Language.Save %>' Visible='<%# OperationManager.CurrentOperation == "CreateUserRole" %>'></asp:Button>
                                                    <asp:Button ID="UpdateButton" runat="server" CausesValidation="True" CommandName="Update"
                                                    Text='<%# Resources.Language.Update %>' Visible='<%# OperationManager.CurrentOperation == "EditUserRole" %>'></asp:Button>
                                                </FieldTemplate></cc:EntityFormButtonsItem>
</cc:EntityForm> 
                    </p>
        </asp:View>
    </asp:MultiView>
</asp:Content>
