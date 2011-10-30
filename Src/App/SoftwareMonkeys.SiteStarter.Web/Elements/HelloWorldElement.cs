using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SoftwareMonkeys.SiteStarter.Web.Elements
{
	/// <summary>
	/// Description of HelloWorldElement.
	/// </summary>
	[ElementAttribute("HelloWorld")]
	public class HelloWorldElement : WebControl, IElement
	{
		private string action = String.Empty;
		[Bindable(true)]
		public string Action
		{
			get { return action; }
			set { action = value; }
		}
		
		private string typeName = String.Empty;
		[Bindable(true)]
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		private string elementName = String.Empty;
		[Bindable(true)]
		public string ElementName
		{
			get { return elementName; }
			set { elementName = value; }
		}
		
		private string message = String.Empty;
		[Bindable(true)]
		public string Message
		{
			get { return message; }
			set {
				MessageLabel.Text = value;
				message = value; }
		}
		
		Label MessageLabel = new Label();
		
		public HelloWorldElement()
		{
		}
		
		protected override void OnLoad(EventArgs e)
		{
			Controls.Add(new LiteralControl("<p>"));
			Controls.Add(new LiteralControl("Hello world!"));
			Controls.Add(new LiteralControl("<br/>"));
			if (Message != String.Empty)
				Controls.Add(MessageLabel);
			Controls.Add(new LiteralControl("</p>"));
			
			base.OnLoad(e);
		}
	}
}
