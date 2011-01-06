<%@ Control Language="C#" ClassName="UserRoleIndex" Inherits="SoftwareMonkeys.SiteStarter.Web.Projections.BaseIndexProjection" autoeventwireup="true" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Properties" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Projections" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business.Security" %>
<%@ Import Namespace="System.Collections.Generic" %>
<script runat="server">

    private void Page_Init(object sender, EventArgs e)
    {
    	Initialize(typeof(UserRole), IndexGrid, true);
        
        IndexGrid.AddDualSortItem(Resources.Language.Name, "Name");
    }

    #region Main functions

    #endregion
    
  private void CreateButton_Click(object sender, EventArgs e)
  {
  		Navigator.Go("Create");
  }
                    
</script>
            <h1>
                        <%= Resources.Language.ManageUserRoles %>
                    </h1>
                        <cc:Result ID="Result1" runat="server">
                        </cc:Result>
                        <p>
                            <%= Resources.Language.ManageUserRolesIntro %>
                        </p>
                        <div id="ActionButtons">
                        	<asp:Button ID="CreateButton" runat="server" Text='<%# Resources.Language.CreateUserRole %>'
                                CssClass="Button" OnClick="CreateButton_Click"></asp:Button>
                                </div>
						<p>
                <cc:IndexGrid ID="IndexGrid" runat="server" HeaderText='<%# Resources.Language.Roles %>' AllowPaging="True" HeaderStyle-CssClass="Heading2" AllowSorting="True"
                                AutoGenerateColumns="False" EmptyDataText='<%# Resources.Language.NoUserRolesFound %>'
                                Width="100%"
                                PageSize="20" DataKeyField="ID">
                                <Columns>
                                    <asp:BoundColumn DataField="Name" HeaderText="Name" SortExpression="Name" />
                                    <asp:TemplateColumn>
                                        <itemtemplate>
                                        <ASP:Hyperlink id=EditButton runat="server"
                                        	enabled='<%# AuthoriseUpdateStrategy.New<UserRole>().Authorise((UserRole)Container.DataItem) %>'
                                        	ToolTip='<%# DynamicLanguage.GetEntityText("EditThisEntity", QueryStrings.Type) %>'
                                        	text='<%# Resources.Language.Edit %>'
                                        	navigateurl='<%# Navigator.GetLink("Edit", "UserRole", Eval("UniqueKey").ToString()) %>'>
										</ASP:Hyperlink>&nbsp;
										<cc:DeleteLink id=DeleteButton runat="server"
											text='<%# Resources.Language.Delete %>'
											ConfirmMessage='<%# DynamicLanguage.GetEntityText("ConfirmDeleteEntity", QueryStrings.Type) %>'
											enabled='<%# AuthoriseDeleteStrategy.New<UserRole>().Authorise((UserRole)Container.DataItem) %>'
											ToolTip='<%# DynamicLanguage.GetEntityText("DeleteThisEntity", QueryStrings.Type) %>'
											navigateurl='<%# Navigator.GetLink("Delete", "UserRole", Eval("UniqueKey").ToString()) %>'>
										</cc:DeleteLink>
</itemtemplate>
                                    </asp:TemplateColumn>
                                </Columns>
                                <ItemStyle CssClass="ListItem" />
                                <PagerStyle HorizontalAlign="Right" Mode="NumericPages" Position="TopAndBottom" />
                                <HeaderStyle CssClass="Heading2" />
                                <AlternatingItemStyle CssClass="ListItem" />
                            </cc:IndexGrid>
                      </p>