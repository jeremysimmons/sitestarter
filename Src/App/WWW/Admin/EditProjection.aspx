<%@ Page Language="C#" EnableEventValidation="false" ValidateRequest="false" MasterPageFile="~/Site.master" %>
<%@ Register tagprefix="cc" namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" assembly="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Projections" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.WebControls" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Properties" %>
<%@ Import namespace="SoftwareMonkeys.SiteStarter.Web.Security" %>
<%@ Import namespace="System.IO" %>
<script runat="server">

	ProjectionInfo CurrentProjection = null;

	private void Page_Load(object sender, EventArgs e)
	{
		Authorisation.EnsureUserCan("Edit", "Projection");
	
		EnablePreviewBox.Attributes.Add("onclick", "triggerPreview();");
	
		string projectionKey = Request.QueryString["Projection"];
	
		if (!IsPostBack)
			EditProjection(projectionKey);
	}

	public void EditProjection(string projectionKey)
	{
		if (projectionKey != null && projectionKey != String.Empty)
			LoadProjection(projectionKey);
		else
			CreateProjection();
	}
	
	public void LoadProjection(string projectionKey)
	{
		string[] parts = projectionKey.Split('-');
		
		ProjectionInfo info = null;
		
		// If the key contains a dash - then it's an Action/TypeName based projection
		if (parts.Length == 2)
			info = ProjectionState.Projections[parts[0], parts[1]];
		else
			info = ProjectionState.Projections[projectionKey];
	
		CurrentProjection = info;
	
		string path = new ProjectionFileNamer().CreateProjectionFilePath(info);
	
		string content = new ProjectionLoader().LoadContentFromFile(path);
		
		ProjectionFilePath.Text = info.ProjectionFilePath;
		
		ProjectionContent.Text = content;
		
		if (Request.Form["QueryStrings"] != null && Request.Form["QueryStrings"] != String.Empty)
			QueryStrings.Text = Request.Form["QueryStrings"];
		else
			QueryStrings.Text = PrepareQueryStrings(Request.Url.Query);
	}
	
	public void CreateProjection()
	{
		CurrentProjection = new ProjectionInfo();
		CurrentProjection.Name = "newprojection";
		CurrentProjection.ProjectionFilePath = new ProjectionFileNamer().CreateRelativeProjectionFilePath(CurrentProjection.Name);
	
		string templatePath = new ProjectionFileNamer().CreateProjectionTemplateFilePath("Default");
	
		ProjectionFilePath.Text = CurrentProjection.ProjectionFilePath;
		ProjectionContent.Text = new ProjectionLoader().LoadContentFromFile(templatePath);
	}
	
	public string PrepareQueryStrings(string original)
	{
		NameValueCollection data = HttpUtility.ParseQueryString(original);
		
		data.Remove("Script");
		
		string output = String.Empty;
		
		foreach (string key in data.Keys)
		{
			output += key + "=" + data[key] + "&";
		}
		
		output = output.Trim('&');
		
		if (output != String.Empty)
			output = "&" + output;
		
		return output.Trim('&');
	}
	
	private string GetPreviewLink()
	{
		string link = new UrlCreator().CreateUrl(CurrentProjection);
		
		if (link.IndexOf("?") > -1)
			link = link + "&";
		else
			link = link + "?";
			
		link = link + "HideTemplate=true";
			
		return link;
	}
