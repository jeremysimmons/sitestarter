
using System;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web;
using System.ComponentModel;
using System.Text;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
	/// <summary>
	/// A control used to deliver a new entity from a sub form into an EntitySelect on the parent form. 
	/// This control is used in conjunction with the EntitySelectDeliverer.
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
		
		// TODO: Obsolete
		/*[Bindable(true)]
		public string EntityIDKey
		{
			get
			{
				if (ViewState["EntityIDKey"] == null)
					ViewState["EntityIDKey"] = string.Empty;
				return (string)ViewState["EntityIDKey"];
			}
			set { ViewState["EntityIDKey"] = value; }
		}*/
		
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
		
		public string SourceEntityType
		{
			get
			{
				if (ViewState["SourceEntityType"] == null)
					ViewState["SourceEntityType"] = String.Empty;
				return (string)ViewState["SourceEntityType"];
			}
			set { ViewState["SourceEntityType"] = value; }
		}
		
		public EntitySelectDeliverer()
		{
		}
		
		protected override void OnLoad(EventArgs e)
		{
			using (LogGroup logGroup = LogGroup.Start("Loading an EntitySelectDeliverer control with the ID: " + ID, NLog.LogLevel.Debug))
			{
				// Script is always registered in OnLoad so that it transfers values to the form even when not loaded on the same PageView
				if (!Page.IsPostBack
				    && Page.Request.QueryString["RequesterID"] != null)
				{
					LogWriter.Debug(@"!Page.IsPostBack && Page.Request.QueryString[""RequesterID""] != null");
					RegisterScript();
				}
				else
				{
					LogWriter.Debug("Skipping register script.");
				}
				
				base.OnLoad(e);
			}
			
		}
		
		protected override void OnPreRender(EventArgs e)
		{
			using (LogGroup logGroup = LogGroup.Start("Pre rendering the EntitySelectDeliverer with the ID: " + ID, NLog.LogLevel.Debug))
			{
				if (Page.IsPostBack)
				{
					LogWriter.Debug("Page.IsPostBack.");
					
					if (Page.Request.QueryString["AutoReturn"] != null
					    && Page.Request.QueryString["AutoReturn"].ToLower() == "true")
					{
						LogWriter.Debug("AutoReturn query string == true. Pre rendering.");
						
						RegisterReturnScript();
					}
					else
						LogWriter.Debug("AutoReturn query string != true. Skipping.");
					
				}
				else
					LogWriter.Debug("!Page.IsPostBack. Skipping.");
			}
			//else
			
			
			
			base.OnPreRender(e);
		}
		
		private void RegisterScript()
		{
			using (LogGroup logGroup = LogGroup.Start("Registering EntitySelectDeliverer script.", NLog.LogLevel.Debug))
			{
				//string url = CreateNavigateUrl();
				
				StringBuilder builder = new StringBuilder();
				
				builder.Append("<script language='javascript' defer>\n");
				builder.Append("function AcceptTransfer_" + ClientID + "(){\n");
				
				if (TransferFields != String.Empty)
				{
					LogWriter.Debug("Transfer fields: " + TransferFields);
					
					foreach (string fieldID in TransferFields.Split(','))
					{
						if (fieldID != String.Empty)
						{
							
							LogWriter.Debug("Transfer fields include: " + fieldID);
							
							Control control = WebControlUtilities.FindControlRecursive(Page, fieldID);
							if (control == null)
								throw new Exception("No control found with ID of '" + fieldID + "'.");
							string clientID = control.ClientID;
							
							builder.Append("var field = document.getElementById('" + clientID + "');\n");
							builder.Append("if (field == null)\n");
							builder.Append("alert('Field not found: " + clientID + "');\n");
							
							builder.Append("field.value = GetTransferValue_" + ClientID + "('" + fieldID + "');\n");
							
							builder.Append("\n");
						}
					}
				}
				else
					LogWriter.Debug("No transfer fields specified.");
				
				builder.Append("}\n");
				
				
				builder.Append("\n");
				builder.Append("function GetTransferValue_" + ClientID + "(id){\n");
				builder.Append("	return window.opener.GetData_" + GetRequesterID() + "(id);\n");
				builder.Append("}\n");
				
				builder.Append("</script>\n");

				
				//if (!Page.ClientScript.IsClientScriptBlockRegistered("EntitySelectDelivererScript"))
				Page.ClientScript.RegisterClientScriptBlock(typeof(EntitySelectDeliverer), "EntitySelectDelivererScript", builder.ToString());


				// Start up script

				StringBuilder startUpBuilder = new StringBuilder();
				startUpBuilder.Append("AcceptTransfer_" + ClientID + "();");
				Page.ClientScript.RegisterStartupScript(typeof(EntitySelectDeliverer), "EntitySelectDelivererStartUpScript", startUpBuilder.ToString(), true);

			}
			
		}
		
		private void RegisterReturnScript()
		{
			using (LogGroup logGroup = LogGroup.Start("Registering EntitySelectDeliverer return script.", NLog.LogLevel.Debug))
			{				
				string text = (string)WebControlUtilities.GetFieldValue(WebControlUtilities.FindControlRecursive(Page,TextControlID), "Text", typeof(String));
				
				StringBuilder builder = new StringBuilder();
				
				builder.Append("<script language='javascript' defer>\n");
				builder.Append("window.opener.AddItem_" + GetRequesterID() + "('" + EntityID + "', '" + WebUtilities.EncodeJsString(text) + "');\n");
				builder.Append("window.close();\n");
				
				
				builder.Append("</script>\n");

				
				if (!Page.ClientScript.IsClientScriptBlockRegistered("EntitySelectDelivererReturnScript"))
					Page.ClientScript.RegisterClientScriptBlock(typeof(EntitySelectDeliverer), "EntitySelectDelivererReturnScript", builder.ToString());
				else
					LogWriter.Debug("Return script already registered.");
			}
		}
		
		private string GetRequesterID()
		{
			string requesterID = string.Empty;
			using (LogGroup logGroup = LogGroup.Start("Retrieving the ID of the requester control from the query string.", NLog.LogLevel.Debug))
			{
				requesterID = Page.Request.QueryString["RequesterID"];
				
				LogWriter.Debug("Requester ID: " + requesterID);
			}
			
			return requesterID;
		}
	}
}
