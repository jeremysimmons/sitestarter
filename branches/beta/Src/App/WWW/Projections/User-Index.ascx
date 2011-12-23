<%@ Control Language="C#" ClassName="UserIndex" Inherits="SoftwareMonkeys.SiteStarter.Web.Projections.BaseIndexProjection" autoeventwireup="true" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business.Security" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Properties" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Projections" %>
<%@ Import Namespace="System.Collections.Generic" %>
<script runat="server">

    private void Page_Init(object sender, EventArgs e)
    {        
    	Initialize(typeof(User), IndexGrid, true);
        
        IndexGrid.AddDualSortItem(Resources.Language.Email, "Email");
        IndexGrid.AddDualSortItem(Resources.Language.FirstName, "FirstName");
        IndexGrid.AddDualSortItem(Resources.Language.LastName, "LastName");
        IndexGrid.AddDualSortItem(Resources.Language.Username, "Username");
        
    }
    
    public override void InitializeInfo()
    {
      	MenuTitle = "Users";
      	MenuCategory = "Administration";
        ShowOnMenu = true;
    }

  private void CreateButton_Click(object sender, EventArgs e)
  {
  		Navigator.Go("Create", "User");
  }
  
  private void SettingsButton_Click(object sender, EventArgs e)
  {
  		Navigator.Go("UserSettings");
  }
                    
</script>
            <h1>
                        <%= Resources.Language.ManageUsers %>
                    </h1>
                        <cc:Result ID="Result1" runat="server">
                        </cc:Result>
                        <p>
                            <%= Resources.Language.ManageUsersIntro %>
                        </p>
                        <div id="ActionButtons">
                        	<asp:Button ID="CreateButton" runat="server" Text='<%# Resources.Language.CreateUser %>'
                                CssClass="Button" OnClick="CreateButton_Click"></asp:Button>
                                <asp:Button ID="SettingsButton" runat="server" Text='<%# Resources.Language.Settings %>'
                                CssClass="Button" OnClick="SettingsButton_Click"></asp:Button>
                                </div>
				<p>
                <cc:IndexGrid ID="IndexGrid" runat="server" AllowPaging="True" DefaultSort="UsernameAscending" HeaderStyle-CssClass="Heading2" AllowSorting="True"
                                AutoGenerateColumns="False"  HeaderText='<%# Resources.Language.Users %>' EmptyDataText='<%# Resources.Language.NoUsersFound %>'
                                Width="100%"
                                PageSize="20" DataKeyField="ID"
                                NavigateUrl='<%# Navigator.GetLink("Index", "User") %>'
                                >
                                <Columns>
                                    <asp:BoundColumn DataField="Username" HeaderText="Username" SortExpression="Username" />
                                    <asp:BoundColumn DataField="FirstName" HeaderText="FirstName" SortExpression="FirstName" />
                                    <asp:BoundColumn DataField="LastName" HeaderText="LastName" SortExpression="LastName" />
                                    <asp:TemplateColumn>
                                        <itemtemplate>	
                                        <%= Resources.Language.IsApproved %>: <%# Eval("IsApproved") %>
										</itemtemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn>
                                        <itemtemplate>	
                                        <%= Resources.Language.IsLockedOut %>: <%# Eval("IsLockedOut") %>
										</itemtemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn>
                                        <itemtemplate>	
                                        <%= Resources.Language.Roles %>: <%# Eval("TotalRoles") %>
										</itemtemplate>
                                    </asp:TemplateColumn>
                                    <asp:BoundColumn DataField="CreationDate" HeaderText="CreationDate" SortExpression="CreationDate" />
                                    <asp:TemplateColumn>
                                        <itemtemplate>	
                                        <ASP:Hyperlink runat="server"
                                        	enabled='<%# Eval("Email") != null && Eval("Email") != String.Empty %>'
                                        	ToolTip='<%# Resources.Language.SendMessage %>'
                                        	text='<%# Resources.Language.Message %>'
                                        	navigateurl='<%# Navigator.GetLink("Send", "Message") + "?RecipientID=" + Eval("ID") %>'>
										</ASP:Hyperlink>
										</itemtemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn>
                                        <itemtemplate>	
                                        <ASP:Hyperlink id=EditButton runat="server"
                                        	enabled='<%# AuthoriseUpdateStrategy.New<User>().IsAuthorised((User)Container.DataItem) %>'
                                        	ToolTip='<%# DynamicLanguage.GetEntityText("EditThisEntity", QueryStrings.Type) %>'
                                        	text='<%# Resources.Language.Edit %>'
                                        	navigateurl='<%# Navigator.GetLink("Edit", (IEntity)Container.DataItem) %>'>
										</ASP:Hyperlink>&nbsp;
										<cc:DeleteLink id=DeleteButton runat="server"
											text='<%# Resources.Language.Delete %>'
											ConfirmMessage='<%# DynamicLanguage.GetEntityText("ConfirmDeleteEntity", QueryStrings.Type) %>'
											enabled='<%# AuthoriseDeleteStrategy.New<User>().IsAuthorised((User)Container.DataItem) %>'
											ToolTip='<%# DynamicLanguage.GetEntityText("DeleteThisEntity", QueryStrings.Type) %>'
											navigateurl='<%# Navigator.GetLink("Delete", (IEntity)Container.DataItem) %>'>
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