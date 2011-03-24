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
<%@ Import Namespace="System.Collections.Generic" %>
<script runat="server">
    private void Page_Init(object sender, EventArgs e)
    {
        Initialize(typeof(UserRole), DataForm);        
        
        CreateController.ActionOnSuccess = "Index";
        EditController.ActionOnSuccess = "Index";
    }

    protected void UsersSelect_DataLoading(object sender, EventArgs e)
    {
        ((EntitySelect)sender).DataSource = IndexStrategy.New<User>().Index();
    }
                    
</script>
                   <h1>
                                <%= OperationManager.CurrentOperation == "CreateUserRole" ? Resources.Language.CreateUserRole : Resources.Language.EditUserRole %>
                            </h1>
                                <cc:Result ID="Result2" runat="server">
                                </cc:Result>
                                <p>
                                   <%= OperationManager.CurrentOperation == "CreateUserRole" ? Resources.Language.CreateUserRoleIntro : Resources.Language.EditUserRoleIntro %></p>  
                            <cc:EntityForm runat="server" id="DataForm" DataSource='<%# DataSource %>' CssClass="Panel" headingtext='<%# OperationManager.CurrentOperation == "CreateUserRole" ? Resources.Language.NewUserRoleDetails : Resources.Language.UserRoleDetails %>' headingcssclass="Heading2" width="100%">
                             <cc:EntityFormTextBoxItem runat="server" PropertyName="Name" TextBox-Width="400" TextBox-Enabled='<%# DataForm.DataSource == null || ((UserRole)DataForm.DataSource).Name != Resources.Language.Administrator %>' FieldControlID="Name" IsRequired="true" text='<%# Resources.Language.Name + ":" %>' RequiredErrorMessage='<%# Resources.Language.UserRoleNameRequired %>'></cc:EntityFormTextBoxItem>
				<cc:EntityFormItem runat="server" PropertyName="Users" FieldControlID="Users" ControlValuePropertyName="SelectedEntities" text='<%# Resources.Language.Users + ":" %>'><FieldTemplate><cc:EntitySelect width="400px" EntityType="SoftwareMonkeys.SiteStarter.Entities.User, SoftwareMonkeys.SiteStarter.Entities" runat="server" ValuePropertyName='Name' id="Users" displaymode="multiple" selectionmode="multiple" NoDataText='<%# "-- " + Resources.Language.NoUsers + " --" %>' OnDataLoading='UsersSelect_DataLoading'></cc:EntitySelect></FieldTemplate></cc:EntityFormItem>
                                  <cc:EntityFormButtonsItem ID="EntityFormButtonsItem1" runat="server"><FieldTemplate><asp:Button ID="SaveButton" runat="server" CausesValidation="True" CommandName="Save"
                                                    Text='<%# Resources.Language.Save %>' Visible='<%# OperationManager.CurrentOperation == "CreateUserRole" %>'></asp:Button>
                                                    <asp:Button ID="UpdateButton" runat="server" CausesValidation="True" CommandName="Update"
                                                    Text='<%# Resources.Language.Update %>' Visible='<%# OperationManager.CurrentOperation == "EditUserRole" %>'></asp:Button>
                                                </FieldTemplate></cc:EntityFormButtonsItem>
								</cc:EntityForm> 
               