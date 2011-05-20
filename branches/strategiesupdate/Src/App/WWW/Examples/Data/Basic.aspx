<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" %>
<%@ Register tagprefix="cc" namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" assembly="SoftwareMonkeys.SiteStarter.Web" %>
<%@ import namespace="SoftwareMonkeys.SiteStarter.Data" %>
<%@ import namespace="SoftwareMonkeys.SiteStarter.Entities" %>
<%@ import namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ import namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ import namespace="System.IO" %>
<script runat="server">
	// Example...

	private void Page_Load(object sender, EventArgs e)
	{
		// Create a new instance of User entity
		User user = CreateUser();

		AddMessage("New user created: " + user.Username);


		// Save the new user object
		SaveUser(user);

		AddMessage("User saved to data store");


		// Retrieve the newly saved user object
		User loadedUser = LoadUser(user.ID);

		AddMessage("User loaded from data store: " + loadedUser.Username);

		// Edit the loaded user object
		User editedUser = loadedUser;
		editedUser.Username = "demo";

		AddMessage("User edited. New username: " + loadedUser.Username);


		// Updated the edited user object
		UpdateUser(editedUser);

		AddMessage("User updated to data store");


		// Get the updated user
		User updatedUser = GetUserByUsername(user.Username);

		AddMessage("Loaded from data store: " + user.Username);


		// Delete the updated user
		DeleteUser(updatedUser);

		AddMessage("User deleted");
	}

	private User CreateUser()
	{
		// Tip: Entities like User are in the SoftwareMonkeys.SiteStarter.Entities namespace.

		User user = new User();
		user.ID = Guid.NewGuid();

		user.FirstName = "Demo";
		user.LastName = "User";
		user.Email = "demouser@softwaremonkeys.net";

		user.Username = "demouser";
		user.Password = Crypter.EncryptPassword("password");

		// Approve the user so they can log in
		user.IsApproved = true;

		return user;
	}

	private void SaveUser(User user)
	{
		// Tip: The DataAccess class is in the SoftwareMonkeys.SiteStarter.Data namespace.

		DataAccess.Data.Saver.Save(user);
	}

	private User LoadUser(Guid userID)
	{
		// Tip: The DataAccess class is in the SoftwareMonkeys.SiteStarter.Data namespace.

		return DataAccess.Data.Reader.GetEntity<User>("ID", userID);

		// Alternatively...

		// return (User)DataAccess.Data.Retriever.Retrieve(typeof(User), "ID", userID);
	}

	private void UpdateUser(User user)
	{
		// Tip: The DataAccess class is in the SoftwareMonkeys.SiteStarter.Data namespace.

		DataAccess.Data.Updater.Update(user);
	}

	private User GetUserByUsername(string username)
	{
		// Tip: The DataAccess class is in the SoftwareMonkeys.SiteStarter.Data namespace.

		return DataAccess.Data.Reader.GetEntity<User>("Username", username);
		
		// Alternatively...

		// return (User)DataAccess.Data.Reader.GetEntity(typeof(User), "Username", username);
	}

	private void DeleteUser(User user)
	{
		// Tip: The DataAccess class is in the SoftwareMonkeys.SiteStarter.Data namespace.

		DataAccess.Data.Deleter.Delete(user);
	}

	// Utilities...
	// The following code can be ignored

	public int MessageNumber = 1;

	public void AddMessage(string message)
	{
		Panel messagePanel = new Panel();
		messagePanel.Controls.Add(new LiteralControl(MessageNumber + ") " + message));
		MessagesContainer.Controls.Add(messagePanel);	

		MessageNumber ++;
	}

	public string GetSource()
	{
		
		string filePath = Server.MapPath(Request.ApplicationPath + "/" + WebUtilities.ConvertAbsoluteUrlToApplicationRelativeUrl(Request.Url.ToString()));

		string source = String.Empty;	
	
		using (StreamReader reader = new StreamReader(File.OpenRead(filePath)))
		{
			source = reader.ReadToEnd();
			reader.Close();
		}

		return source.Replace("<", "&lt;").Replace(">", "&gt;");
	}
</script>
    <asp:Content runat="server" ContentPlaceHolderID="Body">
	<h1>Data Management Demo</h1>
	<p>The code below generates the following output.</p>
    	<cc:result runat="server"/>
	<h2>Output</h2>
	<asp:panel runat="server" id="MessagesContainer"/>
	<h2>Source Code</h2>
	<textarea style="height: 800px; width: 1000px;">
		<%= GetSource() %>
	</textarea>
    </asp:Content>