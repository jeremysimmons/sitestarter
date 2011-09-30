<%@ Page Language="C#" autoeventwireup="true" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<script runat="server">

private void Page_Init(object sender, EventArgs e)
{
	string url = String.Empty;
	using (LogGroup logGroup = LogGroup.StartDebug("Initializing the test change password page."))
	{
		string email = Request.QueryString["Email"];
		
		if (email == null || email == String.Empty)
			throw new Exception("No email specified by the 'Email' query string.");
	
		User user = RetrieveStrategy.New<User>(false).Retrieve<User>("Email", email);
		
		url = Request.ApplicationPath + "/ChangePassword.aspx?u=" + Request.QueryString["Email"] + "&p=" + user.Password;
	}
	
	Response.Redirect(url);
}

</script>