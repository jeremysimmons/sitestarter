using System;
using System.Web.UI.WebControls;
using System.Web.UI;
using SoftwareMonkeys.SiteStarter.Web.Properties;
using SoftwareMonkeys.SiteStarter.Business;
using System.ComponentModel;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
	/// <summary>
	/// A form used to contact a specific user.
	/// </summary>
	public class ContactForm : WebControl
	{
		public Table FormTable;
		public TextBox SubjectField;
		public TextBox MessageField;
		public Button SendButton;
		
		[Browsable(true)]
		[Bindable(true)]
		public string Subject
		{
			get {
				if (ViewState["Subject"] == null)
					ViewState["Subject"] = String.Empty;
				return (string)ViewState["Subject"];
			}
			set { ViewState["Subject"] = value; }
		}
		
		[Browsable(true)]
		[Bindable(true)]
		public string Message
		{
			get {
				if (ViewState["Message"] == null)
					ViewState["Message"] = String.Empty;
				return (string)ViewState["Message"];
			}
			set { ViewState["Message"] = value; }
		}
		
		[Browsable(true)]
		[Bindable(true)]
		public string ToName
		{
			get {
				if (ViewState["ToName"] == null)
					ViewState["ToName"] = String.Empty;
				return (string)ViewState["ToName"];
			}
			set { ViewState["ToName"] = value; }
		}
		
		[Browsable(true)]
		[Bindable(true)]
		public string ToEmail
		{
			get {
				if (ViewState["ToEmail"] == null)
					ViewState["ToName"] = String.Empty;
				return (string)ViewState["ToEmail"];
			}
			set { ViewState["ToEmail"] = value; }
		}
		
		[Browsable(true)]
		[Bindable(true)]
		public string FromName
		{
			get {
				if (ViewState["FromName"] == null)
					ViewState["FromName"] = String.Empty;
				return (string)ViewState["FromName"];
			}
			set { ViewState["FromName"] = value; }
		}
		
		[Browsable(true)]
		[Bindable(true)]
		public string FromEmail
		{
			get {
				if (ViewState["FromEmail"] == null)
					ViewState["FromEmail"] = String.Empty;
				return (string)ViewState["FromEmail"];
			}
			set { ViewState["FromEmail"] = value; }
		}
		
		[Browsable(true)]
		[Bindable(true)]
		public string HeaderCssClass
		{
			get {
				if (ViewState["HeaderCssClass"] == null)
					ViewState["HeaderCssClass"] = String.Empty;
				return (string)ViewState["HeaderCssClass"];
			}
			set { ViewState["HeaderCssClass"] = value; }
		}
		
		[Browsable(true)]
		[Bindable(true)]
		public string HeaderText
		{
			get {
				if (ViewState["HeaderText"] == null)
					ViewState["HeaderText"] = "&nbsp;";
				return (string)ViewState["HeaderText"];
			}
			set { ViewState["HeaderText"] = value; }
		}
		
		[Browsable(true)]
		[Bindable(true)]
		public string LabelCssClass
		{
			get {
				if (ViewState["LabelCssClass"] == null)
					ViewState["LabelCssClass"] = "Label";
				return (string)ViewState["LabelCssClass"];
			}
			set { ViewState["LabelCssClass"] = value; }
		}
		
		[Browsable(true)]
		[Bindable(true)]
		public string FieldContainerCssClass
		{
			get {
				if (ViewState["FieldContainerCssClass"] == null)
					ViewState["FieldContainerCssClass"] = "Field";
				return (string)ViewState["FieldContainerCssClass"];
			}
			set { ViewState["FieldContainerCssClass"] = value; }
		}
		
		[Browsable(true)]
		[Bindable(true)]
		public string FieldCssClass
		{
			get {
				if (ViewState["FieldCssClass"] == null)
					ViewState["FieldCssClass"] = "Field";
				return (string)ViewState["FieldCssClass"];
			}
			set { ViewState["FieldCssClass"] = value; }
		}
		
		[Browsable(true)]
		[Bindable(true)]
		public string SuccessMessage
		{
			get {
				if (ViewState["SuccessMessage"] == null)
					ViewState["SuccessMessage"] = String.Empty;
				return (string)ViewState["SuccessMessage"];
			}
			set { ViewState["SuccessMessage"] = value; }
		}
		
		[Browsable(true)]
		[Bindable(true)]
		public string NavigateUrl
		{
			get {
				if (ViewState["NavigateUrl"] == null)
					ViewState["NavigateUrl"] = String.Empty;
				return (string)ViewState["NavigateUrl"];
			}
			set { ViewState["NavigateUrl"] = value; }
		}
		
		public ContactForm()
		{
		}
		
		protected override void OnInit(EventArgs e)
		{
			
			FormTable = new Table();
			
			FormTable.Width = Unit.Percentage(100);
			
			SubjectField = new TextBox();
			SubjectField.Width = Unit.Pixel(400);
			
			MessageField = new TextBox();
			MessageField.Width = Unit.Pixel(400);
			MessageField.Rows = 10;
			MessageField.TextMode = TextBoxMode.MultiLine;
			
			SendButton = new Button();
			SendButton.Text = Language.Send;
			SendButton.Click += new EventHandler(SendButton_Click);
			
			// Header
			FormTable.Rows.Add(new TableRow());
			FormTable.Rows[0].Cells.Add(new TableCell());
			FormTable.Rows[0].Cells[0].ColumnSpan = 2;
			
			// To
			FormTable.Rows.Add(new TableRow());
			FormTable.Rows[1].Cells.Add(new TableCell());
			FormTable.Rows[1].Cells[0].Controls.Add(new LiteralControl(Language.To + ":"));
			FormTable.Rows[1].Cells.Add(new TableCell());
			
			// Subject
			FormTable.Rows.Add(new TableRow());
			FormTable.Rows[2].Cells.Add(new TableCell());
			FormTable.Rows[2].Cells[0].Controls.Add(new LiteralControl(Language.Subject + ":"));
			FormTable.Rows[2].Cells.Add(new TableCell());
			FormTable.Rows[2].Cells[1].Controls.Add(SubjectField);
			
			// Message
			FormTable.Rows.Add(new TableRow());
			FormTable.Rows[3].Cells.Add(new TableCell());
			FormTable.Rows[3].Cells[0].Controls.Add(new LiteralControl(Language.Message + ":"));
			FormTable.Rows[3].Cells.Add(new TableCell());
			FormTable.Rows[3].Cells[1].Controls.Add(MessageField);
			
			// Button
			FormTable.Rows.Add(new TableRow());
			FormTable.Rows[4].Cells.Add(new TableCell());
			FormTable.Rows[4].Cells[0].Controls.Add(new LiteralControl("&nbsp;"));
			FormTable.Rows[4].Cells.Add(new TableCell());
			//FormTable.Rows[3].Cells[1].CssClass = FieldContainerCssClass;
			FormTable.Rows[4].Cells[1].Controls.Add(SendButton);
			
			Controls.Add(FormTable);
			
			
			base.OnInit(e);
		}
		
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			
			if (!Page.IsPostBack)
			{
				SubjectField.Text = Subject;
				MessageField.Text = Message;
			}
			SubjectField.CssClass = FieldCssClass;
			MessageField.CssClass = FieldCssClass;
			
			FormTable.Rows[0].Cells[0].CssClass = HeaderCssClass;
			FormTable.Rows[0].Cells[0].Controls.Add(new LiteralControl(HeaderText));
			FormTable.Rows[1].Cells[1].CssClass = FieldContainerCssClass;
			FormTable.Rows[1].Cells[0].CssClass = LabelCssClass;
			FormTable.Rows[2].Cells[1].CssClass = FieldContainerCssClass;
			FormTable.Rows[3].Cells[1].CssClass = FieldContainerCssClass;
			FormTable.Rows[4].Cells[0].CssClass = LabelCssClass;
			FormTable.Rows[3].Cells[0].CssClass = LabelCssClass;
			FormTable.Rows[2].Cells[0].CssClass = LabelCssClass;
			
			FormTable.Rows[1].Cells[1].Controls.Add(new LiteralControl(ToName));
			MessageField.Text = Message;
		}

		void SendButton_Click(object sender, EventArgs e)
		{
			SendEmailStrategy.New().SendEmail(SubjectField.Text, MessageField.Text, FromName, FromEmail, ToName, ToEmail);
			
			if (SuccessMessage != String.Empty)
				Result.Display(SuccessMessage);
			
			if (NavigateUrl != String.Empty)
				Page.Response.Redirect(NavigateUrl);
		}
	}
}
