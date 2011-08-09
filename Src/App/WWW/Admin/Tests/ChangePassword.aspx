<%@ Page Language="C#" autoeventwireup="true" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Business" %>
<script runat="server">

private void Page_Init(object sender, EventArgs e)
{
	User user = RetrieveStrategy.New<User>(false).Retrieve<User>("Email", Request.QueryString["Email"]);
	
	Response.Redirect(Request.ApplicationPath + "/Change-Password.aspx?u=" + Request.QueryString["Email"] + "&p=" + user.Password);
}

</script>