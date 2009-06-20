<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" %>

<%@ Register TagPrefix="cc" Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<script runat="server">
private void Page_Load(object sender, EventArgs e)
{
	if (!IsPostBack)
		TestKeywordsControl.Keywords = new string[] { "test", "test2" };//, "test2", "test3", "test4", "test5", "teset6", "test7", "test8", "test9", "test10", "test11", "test12", "Test13", "test14" };
}

private void SubmitButton_Click(object sender, EventArgs e)
{
    Response.Write("sdf");
	foreach (string keyword in TestKeywordsControl.Keywords)
	{
		Response.Write(keyword + "<br/>");
	}
}
</script>
    <asp:Content runat="server" ContentPlaceHolderID="Body">
<cc:KeywordsControl runat="server" id="TestKeywordsControl"/>
<asp:Button runat="Server" id="SubmitButton" text="Submit" onclick="SubmitButton_Click" />
</asp:Content>