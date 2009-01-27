<%@ Page Language="C#" MasterPageFile="../Site.master" AutoEventWireup="true"
     Title="Manage Users" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<script runat="server">
    #region Main functions
    /// <summary>
    /// Displays the index for managing users.
    /// </summary>
    private void ManageUsers()
    {
        OperationManager.StartOperation("ManageUsers", IndexView);

        IndexSource.DataBind();

        IndexView.DataBind();
    }

    /// <summary>
    /// Displays the form for creating a user.
    /// </summary>
    private void CreateUser()
    {
        OperationManager.StartOperation("CreateUser", FormView);

        User user = new User();
        user.ID = Guid.NewGuid();
        DataForm.DataSource = user;

        FormView.DataBind();
    }

    /// <summary>
    /// Saves the newly created user.
    /// </summary>
    private void SaveUser()
    {
        // Save the new user
        DataForm.ReverseBind();
        if (UserFactory.SaveUser((User)DataForm.DataSource))
        {
            // Display the result to the user
            Result.Display(Resources.Language.UserSaved);

            // Show the index again
            ManageUsers();
        }
        else
            Result.DisplayError(Resources.Language.UsernameTaken);
    }

    private void EditUser(Guid userID)
    {
        // Start the operation
        OperationManager.StartOperation("EditUser", FormView);

        // Load the specified user
        DataForm.DataSource = UserFactory.GetUser(userID);

        // Bind the form
        FormView.DataBind();
    }

    private void UpdateUser()
    {
        // Get a fresh copy of the user object
        User user = UserFactory.GetUser(((User)DataForm.DataSource).ID);

        // Transfer data from the form to the object
        DataForm.ReverseBind(user);
        
        // Update the user
        if (UserFactory.UpdateUser(user))
        {
            // Display the result to the user
            Result.Display(Resources.Language.UserUpdated);

            // Show the index again
            ManageUsers();
        }
        else
        {
            Result.DisplayError(Resources.Language.UsernameTaken);
        }
    }

    /// <summary>
    /// Deletes the user with the provided ID.
    /// </summary>
    /// <param name="userID">The ID of the user to delete.</param>
    private void DeleteUser(Guid userID)
    {
        // Delete the specified user
        UserFactory.DeleteUser(UserFactory.GetUser(userID));

        // Display the result to the user
        Result.Display(Resources.Language.UserDeleted);

        // Go back to the index
        ManageUsers();
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
                case "ManageUsers":
                default:
                    ManageUsers();
                    break;
            }
        }
    }

    protected void CreateButton_Click(object sender, EventArgs e)
    {
        // Create a new user
        CreateUser();
    }

    // todo: remove
    /*protected void UserSource_Inserted(object sender, ObjectDataSourceStatusEventArgs e)
    {

    }

    protected void UserSource_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
        // Display the result to the user
        Result.Display("The user was updated successfully.");

        // Show the index again
        ManageUsers();
    }*/

    //  TODO: Remove
    /*protected void IndexGrid_RowEditing(object sender, GridViewEditEventArgs e)
    {
        // Begin editing the selected object
        EditUser(new Guid(IndexGrid.DataKeys[e.NewEditIndex].ToString()));
        e.NewEditIndex = -1;
    }*/
    // TODO: remove
   /* protected void UserSource_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        // Make sure the object has the correct ID
        if (((User)e.InputParameters[0]).ID == Guid.Empty)
            ((User)e.InputParameters[0]).ID = new Guid(UserSource.SelectParameters[0].DefaultValue);
    }*/
    /*protected void IndexGrid_RowDeleted(object sender, GridViewDeleteEventArgs e)
    {
        // Display the result
        Result.Display("The selected user was deleted.");

        // Go back to the index
        ManageUsers();
    }*/
   /* protected void UserSource_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        // Generate a new ID for any new objects
        User user = (User)e.InputParameters[0];
        if (user.ID == Guid.Empty)
            user.ID = Guid.NewGuid();
    }*/

    private void DataForm_EntityCommand(object sender, EntityFormEventArgs e)
    {
        if (e.CommandName == "Save")
        {
            SaveUser();
        }
        else if (e.CommandName == "Update")
        {
            UpdateUser();
        }
        else
            ManageUsers();
    }

    protected void IndexGrid_ItemCommand(object source, DataGridCommandEventArgs e)
    {
        if (e.CommandName == "Edit")
        {
            EditUser(new Guid(IndexGrid.DataKeys[e.Item.ItemIndex].ToString()));
        }
        else if (e.CommandName == "Delete")
        {
            DeleteUser(new Guid(IndexGrid.DataKeys[e.Item.ItemIndex].ToString()));
        }
    }
    #endregion
