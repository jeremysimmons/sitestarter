<%@ Control Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Configuration" %>
<%@ Import Namespace="SoftwareMonkeys.SiteStarter.Diagnostics" %>
<script runat="server">
</script>
<asp:ScriptManager ID="ScriptManager1" runat="server" />

<asp:Label ID="lblTime1" runat="server" /><br />
<asp:Button ID="butTime1" runat="server" Text="Refresh Panel" /><br /><br />

<asp:UpdatePanel ID="UpdatePanel1" runat="server">
<Triggers>
<asp:AsyncPostBackTrigger ControlID="butTime1" EventName="Click" />
<asp:PostBackTrigger ControlID="butTime2" />
</Triggers>

<ContentTemplate>
<asp:Label ID="lblTime2" runat="server" /><br />
<asp:Button ID="butTime2" runat="server" Text="Refresh Page" /><br />
</ContentTemplate>
</asp:UpdatePanel>
<br />

<asp:UpdatePanel ID="UpdatePanel2" runat="server">
<Triggers>
<asp:AsyncPostBackTrigger ControlID="DropDownList1" EventName="SelectedIndexChanged" />
</Triggers>

<ContentTemplate>
<asp:Label ID="lblTime3" runat="server" /><br />
</ContentTemplate>
</asp:UpdatePanel>

<asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="true">
<asp:ListItem>Change</asp:ListItem>
<asp:ListItem>My</asp:ListItem>
<asp:ListItem>Value</asp:ListItem>
</asp:DropDownList>

<div class="AutoBackupSummary">

</div>