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
	MultipleSelect.DataBind();
	MultipleEmptySelect.DataBind();
	SingleSelect.DataBind();
	SingleEmptySelect.DataBind();
	}
}

	private User[] CreateMockUsers()
	{
		int total = 100;

		List<User> users = new List<User>();

		if (ViewState["Users"] != null)
			users.AddRange((User[])ViewState["Users"]);
		else
		{
			for (int i = 0; i < total; i++)
			{
				User user = new User();
				user.ID = Guid.NewGuid();
				user.FirstName = "First" + (i+1);
				user.LastName = "Last" + (i+1);
				user.Email = "Email" + (i+1);

				users.Add(user);
			}
		}
		ViewState["Users"] = users.ToArray();
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
	</Columns>
</ss:IndexGrid>
</asp:Content>