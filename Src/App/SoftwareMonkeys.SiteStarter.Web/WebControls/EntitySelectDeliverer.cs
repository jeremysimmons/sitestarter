
using System;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web;
using System.ComponentModel;
using System.Text;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
	/// <summary>
	/// Description of EntitySelectRequester.
	/// </summary>
	public class EntitySelectDeliverer : WebControl
	{
		public string TextControlID
		{
			get
			{
				if (ViewState["TextControlID"] == null)
					ViewState["TextControlID"] = String.Empty;
				return (string)ViewState["TextControlID"];
			}
			set { ViewState["TextControlID"] = value; }
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
		
		public Guid RequesterEntityID
		{
			get
			{
				if (ViewState["RequesterEntityID"] == null)
					ViewState["RequesterEntityID"] = Guid.Empty;
				return (Guid)ViewState["RequesterEntityID"];
			}
			set { ViewState["RequesterEntityID"] = value; }
		}
		
		
		public string TransferFields
		{
			get
			{
				if (ViewState["TransferFields"] == null)
					ViewState["TransferFields"] = String.Empty;
				return (string)ViewState["TransferFields"];
			}
			set { ViewState["TransferFields"] = value; }
		}
		
		public EntitySelectDeliverer()
		{
		}
		
		protected override void OnInit(EventArgs e)
		{
			/*HyperLink link = new HyperLink();
			link.Text = this.Text;
			link.NavigateUrl = CreateNavigateUrl();
			
			Controls.Add(link);
			 */
			
			if (Page.Request.QueryString["EntityID"] != String.Empty)
				RequesterEntityID = new Guid(Page.Request.QueryString["EntityID"]);
			
			base.OnInit(e);
		}
		
		protected override void OnPreRender(EventArgs e)
		{
			
			if (Page.IsPostBack)
			{
				if (Page.Request.QueryString["AutoReturn"] != null
				    && Page.Request.QueryString["AutoReturn"].ToLower() == "true")
				{
					RegisterReturnScript();
				}
				
			}
			else
				RegisterScript();
			
			
			
			base.OnPreRender(e);
		}
		
		private void RegisterScript()
		{
			//string url = CreateNavigateUrl();
			
			StringBuilder builder = new StringBuilder();
			
			builder.Append("<script language='javascript' defer>\n");
			builder.Append("function AcceptTransfer(){\n");
			
			if (TransferFields != String.Empty)
			{
				foreach (string fieldID in TransferFields.Split(','))
				{
					if (fieldID != String.Empty)
					{
						Control control = WebControlUtilities.FindControlRecursive(Page, fieldID);
						if (control == null)
							throw new Exception("No control found with ID of '" + fieldID + "'.");
						string clientID = control.ClientID;
						
						builder.Append("var field = document.getElementById('" + clientID + "');\n");
						builder.Append("if (field == null)\n");
						builder.Append("alert('Field not found: " + clientID + "');\n");
						
						builder.Append("field.value = GetTransferValue('" + fieldID + "');\n");
						
						builder.Append("\n");
					}
				}
			}
			
			builder.Append("}\n");
			
			
			builder.Append("\n");
			builder.Append("function GetTransferValue(id){\n");
			builder.Append("	return window.opener.GetData(id);\n");
			builder.Append("}\n");
			
			builder.Append("AcceptTransfer();");
			
			builder.Append("</script>\n");

			
			if (!Page.ClientScript.IsClientScriptBlockRegistered("EntitySelectDelivererScript"))
				Page.ClientScript.RegisterClientScriptBlock(typeof(EntitySelectDeliverer), "EntitySelectDelivererScript", builder.ToString());
		}
		
		private void RegisterReturnScript()
		{
			//string url = CreateNavigateUrl();
			
			string text = (string)WebControlUtilities.GetFieldValue(WebControlUtilities.FindControlRecursive(Page,TextControlID), "Text", typeof(String));
			
			StringBuilder builder = new StringBuilder();
			
			builder.Append("<script language='javascript' defer>\n");
			builder.Append("window.opener.AddItem('" + EntityID + "', '" + WebUtilities.EncodeJsString(text) + "');\n");
			builder.Append("window.close();\n");
			
			
			builder.Append("</script>\n");

			
			if (!Page.ClientScript.IsClientScriptBlockRegistered("EntitySelectDelivererReturnScript"))
				Page.ClientScript.RegisterClientScriptBlock(typeof(EntitySelectDeliverer), "EntitySelectDelivererReturnScript", builder.ToString());
		}
	}
}
