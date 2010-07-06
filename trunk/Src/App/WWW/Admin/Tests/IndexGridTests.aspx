<%@ Page
	Language           = "C#"
	AutoEventWireup    = "true"
	ValidateRequest    = "false"
	EnableSessionState = "false"
%>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web" %>
<script runat="server">
	public void Page_Load(object sender, EventArgs e)
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Loading the IndexGridTests page.", NLog.LogLevel.Debug))
		{
			if (!IsPostBack)
			{
				AppLogger.Debug("!IsPostBack");
				
				IndexGrid1.DataSource = UserFactory.Current.GetUsers();
				
				//UserFactory.Current.Activate((User[])EntityTree1.DataSource, "Roles");
				
				AppLogger.Debug("DataBinding");
				DataBind();
			}
			else
				AppLogger.Debug("IsPostBack");
		}
	}

</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>IndexGridTests</title>

		<meta http-equiv="content-type" content="text/html; charset=utf-8" />
		<meta http-equiv="CACHE-CONTROL" content="NO-CACHE" />
		<meta http-equiv="PRAGMA" content="NO-CACHE" />

		<link href="SoftwareMonkeys.SiteStarter.WWW.Admin.Tests.css" type="text/css" rel="stylesheet" />
		
	</head>
	<body>
		<form id="Form_EntityTreeTests" method="post" runat="server">

<div>
		Standard test:<br/>
			<cc:IndexGrid ID="IndexGrid1" runat="server" HeaderText='Users' AllowPaging="True" DefaultSort="UsernameAscending" HeaderStyle-CssClass="Heading2" AllowSorting="True"
                                AutoGenerateColumns="False" EmptyDataText='<%# Resources.Language.NoUsersFound %>'
                                Width="100%"
                                PageSize="20" DataKeyField="ID">
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
                            </cc:IndexGrid><br/>
                                  </div>

<div>
		No data test:<br/>
			<cc:IndexGrid ID="IndexGrid2" HeaderText='Users' runat="server" AllowPaging="True" DefaultSort="UsernameAscending" HeaderStyle-CssClass="Heading2" AllowSorting="True"
                                AutoGenerateColumns="False" EmptyDataText='<%# Resources.Language.NoUsersFound %>'
                                Width="100%"
                                PageSize="20" DataKeyField="ID">
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
                            </cc:IndexGrid><br/>
                                  </div>
	
                                  

		</form>
	</body>
</html>
