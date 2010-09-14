<%@ Page Language="C#" MasterPageFile="../Site.master" AutoEventWireup="true"
     Title="Manage Keywords" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<script runat="server">
    #region Main functions
    /// <summary>
    /// Displays the index for managing keywords.
    /// </summary>
    private void ManageKeywords()
    {
        OperationManager.StartOperation("ManageKeywords", IndexView);

		PagingLocation location = new PagingLocation(IndexGrid.CurrentPageIndex, IndexGrid.PageSize);
        
        IndexGrid.DataSource = StrategyState.Strategies.NewIndexer(typeof(Keyword))
        	.Get<Keyword>(location, IndexGrid.CurrentSort);

        IndexView.DataBind();
    }

    /// <summary>
    /// Displays the form for creating a keyword.
    /// </summary>
    private void CreateKeyword()
    {
        OperationManager.StartOperation("CreateKeyword", FormView);

        Keyword keyword = new Keyword();
        keyword.ID = Guid.NewGuid();
        DataForm.DataSource = keyword;

        FormView.DataBind();
    }

    /// <summary>
    /// Saves the newly created keyword.
    /// </summary>
    private void SaveKeyword()
    {
        // Save the new keyword
        DataForm.ReverseBind();
        if (StrategyState.Strategies.NewUniqueSaver(typeof(Keyword))
        	.Save((Keyword)DataForm.DataSource))
        {
            // Display the result to the keyword
            Result.Display(Resources.Language.KeywordSaved);

            // Show the index again
            ManageKeywords();
        }
        else
            Result.DisplayError(Resources.Language.KeywordNameTaken);
    }

    private void EditKeyword(Guid keywordID)
    {
        // Start the operation
        OperationManager.StartOperation("EditKeyword", FormView);

        // Load the specified keyword
        DataForm.DataSource = KeywordFactory.GetKeyword(keywordID);

        // Bind the form
        FormView.DataBind();
    }

    private void UpdateKeyword()
    {
        // Get a fresh copy of the keyword object
        Keyword keyword = KeywordFactory.GetKeyword(((Keyword)DataForm.DataSource).ID);

        // Transfer data from the form to the object
        DataForm.ReverseBind(keyword);
        
        // Update the keyword
        if (KeywordFactory.UpdateKeyword(keyword))
        {
            // Display the result to the keyword
            Result.Display(Resources.Language.KeywordUpdated);

            // Show the index again
            ManageKeywords();
        }
        else
        {
            Result.DisplayError(Resources.Language.KeywordNameTaken);
        }
    }

    /// <summary>
    /// Deletes the keyword with the provided ID.
    /// </summary>
    /// <param name="keywordID">The ID of the keyword to delete.</param>
    private void DeleteKeyword(Guid keywordID)
    {
        // Delete the specified keyword
        KeywordFactory.DeleteKeyword(KeywordFactory.GetKeyword(keywordID));

        // Display the result to the keyword
        Result.Display(Resources.Language.KeywordDeleted);

        // Go back to the index
        ManageKeywords();
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
                case "ManageKeywords":
                default:
                    ManageKeywords();
                    break;
            }
        }
    }

    protected void CreateButton_Click(object sender, EventArgs e)
    {
        // Create a new keyword
        CreateKeyword();
    }

    private void DataForm_EntityCommand(object sender, EntityFormEventArgs e)
    {
        if (e.CommandName == "Save")
        {
            SaveKeyword();
        }
        else if (e.CommandName == "Update")
        {
            UpdateKeyword();
        }
        else
            ManageKeywords();
    }

    protected void IndexGrid_ItemCommand(object source, DataGridCommandEventArgs e)
    {
        if (e.CommandName == "Edit")
        {
            EditKeyword(new Guid(IndexGrid.DataKeys[e.Item.ItemIndex].ToString()));
        }
        else if (e.CommandName == "Delete")
        {
            DeleteKeyword(new Guid(IndexGrid.DataKeys[e.Item.ItemIndex].ToString()));
        }
    }

    #endregion