</script>
<asp:Content ID="Body" ContentPlaceHolderID="Body" runat="Server">
    <asp:ObjectDataSource ID="IndexSource" runat="server" DataObjectTypeName="SoftwareMonkeys.SiteStarter.Entities.User"
        InsertMethod="SaveUser" OldValuesParameterFormatString="original_{0}" SelectMethod="GetUsers"
        TypeName="SoftwareMonkeys.SiteStarter.Business.UserFactory" DeleteMethod="DeleteUser"
        UpdateMethod="UpdateUser"></asp:ObjectDataSource>
    <asp:MultiView ID="PageView" runat="server">
        <asp:View ID="IndexView" runat="server">
            <table class="OuterPanel">
                <tr>
                    <td class="Heading1">
                        <%# Resources.Language.ManageUsers %></td>
                </tr>
                <tr>
                    <td>
                        <cc:Result runat="server" ID="IndexResult">
                        </cc:Result>
                        <p>
                            <%# Resources.Language.ManageUsersIntro %></p>
                        <p>
                            <asp:Button ID="CreateButton" runat="server" OnClick="CreateButton_Click" Text='<%# Resources.Language.CreateUser %>'
                                CommandName="New" />&nbsp;</p>
                        <p>
                            <cc:IndexGrid ID="IndexGrid" runat="server" AllowPaging="True" HeaderStyle-CssClass="Heading2" AllowSorting="True"
                                AutoGenerateColumns="False" DataSourceID="IndexSource" EmptyDataText='<%# Resources.Language.NoUsersFound %>'
                                Width="100%"
                                PageSize="2" OnItemCommand="IndexGrid_ItemCommand" DataKeyField="ID">
                                <Columns>
                                    <asp:BoundColumn DataField="Username" HeaderText="Username" SortExpression="Username" />
                                    <asp:BoundColumn DataField="Email" HeaderText="Email" SortExpression="Email" />
                                    <asp:BoundColumn DataField="IsLockedOut" HeaderText="IsLockedOut" SortExpression="IsLockedOut" />
                                    <asp:BoundColumn DataField="IsApproved" HeaderText="IsApproved" SortExpression="IsApproved" />
                                    <asp:BoundColumn DataField="CreationDate" HeaderText="CreationDate" SortExpression="CreationDate" />
                                    <asp:TemplateColumn>
                                        <itemtemplate>
<asp:LinkButton ID="LinkButton1" runat="server" text='<%# Resources.Language.Edit %>' ToolTip='<%# Resources.Language.EditUserToolTip %>' CommandName="Edit"></asp:LinkButton><asp:LinkButton ID="LinkButton2" ToolTip='<%# Resources.Language.DeleteUserToolTip %>' runat="server" text='<%# Resources.Language.Delete %>' CommandName="Delete"></asp:LinkButton>
                                      
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
                        <%# OperationManager.CurrentOperation == "CreateUser" ? Resources.Language.CreateUser : Resources.Language.EditUser %>
                    </td>
                </tr>
                <tr>
                    <td>
                        <p>
                          <cc:Result runat="server"></cc:Result>
                             <%# OperationManager.CurrentOperation == "CreateUser" ? Resources.Language.CreateUserIntro : Resources.Language.EditUserIntro %>
                         </p>
                           <cc:EntityForm runat="server" id="DataForm" OnEntityCommand="DataForm_EntityCommand" CssClass="Panel" headingtext='<%# OperationManager.CurrentOperation == "CreateUser" ? Resources.Language.NewUserDetails : Resources.Language.UserDetails %>' headingcssclass="Heading2" width="100%">
