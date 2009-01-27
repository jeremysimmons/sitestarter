<%@ Page Language="C#" MasterPageFile="~/Site.master" Title="Register" AutoEventWireup="true" %>
<%@ Register TagPrefix="cc" Assembly="SoftwareMonkeys.SiteStarter.Web" Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
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
        OperationManager.StartOperation("Register");

        User user = new User();
        user.ID = Guid.NewGuid();
        user.IsApproved = true; // TODO: Check if users should automatically be approved
        
        DataForm.DataSource = user;

        DataBind();
    }

    /// <summary>
    /// Submits the newly registered user to be saved.
    /// </summary>
    private void SubmitRegister()
    {
        DataForm.ReverseBind();
     
        // Encrypt the password
        ((User)DataForm.DataSource).Password = Crypter.EncryptPassword(((User)DataForm.DataSource).Password);
           
        if (UserFactory.SaveUser((User)DataForm.DataSource))
        {
            // Display the result to the user
            Result.Display(Resources.Language.RegistrationSuccessful);

            // Go to the account page
            FormsAuthentication.SetAuthCookie(((User)DataForm.DataSource).Username, false);
            Response.Redirect(Request.ApplicationPath + "/Members/Default.aspx");
        }
        else
            Result.DisplayError(Resources.Language.UsernameTaken);
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
<asp:Content ID="Content1" ContentPlaceHolderID="Body" Runat="Server">
<div class="Heading1"><%# Resources.Language.Register %></div>
<p>
<cc:Result runat="Server"></cc:Result>
<%# Resources.Language.RegisterIntro %></p>
    <cc:EntityForm runat="server" id="DataForm" OnEntityCommand="DataForm_EntityCommand" CssClass="Panel" headingtext='<%# OperationManager.CurrentOperation == "CreateUser" ? Resources.Language.NewUserDetails : Resources.Language.UserDetails %>' headingcssclass="Heading2" width="100%">
<cc:EntityFormTextBoxItem runat="server" PropertyName="FirstName" TextBox-Width="400" FieldControlID="FirstName" IsRequired="true" text='<%# Resources.Language.FirstName + ":" %>' RequiredErrorMessage='<%# Resources.Language.FirstNameRequired %>'></cc:EntityFormTextBoxItem>
<cc:EntityFormTextBoxItem runat="server" PropertyName="LastName" TextBox-Width="400" FieldControlID="LastName" IsRequired="true" text='<%# Resources.Language.LastName + ":" %>' RequiredErrorMessage='<%# Resources.Language.LastNameRequired %>'></cc:EntityFormTextBoxItem>
   <cc:EntityFormTextBoxItem runat="server" PropertyName="Email" TextBox-Width="400" FieldControlID="Email" IsRequired="true" text='<%# Resources.Language.Email + ":" %>' RequiredErrorMessage='<%# Resources.Language.EmailRequired %>'></cc:EntityFormTextBoxItem>
                                    <cc:EntityFormTextBoxItem runat="server" PropertyName="Username" TextBox-Width="400" FieldControlID="Username" IsRequired="true" text='<%# Resources.Language.Username + ":" %>' RequiredErrorMessage='<%# Resources.Language.UsernameRequired %>'></cc:EntityFormTextBoxItem>
                                    <cc:EntityFormPasswordItem runat="server" PropertyName="Password" TextBox-Width="400" FieldControlID="Password" IsRequired='<%# OperationManager.CurrentOperation == "CreateUser" %>' text='<%# Resources.Language.Password + ":" %>' RequiredErrorMessage='<%# Resources.Language.PasswordRequired %>'></cc:EntityFormPasswordItem>
                                     <cc:EntityFormPasswordConfirmItem runat="server" PropertyName="PasswordConfirm" TextBox-Width="400" FieldControlID="PasswordConfirm" IsRequired='<%# OperationManager.CurrentOperation == "CreateUser" %>' text='<%# Resources.Language.PasswordConfirm + ":" %>' CompareTo="Password" CompareToErrorMessage='<%# Resources.Language.PasswordsDontMatch %>'></cc:EntityFormPasswordConfirmItem>
                                     <cc:EntityFormButtonsItem runat="server"><FieldTemplate><asp:Button ID="RegisterButton" runat="server" CausesValidation="True" CommandName="Register"
                                                    Text='<%# Resources.Language.Register %>'></asp:Button>
                                                   </FieldTemplate></cc:EntityFormButtonsItem>
</cc:EntityForm> 
</asp:Content>
