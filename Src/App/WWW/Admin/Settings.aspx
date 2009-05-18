<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Title="Manage Settings" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Properties" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<script runat="server">
    
    #region Main functions
    /// <summary>
    /// Displays the index for managing configs.
    /// </summary>
    private void ManageSettings()
    {
        OperationManager.StartOperation("ManageSettings", IndexView);

	    Authorisation.EnsureUserCan("View", typeof(AppConfig));

        //IndexGrid.DataSource = configs;

        IndexView.DataBind();
    }

    /// <summary>
    /// Displays the form for editing the specified entity.
    /// </summary>
    private void EditAppSettings()
    {
        // Start the operation
        OperationManager.StartOperation("EditAppSettings", AppSettingsFormView);

        AppConfig config = (AppConfig)Config.Application;

        Authorisation.EnsureUserCan("Edit", typeof(AppConfig));

        // Load the specified config
        //AppSettingsForm.DataSource = config;

        // Bind the form
        AppSettingsFormView.DataBind();
    }

    /// <summary>
    /// Updates the currently edited entity.
    /// </summary>
    private void UpdateSettings()
    {
        // Get a fresh copy of the config object
        AppConfig config = (AppConfig)Config.Application;

        config.EnableVirtualServer = EnableVirtualServer.Checked;
        config.EnableVirtualServerRegistration = EnableVirtualServerRegistration.Checked;
        config.AutoApproveVirtualServerRegistration = AutoApproveVirtualServerRegistration.Checked;

        // Update the config
        SoftwareMonkeys.SiteStarter.Configuration.ConfigFactory.SaveConfig(Request.MapPath(Request.ApplicationPath + "/App_Data"), config, WebUtilities.GetLocationVariation(Request.Url));

		Config.Application = config;

        // Display the result to the config
        Result.Display(Resources.Language.SettingsUpdated);

        // Show the index again
        ManageSettings();
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
                case "EditAppSettings":
                    EditAppSettings();
                    break;
                case "ManageSettings":
                default:
                    ManageSettings();
                    break;
            }
        }
    }

    private void AppSettingsForm_EntityCommand(object sender, EntityFormEventArgs e)
    {
        if (e.CommandName == "Update")
        {
            UpdateSettings();
        }
        else
            ManageSettings();
    }

    private void UpdateButton_Click(object sender, EventArgs e)
    {
        UpdateSettings();
    }

    private void CancelButton_Click(object sender, EventArgs e)
    {
        ManageSettings();
    }
    #endregion

</script>
<asp:Content ID="Body" ContentPlaceHolderID="Body" runat="Server">
    <asp:MultiView ID="PageView" runat="server">
        <asp:View ID="IndexView" runat="server">
            <table class="OuterPanel">
                <tr>
                    <td class="Heading1">
                        <%# Resources.Language.Settings %></td>
                </tr>
                <tr>
                    <td>
                        <cc:Result runat="server" ID="IndexResult">
                        </cc:Result>
                        <p>
                            <%# Resources.Language.SettingsIntro %></p>
                        </td>
                        </tr>
                        <tr>
                        <td>
                            <a href="Settings.aspx?a=EditAppSettings"><%# Resources.Language.ApplicationSettings %></a>
                    </td>
                </tr>
            </table>
        </asp:View>
        <asp:View ID="AppSettingsFormView" runat="server">
     
                         
            <table class="OuterPanel">
                <tr>
                    <td class="Heading1">
                        <%# Resources.Language.ApplicationSettings %>
                    </td>
                </tr>
                <tr>
                    <td>
                          <cc:Result runat="server"></cc:Result>
                        <p>
                             <%# Resources.Language.EditApplicationSettingsIntro %>
                         </p>
                                               
                            <table class="BodyPanel" width="100%">
                                <tr class="Heading2">
                                    <td colspan="2" class="Heading2"><%# Resources.Language.VirtualServer %></td>
                                </tr>
                                <tr>
                                    <td class="FieldLabel">&nbsp;&nbsp;&nbsp;<%# Resources.Language.EnableVirtualServer + ":" %></td>
                                    <td class="Field"><asp:Checkbox runat="server" ID="EnableVirtualServer" Text='<%# Resources.Language.EnableVirtualServerNote %>' Checked='<%# Config.Application.EnableVirtualServer %>' /></td>
                                </tr>
                                <tr>
                                    <td class="FieldLabel">&nbsp;&nbsp;&nbsp;<%# Resources.Language.EnableRegistration + ":"%></td>
                                    <td class="Field" id="EnableVirtualServerRegistrationRow">&nbsp;&nbsp;&nbsp;> <asp:Checkbox runat="server" ID="EnableVirtualServerRegistration" Text='<%# Resources.Language.EnableVirtualServerRegistrationNote %>' Checked='<%# Config.Application.EnableVirtualServerRegistration %>' /></td>
                                </tr>
                                 <tr>
                                    <td class="FieldLabel">&nbsp;&nbsp;&nbsp;<%# Resources.Language.AutoApproveRegistration + ":"%></td>
                                    <td class="Field" id="AutoApproveVirtualServerRegistrationRow">&nbsp;&nbsp;&nbsp;> <asp:Checkbox runat="server" ID="AutoApproveVirtualServerRegistration" Text='<%# Resources.Language.AutoApproveVirtualServerRegistrationNote %>' Checked='<%# Config.Application.AutoApproveVirtualServerRegistration %>' /></td>
                                </tr>
                                <tr>
                                    <td></td><td>
                                                    <asp:Button ID="UpdateButton" runat="server" CausesValidation="True" OnClick="UpdateButton_Click" CommandName="Update"
                                                    Text='<%# Resources.Language.Update %>'></asp:Button>
                                                <asp:Button ID="CancelButton" runat="server" CausesValidation="False" OnClick="CancelButton_Click" CommandName="Cancel"
                                                    Text='<%# Resources.Language.Cancel %>' OnClientClick='<%# "return confirm (\"" +  Resources.Language.ConfirmCancelEditSettings + "\")" %>'>
                                                </asp:Button>
                                                </td></tr></table>
                    </td>
                </tr>
            </table>
                <script language="javascript">
                
						document.getElementById('<%= EnableVirtualServer.ClientID %>').onclick = RefreshForm;
                
                         function RefreshForm()
                         {
                            var enableVirtualServer = document.getElementById('<%= EnableVirtualServer.ClientID %>').checked;
                            
                            if (!enableVirtualServer)
                            {
                                document.getElementById('EnableVirtualServerRegistrationRow').setAttribute("disabled", true);
                                document.getElementById('AutoApproveVirtualServerRegistrationRow').setAttribute("disabled", true);
                            }
                            else
                            {
                                document.getElementById('EnableVirtualServerRegistrationRow').setAttribute("disabled", false);
                                document.getElementById('AutoApproveVirtualServerRegistrationRow').setAttribute("disabled", false);
                            }
                         }
                         
                         RefreshForm();
                         </script>
        </asp:View>
    </asp:MultiView>
</asp:Content>
