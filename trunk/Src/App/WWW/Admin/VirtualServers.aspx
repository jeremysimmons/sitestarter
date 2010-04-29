<%@ Page Language="C#" MasterPageFile="../Site.master" AutoEventWireup="true"
     Title="Manage VirtualServers" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<script runat="server">
    #region Main functions
    /// <summary>
    /// Displays the index for managing servers.
    /// </summary>
    private void ManageVirtualServers()
    {
        OperationManager.StartOperation("ManageVirtualServers", IndexView);

        
        IndexGrid.DataSource = VirtualServerFactory.Current.GetVirtualServers();

        IndexView.DataBind();
    }

    /// <summary>
    /// Displays the form for creating a server.
    /// </summary>
    private void CreateVirtualServer()
    {
        OperationManager.StartOperation("CreateVirtualServer", FormView);

        VirtualServer server = new VirtualServer();
        server.ID = Guid.NewGuid();
        DataForm.DataSource = server;

        FormView.DataBind();
    }

    /// <summary>
    /// Saves the newly created server.
    /// </summary>
    private void SaveVirtualServer()
    {
        // Save the new server
        DataForm.ReverseBind();
        if (VirtualServerFactory.Current.SaveVirtualServer((VirtualServer)DataForm.DataSource))
        {
            // Display the result to the server
            Result.Display(Resources.Language.VirtualServerSaved);

            // Show the index again
            ManageVirtualServers();
        }
        else
            Result.DisplayError(Resources.Language.VirtualServerNameTaken);
    }

    private void EditVirtualServer(Guid serverID)
    {
        // Start the operation
        OperationManager.StartOperation("EditVirtualServer", FormView);

        // Load the specified server
        DataForm.DataSource = VirtualServerFactory.Current.GetVirtualServer(serverID);

        // Bind the form
        FormView.DataBind();
    }

    private void UpdateVirtualServer()
    {
        // Get a fresh copy of the server object
        VirtualServer server = VirtualServerFactory.Current.GetVirtualServer(((VirtualServer)DataForm.DataSource).ID);

        // Transfer data from the form to the object
        DataForm.ReverseBind(server);
        
        // Update the server
        if (VirtualServerFactory.Current.UpdateVirtualServer(server))
        {
            // Display the result to the server
            Result.Display(Resources.Language.VirtualServerUpdated);

            // Show the index again
            ManageVirtualServers();
        }
        else
        {
            Result.DisplayError(Resources.Language.VirtualServerNameTaken);
        }
    }

    /// <summary>
    /// Deletes the server with the provided ID.
    /// </summary>
    /// <param name="serverID">The ID of the server to delete.</param>
    private void DeleteVirtualServer(Guid serverID)
    {
        // Delete the specified server
        VirtualServerFactory.Current.DeleteVirtualServer(VirtualServerFactory.Current.GetVirtualServer(serverID));

        // Display the result to the server
        Result.Display(Resources.Language.VirtualServerDeleted);

        // Go back to the index
        ManageVirtualServers();
    }
    #endregion

    #region Event handlers
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Perform the appropriate action based on the query string (parameter: "a")
            switch (QueryStrings.Action)
            {
                case "ManageVirtualServers":
                default:
                    ManageVirtualServers();
                    break;
            }
        }
    }

    protected void CreateButton_Click(object sender, EventArgs e)
    {
        // Create a new server
        CreateVirtualServer();
    }

    // todo: remove
    /*protected void VirtualServerSource_Inserted(object sender, ObjectDataSourceStatusEventArgs e)
    {

    }

    protected void VirtualServerSource_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
        // Display the result to the server
        Result.Display("The server was updated successfully.");

        // Show the index again
        ManageVirtualServers();
    }*/

    //  TODO: Remove
    /*protected void IndexGrid_RowEditing(object sender, GridViewEditEventArgs e)
    {
        // Begin editing the selected object
        EditVirtualServer(new Guid(IndexGrid.DataKeys[e.NewEditIndex].ToString()));
        e.NewEditIndex = -1;
    }*/
    // TODO: remove
   /* protected void VirtualServerSource_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        // Make sure the object has the correct ID
        if (((VirtualServer)e.InputParameters[0]).ID == Guid.Empty)
            ((VirtualServer)e.InputParameters[0]).ID = new Guid(VirtualServerSource.SelectParameters[0].DefaultValue);
    }*/
    /*protected void IndexGrid_RowDeleted(object sender, GridViewDeleteEventArgs e)
    {
        // Display the result
        Result.Display("The selected server was deleted.");

        // Go back to the index
        ManageVirtualServers();
    }*/
   /* protected void VirtualServerSource_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        // Generate a new ID for any new objects
        VirtualServer server = (VirtualServer)e.InputParameters[0];
        if (server.ID == Guid.Empty)
            server.ID = Guid.NewGuid();
    }*/

    private void DataForm_EntityCommand(object sender, EntityFormEventArgs e)
    {
        if (e.CommandName == "Save")
        {
            SaveVirtualServer();
        }
        else if (e.CommandName == "Update")
        {
            UpdateVirtualServer();
        }
        else
            ManageVirtualServers();
    }

    protected void IndexGrid_ItemCommand(object source, DataGridCommandEventArgs e)
    {
        if (e.CommandName == "Edit")
        {
            EditVirtualServer(new Guid(IndexGrid.DataKeys[e.Item.ItemIndex].ToString()));
        }
        else if (e.CommandName == "Delete")
        {
            DeleteVirtualServer(new Guid(IndexGrid.DataKeys[e.Item.ItemIndex].ToString()));
        }
    }

    protected void PrimaryAdministratorSelect_DataLoading(object sender, EventArgs e)
    {
        ((EntitySelect)sender).DataSource = UserFactory.Current.GetUsers();
    }
    #endregion
