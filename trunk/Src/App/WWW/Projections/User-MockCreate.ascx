<%@ Control Language="C#" ClassName="MockCreateUserProjection" Inherits="SoftwareMonkeys.SiteStarter.Web.Projections.BaseProjection" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Properties" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business.Security" %>
<%@ Import Namespace="System.Collections.Generic" %>
<script runat="server">
	public int TotalUsers = 1;

    private void Page_Load(object sender, EventArgs e)
    {
	CreateMockUsers();
    }

	private void CreateMockUsers()
	{
		if (Request.QueryString["Total"] != null)
			TotalUsers = Int32.Parse(Request.QueryString["Total"]);

		using (Batch batch = BatchState.StartBatch())
		{
			CreateMockUsers(TotalUsers);
		}
	}

	private void CreateMockUsers(int total)
	{
		for (int i = 0; i < total; i++)
		{	
			CreateMockUser(i);
		}
	}

	private void CreateMockUser(int position)
	{
		int number = position+1;

		User user = CreateStrategy.New<User>(false).Create<User>();
		user.ID = Guid.NewGuid();
		user.Username = "Username" + number;
		user.FirstName = "FirstName" + number;
		user.LastName = "LastName" + number;
		user.Email = "test" + position.ToString() + "@softwaremonkeys.net";
		user.Password = Crypter.EncryptPassword("pass");
		user.IsApproved = true;
		user.EnableNotifications = true;
		
		SaveStrategy.New(user, false).Save(user);
	}
	
	  public override void InitializeInfo()
	  {
	  	MenuTitle = "MockCreateUser";
	  	MenuCategory = "TestCategory";
	  	ShowOnMenu = false;
	  	ActionAlias = "Create";
	  }    
</script>
<h1>Creating Mock Users</h1>
<p><b>...done!</b></p>
<p><%= TotalUsers %> users created</p>
               