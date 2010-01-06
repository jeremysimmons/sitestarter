
using System;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web;
using System.ComponentModel;
using System.Text;
using System.Collections.Specialized;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
	/// <summary>
	/// Description of EntitySelectRequester.
	/// </summary>
	public class EntitySelectRequester : WebControl
	{
		
		public string Text
		{
			get
			{
				if (ViewState["Text"] == null)
					ViewState["Text"] = String.Empty;
				return (string)ViewState["Text"];
			}
			set { ViewState["Text"] = value; }
		}
		
		public string DeliveryPage
		{
			get
			{
				if (ViewState["DeliveryPage"] == null)
					ViewState["DeliveryPage"] = String.Empty;
				return (string)ViewState["DeliveryPage"];
			}
			set { ViewState["DeliveryPage"] = value; }
		}
		
		public string CommandKey
		{
			get
			{
				if (ViewState["CommandKey"] == null)
					ViewState["CommandKey"] = String.Empty;
				return (string)ViewState["CommandKey"];
			}
			set { ViewState["CommandKey"] = value; }
		}
		
		public string CommandName
		{
			get
			{
				if (ViewState["CommandName"] == null)
					ViewState["CommandName"] = String.Empty;
				return (string)ViewState["CommandName"];
			}
			set { ViewState["CommandName"] = value; }
		}
		
		public string EntitySelectControlID
		{
			get
			{
				if (ViewState["EntitySelectControlID"] == null)
					ViewState["EntitySelectControlID"] = String.Empty;
				return (string)ViewState["EntitySelectControlID"];
			}
			set { ViewState["EntitySelectControlID"] = value; }
		}
		
		/*public string EntityIDKey
		{
			get
			{
				if (ViewState["EntityIDKey"] == null)
					ViewState["EntityIDKey"] = String.Empty;
				return (string)ViewState["EntityIDKey"];
			}
			set { ViewState["EntityIDKey"] = value; }
		}*/
		
		[Bindable(true)]
		public Guid EntityID
		{
			get
			{
				if (ViewState["EntityID"] == null)
					ViewState["EntityID"] = Guid.Empty;
				return (Guid)ViewState["EntityID"];
			}
			set { ViewState["EntityID"] = value; }
		}
		
		public string TransferData
		{
			get
			{
				if (ViewState["TransferData"] == null)
					ViewState["TransferData"] = String.Empty;
				return (string)ViewState["TransferData"];
			}
			set { ViewState["TransferData"] = value; }
		}
		
		public EntitySelectRequester()
		{
		}
		
		protected override void OnInit(EventArgs e)
		{
			HyperLink link = new HyperLink();
			link.Text = this.Text;
			link.NavigateUrl = "javascript:NewItem();";
			
			//CreateNavigateUrl();
			
			Controls.Add(link);
			
			
			base.OnInit(e);
		}
		
		protected override void OnPreRender(EventArgs e)
		{
			
			RegisterScript();
			
			base.OnPreRender(e);
		}
		
		private void RegisterScript()
		{
			string selectClientID = WebUtilities.FindControlRecursive(Page,EntitySelectControlID).ClientID;
			
			string url = CreateNavigateUrl();
			
			StringBuilder builder = new StringBuilder();
			
			NameValueCollection transferData = ParseTransferData();
			
			builder.Append("<script language='javascript' defer>\n");
			builder.Append("function GetData(key){\n");
			
			foreach (string key in transferData.Keys)
			{
				Control control = WebUtilities.FindControlRecursive(Page,transferData[key]);
				if (control == null)
					throw new Exception("No control found with ID of '" + transferData[key] + "'.");
				string clientID = control.ClientID;
				
				builder.Append("if (key == '" + WebUtilities.EncodeJsString(key) + "'){\n");
				builder.Append("	return document.getElementById('" + clientID + "').value;\n");
				builder.Append("}\n");
			}
			builder.Append("}\n");
			
			
			builder.Append("\n");
			builder.Append("function NewItem(){\n");
			builder.Append("	var url = '" + WebUtilities.EncodeJsString(CreateNavigateUrl()) + "';\n");
			builder.Append("	var win = window.open(url);\n");
			//builder.Append("	var url = " + CreateNavigateUrl() + "\n");
			builder.Append("}\n");
			
			builder.Append("\n");
			
			builder.Append(@"function AddItem(id, text){
			 				var newOption = document.createElement('option');
		    				newOption.value = id;
		    				newOption.text = text;
		    				newOption.selected = true;
		    				alert(id + ' - ' + text);

		    				var field = document.getElementById('" + selectClientID + @"')
		    
            				field.appendChild(newOption)
							}
							");
			
			builder.Append("</script>\n");

			
			if (!Page.ClientScript.IsClientScriptBlockRegistered("EntitySelectRequesterScript"))
				Page.ClientScript.RegisterClientScriptBlock(typeof(EntitySelectRequester), "EntitySelectRequesterScript", builder.ToString());
		}
		
		private UriBuilder AppendQueryString(UriBuilder builder, string queryToAppend)
		{
			
			if (builder.Query != null && builder.Query.Length > 1)
				builder.Query = builder.Query.Substring(1) + "&" + queryToAppend;
			else
				builder.Query = queryToAppend;

			return builder;
		}
		
		private string CreateNavigateUrl()
		{
			string url = ConvertRelativeUrlToAbsoluteUrl(DeliveryPage);

			UriBuilder builder = new UriBuilder(url);
			
			
			//Uri uri = new Uri(baseUri, DeliveryPage);// + "&a=CreateBug&IssueID=' + issueID + '&HideTemplate=true&Title=' + UrlEncode(title) + "&Description=" + UrlEncode(description)
			builder = AppendQueryString(builder, CommandKey + "=" + CommandName);
			builder = AppendQueryString(builder,  "HideTemplate=true");
			builder = AppendQueryString(builder,  "AutoReturn=true");
			//builder = AppendQueryString(builder,  EntityIDKey + "=" + EntityID.ToString());
			builder = AppendQueryString(builder,  "EntityID=" + EntityID.ToString());
			
			
			return builder.ToString();
			
		}
		
		public string ConvertRelativeUrlToAbsoluteUrl(string relativeUrl)
		{
			if (Page.Request.IsSecureConnection)
				return string.Format("https://{0}{1}", Page.Request.Url.Host, Page.ResolveUrl(relativeUrl));
			else
				return string.Format("http://{0}{1}", Page.Request.Url.Host, Page.ResolveUrl(relativeUrl));
		}
		
		public NameValueCollection ParseTransferData()
		{
			NameValueCollection data = new NameValueCollection();
			
			string[] parts = TransferData.Split('&');
			
			foreach (string part in parts)
			{
				string[] subParts = part.Split('=');
				
				data.Add(subParts[0], subParts[1]);
			}
			
			return data;
		}
	}
}