</script>
<asp:Content ID="Body" ContentPlaceHolderID="Body" runat="Server">
    <asp:MultiView ID="PageView" runat="server">
        <asp:View ID="IndexView" runat="server">
            <table class="OuterPanel">
                <tr>
                    <td class="Heading1">
                        <%# Resources.Language.ManageVirtualServers %></td>
                </tr>
                <tr>
                    <td>
                        <cc:Result runat="server" ID="IndexResult">
                        </cc:Result>
                        <p>
                            <%# Resources.Language.ManageVirtualServersIntro %></p>
                        <p>
                            <asp:Button ID="CreateButton" runat="server" OnClick="CreateButton_Click" Text='<%# Resources.Language.CreateVirtualServer %>'
                                CommandName="New" />&nbsp;</p>
                        <p>
                            <cc:IndexGrid ID="IndexGrid" runat="server" AllowPaging="True" HeaderStyle-CssClass="Heading2" AllowSorting="True"
                                AutoGenerateColumns="False" EmptyDataText='<%# Resources.Language.NoVirtualServersFound %>'
                                Width="100%"
                                PageSize="2" OnItemCommand="IndexGrid_ItemCommand" DataKeyField="ID">
                                <Columns>
                                    <asp:BoundColumn DataField="Name" HeaderText="Name" SortExpression="Name" />
                                    <asp:TemplateColumn>
                                        <itemtemplate>
<asp:LinkButton ID="LinkButton1" runat="server" text='<%# Resources.Language.Edit %>' ToolTip='<%# Resources.Language.EditVirtualServerToolTip %>' CommandName="Edit"></asp:LinkButton>
<asp:LinkButton ID="LinkButton2" ToolTip='<%# Resources.Language.DeleteVirtualServerToolTip %>' runat="server" text='<%# Resources.Language.Delete %>' CommandName="Delete"></asp:LinkButton>
                                      
</itemtemplate>
                                    </asp:TemplateColumn>
                                </Columns>
                                <ItemStyle CssClass="ListItem" />
                                <PagerStyle HorizontalAlign="Right" Mode="NumericPages" Position="TopAndBottom" />
                                <HeaderStyle CssClass="Heading2" />
                                <AlternatingItemStyle CssClass="ListItem" />
                            </cc:IndexGrid>
                        </p>
                    </td>
                </tr>
            </table>
        </asp:View>
        <asp:View ID="FormView" runat="server">
            <table class="OuterPanel">
                <tr>
                    <td class="Heading1">
                        <%# OperationManager.CurrentOperation == "CreateVirtualServer" ? Resources.Language.CreateVirtualServer : Resources.Language.EditVirtualServer %>
                    </td>
                </tr>
                <tr>
                    <td>
                        <p>
                          <cc:Result runat="server"></cc:Result>
                             <%# OperationManager.CurrentOperation == "CreateVirtualServer" ? Resources.Language.CreateVirtualServerIntro : Resources.Language.EditVirtualServerIntro %>
                         </p>
                           <cc:EntityForm runat="server" id="DataForm" OnEntityCommand="DataForm_EntityCommand" CssClass="Panel" headingtext='<%# OperationManager.CurrentOperation == "CreateVirtualServer" ? Resources.Language.NewVirtualServerDetails : Resources.Language.VirtualServerDetails %>' headingcssclass="Heading2" width="100%">
                             <cc:EntityFormTextBoxItem runat="server" PropertyName="Name" TextBox-Width="400" FieldControlID="Name" IsRequired="true" text='<%# Resources.Language.Name + ":" %>' RequiredErrorMessage='<%# Resources.Language.VirtualServerNameRequired %>'></cc:EntityFormTextBoxItem>
				<cc:EntityFormItem runat="server" PropertyName="PrimaryAdministratorID" FieldControlID="PrimaryAdministrator" ControlValuePropertyName="SelectedEntityID" text='<%# Resources.Language.PrimaryAdministrator + ":" %>'><FieldTemplate><cc:EntitySelect width="400px" EntityType="SoftwareMonkeys.SiteStarter.Entities.User, SoftwareMonkeys.SiteStarter.Entities" runat="server" ValuePropertyName='Name' id="PrimaryAdministrator" NoDataText='<%# "-- " + Resources.Language.SelectUser + " --" %>' OnDataLoading='PrimaryAdministratorSelect_DataLoading'></cc:EntitySelect></FieldTemplate></cc:EntityFormItem>
                                <cc:EntityFormKeywordsItem runat="server" PropertyName="Keywords" KeywordsControl-CssClass="Field" ControlValuePropertyName="Keywords" Text='<%# Resources.Language.Keywords + ":" %>'/>
                               <cc:EntityFormButtonsItem ID="EntityFormButtonsItem1" runat="server"><FieldTemplate><asp:Button ID="SaveButton" runat="server" CausesValidation="True" CommandName="Save"
                                                    Text='<%# Resources.Language.Save %>' Visible='<%# OperationManager.CurrentOperation == "CreateVirtualServer" %>'></asp:Button>
                                                    <asp:Button ID="UpdateButton" runat="server" CausesValidation="True" CommandName="Update"
                                                    Text='<%# Resources.Language.Update %>' Visible='<%# OperationManager.CurrentOperation == "EditVirtualServer" %>'></asp:Button>
                                                <asp:Button ID="SaveCancelButton" runat="server" CausesValidation="False" CommandName="Cancel"
                                                    Text='<%# Resources.Language.Cancel %>'>
                                                </asp:Button></FieldTemplate></cc:EntityFormButtonsItem>
</cc:EntityForm> 
                    </td>
                </tr>
            </table>
        </asp:View>
    </asp:MultiView>
</asp:Content>
