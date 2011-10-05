<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" %>
<script runat="server">
private void Page_Load(object sender, EventArgs e)
{
	Response.Redirect("Home.aspx");
}
</script>
<asp:Content runat="server" ContentPlaceHolderID="Body">
Redirecting...	
</asp:Content>