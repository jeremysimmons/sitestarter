<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>
	Report Issue
</title>
</head>
<body>
	<form runat="server">
            <h1>
                 Report Issue
            </h1>
            
            <p class="Intro">
            	Please describe the issue in the form below. Most fields are optional so that you can report the issue quickly, but the more information that is provided the easier it is to resolve.</p>  
            	
               <table id="ctl00_Body_ctl00_DataForm" class="Panel" border="0" style="width:100%;">
	<tr>
		<td class="Heading2" colspan="2">New Issue Details</td>
	</tr><tr>

		<td class="FieldLabel">Subject: <span class='Required'>*</span></td><td><input name="ctl00$Body$ctl00$Subject" type="text" id="ctl00_Body_ctl00_Subject" style="width:400px;" /><span id="ctl00_Body_ctl00_SubjectReqVal" style="color:Red;visibility:hidden;"> &laquo; </span></td>
	</tr><tr>
		<td class="FieldLabel">Description:</td><td><textarea name="ctl00$Body$ctl00$Description" rows="6" cols="20" id="ctl00_Body_ctl00_Description" style="width:400px;"></textarea></td>
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
		<td class="FieldLabel">Project: <span class='Required'>*</span></td><td><select name="ctl00$Body$ctl00$Project" id="ctl00_Body_ctl00_Project" class="Field" style="width:400px;">
			<option value="00000000-0000-0000-0000-000000000000">-- Select Project --</option>

		</select><span id="ctl00_Body_ctl00_ProjectReqVal" style="color:Red;visibility:hidden;"> &laquo; </span></td>

	</tr><tr>
		<td class="FieldLabel">Project Version:</td><td><input name="ctl00$Body$ctl00$ProjectVersion" type="text" id="ctl00_Body_ctl00_ProjectVersion" style="width:200px;" /></td>
	</tr><tr>
		<td class="FieldLabel"></td><td>
								<input type="submit" name="ctl00$Body$ctl00$SaveButton" value="Save" onclick="javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;ctl00$Body$ctl00$SaveButton&quot;, &quot;&quot;, true, &quot;&quot;, &quot;&quot;, false, false))" id="ctl00_Body_ctl00_SaveButton" class="FormButton" />
								&nbsp;

						</td>
	</tr>

</table>
                            			<script language="javascript">
				function setFieldValue(shortId, v)
				{
					if (shortId == "Subject")
					{
						var field = document.getElementById('ctl00_Body_ctl00_Subject');

						field.value = v;
						
					}
					if (shortId == "Description")
					{
						var field = document.getElementById('ctl00_Body_ctl00_Description');
						
						field.value = v;
						
					}
				}
			</script>
    
</form>
</body>
</html>