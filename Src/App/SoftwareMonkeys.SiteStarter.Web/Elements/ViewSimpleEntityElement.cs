using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Web.Elements
{
	/// <summary>
	///
	/// </summary>
	[Element("View", "ISimple")]
	public class ViewSimpleEntityElement : BaseElement
	{
		private ISimple dataSource;
		public ISimple DataSource
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
		
		public ViewSimpleEntityElement()
		{
		}
		
		protected override void CreateChildControls()
		{
			Table table = new Table();
			table.Width = Width;
			table.Rows.Add(new TableRow());
			table.Rows[0].Cells.Add(new TableCell());
			table.Rows[0].Cells[0].CssClass = "Heading2";
			table.Rows[0].Cells[0].Width = Unit.Percentage(100);
			table.Rows[0].Cells[0].ColumnSpan = 2;
			table.Rows[0].Cells[0].Controls.Add(new LiteralControl(HeadingText));
			
			if (DataSource != null)
			{
				table.Rows.Add(new TableRow());
				table.Rows[1].Cells.Add(new TableCell());
				table.Rows[1].Cells[0].Controls.Add(new LiteralControl(DataSource.Title));
				
				table.Rows.Add(new TableRow());
				table.Rows[2].Cells.Add(new TableCell());
				table.Rows[2].Cells[0].Controls.Add(new LiteralControl(DataSource.Description));
			}
			
			Controls.Add(table);
			
			base.CreateChildControls();
		}
	}
}
