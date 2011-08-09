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
    /// Displays the index for managing configs.
    /// </summary>
    private void ManageSettings()
    {
        OperationManager.StartOperation("ManageSettings", IndexView);

	    Authorisation.EnsureUserCan("Edit", "Settings");

        //IndexGrid.DataSource = configs;

        IndexView.DataBind();
    }

    #endregion

    #region Event handlers
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
                    ManageSettings();
        }
    }

    #endregion

</script>
    <asp:MultiView ID="PageView" runat="server">
        <asp:View ID="IndexView" runat="server">
            <h1>
                        <%# Resources.Language.Settings %></h1>

                        <cc:Result runat="server" ID="IndexResult">
                        </cc:Result>
                        <p>
                            <%# Resources.Language.SettingsIntro %></p>
			<ul>
				<li>
		                         <a href='<%= Navigator.GetLink("Edit", "WebSiteSettings") %>' id='WebSiteSettingsLink'><%# Resources.Language.WebSiteSettings %></a>
				</li>
				<li>
		                         <a href='<%= Navigator.GetLink("Edit", "UserSettings") %>' id='UserSettingsLink'><%# Resources.Language.UserSettings %></a>
				</li>
				<li>
		                         <a href='<%= Navigator.GetLink("Edit", "EmailSettings") %>' id='EmailSettingsLink'><%# Resources.Language.EmailSettings %></a> - <a href='<%= Navigator.GetLink("Test", "Smtp") %>' id='TestSmtpLink'><%# Resources.Language.TestSmtpSettings %></a>
				</li>
			</ul>
        </asp:View>
    </asp:MultiView>