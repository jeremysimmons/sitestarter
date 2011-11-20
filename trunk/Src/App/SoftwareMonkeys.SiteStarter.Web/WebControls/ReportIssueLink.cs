using System;
using System.ComponentModel;
using System.Configuration;
using System.Web.UI.WebControls;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Web.Properties;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
	/// <summary>
	/// 
	/// </summary>
	public class ReportIssueLink : WebControl
	{
		HyperLink LinkControl = null;
		
		[Browsable(true)]
		[Bindable(true)]
		public string Text
		{
			get
			{
				if (ViewState["Text"] == null)
					ViewState["Text"] = Language.ReportIssue + " &raquo;";
				return (string)ViewState["Text"];
			}
			set { ViewState["Text"] = value; }
		}
		
		[Browsable(true)]
		[Bindable(true)]
		public string NavigateUrl
		{
			get
			{
				if (ViewState["NavigateUrl"] == null)
					ViewState["NavigateUrl"] = String.Empty;
				return (string)ViewState["NavigateUrl"];
			}
			set { ViewState["NavigateUrl"] = value; }
		}
		
		[Browsable(true)]
		[Bindable(true)]
		public string IssueSubject
		{
			get
			{
				if (ViewState["IssueSubject"] == null)
					ViewState["IssueSubject"] = String.Empty;
				return (string)ViewState["IssueSubject"];
			}
			set { ViewState["IssueSubject"] = value; }
		}
		
		[Browsable(true)]
		[Bindable(true)]
		public string IssueDescription
		{
			get
			{
				if (ViewState["IssueDescription"] == null)
					ViewState["IssueDescription"] = String.Empty;
				return (string)ViewState["IssueDescription"];
			}
			set { ViewState["IssueDescription"] = value; }
		}
		
		public ReportIssueLink()
		{
		}
		
		protected override void OnInit(EventArgs e)
		{
			EnsureChildControls();
			
			base.OnInit(e);
		}
		
		protected override void CreateChildControls()
		{
			if (NavigateUrl == String.Empty)
				NavigateUrl = GetReportIssueUrl();
			
			LinkControl = new HyperLink();
			LinkControl.Text = Text;
			LinkControl.NavigateUrl = "javascript:ReportIssue_" + ClientID + "();";
			
			//<!--add key="ReportIssueUrl" value="http://www.softwaremonkeys.net/hub/Report-Issue.aspx?CurrentProjectID=${Project.ID}&amp;ProjectVersion=${Project.Version}" /-->
			//<!--add key="PostSuggestionUrl" value="http://www.softwaremonkeys.net/hub/Post-Suggestion.aspx?CurrentProjectID=${Project.ID}&amp;ProjectVersion=${Project.Version}" /-->
			
			Controls.Add(LinkControl);
			
			
			base.CreateChildControls();
		}
		
		private string GetReportIssueUrl()
		{
			string path = ConfigurationSettings.AppSettings["ReportIssueUrl"];
			path = path.Replace("${Project.ID}", ConfigurationSettings.AppSettings["UniversalProjectID"]);
			
			path = path.Replace("${Project.Version}", VersionUtilities.GetCurrentVersion().ToString());
			
			return path;
		}
		
		protected override void OnPreRender(EventArgs e)
		{
			RegisterScript();
			
			base.OnPreRender(e);
		}
		
		protected void RegisterScript()
		{
			// TODO: See if script can be reduced in size to boost performance
			string script = @"<script language=""javascript"">
			
			var reportIssue_" + ClientID + @"_window;
			
			function ReportIssue_" + ClientID + @"()
			{
				var path = '" + GetReportIssueUrl() + @"';
				
				if (reportIssue_" + ClientID + @"_window == null)
				{
					reportIssue_" + ClientID + @"_window = window.open(path,
						'ReportError_" + Guid.NewGuid().ToString().Substring(0, 7) + @"',
						'menubar=no,height=800,width=800,resizable=yes,toolbar=no,location=yes,scrollbars=yes');
				}
				
				if (!reportIssue_" + ClientID + @"_window.setFieldValue)
				{
					setTimeout(""ReportIssue_" + ClientID + @"()"", 1000);
				}
				else
				{
					reportIssue_" + ClientID + @"_window.setFieldValue(""Subject"", GetIssueSubject());
					reportIssue_" + ClientID + @"_window.setFieldValue(""Description"", GetIssueDescription());
				}
			}
	
			function GetIssueSubject()
			{
				return """ + WebUtilities.EncodeJsString(IssueSubject) + @""";
			}
			
			function GetIssueDescription()
			{
				return """ + WebUtilities.EncodeJsString(IssueDescription) + @""";
			}
			</script>";
			
			string key = "ReportIssueScript_" + ClientID;
			
			if (!Page.ClientScript.IsClientScriptBlockRegistered(key))
				Page.ClientScript.RegisterClientScriptBlock(GetType(), key, script);
		}
	}
}
