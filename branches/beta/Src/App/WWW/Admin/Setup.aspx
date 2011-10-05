<%@ Page language="c#" AutoEventWireup="true" MasterPageFile="~/Site.master" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Data" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Projections" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Parts" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Controllers" %>
<%@ Import namespace="System.Collections.Generic" %>
<%@ Import namespace="System.IO" %>
<script language="C#" runat="server">


private void Page_Load(object sender, EventArgs e)
{
	Response.Redirect("QuickSetup.aspx");
}

</script>
<asp:Content runat="server" ContentPlaceHolderID="Body">
		</asp:Content>
