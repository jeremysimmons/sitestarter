<%@ Control Language="C#" ClassName="UserSettingsProjection" Inherits="SoftwareMonkeys.SiteStarter.Web.Projections.BaseProjection" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Properties" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<script runat="server">
    #region Main functions

    /// <summary>
    /// Displays the form for editing the specified entity.
    /// </summary>
    private void EditSettings()
    {        
    	using (LogGroup logGroup = LogGroup.Start("Displaying the form to edit the user settings.", NLog.LogLevel.Debug))
    	{
	        // Start the operation
	        OperationManager.StartOperation("EditSettings", SettingsFormView);
	
	        AppConfig config = (AppConfig)Config.Application;
	
	        Authorisation.EnsureUserCan("Edit", "Settings");
	
	        // Bind the form
	        SettingsFormView.DataBind();
        }
    }

    /// <summary>
    /// Updates the currently edited entity.
    /// </summary>
    private void UpdateSettings()
    {
    	using (LogGroup logGroup = LogGroup.Start("Updating the user settings.", NLog.LogLevel.Debug))
    	{
		    // Get a fresh copy of the config object
		    AppConfig config = (AppConfig)Config.Application;
		
		    config.Settings["EnableUserRegistration"] = EnableUserRegistration.Checked;
		    config.Settings["AutoApproveNewUsers"] = AutoApproveNewUsers.Checked;
		
		    // Update the config
		    config.Save();
		    
			Config.Application = config;
		
		    // Display the result to the user
		    Result.Display(Resources.Language.SettingsUpdated);
		
		    // Show the index again
		    Navigator.Go("Settings");
        }
    }

    #endregion

    #region Event handlers
    protected void Page_Load(object sender, EventArgs e)
    {
    	using (LogGroup logGroup = LogGroup.Start("Loading the edit user settings page.", NLog.LogLevel.Debug))
    	{
	        if (!IsPostBack)
	        {
	            EditSettings();
	        }
        }
    }

    private void UpdateButton_Click(object sender, EventArgs e)
    {
    	using (LogGroup logGroup = LogGroup.Start("Handling the click event of the update button.", NLog.LogLevel.Debug))
    	{
        	UpdateSettings();
        }
    }

    #endregion

</script>
    <asp:MultiView ID="PageView" runat="server">
        <asp:View ID="SettingsFormView" runat="server">
     
                         <h1>
                        <%# Resources.Language.UserSettings %>
                        </h1>
                          <cc:Result runat="server"></cc:Result>
                        <p>
                             <%# Resources.Language.EditUserSettingsIntro %>
                         </p>
                                               
                            <table class="BodyPanel" width="100%">
                                <tr>
                                    <td colspan="2" class="Heading2"><%# Resources.Language.UserSettings %></td>
                                </tr>
                                 <tr>
                                    <td class="FieldLabel">&nbsp;&nbsp;&nbsp;<%# Resources.Language.EnableUserRegistration + ":"%></td>
                                    <td class="Field">
                                    <asp:CheckBox runat="server" ID="EnableUserRegistration" CssClass="Form" width="400px" Checked='<%# Config.Application.Settings.GetBool("EnableUserRegistration") %>' text='<%# Resources.Language.EnableUserRegistrationNote %>' /></td>
                                </tr>
                                
                                 <tr>
                                    <td class="FieldLabel">&nbsp;&nbsp;&nbsp;<%# Resources.Language.AutoApproveNewUsers + ":"%></td>
                                    <td class="Field">
                                    <asp:CheckBox runat="server" ID="AutoApproveNewUsers" CssClass="Form" width="400px" Checked='<%# Config.Application.Settings.GetBool("AutoApproveNewUsers") %>' text='<%# Resources.Language.AutoApproveNewUsersNote %>' /></td>
                                </tr>
                                <tr>
                                    <td></td><td>
                                                    <asp:Button ID="UpdateButton" runat="server" CausesValidation="True" OnClick="UpdateButton_Click"
                                                    Text='<%# Resources.Language.Update %>'></asp:Button>
                                                </td></tr></table>
        </asp:View>
    </asp:MultiView>