<%@ Control Language="C#" ClassName="CreateEditProjection" Inherits="SoftwareMonkeys.SiteStarter.Web.Projections.BaseProjection" %>
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
	protected string ToUserName = String.Empty;

  private void Page_Load(object sender, EventArgs e)
  {
	  	if (!IsPostBack)
	  	{
  			Guid toUserID = new Guid(Request.QueryString["RecipientID"]);
  			
  			User toUser = RetrieveStrategy.New<User>().Retrieve<User>("ID", toUserID);
  			
  			if (toUser == null)
  				throw new InvalidOperationException("No user found with the recipient ID specified.");
  				
  			ToUserName = toUser.Name;
	  	
	  		DataBind();
	  	}
  }
  
  private void SendButton_Click(object sender, EventArgs e)
  {
  			Guid toUserID = new Guid(Request.QueryString["RecipientID"]);
  
  	User fromUser = AuthenticationState.User;
  	User toUser = RetrieveStrategy.New<User>().Retrieve<User>("ID", toUserID);
  
  	SendEmailStrategy.New().SendEmail(Subject.Text, Message.Text, fromUser, toUser);
  	
  	Result.Display(Resources.Language.EmailSent);
  	
  	Navigator.Go("Index", "User");
  }
</script>
                   <h1>
                                <%= Resources.Language.SendMessage %>
                            </h1>
                                <cc:Result ID="Result2" runat="server">
                                </cc:Result>
                                <p class="Intro">
                                    <%= Resources.Language.SendMessageIntro %></p>  
                            <h2><%= Resources.Language.MessageDetails %></h2>
                            <table width="100%">
                            <tr>
                            	<td>
                            		To:
                            	</td>
                            	<td>
                            		<%= ToUserName %>
                            	</td>
                            </tr>
                            <tr>
                            	<td>
                            		Subject:
                            	</td>
                            	<td>
                            		<asp:TextBox runat="server" id="Subject" class="Field" width="400px" />
                            	</td>
                            </tr>
                            <tr>
                            	<td>
                            		Message:
                            	</td>
                            	<td>
                            		<asp:TextBox TextMode="Multiline" runat="server" id="Message" class="Field" width="400px" rows="10" />
                            	</td>
                            </tr>
                            <tr>
                            	<td>
                            		
                            	</td>
                            	<td>
                            		<asp:Button runat="server" id="SendButton" class="FormButton" text='<%# Resources.Language.Send %>' onclick="SendButton_Click" />
                            	</td>
                            </tr>
                            </table>
						</cc:EntityForm> 
               