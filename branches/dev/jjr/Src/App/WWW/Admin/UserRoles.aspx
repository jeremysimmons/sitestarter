<%@ Page Language="C#" MasterPageFile="../Site.master" AutoEventWireup="true"
     Title="Manage UserRoles" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<script runat="server">
    #region Main functions
    /// <summary>
    /// Displays the index for managing roles.
    /// </summary>
    private void ManageUserRoles()
    {
        OperationManager.StartOperation("ManageUserRoles", IndexView);

        
        IndexGrid.DataSource = UserRoleFactory.GetUserRoles();

        IndexView.DataBind();
    }

    /// <summary>
    /// Displays the form for creating a role.
    /// </summary>
    private void CreateUserRole()
    {
        OperationManager.StartOperation("CreateUserRole", FormView);

        UserRole role = new UserRole();
        role.ID = Guid.NewGuid();
        DataForm.DataSource = role;

        FormView.DataBind();
    }

    /// <summary>
    /// Saves the newly created role.
    /// </summary>
    private void SaveUserRole()
    {
        // Save the new role
        DataForm.ReverseBind();
        if (UserRoleFactory.SaveUserRole((UserRole)DataForm.DataSource))
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
        // Start the operation
        OperationManager.StartOperation("EditUserRole", FormView);

        // Load the specified role
        DataForm.DataSource = UserRoleFactory.GetUserRole(roleID);

        // Bind the form
        FormView.DataBind();
    }

    private void UpdateUserRole()
    {
        // Get a fresh copy of the role object
        UserRole role = UserRoleFactory.GetUserRole(((UserRole)DataForm.DataSource).ID);

        // Transfer data from the form to the object
        DataForm.ReverseBind(role);
        
        // Update the role
        if (UserRoleFactory.UpdateUserRole(role))
        {
            // Display the result to the role
            Result.Display(Resources.Language.UserRoleUpdated);

            // Show the index again
            ManageUserRoles();
        }
        else
        {
            Result.DisplayError(Resources.Language.UserRoleNameTaken);
        }
    }

    /// <summary>
    /// Deletes the role with the provided ID.
    /// </summary>
    /// <param name="roleID">The ID of the role to delete.</param>
    private void DeleteUserRole(Guid roleID)
    {
        // Delete the specified role
        UserRoleFactory.DeleteUserRole(UserRoleFactory.GetUserRole(roleID));

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

    // todo: remove
    /*protected void UserRoleSource_Inserted(object sender, ObjectDataSourceStatusEventArgs e)
    {

    }

    protected void UserRoleSource_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
        // Display the result to the role
        Result.Display("The role was updated successfully.");

        // Show the index again
        ManageUserRoles();
    }*/

    //  TODO: Remove
    /*protected void IndexGrid_RowEditing(object sender, GridViewEditEventArgs e)
    {
        // Begin editing the selected object
        EditUserRole(new Guid(IndexGrid.DataKeys[e.NewEditIndex].ToString()));
        e.NewEditIndex = -1;
    }*/
    // TODO: remove
   /* protected void UserRoleSource_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        // Make sure the object has the correct ID
        if (((UserRole)e.InputParameters[0]).ID == Guid.Empty)
            ((UserRole)e.InputParameters[0]).ID = new Guid(UserRoleSource.SelectParameters[0].DefaultValue);
    }*/
    /*protected void IndexGrid_RowDeleted(object sender, GridViewDeleteEventArgs e)
    {
        // Display the result
        Result.Display("The selected role was deleted.");

        // Go back to the index
        ManageUserRoles();
    }*/
   /* protected void UserRoleSource_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        // Generate a new ID for any new objects
        UserRole role = (UserRole)e.InputParameters[0];
        if (role.ID == Guid.Empty)
            role.ID = Guid.NewGuid();
    }*/

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
        ((EntitySelect)sender).DataSource = UserFactory.GetUsers();
    }
    #endregion
</script>
<asp:Content ID="Body" ContentPlaceHolderID="Body" runat="Server">
    <asp:MultiView ID="PageView" runat="server">
        <asp:View ID="IndexView" runat="server">
            <table class="OuterPanel">
                <tr>
                    <td class="Heading1">
                        <%# Resources.Language.ManageUserRoles %></td>
                </tr>
                <tr>
                    <td>
                        <cc:Result runat="server" ID="IndexResult">
                        </cc:Result>
                        <p>
                            <%# Resources.Language.ManageUserRolesIntro %></p>
                        <p>
                            <asp:Button ID="CreateButton" runat="server" OnClick="CreateButton_Click" Text='<%# Resources.Language.CreateUserRole %>'
                                CommandName="New" />&nbsp;</p>
                        <p>
                            <cc:IndexGrid ID="IndexGrid" runat="server" AllowPaging="True" HeaderStyle-CssClass="Heading2" AllowSorting="True"
                                AutoGenerateColumns="False" EmptyDataText='<%# Resources.Language.NoUserRolesFound %>'
                                Width="100%"
                                PageSize="2" OnItemCommand="IndexGrid_ItemCommand" DataKeyField="ID">
                                <Columns>
                                    <asp:BoundColumn DataField="Name" HeaderText="Name" SortExpression="Name" />
                                    <asp:TemplateColumn>
                                        <itemtemplate>
