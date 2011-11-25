<%@ Control Language="C#" ClassName="RegisterEditProjection" Inherits="SoftwareMonkeys.SiteStarter.Web.Projections.BaseProjection" %>
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

        config.Settings["SmtpServer"] = SmtpServer.Text;
        config.Settings["EnableSmtpAuthentication"] = EnableSmtpAuthentication.Checked;
        config.Settings["SmtpUsername"] = SmtpUsername.Text;
        
        if (SmtpPassword.Text != String.Empty)
        	config.Settings["SmtpPassword"] = SmtpPassword.Text;
        	
        config.Settings["SystemEmail"] = SystemEmail.Text;
    
    	config.Save();
		
		Config.Application = config;

        // Display the result
        Result.Display(Resources.Language.SettingsUpdated);

        // Show the index again
            Navigator.Go("Settings");
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
            Navigator.Go("Index", "Settings");
    }

    private void UpdateButton_Click(object sender, EventArgs e)
    {
        UpdateSettings();
    }

    private void CancelButton_Click(object sender, EventArgs e)
    {
            Navigator.Go("Index", "Settings");
    }
    #endregion

</script>
    <asp:MultiView ID="PageView" runat="server">
        <asp:View ID="SettingsFormView" runat="server">
     
                         <h1>
                        <%# Resources.Language.EmailSettings %>
                        </h1>
                          <cc:Result runat="server"></cc:Result>
                        <p>
                             <%# Resources.Language.EditEmailSettingsIntro %>
                         </p>
                                               
                            <table class="BodyPanel" width="100%">
                                <tr>
                                    <td colspan="2" class="Heading2"><%# Resources.Language.EmailSettings %></td>
                                </tr>
                                 <tr>
                                    <td class="FieldLabel"><%# Resources.Language.SystemEmail + ":"%></td>
                                    <td class="Field">
                                    <asp:TextBox runat="server" ID="SystemEmail" CssClass="Form" width="400px" Text='<%# Config.Application.Settings.GetString("SystemEmail") %>' /></td>
                                </tr>
                                <tr>
                                    <td colspan="2" class="Heading2"><%# Resources.Language.SmtpSettings %></td>
                                </tr>
                                 <tr>
                                    <td class="FieldLabel"><%# Resources.Language.SmtpServer + ":"%></td>
                                    <td class="Field">
                                    <asp:TextBox runat="server" ID="SmtpServer" CssClass="Form" width="400px" Text='<%# Config.Application.Settings.GetString("SmtpServer") %>' /></td>
                                </tr>
                                
                                <tr>
                                    <td class="FieldLabel"><%# Resources.Language.EnableSmtpAuthentication + ":"%></td>
                                    <td class="Field">
                                    <asp:CheckBox runat="server" ID="EnableSmtpAuthentication" CssClass="Form" Checked='<%# Config.Application.Settings.GetBool("EnableSmtpAuthentication") %>' /></td>
                                </tr>
                                 <tr>
                                    <td class="FieldLabel"><%# Resources.Language.SmtpServer + ":"%></td>
                                    <td class="Field">
                                    <asp:TextBox runat="server" ID="SmtpUsername" CssClass="Form" width="400px" Text='<%# Config.Application.Settings.GetString("SmtpUsername") %>' /></td>
                                </tr>
                                 <tr>
                                    <td class="FieldLabel"><%# Resources.Language.SmtpPassword + ":"%></td>
                                    <td class="Field">
                                    <asp:TextBox runat="server" ID="SmtpPassword" CssClass="Form" width="400px" Text='<%# Config.Application.Settings.GetString("SmtpPassword") %>' /></td>
                                </tr>
                                
                                <tr>
                                    <td></td><td>
                                                    <asp:Button ID="UpdateButton" runat="server" CausesValidation="True" OnClick="UpdateButton_Click" CommandName="Update"
                                                    Text='<%# Resources.Language.Update %>'></asp:Button>
                                                </td></tr></table>
        </asp:View>
    </asp:MultiView>