</script>
<asp:Content ID="BodyContent" ContentPlaceHolderID="Body" runat="server">
<script type="text/javascript">
	function triggerPreview()
	{
		var cb = document.getElementById('<%= EnablePreviewBox.ClientID %>');
		var isEnabled = cb.checked;
		
		if (isEnabled)
			displayPreview('<%= GetPreviewLink() %>');
		else
			displayPreview("");
	}
	
	function save()
	{
		var receiveReq = createHttpRequest();
							
		var output = '';
		
		var url = '<%= Request.ApplicationPath + "/Admin/SaveProjection.aspx" %>';
		
		var projectionPath = document.getElementById('<%= ProjectionFilePath.ClientID %>').value;
		var projectionContent = document.getElementById('<%= ProjectionContent.ClientID %>').value;
		var originalProjectionPath = document.getElementById('OriginalProjectionFilePath').value;
		
		var data = "ProjectionPath=" + projectionPath
			+ "&ProjectionContent=" + projectionContent
			+ "&OriginalProjectionPath=" + originalProjectionPath;
		
		//If our XmlHttpRequest object is not in the middle of a request, start the new call.
		if (receiveReq.readyState == 4 || receiveReq.readyState == 0) {
			
			receiveReq.open("POST", url, false);
							 
			receiveReq.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
			receiveReq.setRequestHeader("Content-length", data.length);
			receiveReq.setRequestHeader("Connection", "close");
							 
			receiveReq.send(data);
			
			output = receiveReq.responseText;
		}
		
		if (output.indexOf("Exception") > -1)
			alert("An error occurred. Check the logs for more info.");
		else
			window.status = '<%= Resources.Language.ProjectionSaved %>';
			
		triggerPreview();
	}
	
	function saveClose()
	{
		save();
		close();
	}
	
	function close()
	{
		window.location.href = '<%= WebUtilities.ConvertApplicationRelativeUrlToAbsoluteUrl(new UrlCreator().CreateUrl(CurrentProjection)) %>';
	}
	
	function insertTab(o, e)
	{
		var kC = e.keyCode ? e.keyCode : e.charCode ? e.charCode : e.which;
		if (kC == 9 && !e.shiftKey && !e.ctrlKey && !e.altKey)
		{
			var oS = o.scrollTop;
			if (o.setSelectionRange)
			{
				var sS = o.selectionStart;
				var sE = o.selectionEnd;
				o.value = o.value.substring(0, sS) + "\t" + o.value.substr(sE);
				o.setSelectionRange(sS + 1, sS + 1);
				o.focus();
			}
			else if (o.createTextRange)
			{
				document.selection.createRange().text = "\t";
				e.returnValue = false;
			}
			o.scrollTop = oS;
			if (e.preventDefault)
			{
				e.preventDefault();
			}
			return false;
		}
		return true;
	}
	
	window.onload = function()
	{
		triggerPreview();
	}
	
	document.onkeypress=function(e)
	{
		if((e.which == 83 || e.which == 115)
			&& e.ctrlKey)
		{
			save();
			
			e.preventDefault();
			
			return false;
			
		}
	}
	
</script>
<div class="Trail"><a href='<%= Request.ApplicationPath %>'><%= Resources.Language.Home %></a> &gt; <a href='<%= Request.ApplicationPath.TrimEnd('/') + "/Admin/Cache.aspx" %>'>Cache</a> &gt; <a href='<%= Request.ApplicationPath.TrimEnd('/') + "/Admin/Projections.aspx" %>'><%= Resources.Language.Projections %></a></div>
<h1><%= (CurrentProjection == null ? Resources.Language.Create : Resources.Language.Edit) + " " + Resources.Language.Projection %></h1>
<p>
	<%= Resources.Language.FilePath %>: <asp:textbox runat="server" id="ProjectionFilePath" width="400px"/>
</p>
<p>
	<asp:textbox runat="server" id="ProjectionContent" style="width: 100%; height: 400px;" onkeydown="insertTab(this, event);" TextMode="Multiline" wrap="false"/>
</p>
<p>
	<%= Resources.Language.ParametersQueryString %>: <asp:textbox runat="server" id="QueryStrings" width="100%"/>
</p>
<p>
	<input type="button" onClick="saveClose();" value='<%= "&laquo; " + Resources.Language.Save + " &amp; " + Resources.Language.Close %>' />&nbsp;<input type="button" id="SaveContinueButton" onClick="save();" value='<%= Resources.Language.Save + " &amp; " + Resources.Language.Continue %>' /> (<%= Resources.Language.HitCtrlSToSave %>)
</p>
<hr/>
<p><%= Resources.Language.EnablePreview %>: <asp:CheckBox runat="Server" id="EnablePreviewBox" /></p>
<cc:PreviewControl runat="server" CssClass="PreviewPanel" />
<input type="hidden" name="OriginalProjectionFilePath" id="OriginalProjectionFilePath" value='<%= CurrentProjection != null ? CurrentProjection.ProjectionFilePath : String.Empty %>' />
</asp:Content>