<asp:LinkButton ID="LinkButton1" runat="server" text='<%# Resources.Language.Edit %>' ToolTip='<%# Resources.Language.EditUserRoleToolTip %>' CommandName="Edit"></asp:LinkButton>
<asp:LinkButton ID="LinkButton2" ToolTip='<%# Resources.Language.DeleteUserRoleToolTip %>' runat="server" text='<%# Resources.Language.Delete %>' CommandName="Delete"></asp:LinkButton>
                                      
</itemtemplate>
                                    </asp:TemplateColumn>
                                </Columns>
                                <ItemStyle CssClass="ListItem" />
                                <PagerStyle HorizontalAlign="Right" Mode="NumericPages" Position="TopAndBottom" />
                                <HeaderStyle CssClass="Heading2" />
                                <AlternatingItemStyle CssClass="ListItem" />
                            </cc:IndexGrid>
                        </p>
                    </td>
                </tr>
            </table>
        </asp:View>
        <asp:View ID="FormView" runat="server">
            <table class="OuterPanel">
                <tr>
                    <td class="Heading1">
                        <%# OperationManager.CurrentOperation == "CreateUserRole" ? Resources.Language.CreateUserRole : Resources.Language.EditUserRole %>
                    </td>
                </tr>
                <tr>
                    <td>
                        <p>
                          <cc:Result runat="server"></cc:Result>
                             <%# OperationManager.CurrentOperation == "CreateUserRole" ? Resources.Language.CreateUserRoleIntro : Resources.Language.EditUserRoleIntro %>
                         </p>
                           <cc:EntityForm runat="server" id="DataForm" OnEntityCommand="DataForm_EntityCommand" CssClass="Panel" headingtext='<%# OperationManager.CurrentOperation == "CreateUserRole" ? Resources.Language.NewUserRoleDetails : Resources.Language.UserRoleDetails %>' headingcssclass="Heading2" width="100%">
                             <cc:EntityFormTextBoxItem runat="server" PropertyName="Name" TextBox-Width="400" FieldControlID="Name" IsRequired="true" text='<%# Resources.Language.Name + ":" %>' RequiredErrorMessage='<%# Resources.Language.UserRoleNameRequired %>'></cc:EntityFormTextBoxItem>
				<cc:EntityFormItem runat="server" PropertyName="UserIDs" FieldControlID="Users" ControlValuePropertyName="SelectedEntityIDs" text='<%# Resources.Language.Users + ":" %>'><FieldTemplate><cc:EntitySelect width="400px" EntityType="SoftwareMonkeys.SiteStarter.Entities.User, SoftwareMonkeys.SiteStarter.Entities" runat="server" ValuePropertyName='Name' id="Users" displaymode="multiple" selectionmode="multiple" NoDataText='<%# "-- " + Resources.Language.NoUsers + " --" %>' OnDataLoading='UsersSelect_DataLoading'></cc:EntitySelect></FieldTemplate></cc:EntityFormItem>
                                  <cc:EntityFormButtonsItem ID="EntityFormButtonsItem1" runat="server"><FieldTemplate><asp:Button ID="SaveButton" runat="server" CausesValidation="True" CommandName="Save"
                                                    Text='<%# Resources.Language.Save %>' Visible='<%# OperationManager.CurrentOperation == "CreateUserRole" %>'></asp:Button>
                                                    <asp:Button ID="UpdateButton" runat="server" CausesValidation="True" CommandName="Update"
                                                    Text='<%# Resources.Language.Update %>' Visible='<%# OperationManager.CurrentOperation == "EditUserRole" %>'></asp:Button>
                                                <asp:Button ID="SaveCancelButton" runat="server" CausesValidation="False" CommandName="Cancel"
                                                    Text='<%# Resources.Language.Cancel %>'>
                                                </asp:Button></FieldTemplate></cc:EntityFormButtonsItem>
</cc:EntityForm> 
                    </td>
                </tr>
            </table>
        </asp:View>
    </asp:MultiView>
</asp:Content>
