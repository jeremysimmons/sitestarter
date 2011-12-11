<%@ Page Language="C#" autoeventwireup="true" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>
	Report Issue
</title>
<style>
body
{
	font-family: verdana;
	font-size: 11px;
}
</style>
</head>
<body>
	<form method="post" action="TestReportIssue.aspx">
            <h1>
                 Report Issue
            </h1>
	<% if (Request.Form["Subject"] == null) { %>
            <p class="Intro">
            	IMPORTANT! This application is running as a development/debugging installation. Therefore this test page has been used instead of the official link so that functional testing can complete without remote dependencies.
            	DO NOT use this form to report actual issues because it won't be logged.</p>  
            	
               <table id="ctl00_Body_ctl00_DataForm" class="Panel" border="0" style="width:100%;">
	<tr>
		<td class="Heading2" colspan="2">New Issue Details</td>
	</tr><tr>

		<td class="FieldLabel">Subject: <span class='Required'>*</span></td><td><input name="Subject" type="text" id="Subject" style="width:400px;" /><span id="ctl00_Body_ctl00_SubjectReqVal" style="color:Red;visibility:hidden;"> &laquo; </span></td>
	</tr><tr>
		<td class="FieldLabel">Description:</td><td><textarea name="Description" rows="6" cols="20" id="Description" style="width:400px;"></textarea></td>
	</tr><tr>
		<td class="FieldLabel">How to recreate:</td><td><textarea name="ctl00$Body$ctl00$HowToRecreate" rows="6" cols="20" id="ctl00_Body_ctl00_HowToRecreate" style="width:400px;"></textarea></td>

	</tr><tr>
		<td class="FieldLabel">Name:</td><td><input name="ctl00$Body$ctl00$ReporterName" type="text" id="ctl00_Body_ctl00_ReporterName" style="width:400px;" /></td>
	</tr><tr>
		<td class="FieldLabel">Email:</td><td><input name="ctl00$Body$ctl00$ReporterEmail" type="text" id="ctl00_Body_ctl00_ReporterEmail" style="width:400px;" /></td>
	</tr><tr>
		<td class="FieldLabel">Phone:</td><td><input name="ctl00$Body$ctl00$ReporterPhone" type="text" id="ctl00_Body_ctl00_ReporterPhone" style="width:400px;" /></td>
	</tr><tr>

		<td class="FieldLabel">Request Reply:</td><td><input id="ctl00_Body_ctl00_NeedsReply" type="checkbox" name="ctl00$Body$ctl00$NeedsReply" /><label for="ctl00_Body_ctl00_NeedsReply">Please notify me when this issue has been resolved.</label></td>
	</tr><tr>
		<td class="FieldLabel">Project: <span class='Required'>*</span></td><td><input name="ProjectID" type="text" id="ProjectID" style="width:400px;" value='<%= Request.QueryString["ProjectID"] %>' />

	</tr><tr>
		<td class="FieldLabel">Project Version:</td><td><input name="ProjectVersion" type="text" id="ProjectVersion" style="width:200px;" value='<%= Request.QueryString["ProjectVersion"] %>' /></td>
	</tr><tr>
		<td class="FieldLabel"></td><td>
								<input type="submit" name="ctl00$Body$ctl00$SaveButton" value="Save" id="ctl00_Body_ctl00_SaveButton" class="FormButton" />
								&nbsp;

						</td>
	</tr>

</table>
                            			<script language="javascript">
				function setFieldValue(shortId, v)
				{
					if (shortId == "Subject")
					{
						var field = document.getElementById('Subject');

						field.value = v;
						
					}
					if (shortId == "Description")
					{
						var field = document.getElementById('Description');
						
						field.value = v;
						
					}
				}
			</script>
	<% } %>
	<% if (Request.Form["Subject"] != null) { %>
	<h2>Submitted Data</h2>
    <p>
    Subject: <%= Request.Form["Subject"] %>
    </p>
    <p>
    Description: <%= Request.Form["Description"].Replace(Environment.NewLine, "<br/>") %>
    </p>
    <p>
    Project ID: <%= Request.Form["ProjectID"] != null ? Request.Form["ProjectID"] : "[n/a]" %>
    </p>
    <p>
    Project Version: <%= Request.Form["ProjectVersion"] != null ? Request.Form["ProjectVersion"] : "[n/a]" %>
    </p>
    <% } %>
</form>
</body>
</html>