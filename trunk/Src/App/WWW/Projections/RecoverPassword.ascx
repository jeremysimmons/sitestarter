<%@ Control Language="C#" ClassName="RecoverPasswordProjection" Inherits="SoftwareMonkeys.SiteStarter.Web.Projections.BaseProjection" %>
<%@ Register Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" Assembly="SoftwareMonkeys.SiteStarter.Web" TagPrefix="cc" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Web.Properties" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business.Security" %>
<%@ Import Namespace="System.Collections.Generic" %>
<script runat="server">
    private void Page_Load(object sender, EventArgs e)
    {
    	if (!IsPostBack)
    		DataBind();
    }
    
    private void RecoverButton_Click(object sender, EventArgs e)
    {
    	if (!RecoverPasswordStrategy.New(false).ResetViaEmail(EmailAddress.Text,
    		Resources.Language.ResetPasswordEmailSubject,
    		Resources.Language.ResetPasswordEmailMessage,
    		WebUtilities.ConvertRelativeUrlToAbsoluteUrl(Request.ApplicationPath)))
    	{
    		Result.DisplayError(Resources.Language.EmailNotFound);
    	}
    }
                    
</script>
<% if (!IsPostBack) { %>
<h1>
            <%= Resources.Language.RecoverPassword %>
        </h1>
            <cc:Result ID="Result1" runat="server">
            </cc:Result>
            <p class="Intro">
                <%= Resources.Language.RecoverPasswordIntro %></p>
<p>
<asp:textbox runat="server" id="EmailAddress" width="400px" /></p>
<p>
<asp:Button runat="server" id="RecoverButton" text='<%# Resources.Language.Recover %>' onclick="RecoverButton_Click" />
                            
               <% } else { %>
        <h1>
            <%= Resources.Language.RecoverPassword %>
        </h1>
        <p><%= Resources.Language.RecoverEmailSent %></p>
               <% } %>