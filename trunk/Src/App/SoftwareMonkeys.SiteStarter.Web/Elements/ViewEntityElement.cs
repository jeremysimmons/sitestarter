using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Web.Projections;

namespace SoftwareMonkeys.SiteStarter.Web.Elements
{
	/// <summary>
	///
	/// </summary>
	[Element("View", "IEntity")]
	public class ViewEntityElement : BaseElement
	{
		private IEntity dataSource;
		public IEntity DataSource
		{
			get { return dataSource; }
			set { dataSource = value; }
		}
		
		private string headingText = String.Empty;
		public string HeadingText
		{
			get {
				if (headingText == String.Empty && DataSource != null)
					headingText = DataSource.ShortTypeName;
				return headingText; }
			set { headingText = value; }
		}
		
		public bool EnableViewLink
		{
			get {
				if (ViewState["EnableViewLink"] == null)
				{
					if (DataSource == null)
						return false;
					else
						return ProjectionState.Projections.Contains("View", DataSource);
				}
				else
					return (bool)ViewState["EnableViewLink"];
			}
			set { ViewState["EnableViewLink"] = value; }
		}
		
		public ViewEntityElement()
		{
		}
		
		protected override void CreateChildControls()
		{
			Table table = new Table();
			table.Width = Width;
			table.CssClass = "Panel";
			table.Rows.Add(new TableRow());
			table.Rows[0].Cells.Add(new TableCell());
			table.Rows[0].Cells[0].CssClass = "Heading2";
			table.Rows[0].Cells[0].Width = Unit.Percentage(100);
			table.Rows[0].Cells[0].ColumnSpan = 2;
			table.Rows[0].Cells[0].Controls.Add(new LiteralControl(HeadingText));
			
			table.Rows.Add(new TableRow());
			table.Rows[1].Cells.Add(new TableCell());
			if (DataSource != null)
			{
				Control control = GetTitleControl(DataSource.ToString());
				table.Rows[1].Cells[0].Controls.Add(control);
			}
			
			Controls.Add(table);
			
			base.CreateChildControls();
		}
		
		protected virtual Control GetTitleControl(string titleText)
		{
			if (EnableViewLink)
			{
				HyperLink link = new HyperLink();
				link.Text = titleText;
				
				link.NavigateUrl = new UrlCreator().CreateUrl("View", DataSource.ShortTypeName);
				
				return link;
			}
			else
				return new LiteralControl(titleText);
			
		}
	}
}
