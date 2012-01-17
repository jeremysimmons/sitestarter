<%@ Page Language="C#" MasterPageFile="~/Site.master" Title="Test Entity Select" %>
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
<%@ Import namespace="System.Collections.Generic" %>
<script runat="server">

private void Page_Load(object sender, EventArgs e)
{
	if (!IsPostBack)
	{
		Guid[] ids = new Guid[]{};
		
		ids = GetIDsFromQueryString();
		
		User[] entities = DataAccess.Data.Indexer.GetEntities<User>(ids);
	
		SelectedIndex.DataSource = entities;
		SelectedIndex.DataBind();
	
		
		SingleEmptySelect.SelectedEntity = entities.Length > 0 ? entities[0] : null;
		SingleEmptySelect.DataBind();
	}
}

	private Guid[] GetIDsFromQueryString()
	{
		List<Guid> ids = new List<Guid>();
			
		if (Request.QueryString["UserIDs"] != null
			&& Request.QueryString["UserIDs"] != string.Empty)
		{
			foreach (string idString in Request.QueryString["UserIDs"].Split(','))
			{
				ids.Add(new Guid(idString));
			}
		}
			
		return ids.ToArray();
	}

	private User[] CreateMockUsers()
	{
		int total = 100;

		List<User> users = new List<User>();

		//if (ViewState["Users"] != null)
		//	users.AddRange((User[])ViewState["Users"]);
		//else
		//{
			for (int i = 0; i < total; i++)
			{
				User user = new User();
				user.ID = Guid.NewGuid();
				user.FirstName = "First" + (i+1);
				user.LastName = "Last" + (i+1);
				user.Email = "Email" + (i+1);
				user.Username = "Username" + (i+1);

				users.Add(user);
			}
		//}
		//ViewState["Users"] = users.ToArray();
		return users.ToArray();
	}


    protected void TestSelect_DataLoading(object sender, EventArgs e)
    {
	        ((EntitySelect)sender).DataSource = IndexStrategy.New<User>().Index<User>();
    }

    private void GoSingleEmptyButton_Click(object sender, EventArgs e)
    {
		ShowSelected(SingleEmptySelect.SelectedEntities);
    }

	private void ShowSelected(IEntity[] entities)
	{
		string idsString = String.Empty;
		
		foreach (IEntity entity in entities)
			idsString = idsString + "," + entity.ID.ToString();
			
		idsString = idsString.Trim(',');
	
		Response.Redirect("EntitySelectTest.aspx?UserIDs=" + idsString);
	
	}
	
	private void CreateUsersButton_Click(object sender, EventArgs e)
	{
		User[] mockUsers = CreateMockUsers();
		
		foreach (User user in mockUsers)
		{
			SaveStrategy.New(user).Save(user);
		}
		
		Response.Redirect("EntitySelectTest_SingleEmpty.aspx");
	}
</script>
<asp:Content ID="Content1" ContentPlaceHolderID="Body" Runat="Server">
<script language="javascript">

</script>
<h2>Empty Single</h2>
<cc:EntitySelect runat="server" id="SingleEmptySelect" NoDataText="No data found." width="400px" SelectionMode="Single" EntityType="SoftwareMonkeys.SiteStarter.Entities.User" IsReference="false"/>
<asp:button runat="server" text="Go" onclick="GoSingleEmptyButton_Click" />
<ss:IndexGrid runat="server" id="SelectedIndex" HeaderText="Selected Entities">
	<Columns>
		<asp:BoundColumn DataField="Username"/>
		<asp:BoundColumn DataField="Name"/>
	</Columns>
</ss:IndexGrid>
</asp:Content>