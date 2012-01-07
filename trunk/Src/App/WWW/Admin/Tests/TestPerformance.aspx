<%@ Page Language="C#" %>
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
			UserRole role = CreateMockRole(1);
			CreateMockUsers(role);
		}
    }

	private void CreateMockUsers(UserRole role)
	{
			CreateMockUsers(200, role);
	}

	private void CreateMockUsers(int total, UserRole role)
	{
		for (int i = 0; i < total; i++)
		{	
			CreateMockUser(i, role);
		}
	}


	private void CreateMockUser(int position, UserRole role)
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
		user.Roles = new UserRole[]{role};
		
		SaveStrategy.New(user, false).Save(user);
	}
	
	private UserRole CreateMockRole(int position)
	{
		int number = position+1;

		UserRole role = CreateStrategy.New<UserRole>(false).Create<UserRole>();
		role.ID = Guid.NewGuid();
		role.Name = "TestRole" + number;
		
		SaveStrategy.New(role, false).Save(role);
		
		return role;
	}
</script>
<html>
<body>
Done
</body>
</html>
