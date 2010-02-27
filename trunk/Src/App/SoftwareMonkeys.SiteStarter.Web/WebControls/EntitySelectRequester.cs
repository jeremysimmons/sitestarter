
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
		protected HyperLink TextLink;
		
		[Bindable(true)]
		public string Text
		{
			get
			{
				if (ViewState["Text"] == null)
					ViewState["Text"] = String.Empty;
				return (string)ViewState["Text"];
			}
			set { ViewState["Text"] = value;
				TextLink.Text = value;
			}
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
		
		public string EntityType
		{
			get
			{
				if (ViewState["EntityType"] == null)
					ViewState["EntityType"] = String.Empty;
				return (string)ViewState["EntityType"];
			}
			set { ViewState["EntityType"] = value; }
		}
		
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
		
		[Browsable(true)]
		public Unit WindowHeight
		{
			get
			{
				if (ViewState["WindowHeight"] == null)
					ViewState["WindowHeight"] = Unit.Pixel(400);
				return (Unit)ViewState["WindowHeight"];
			}
			set { ViewState["WindowHeight"] = value; }
		}
		
		[Browsable(true)]
		public Unit WindowWidth
		{
			get
			{
				if (ViewState["WindowWidth"] == null)
					ViewState["WindowWidth"] = Unit.Pixel(400);
				return (Unit)ViewState["WindowWidth"];
			}
			set { ViewState["WindowWidth"] = value; }
		}
		
		public EntitySelectRequester()
		{
		}
		
		protected override void OnInit(EventArgs e)
		{
			TextLink = new HyperLink();
			TextLink.Text = this.Text;
			TextLink.NavigateUrl = "javascript:NewItem_" + ClientID + "();";
			
			//CreateNavigateUrl();
			
			Controls.Add(TextLink);
			
			
			base.OnInit(e);
		}
		
		protected override void OnPreRender(EventArgs e)
		{
			
			RegisterScript();
			
			base.OnPreRender(e);
		}
		
		private void RegisterScript()
		{
			string selectClientID = WebControlUtilities.FindControlRecursive(Page,EntitySelectControlID).ClientID;
			
			string url = CreateNavigateUrl();
			
			StringBuilder builder = new StringBuilder();
			
			NameValueCollection transferData = ParseTransferData();
			
			builder.Append("<script language='javascript' defer>\n");
			builder.Append("function GetData_" + ClientID + "(key){\n");
			
			foreach (string key in transferData.Keys)
			{
				Control control = WebControlUtilities.FindControlRecursive(Page,transferData[key]);
				if (control == null)
					throw new Exception("No control found with ID of '" + transferData[key] + "'.");
				string clientID = control.ClientID;
				
				builder.Append("if (key == '" + WebUtilities.EncodeJsString(key) + "'){\n");
				builder.Append("	return document.getElementById('" + clientID + "').value;\n");
				builder.Append("}\n");
			}
			builder.Append("}\n");
			
			
			string height = WindowHeight.ToString().Replace("px", "");
			string width = WindowWidth.ToString().Replace("px", "");

			builder.Append("\n");
			builder.Append("function NewItem_" + ClientID + "(){\n");
			builder.Append("	var url = '" + WebUtilities.EncodeJsString(CreateNavigateUrl()) + "';\n");
			builder.Append("	var settings = 'Location=0,Scrollbars=1,Height=" + height + ",Width=" + width + "';\n");
			builder.Append("	var win = window.open(url, 'AddItem_" + Guid.NewGuid().ToString().Replace("-", "_") + "', settings);\n");
			//builder.Append("	var url = " + CreateNavigateUrl() + "\n");
			builder.Append("}\n");
			
			builder.Append("\n");
			
			builder.Append(@"function AddItem_" + ClientID + @"(id, text){
			
							if (id == '" + Guid.Empty.ToString() + @"')
			               		alert('Cannot add item with Guid.Empty value.');
			
			 				var newOption = document.createElement('option');
		    				newOption.value = id;
		    				newOption.text = text;
		    				newOption.selected = true;
		    				
		    				// Only used while debugging. Leave commented out
		    				//alert(id + ' - ' + text);

		    				var field = document.getElementById('" + selectClientID + @"')
		    				
		    				if (field == null)
		    					alert('" + selectClientID + @" field not found.');
		    
            				//field.appendChild(newOption)
            				
            				try {
							    field.add(newOption, null); // standards compliant; doesn't work in IE
							  }
							  catch(ex) {
							    field.add(newOption); // IE only
							  }

							}
							");
			
			builder.Append("</script>\n");

			
			//if (!Page.ClientScript.IsClientScriptBlockRegistered("EntitySelectRequesterScript"))
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
			builder = AppendQueryString(builder,  EntityType + "ID=" + EntityID.ToString());
			builder = AppendQueryString(builder,  "RequesterID=" + ClientID);
			
			
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
