<%@ Page Language="C#" MasterPageFile="~/Site.master" Title="Untitled Page" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business.Security" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Register TagPrefix="cc" Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" %>
<script runat="server">
    private void Page_Load(object sender, EventArgs e)
    {
        ViewAccount();
    }

    private void ViewAccount()
    {
        OperationManager.StartOperation("ViewAccount", DetailsView);

        DetailsForm.DataSource = AuthenticationState.User;
        
        DetailsView.DataBind();
    }

    private void Register()
    {
        OperationManager.StartOperation("Register", FormView);

        User user = new User();
        user.ID = Guid.NewGuid();

        DataForm.DataSource = user;
        DataForm.DataBind();
        FormView.DataBind();
    }


    private void EditAccount()
    {
        OperationManager.StartOperation("EditAccount", FormView);

        DataForm.DataSource = RetrieveStrategy.New<User>().Retrieve<User>("Username", AuthenticationState.Username);
throw new Exception(DataForm.DataSource.ToString());
        //DataForm.DataBind();
        FormView.DataBind();
    }

    private void UpdateAccount()
    {
        if (IsValid)
        {
            // Get a fresh copy of the user object
            User user = RetrieveStrategy.New<User>().Retrieve<User>("Username", AuthenticationState.Username);

			string originalUsername = user.Username;
            string originalPassword = user.Password;
         
            // Reverse-bind the data
            DataForm.ReverseBind(user);

            if (user.Password == String.Empty)
                user.Password = originalPassword;
            else
                user.Password = Crypter.EncryptPassword(user.Password);

            // Update the user object
            if (UpdateStrategy.New<User>().Update(user))
            {
                // Display the result to the user
                Result.Display(Resources.Language.AccountUpdated);
                
	            // If the user changed their own username they need to sign in again
	            if (!originalUsername.Equals(user.Username))
	            	Response.Redirect("Logout.aspx");

                // Show the index again
                ViewAccount();
            }
            else
            {
                // Display the result to the user
                Result.Display(Resources.Language.UsernameTaken);

                // Show the index again
                EditAccount();
            }
        }
    }
   /* private void DeleteAccount(Guid userID)
    {
        // Delete the User
        UserFactory.DeleteUser(UserFactory.GetUser(userID));

        // Inform the user of the result
       // Result.Display(Language.UserDeleted); // todo: language pack
        Result.Display("The user was deleted successfully.");
        // View the account
        ViewAccount();
    }*/
    private void DataForm_EntityCommand(object sender, EntityFormEventArgs e)
    {
        if (e.CommandName == "Update")
        {
            UpdateAccount();
        }
        else
            ViewAccount();
    }

    private void DetailsForm_EntityCommand(object sender, EntityFormEventArgs e)
    {
        if (e.CommandName == "Edit")
        {
            EditAccount();
        }
        else
            ViewAccount();
    }
</script>
<asp:Content ID="Body" ContentPlaceHolderID="Body" Runat="Server">
<asp:MultiView runat="server">
<asp:View runat="server" ID="DetailsView">
<div class="Heading1"><%= Resources.Language.MyAccount %></div>
<p><cc:Result runat="server"></cc:Result>
<%= Resources.Language.MyAccountIntro %></p>
<cc:EntityForm runat="server" id="DetailsForm" CssClass="Panel" OnEntityCommand="DetailsForm_EntityCommand" headingtext='<%# Resources.Language.AccountDetails %>' headingcssclass="Heading2" width="100%">
<cc:EntityFormLabelItem runat="server" PropertyName="FirstName" FieldControlID="FirstNameLabel" text='<%# Resources.Language.FirstName + ":" %>'></cc:EntityFormLabelItem>
<cc:EntityFormLabelItem runat="server" PropertyName="LastName" FieldControlID="LastNameLabel" text='<%# Resources.Language.LastName + ":" %>'></cc:EntityFormLabelItem>
   <cc:EntityFormLabelItem runat="server" PropertyName="Email" FieldControlID="EmailLabel" text='<%# Resources.Language.Email + ":" %>'></cc:EntityFormLabelItem>
      <cc:EntityFormLabelItem runat="server" PropertyName="Username" FieldControlID="UsernameLabel" text='<%# Resources.Language.Username + ":" %>'></cc:EntityFormLabelItem>
