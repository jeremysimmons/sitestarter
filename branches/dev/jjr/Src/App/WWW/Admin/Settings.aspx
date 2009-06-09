<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Title="Manage Settings" %>
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
    /// Displays the index for managing configs.
    /// </summary>
    private void ManageSettings()
    {
        OperationManager.StartOperation("ManageSettings", IndexView);

	    Authorisation.EnsureUserCan("View", typeof(AppConfig));

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
                            <a href="WebSiteSettings.aspx"><%# Resources.Language.WebSiteSettings %></a><br/>
                            <a href="VirtualServerSettings.aspx"><%# Resources.Language.VirtualServerSettings %></a><br/>
                            <a href="EmailSettings.aspx"><%# Resources.Language.EmailSettings %></a><br/>
                    </td>
                </tr>
            </table>
        </asp:View>
    </asp:MultiView>
</asp:Content>
