<%@ Control Language="C#" ClassName="RegisterEditProjection" Inherits="SoftwareMonkeys.SiteStarter.Web.Projections.BaseProjection" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Register Assembly="FredCK.FCKeditorV2" Namespace="FredCK.FCKeditorV2" TagPrefix="fckeditor" %>
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
    /// Displays the form for editing the specified entity.
    /// </summary>
    private void EditSettings()
    {        
        // Start the operation
        OperationManager.StartOperation("EditSettings", SettingsFormView);

        AppConfig config = (AppConfig)Config.Application;

        Authorisation.EnsureUserCan("Edit", "Settings");

        // Bind the form
        SettingsFormView.DataBind();
    }

    /// <summary>
    /// Updates the currently edited entity.
    /// </summary>
    private void UpdateSettings()
    {
        // Get a fresh copy of the config object
        AppConfig config = (AppConfig)Config.Application;

        config.Title = ApplicationTitle.Text;
    
        // Update the config
        SoftwareMonkeys.SiteStarter.Configuration.ConfigFactory<AppConfig>.SaveConfig(Request.MapPath(Request.ApplicationPath + "/App_Data"), config, WebUtilities.GetLocationVariation(Request.Url));

		Config.Application = config;

        // Display the result to the config
        Result.Display(Resources.Language.SettingsUpdated);

        // Show the index again
        Response.Redirect("Settings.aspx");
    }

    #endregion

    #region Event handlers
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            EditSettings();
        }
    }

    private void SettingsForm_EntityCommand(object sender, EntityFormEventArgs e)
    {
        if (e.CommandName == "Update")
        {
            UpdateSettings();
        }
        else
            Response.Redirect("Settings.aspx");
    }

    private void UpdateButton_Click(object sender, EventArgs e)
    {
        UpdateSettings();
    }

    private void CancelButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("Settings.aspx");
    }
    #endregion

</script>
    <asp:MultiView ID="PageView" runat="server">
        <asp:View ID="SettingsFormView" runat="server">
     
                         <h1>
                        <%# Resources.Language.WebSiteSettings %>
                        </h1>
                          <cc:Result runat="server"></cc:Result>
                        <p>
                             <%# Resources.Language.EditWebSiteSettingsIntro %>
                         </p>
                                               
                            <table class="BodyPanel" width="100%">
                                <tr>
                                    <td colspan="2" class="Heading2"><%# Resources.Language.WebSiteSettings %></td>
                                </tr>
                                 <tr>
                                    <td class="FieldLabel">&nbsp;&nbsp;&nbsp;<%# Resources.Language.Title + ":"%></td>
                                    <td class="Field">
                                    <asp:TextBox runat="server" ID="ApplicationTitle" CssClass="Form" width="400px" Text='<%# Config.Application.Title %>' /></td>
                                </tr>
                                
                                <tr>
                                    <td></td><td>
                                                    <asp:Button ID="UpdateButton" runat="server" CausesValidation="True" OnClick="UpdateButton_Click" CommandName="Update"
                                                    Text='<%# Resources.Language.Update %>'></asp:Button>
                                                </td></tr></table>
        </asp:View>
    </asp:MultiView>