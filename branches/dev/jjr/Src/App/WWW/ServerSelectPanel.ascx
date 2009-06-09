<%@ Control Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Business" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.State" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<script runat="Server">
    protected string currentServerName = "[" + Resources.Language.Default + "]";
    
    private void Page_Load(object sender, EventArgs e)
    {
        currentServerName = VirtualServerState.VirtualServerName == null || VirtualServerState.VirtualServerName == String.Empty ? "[Default]" : VirtualServerState.VirtualServerName;
        
        DataBind();
        
        Visible = Config.Application.EnableVirtualServer && Config.Application.EnableVirtualServerRegistration;
        
    }

    private void SwitchVirtualServerButton_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.ApplicationPath + "/VirtualServer.aspx?a=Switch&Server=" + NewServerName.Text);
    }
</script>
<div class="Heading2"><%= Resources.Language.VirtualServer %></div>
<div width="200px" class="VirtualServerPanel">
<table width="90%">
<tr>
<td width="50%" valign="top">
<div><b><%= Resources.Language.CurrentVirtualServer %></b></div>
<p>&nbsp;&nbsp;&nbsp;&nbsp;<%# currentServerName %></p>
<div><b><%= Resources.Language.SwitchServer %></b></div>
<p>&nbsp;&nbsp;&nbsp;&nbsp;<%= Resources.Language.ServerNameOrID %>:</p>
<p>&nbsp;&nbsp;&nbsp;&nbsp;<asp:textbox runat="server" id="NewServerName" /> <asp:LinkButton runat="server" ID="SwitchVirtualServerButton" OnClick="SwitchVirtualServerButton_Click" Text='<%# Resources.Language.Go + " &raquo;" %>' /></p>
</td>
<td valign="top">
<div><b><%= Resources.Language.CreateServer %></b></div>
<p>&nbsp;&nbsp;&nbsp;&nbsp;<a href='<%= Request.ApplicationPath + "/VirtualServer.aspx?a=Create" %>'><%= Resources.Language.NewVirtualServer + " &raquo;" %></a></p>
<div><b><%= Resources.Language.WhatIsAVirtualServer %></b></div>
<p>&nbsp;&nbsp;&nbsp;&nbsp;<a href='<%= Request.ApplicationPath + "/Admin/HelpRaw.aspx?a=VirtualServersInfo" %>'><%= Resources.Language.TellMeMoreAboutVirtualServers + " &raquo;" %></a></p>
</td>
</tr>
</table>
</div>