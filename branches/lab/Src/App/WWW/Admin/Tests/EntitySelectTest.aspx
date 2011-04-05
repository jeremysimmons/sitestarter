<%@ Page Language="C#" MasterPageFile="~/Site.master" Title="Test Entity Select" trace="true" %>
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
	
		MultipleSelect.DataBind();
		MultipleEmptySelect.DataBind();
		SingleSelect.DataBind();
		SingleEmptySelect.DataBind();
		
		SetDefaultSelections();
	}
}

	private void SetDefaultSelections()
	{
		User[] users = CreateMockUsers();
	
		MultipleSelect.SelectedEntities = new User[] {users[2], users[4]};
		SingleSelect.SelectedEntities = new User[] {users[0]};
	}

	private Guid[] GetUserIDs()
	{
		return new Guid[]{
			new Guid("58762847-9cd0-454f-80c3-2f96f44f6e2a"),
			new Guid("c4cec450-8286-4019-8a04-ad626285881d"),
			new Guid("caba4ebe-79da-4a6f-8ce9-666f361240da"),
			new Guid("14e48b84-94cb-4fdf-b3e6-94e46712aa2c"),
			new Guid("86941539-6920-40c5-8770-a23498fab800"),
			new Guid("272961ab-6e07-4df5-ad53-80759f62d41d"),
			new Guid("555ed112-aa55-4a5d-8f81-7bc124bd4c27"),
			new Guid("ebadc2bb-682e-4112-a9f5-99d8e789545f"),
			new Guid("52c9c4c5-6650-4e71-851f-f22585c6a12a"),
			new Guid("53b1befa-e84e-4b92-9063-2481fb79d9b1")

		};
	}

	private User[] CreateMockUsers()
	{
		int total = 10;
		Guid[] ids = GetUserIDs();

		List<User> users = new List<User>();

		//if (ViewState["Users"] != null)
		//	users.AddRange((User[])ViewState["Users"]);
		//else
		//{
			for (int i = 0; i < total; i++)
			{
				User user = new User();
				user.ID = ids[i];
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
	        ((EntitySelect)sender).DataSource = CreateMockUsers();
    }
    
    private void GoMultipleButton_Click(object sender, EventArgs e)
    {
	ShowSelected(MultipleSelect.SelectedEntities);
    }
    
    private void GoMultipleEmptyButton_Click(object sender, EventArgs e)
    {
	ShowSelected(MultipleEmptySelect.SelectedEntities);
    }
    
    private void GoSingleButton_Click(object sender, EventArgs e)
    {
	ShowSelected(SingleSelect.SelectedEntities);
    }

    private void GoSingleEmptyButton_Click(object sender, EventArgs e)
    {
	ShowSelected(SingleSelect.SelectedEntities);
    }

	private void ShowSelected(IEntity[] entities)
	{
		SelectedIndex.DataSource = entities;
		SelectedIndex.DataBind();
	}
</script>
<asp:Content ID="Content1" ContentPlaceHolderID="Body" Runat="Server">
<script language="javascript">

</script>
<h1>Multiple</h1>
<h2>Normal</h2>
<cc:EntitySelect runat="server" id="MultipleSelect" NoDataText="No data found." width="400px" SelectionMode="Multiple" EntityType="SoftwareMonkeys.SiteStarter.Entities.User" OnDataLoading="TestSelect_DataLoading" />
<asp:button runat="server" text="Go" onclick="GoMultipleButton_Click" />
<hr/>
<h2>Empty Multiple</h2>
<cc:EntitySelect runat="server" id="MultipleEmptySelect" NoDataText="No data found." width="400px" SelectionMode="Multiple" EntityType="SoftwareMonkeys.SiteStarter.Entities.User"/>
<asp:button runat="server" text="Go" onclick="GoMultipleEmptyButton_Click" />
<hr/>
<h1>Single</h1>
<cc:EntitySelect runat="server" id="SingleSelect" NoDataText="No data found." width="400px" SelectionMode="Single" EntityType="SoftwareMonkeys.SiteStarter.Entities.User" OnDataLoading="TestSelect_DataLoading" />
<asp:button runat="server" text="Go" onclick="GoSingleButton_Click" />
<hr/>
<h2>Empty Single</h2>
<cc:EntitySelect runat="server" id="SingleEmptySelect" NoDataText="No data found." width="400px" SelectionMode="Single" EntityType="SoftwareMonkeys.SiteStarter.Entities.User"/>
<asp:button runat="server" text="Go" onclick="GoSingleEmptyButton_Click" />
<ss:IndexGrid runat="server" id="SelectedIndex" HeaderText="Selected Entities">
	<Columns>
		<asp:BoundColumn DataField="Username"/>
		<asp:BoundColumn DataField="Email"/>
		<asp:BoundColumn DataField="FirstName"/>
		<asp:BoundColumn DataField="LastName"/>
		<asp:BoundColumn DataField="IsApproved"/>
	</Columns>
</ss:IndexGrid>
</asp:Content>