</script>
<asp:Content ID="Body" ContentPlaceHolderID="Body" runat="Server">
    <asp:MultiView ID="PageView" runat="server">
        <asp:View ID="IndexView" runat="server">
            <table class="OuterPanel">
                <tr>
                    <td class="Heading1">
                        <%# Resources.Language.ManageKeywords %></td>
                </tr>
                <tr>
                    <td>
                        <cc:Result runat="server" ID="IndexResult">
                        </cc:Result>
                        <p>
                            <%# Resources.Language.ManageKeywordsIntro %></p>
                        <p>
                            <asp:Button ID="CreateButton" runat="server" OnClick="CreateButton_Click" Text='<%# Resources.Language.CreateKeyword %>'
                                CommandName="New" />&nbsp;</p>
                        <p>
                            <cc:IndexGrid ID="IndexGrid" runat="server" AllowPaging="True" HeaderStyle-CssClass="Heading2" AllowSorting="True"
                                AutoGenerateColumns="False" EmptyDataText='<%# Resources.Language.NoKeywordsFound %>'
                                Width="100%"
                                PageSize="2" OnItemCommand="IndexGrid_ItemCommand" DataKeyField="ID">
                                <Columns>
                                    <asp:BoundColumn DataField="Name" HeaderText="Name" SortExpression="Name" />
                                    <asp:TemplateColumn>
                                        <itemtemplate>
<asp:LinkButton ID="LinkButton1" runat="server" text='<%# Resources.Language.Edit %>' ToolTip='<%# Resources.Language.EditKeywordToolTip %>' CommandName="Edit"></asp:LinkButton>
<asp:LinkButton ID="LinkButton2" ToolTip='<%# Resources.Language.DeleteKeywordToolTip %>' runat="server" text='<%# Resources.Language.Delete %>' CommandName="Delete"></asp:LinkButton>
                                      
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
                        <%# OperationManager.CurrentOperation == "CreateKeyword" ? Resources.Language.CreateKeyword : Resources.Language.EditKeyword %>
                    </td>
                </tr>
                <tr>
                    <td>
                        <p>
                          <cc:Result runat="server"></cc:Result>
                             <%# OperationManager.CurrentOperation == "CreateKeyword" ? Resources.Language.CreateKeywordIntro : Resources.Language.EditKeywordIntro %>
                         </p>
                           <cc:EntityForm runat="server" id="DataForm" OnEntityCommand="DataForm_EntityCommand" CssClass="Panel" headingtext='<%# OperationManager.CurrentOperation == "CreateKeyword" ? Resources.Language.NewKeywordDetails : Resources.Language.KeywordDetails %>' headingcssclass="Heading2" width="100%">
                             <cc:EntityFormTextBoxItem runat="server" PropertyName="Name" TextBox-Width="400" FieldControlID="Name" IsRequired="true" text='<%# Resources.Language.Name + ":" %>' RequiredErrorMessage='<%# Resources.Language.KeywordNameRequired %>'></cc:EntityFormTextBoxItem>
                             <cc:EntityFormTextBoxItem runat="server" PropertyName="Description" TextBox-Width="400" FieldControlID="Description" text='<%# Resources.Language.Description + ":" %>' textbox-rows="6" textbox-textmode="multiline"></cc:EntityFormTextBoxItem>
                                  <cc:EntityFormButtonsItem ID="EntityFormButtonsItem1" runat="server"><FieldTemplate><asp:Button ID="SaveButton" runat="server" CausesValidation="True" CommandName="Save"
                                                    Text='<%# Resources.Language.Save %>' Visible='<%# OperationManager.CurrentOperation == "CreateKeyword" %>'></asp:Button>
                                                    <asp:Button ID="UpdateButton" runat="server" CausesValidation="True" CommandName="Update"
                                                    Text='<%# Resources.Language.Update %>' Visible='<%# OperationManager.CurrentOperation == "EditKeyword" %>'></asp:Button>
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