<cc:EntityFormTextBoxItem runat="server" PropertyName="FirstName" TextBox-Width="400" FieldControlID="FirstName" IsRequired="true" text='<%# Resources.Language.FirstName + ":" %>' RequiredErrorMessage='<%# Resources.Language.FirstNameRequired %>'></cc:EntityFormTextBoxItem>
<cc:EntityFormTextBoxItem runat="server" PropertyName="LastName" TextBox-Width="400" FieldControlID="LastName" IsRequired="true" text='<%# Resources.Language.LastName + ":" %>' RequiredErrorMessage='<%# Resources.Language.LastNameRequired %>'></cc:EntityFormTextBoxItem>
   <cc:EntityFormTextBoxItem runat="server" PropertyName="Email" TextBox-Width="400" FieldControlID="Email" IsRequired="true" text='<%# Resources.Language.Email + ":" %>' RequiredErrorMessage='<%# Resources.Language.EmailRequired %>'></cc:EntityFormTextBoxItem>
                                    <cc:EntityFormTextBoxItem runat="server" PropertyName="Username" TextBox-Width="400" FieldControlID="Username" IsRequired="true" text='<%# Resources.Language.Username + ":" %>' RequiredErrorMessage='<%# Resources.Language.UsernameRequired %>'></cc:EntityFormTextBoxItem>
                                    <cc:EntityFormPasswordItem runat="server" PropertyName="Password" TextBox-Width="400" FieldControlID="Password" IsRequired='<%# OperationManager.CurrentOperation == "CreateUser" %>' text='<%# Resources.Language.Password + ":" %>' RequiredErrorMessage='<%# Resources.Language.PasswordRequired %>'></cc:EntityFormPasswordItem>
                                     <cc:EntityFormPasswordConfirmItem runat="server" PropertyName="PasswordConfirm" TextBox-Width="400" FieldControlID="PasswordConfirm" IsRequired='<%# OperationManager.CurrentOperation == "CreateUser" %>' text='<%# Resources.Language.PasswordConfirm + ":" %>' CompareTo="Password" CompareToErrorMessage='<%# Resources.Language.PasswordsDontMatch %>'></cc:EntityFormPasswordConfirmItem>
                                     <cc:EntityFormCheckBoxItem runat="server" PropertyName="IsApproved" Text='<%# Resources.Language.IsApproved + ":" %>' FieldControlID="IsApproved" TextBox-Text='<%# Resources.Language.IsApprovedNote %>'></cc:EntityFormCheckBoxItem>
                                      <cc:EntityFormCheckBoxItem runat="server" PropertyName="IsLockedOut" Text='<%# Resources.Language.IsLockedOut + ":" %>' FieldControlID="IsLockedOut" TextBox-Text='<%# Resources.Language.IsLockedOutNote %>'></cc:EntityFormCheckBoxItem>
                                   <cc:EntityFormButtonsItem ID="EntityFormButtonsItem1" runat="server"><FieldTemplate><asp:Button ID="SaveButton" runat="server" CausesValidation="True" CommandName="Save"
                                                    Text='<%# Resources.Language.Save %>' Visible='<%# OperationManager.CurrentOperation == "CreateUser" %>'></asp:Button>
                                                    <asp:Button ID="UpdateButton" runat="server" CausesValidation="True" CommandName="Update"
                                                    Text='<%# Resources.Language.Update %>' Visible='<%# OperationManager.CurrentOperation == "EditUser" %>'></asp:Button>
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
