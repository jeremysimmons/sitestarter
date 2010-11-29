<%@ Control Language="C#" ClassName="CreateEditProjection" Inherits="SoftwareMonkeys.WorkHub.Web.Projections.BaseProjection" %>
<%@ Register Namespace="SoftwareMonkeys.WorkHub.Web.WebControls" Assembly="SoftwareMonkeys.WorkHub.Web" TagPrefix="cc" %>
<%@ Import Namespace="SoftwareMonkeys.WorkHub.Business" %>
<%@ Import Namespace="SoftwareMonkeys.WorkHub.Web" %>
<%@ Import Namespace="SoftwareMonkeys.WorkHub.Web.Security" %>
<%@ Import Namespace="SoftwareMonkeys.WorkHub.Web.WebControls" %>
<%@ Import Namespace="SoftwareMonkeys.WorkHub.Web.Properties" %>
<%@ Import Namespace="SoftwareMonkeys.WorkHub.Data" %>
<%@ Import Namespace="SoftwareMonkeys.WorkHub.Diagnostics" %>
<%@ Import Namespace="SoftwareMonkeys.WorkHub.Entities" %>
<%@ Import Namespace="SoftwareMonkeys.WorkHub.Business.Security" %>
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
               