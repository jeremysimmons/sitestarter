<%@ Control Language="C#" ClassName="UserAccountProjection" Inherits="SoftwareMonkeys.SiteStarter.Web.Projections.BaseProjection" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business.Security" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Register TagPrefix="cc" Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" %>
<script runat="server">
    private void Page_Load(object sender, EventArgs e)
    {
        ViewAccount();
    }

    private void ViewAccount()
    {
    	Authorisation.EnsureIsAuthenticated();
    
        OperationManager.StartOperation("ViewAccount", DetailsView);

        DetailsForm.DataSource = AuthenticationState.User;
        
        DetailsView.DataBind();
    }

    private void EditAccount()
    {
    	Navigator.Go("Edit", AuthenticationState.User);
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
<asp:MultiView runat="server">
<asp:View runat="server" ID="DetailsView">
<h1><%= Resources.Language.MyDetails %></h1>
<cc:Result runat="server"></cc:Result>
<p><%= Resources.Language.MyDetailsIntro %></p>
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
</asp:MultiView>