<cc:EntityFormButtonsItem runat="server"><FieldTemplate><asp:Button ID="ViewEditButton" runat="server" CausesValidation="False" CommandName="Edit"
                                                    Text='<%# Resources.Language.Edit %>'>
                                                </asp:Button></FieldTemplate></cc:EntityFormButtonsItem>
</cc:EntityForm>
</asp:View>
<asp:View runat="server" ID="FormView">
<div class="Heading1"><%= Resources.Language.EditAccount  %></div>
<p><cc:Result runat="server"></cc:Result><%= Resources.Language.EditAccountIntro %></p>
<cc:EntityForm runat="server" id="DataForm" OnEntityCommand="DataForm_EntityCommand" CssClass="Panel" headingtext='<%# OperationManager.CurrentOperation == "Register" ? Resources.Language.Register : Resources.Language.AccountDetails %>' headingcssclass="Heading2" width="100%">
<cc:EntityFormTextBoxItem runat="server" PropertyName="FirstName" TextBox-Width="400" FieldControlID="FirstName" IsRequired="true" text='<%# Resources.Language.FirstName + ":" %>' RequiredErrorMessage='<%# Resources.Language.FirstNameRequired %>'></cc:EntityFormTextBoxItem>
<cc:EntityFormTextBoxItem runat="server" PropertyName="LastName" TextBox-Width="400" FieldControlID="LastName" IsRequired="true" text='<%# Resources.Language.LastName + ":" %>' RequiredErrorMessage='<%# Resources.Language.LastNameRequired %>'></cc:EntityFormTextBoxItem>
   <cc:EntityFormTextBoxItem runat="server" PropertyName="Email" TextBox-Width="400" FieldControlID="Email" IsRequired="true" text='<%# Resources.Language.Email + ":" %>' RequiredErrorMessage='<%# Resources.Language.EmailRequired %>'></cc:EntityFormTextBoxItem>
                                    <cc:EntityFormTextBoxItem runat="server" PropertyName="Username" TextBox-Width="400" FieldControlID="Username" IsRequired="true" text='<%# Resources.Language.Username + ":" %>' RequiredErrorMessage='<%# Resources.Language.UsernameRequired %>'></cc:EntityFormTextBoxItem>
                                    <cc:EntityFormPasswordItem runat="server" PropertyName="Password" TextBox-Width="400" FieldControlID="Password" IsRequired='<%# OperationManager.CurrentOperation == "Register" %>' text='<%# Resources.Language.Password + ":" %>' RequiredErrorMessage='<%# Resources.Language.PasswordRequired %>'></cc:EntityFormPasswordItem>
                                     <cc:EntityFormPasswordConfirmItem runat="server" PropertyName="PasswordConfirm" TextBox-Width="400" FieldControlID="PasswordConfirm" text='<%# Resources.Language.PasswordConfirm + ":" %>' CompareTo="Password" CompareToErrorMessage='<%# Resources.Language.PasswordsDontMatch %>'></cc:EntityFormPasswordConfirmItem>
                                   <cc:EntityFormButtonsItem runat="server"><FieldTemplate><asp:Button ID="SaveButton" runat="server" CausesValidation="True" CommandName="Save"
                                                    Text='<%# Resources.Language.Save %>' Visible='<%# OperationManager.CurrentOperation == "Register" %>'></asp:Button>
                                                    <asp:Button ID="UpdateButton" runat="server" CausesValidation="True" CommandName="Update"
                                                    Text='<%# Resources.Language.Update %>' Visible='<%# OperationManager.CurrentOperation == "EditAccount" %>'></asp:Button>
                                                <asp:Button ID="SaveCancelButton" runat="server" CausesValidation="False" CommandName="Cancel"
                                                    Text='<%# Resources.Language.Cancel %>'>
                                                </asp:Button></FieldTemplate></cc:EntityFormButtonsItem>
</cc:EntityForm> 
</asp:View>
</asp:MultiView>  
</asp:Content>

