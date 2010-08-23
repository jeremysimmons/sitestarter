<%@ Page Language="C#" MasterPageFile="~/Site.master" Title="Backup" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="ss" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import Namespace="System.Reflection" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import namespace="System.IO" %>
<%@ Import namespace="System.Xml" %>
<%@ Import namespace="System.Xml.Serialization" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Data" %>
<script runat="server">

private void Page_Load(object sender, EventArgs e)
{
	using (Batch batch = BatchState.StartBatch())
	{
		CreateTestUsers(1000);
	}
	
	Response.Write("Done");
}

private void CreateTestUsers(int count)
{
	for (int i = 1; i <= count; i++)
	{
		CreateTestUser(i);
	}
}

private void CreateTestUser(int number)
{
	User user = new User();
	user.ID = Guid.NewGuid();
	user.FirstName = "Test #" + number.ToString();
	user.LastName = "User #" + number.ToString();
	user.Username = "TestUser" + number.ToString ();
	user.Password = Crypter.EncryptPassword("pass");
	user.Email = "test" + number.ToString() + "@softwaremonkeys.net";
	
	UserFactory.Current.SaveUser(user);

}
</script>
<asp:Content ID="Content1" ContentPlaceHolderID="Body" Runat="Server">

</asp:Content>
