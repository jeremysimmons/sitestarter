using System;
using System.Web.UI.WebControls;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Web.Properties;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
	/// <summary>
	/// Displays a drop down list for selecting a command (ie. a combination of "action" and "type").
	/// </summary>
	public class CommandSelect : WebControl
	{
		public DropDownList DropDownList = null;
		public Label TextLabel = null;
		
		private string commands = String.Empty;
		/// <summary>
		/// Gets/sets a commmands string. Example "User Index|Create User".
		/// </summary>
		public string Commands
		{
			get { return commands; }
			set { commands = value; }
		}
		
		public string SelectedCommand
		{
			get
			{
				return DropDownList.SelectedValue;
			}
			set
			{
				foreach (ListItem item in DropDownList.Items)
				{
					item.Selected = (item.Value == value);
				}
			}
		}
		
		private string text = Language.AndGoTo;
		/// <summary>
		/// Gets/sets the text displayed next to the drop down list.
		/// </summary>
		public string Text
		{
			get { return text; }
			set { text = value;
				TextLabel.Text = value;
				if (value.Trim() != String.Empty)
				    TextLabel.Text += " ";
			}
		}
				
		public CommandSelect()
		{
			CssClass = "CommandSelect";
		}
		
		protected override void OnInit(EventArgs e)
		{
			TextLabel = new Label();
			Controls.Add(TextLabel);
			TextLabel.Text = Text + " ";
			
			DropDownList = new DropDownList();
			Controls.Add(DropDownList);			
			
			string[] items = Commands.Split('|');
			
			foreach (String item in items)
			{				
				DropDownList.Items.Add(new ListItem(item, item));
			}
			
			base.OnInit(e);
		}
	}
}
