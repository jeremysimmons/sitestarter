using System;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
	/// <summary>
	/// 
	/// </summary>
	public class DeleteLink : HyperLink
	{
		[Bindable(true)]
		public string ConfirmMessage
		{
			get { return (string)ViewState["ConfirmMessage"]; }
			set { ViewState["ConfirmMessage"] = value; }
		}
		
		public DeleteLink()
		{
		}
		
		protected override void OnLoad(EventArgs e)
		{
			string script = "return confirm('" + Escape(ConfirmMessage) + "');";
			
			Attributes.Add("onclick", script);
			
			base.OnLoad(e);
		}
		
		private string Escape(string input)
		{
			return WebUtilities.EncodeJsString(input);
		}
	}
}
