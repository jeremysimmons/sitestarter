<%@ Control Language="C#" ClassName="ChangePasswordProjection" Inherits="SoftwareMonkeys.SiteStarter.Web.Projections.BaseProjection" %>
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
    	using (LogGroup logGroup = LogGroup.StartDebug("Loading the ChangePassword page."))
    	{
	    	if (Request.QueryString["u"] == null || Request.QueryString["u"] == String.Empty)
	    	{
	    		LogWriter.Error("No user email parameter (\"u\" query string) provided. Redirecting back to recover page.");
	    	
	    		Navigator.Go("RecoverPassword");
	    	}
	    		
	    	if (Request.QueryString["p"] == null || Request.QueryString["p"] == String.Empty)
	    	{
	    		LogWriter.Error("No temporary password parameter (\"p\" query string) provided. Redirecting back to recover page.");
	    		
	    		Navigator.Go("RecoverPassword");
	    	}
	    		
	    	string emailAddress = Request.QueryString["u"];
	    	
	    	LogWriter.Debug("Email address: " + emailAddress);
	    	
	    	User user = RetrieveStrategy.New<User>(false).Retrieve<User>("Email", emailAddress);
	    	
	    	if (user == null)
	    	{
	    		LogWriter.Debug("Can't find user with the email address '" + emailAddress + "'.");
	    		
	    		Navigator.Go("RecoverPassword");
	    	}
	    		
	    	string temporaryPassword = Request.QueryString["p"];
	    	
	    	LogWriter.Debug("Temporary password: " + temporaryPassword);
	    		
	    	// TODO: Check if needs to be encrypted
	    	
	    	if (user.Password != temporaryPassword)
	    	{
	    		LogWriter.Debug("User password is not the temporary password. Cannot change password here.");
	    		
	    		Navigator.Go("RecoverPassword");
    		}
    	
	    	if (!IsPostBack)
	    		DataBind();
    	}
    }
    
    private void UpdateButton_Click(object sender, EventArgs e)
    {
    	string emailAddress = Request.QueryString["u"];
    	
    	string temporaryPassword = Request.QueryString["p"];
    // TODO: Add form validation for confirm password
    	RecoverPasswordStrategy.New(false).ChangePassword(emailAddress, temporaryPassword, Password.Text);
    	
    	User user = RetrieveStrategy.New<User>(false).Retrieve<User>("Email", emailAddress);
    	
    	Authentication.SetAuthenticatedUsername(user.Username);
    	
    	Navigator.Go("Details", "User");
    }
                    
</script>
                   <h1>
                                <%= Resources.Language.ChangePassword %>
                            </h1>
                                <cc:Result ID="Result2" runat="server">
                                </cc:Result>
                                <p class="Intro">
                                    <%= Resources.Language.ChangePasswordIntro %></p>
<p>
<%= Resources.Language.Password %>: <asp:textbox runat="server" id="Password" TextMode="password" width="200px" /></p>
<p>
<%= Resources.Language.PasswordConfirm %>: <asp:textbox runat="server" id="PasswordConfirm" TextMode="password" width="200px" /></p>
<p>
<asp:Button runat="server" id="UpdateButton" text='<%# Resources.Language.Update %>' onclick="UpdateButton_Click" />
                            
               