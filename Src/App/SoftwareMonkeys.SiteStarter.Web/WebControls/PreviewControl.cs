using System;
using System.Web.UI.WebControls;
using SoftwareMonkeys.SiteStarter.Web.Properties;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
	/// <summary>
	/// Displays a preview of a specified page.
	/// </summary>
	public class PreviewControl : Panel
	{
		public string PreviewUrl = String.Empty;
		
		public PreviewControl()
		{
		}
		
		protected override void OnInit(EventArgs e)
		{
			Initialize();
			
			base.OnInit(e);
			
		}
		
		protected void Initialize()
		{
			InitializeJavaScript();
		}
		
		protected void InitializeJavaScript()
		{
			InitializeHttpRequestsScript();
			
			InitializePreviewFunctionScript();
			
			InitializeLoadPreviewScript();
			
		}
		
		protected void InitializePreviewFunctionScript()
		{
			string script = @"<script type=""text/javascript"">
				
				function displayPreview(url){
					var receiveReq = createHttpRequest();
							
					var output = '';
					
					var holder = document.getElementById('" + ClientID  + @"');

					if (holder)
						holder.innerHTML = '" + Language.Loading + @"...';
					
					if (url != '')
					{
						//If our XmlHttpRequest object is not in the middle of a request, start the new call.
						if (receiveReq.readyState == 4 || receiveReq.readyState == 0) {
							
							receiveReq.open(""GET"", url, false);
											 
							receiveReq.send(null);
							
							output = receiveReq.responseText;
		
						}
					}
					
					if (holder)
						holder.innerHTML = output;
				}
			</script>";
			
			Page.ClientScript.RegisterClientScriptBlock(GetType(), "PreviewControlScript", script);
		}
		
		protected void InitializeLoadPreviewScript()
		{
			string script = @"<script type=""text/javascript"">
			window.onload = function()
			{
				displayPreview('" + PreviewUrl + @"');
			}
			</script>";
			
			if (Visible && Enabled && PreviewUrl != String.Empty)
				Page.ClientScript.RegisterClientScriptBlock(GetType(), "LoadPreviewControlScript", script);
				
		}
		
		protected void InitializeHttpRequestsScript()
		{
			string script = Page.Request.ApplicationPath + "/js/httpRequests.js";
			string key = "HttpRequestsScript";
			
			if (!Page.ClientScript.IsClientScriptIncludeRegistered(key))
				Page.ClientScript.RegisterClientScriptInclude(GetType(), key, script);
		}
	}
}
