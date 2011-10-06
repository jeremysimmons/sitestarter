using System;
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
		public string Action
		{
			get { return action; }
			set { action = value; }
		}
		
		private string typeName = String.Empty;
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		private string elementName = String.Empty;
		public string ElementName
		{
			get { return elementName; }
			set { elementName = value; }
		}
		
		private string message = String.Empty;
		public string Message
		{
			get { return message; }
			set { message = value; }
		}
		
		public HelloWorldElement()
		{
		}
		
		protected override void OnLoad(EventArgs e)
		{
			Controls.Add(new LiteralControl("<p>"));
			Controls.Add(new LiteralControl("Hello world!"));
			Controls.Add(new LiteralControl("<br/>"));
			if (Message != String.Empty)
				Controls.Add(new LiteralControl(Message));
			Controls.Add(new LiteralControl("</p>"));
			
			base.OnLoad(e);
		}
	}
}
