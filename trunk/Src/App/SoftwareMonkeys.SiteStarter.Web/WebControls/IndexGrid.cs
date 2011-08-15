using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using SoftwareMonkeys.SiteStarter.Entities;
using System.ComponentModel;
using System.Collections;
using SoftwareMonkeys.SiteStarter.Web.Properties;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Text.RegularExpressions;
using System.Web;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
	public class IndexGrid : DataGrid
	{
		#region Sorting
		[Obsolete("Use SortCommand event.")]
		public event EventHandler SortChanged;

		[Obsolete("Use RaiseSortCommand event.")]
		protected void RaiseSortChanged()
		{
			if (SortChanged != null)
				SortChanged(this, EventArgs.Empty);
		}
		
		protected override void OnSortCommand(DataGridSortCommandEventArgs e)
		{
			// Raise the obsolete event in case it's still in use
			RaiseSortChanged();
			
			base.OnSortCommand(e);
		}

		/// <summary>
		/// Selects the appropriate sort item.
		/// </summary>
		private void SelectSortItem()
		{
			using (LogGroup logGroup = LogGroup.Start("Selecting the current sort item.", NLog.LogLevel.Debug))
			{
				if (Sort != null)
				{
					LogWriter.Debug("Current sort: " + CurrentSort);
					LogWriter.Debug("Default sort: " + DefaultSort);
					if (CurrentSort != null && CurrentSort.Trim().Length > 0)
					{
						LogWriter.Debug("Using CurrentSort");
						Sort.SelectedIndex = Sort.Items.IndexOf(Sort.Items.FindByValue(CurrentSort));
					}
					else
					{
						LogWriter.Debug("Using DefaultSort");
						Sort.SelectedIndex = Sort.Items.IndexOf(Sort.Items.FindByValue(DefaultSort));
					}
				}
				else
					LogWriter.Debug("Sort control is null");
			}
		}

		/// <summary>
		/// Clears all the sort items.
		/// </summary>
		public void ClearSortItems()
		{
			Sort.Items.Clear();
		}
		
		/// <summary>
		/// Adds an item for the provided property.
		/// </summary>
		/// <param name="propertyText">The text of the property to add.</param>
		/// <param name="propertyName">The name of the item to add.</param>
		public void AddDualSortItem(string propertyText, string propertyName)
		{
			AddSortItem(propertyText + " " + Language.Asc, propertyName + "Ascending");
			AddSortItem(propertyText + " " + Language.Desc, propertyName + "Descending");
		}

		/// <summary>
		/// Adds an item with the provided text and value to the sort dropdown.
		/// </summary>
		/// <param name="text">The text of the item to add.</param>
		/// <param name="value">The value of the item to add.</param>
		public void AddSortItem(string text, string value)
		{
			Sort.Items.Add(new ListItem(text, value));
		}

		/// <summary>
		/// The sort control.
		/// </summary>
		public DropDownList Sort;

		/// <summary>
		/// Gets/sets the default sort expression to use when displaying the index.
		/// </summary>
		[Bindable(true)]
		[Browsable(true)]
		public string DefaultSort
		{
			get { return (string)ViewState["DefaultSort"]; }
			set
			{
				ViewState["DefaultSort"] = value;
				// TODO: Clean up
				//if (ViewState["CurrentSort"] == null || ViewState["CurrentSort"] == String.Empty)
				//	CurrentSort = value;
			}
		}

		/// <summary>
		/// Gets/sets the default sort expression to use when displaying the index.
		/// </summary>
		public string CurrentSort
		{
			get {
				// TODO: Clean up
				// Check if value from drop down list should be set to CurrentSort upon post back
				//if (Sort != null)
				//	ViewState["CurrentSort"] = Sort.SelectedValue;
				//else{
				if (ViewState["CurrentSort"] == null || (string)ViewState["CurrentSort"] == String.Empty)
					ViewState["CurrentSort"] = DefaultSort;
				//}
				
				return (string)ViewState["CurrentSort"]; }
			set
			{
				ViewState["CurrentSort"] = value;
				if (Sort != null)
					SelectSortItem();
			}
		}
		#endregion

		
		public string ItemMouseOverCssClass
		{
			get {
				if (ViewState["ItemMouseOverCssClass"] == null)
					ViewState["ItemMouseOverCssClass"] = "ListItemOver";
				return (string)ViewState["ItemMouseOverCssClass"];
			}
			set { ViewState["ItemMouseOverCssClass"] = value; }
		}
		
		public string ItemMouseOutCssClass
		{
			get {
				if (ViewState["ItemMouseOutCssClass"] == null)
					ViewState["ItemMouseOutCssClass"] = "ListItem";
				return (string)ViewState["ItemMouseOutCssClass"];
			}
			set { ViewState["ItemMouseOutCssClass"] = value; }
		}
		
		public string NavigateUrl
		{
			get {
				if (ViewState["NavigateUrl"] == null)
					ViewState["NavigateUrl"] = String.Empty;
				return (string)ViewState["NavigateUrl"];
			}
			set { ViewState["NavigateUrl"] = value; }
		}

		private int expandedCount = 0;
		/// <summary>
		/// Gets/sets the number of expanded IDs used.
		/// </summary>
		protected int ExpandedCount
		{
			get { return expandedCount; }
			set { expandedCount = value; }
		}

		public PlaceHolder CustomHolder;

		/// <summary>
		/// Gets/sets a value indicating whether to display the sort control.
		/// </summary>
		[Bindable(true)]
		[Browsable(true)]
		public bool ShowSort
		{
			get
			{
				if (ViewState["ShowSort"] == null)
					ViewState["ShowSort"] = true;
				return (bool)ViewState["ShowSort"];
			}
			set
			{
				ViewState["ShowSort"] = value;
			}
		}

		/// <summary>
		/// Gets/sets the heading text for the grid.
		/// </summary>
		[Bindable(true)]
		[Browsable(true)]
		public string HeaderText
		{
			get
			{
				if (ViewState["HeaderText"] == null)
					ViewState["HeaderText"] = String.Empty;
				return (string)ViewState["HeaderText"];
			}
			set
			{
				ViewState["HeaderText"] = value;
			}
		}

		/// <summary>
		/// Gets/sets the Entity data required for this control.
		/// </summary>
		[Browsable(false)]
		public new IEntity[] DataSource
		{
			get
			{return (IEntity[])base.DataSource;
			}
			set { base.DataSource = value;}
		}

		/// <summary>
		/// Gets/sets a boolean value indicating whether the grid is displaying the first page.
		/// </summary>
		public bool IsFirstPage
		{
			get { return CurrentPageIndex == 0; }
		}

		/// <summary>
		/// Gets/sets a boolean value indicating whether the grid is displaying the last page.
		/// </summary>
		public bool IsLastPage
		{
			get { return CurrentPageIndex == PageCount-1; }
		}

		/// <summary>
		/// Gets an array of IDs of records selected by the user.
		/// </summary>
		public Guid[] SelectedDataKeys
		{
			get
			{
				ArrayList keys = new ArrayList();

				foreach (DataGridItem item in this.Items)
				{
					object control = item.FindControl("SelectRecord");

					if (control != null)
					{
						CheckBox selectRecord = (CheckBox)control;

						if (selectRecord.Checked)
							keys.Add(new Guid(DataKeys[item.ItemIndex].ToString()));
					}
				}

				return (Guid[])keys.ToArray(typeof(Guid));
			}
		}
		
		// This one is obsolete
		/// <summary>
		/// Gets/sets the text displayed when the data is empty.
		/// </summary>
		public string EmptyDataText // TODO: Finish adding support for this property
		{
			get
			{
				return NoDataText;
			}
			set { NoDataText = value; }
		}
		
		/// <summary>
		/// Gets/sets the text displayed when the data is empty.
		/// </summary>
		public string NoDataText
		{
			get
			{
				if (ViewState["NoDataText"] == null)
					ViewState["NoDataText"] = String.Empty;
				return (string)ViewState["NoDataText"];
			}
			set { ViewState["NoDataText"] = value; }
		}
		
		
		/// <summary>
		/// Gets/sets the CSS class of the text displayed when the data is empty.
		/// </summary>
		public string NoDataTextCssClass
		{
			get
			{
				if (ViewState["NoDataTextCssClass"] == null)
					ViewState["NoDataTextCssClass"] = "NoDataText";
				return (string)ViewState["NoDataTextCssClass"];
			}
			set { ViewState["NoDataTextCssClass"] = value; }
		}
		

		protected override void OnInit(EventArgs e)
		{
			using (LogGroup logGroup = LogGroup.Start("Initializing IndexGrid control: " + ID, NLog.LogLevel.Debug))
			{
				if (Page.Request.QueryString["Page"] != String.Empty)
					CurrentPageIndex = QueryStrings.PageIndex;
				
				if (Page.Request.QueryString["Sort"] != String.Empty)
					CurrentSort = QueryStrings.Sort;
				
				
				CssClass = "BodyPanel";
				AutoGenerateColumns = false;
				Width = Unit.Percentage(100);
				CellPadding = 0;
				GridLines = GridLines.None;
				ItemStyle.CssClass = "ListItem";
				AlternatingItemStyle.CssClass = "ListItem";
				PagerStyle.Visible = true;
				PagerStyle.HorizontalAlign = HorizontalAlign.Right;
				PagerStyle.Mode = PagerMode.NumericPages;
				PagerStyle.Position = PagerPosition.TopAndBottom;
				ShowFooter = true;
				AllowCustomPaging = true;
				
				Sort = new DropDownList();
				Sort.ID = "Sort";
				Sort.AutoPostBack = true;
				Sort.CssClass = "Field";
				Sort.SelectedIndexChanged += new EventHandler(Sort_SelectedIndexChanged);
				
				// If post back then
				if (!Page.IsPostBack)
				{
					SelectSortItem();
				}

				LogWriter.Debug("Default sort: " + DefaultSort);
				LogWriter.Debug("Current sort: " + CurrentSort);
				
				CustomHolder = new PlaceHolder();
				CustomHolder.ID = "CustomHolder";				
			}
			
			base.OnInit(e);

		}

		protected override void OnLoad(EventArgs e)
		{
			using (LogGroup logGroup = LogGroup.Start("Initializing IndexGrid control: " + ID, NLog.LogLevel.Debug))
			{

				
				base.OnLoad(e);
				
				// If no sort items were added then hide the sort list
				if (Sort.Items.Count == 0)
					Sort.Visible = false;
			}
		}
		void Sort_SelectedIndexChanged(object sender, EventArgs e)
		{
			HttpContext.Current.Response.Redirect(CompileNavigateUrl(CurrentPageIndex, Sort.SelectedValue));
		}

		protected override void OnItemCreated(DataGridItemEventArgs e)
		{
			using (LogGroup logGroup = LogGroup.Start("Customizing a grid item upon creation (ItemCreated event).", NLog.LogLevel.Debug))
			{
				if (e.Item.ItemType == ListItemType.Pager)
				{
					CustomizePager(e);
				}
				else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
				{
					CustomizeItem(e);
				}
				else if (e.Item.ItemType == ListItemType.Header)
				{
					CustomizeHeader(e);
				}
				else if (e.Item.ItemType == ListItemType.Footer)
				{
					CustomizeFooter(e);
				}
			}

			base.OnItemCreated(e);
		}
		
		protected void CustomizeHeader(DataGridItemEventArgs e)
		{
			using (LogGroup logGroup = LogGroup.Start("Customizing a header item.", NLog.LogLevel.Debug))
			{
				
				// Reduce to a single cell in the header
				e.Item.Cells[0].ColumnSpan = 1;
				while (e.Item.Cells.Count > 1)
				{
					e.Item.Cells.Remove(e.Item.Cells[1]);
					e.Item.Cells[0].ColumnSpan++;
				}

				if (e.Item.Cells[0].Controls.Count > 0)
					e.Item.Cells[0].Controls.RemoveAt(0);

				Panel panel = CreateHeaderPanel();
				
				e.Item.Cells[0].Controls.Add(panel);
				e.Item.Cells[0].Style.Add("padding", "0px");
				
				
			}
		}
		
		private Panel CreateHeaderPanel()
		{
			Panel panel = new Panel();
			panel.CssClass = "Heading2";
			panel.Style.Add("clear", "both");
			panel.Controls.Add(CreateTitlePanel());
			panel.Controls.Add(CreateRightPanel());
			
			return panel;

		}
		
		private Panel CreateTitlePanel()
		{
			
			Panel panel = new Panel();
			panel.CssClass = "Heading2Left";
			panel.Controls.Add(new LiteralControl(HeaderText));
			panel.Style.Add("float", "left");
			
			return panel;
		}
		
		private Panel CreateRightPanel()
		{
			// Outer panel
			Panel panel = new Panel();
			panel.CssClass = "Heading2Right";
			panel.Style.Add("float", "right");
			panel.HorizontalAlign = HorizontalAlign.Right;
			
			// Paging panel
			PlaceHolder pagingPanel = CreatePagingPanel();

			// Sort panel
			PlaceHolder sortPanel = new PlaceHolder();
			
			if (ShowSort)
			{
				if (Sort != null)
					panel.Controls.Add(Sort);
			}
			
			panel.Controls.Add(sortPanel);
			panel.Controls.Add(pagingPanel);
			
			return panel;
		}
		
		/*
		private Table CreateHeaderTable()
		{
			// Add the table
			Table table = new Table();
			table.ID = "HeaderTable";
			table.Width = Unit.Percentage(100);
			table.Rows.Add(new TableRow());


			// Create cell 1
			TableCell cell1 = new TableCell();
			cell1.Width = Unit.Percentage(80);
			cell1.ID = "HeaderCell";
			cell1.Text = HeaderText;
			//cell1.CssClass = "";

			// Create cell 2
			TableCell cell2 = new TableCell();
			cell2.ID = "CustomCell";
			if (CustomHolder != null)
				cell2.Controls.Add(CustomHolder);
			cell2.CssClass = "CustomContainer";
			cell2.Wrap = false;
			cell2.HorizontalAlign = HorizontalAlign.Right;

			// Create cell 3
			TableCell cell3 = new TableCell();
			cell3.Width = Unit.Percentage(50);
			cell3.HorizontalAlign = HorizontalAlign.Right;
			cell3.CssClass = "SortContainer";
			cell3.ID = "SortCell";

			if (ShowSort)
			{
				//cell3.Controls.Add(new LiteralControl(TextHelper.Get("SortLabel") + "&nbsp;"));
				if (Sort != null)
					cell3.Controls.Add(Sort);
			}

			// Create cell 4
			TableCell cell4 = CreatePagingCell();
			cell4.Width = Unit.Percentage(50);
			cell4.HorizontalAlign = HorizontalAlign.Right;
			cell4.CssClass = "PagingContainer";
			cell4.ID = "PagingCell1";
			cell4.Wrap = false;
			cell4.Visible = AllowPaging && DataSource != null && DataSource.Length > 0;

			table.Rows[0].Cells.Add(cell1);
			table.Rows[0].Cells.Add(cell2);
			table.Rows[0].Cells.Add(cell3);
			table.Rows[0].Cells.Add(cell4);
			
			return table;
		}*/

		protected void CustomizeFooter(DataGridItemEventArgs e)
		{
			
			using (LogGroup logGroup = LogGroup.Start("Customizing a footer item.", NLog.LogLevel.Debug))
			{
				e.Item.Visible = ShowFooter;
				
				if (ShowFooter)
				{
					// Reduce to a single cell in the header
					e.Item.Cells[0].ColumnSpan = 1;
					while (e.Item.Cells.Count > 1)
					{
						e.Item.Cells.Remove(e.Item.Cells[1]);
						e.Item.Cells[0].ColumnSpan++;
					}

					if (e.Item.Cells[0].Controls.Count > 0)
						e.Item.Cells[0].Controls.RemoveAt(0);
					
					e.Item.Cells[0].HorizontalAlign = HorizontalAlign.Right;

					e.Item.Visible = ShowFooter;
					
					e.Item.Cells[0].Controls.Add(CreatePagingPanel());
				}
			}
		}
		
		protected void CustomizePager(DataGridItemEventArgs e)
		{
			
			using (LogGroup logGroup = LogGroup.Start("Customizing a pager item.", NLog.LogLevel.Debug))
			{
				e.Item.Cells[0].Controls.Clear();			
				

				e.Item.Cells.RemoveAt(0);
			}
		}
		
		protected void CustomizeItem(DataGridItemEventArgs e)
		{
			
			using (LogGroup logGroup = LogGroup.Start("Customizing a general or alternating item.", NLog.LogLevel.Debug))
			{
				e.Item.Attributes.Add("onmouseover", "this.className='" + ItemMouseOverCssClass + "';");
				e.Item.Attributes.Add("onmouseout", "this.className='" + ItemMouseOutCssClass + "';");
				
				// TODO: Check if expansion is needed
				//e.Item.Attributes.Add("onclick", (EnableExpansion ? "ToggleExpansion('" + ClientID + "', " + e.Item.ItemIndex + ")" : String.Empty));
			}
		}

		protected override void OnPageIndexChanged(DataGridPageChangedEventArgs e)
		{
			CurrentPageIndex = e.NewPageIndex;
			
			base.OnPageIndexChanged (e);
		}		
		
		private void InitializeNoDataText()
		{
			Controls.Clear();
			
			LogWriter.Debug("DataSource is empty. Adding [NoDataText] to control.");
			
			Table table = new Table();
			
			Controls.Add(table);
			
			// Header row
			DataGridItem headerRow = new DataGridItem(Items.Count, 0, ListItemType.Header);
			table.Rows.Add(headerRow);
			
			// Header cell
			headerRow.Cells.Add(new TableCell());
			headerRow.Cells[0].Controls.Add(CreateHeaderPanel());
			headerRow.CssClass = "Heading2";
			
			// Text cell
			TableCell textCell = new TableCell();
			textCell.Controls.Add(new LiteralControl("<div class='" + NoDataTextCssClass + "' nowrap>" + NoDataText + "</div>"));
			
			DataGridItem textRow = new DataGridItem(Items.Count, 0, ListItemType.Item);
			
			textRow.Cells.Add(textCell);
			
			table.Rows.Add(textRow);
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (DataSource == null || DataSource.Length == 0)
			{
				InitializeNoDataText();
			}
			else
			{
				LogWriter.Debug("DataSource is not empty.");
				LogWriter.Debug("DataSource.Length: " + DataSource.Length);
			}
			
			if (DataSource == null || DataSource.Length == 0)
			{
				ShowFooter = true;
			}

			if (PageCount <= 1)
				PagerStyle.Visible = false;
			
			base.OnPreRender(e);
		}
		
		private string AddPageQueryString(string url, int pageIndex)
		{
			string separator = "?";
			if (url.IndexOf("?") > -1)
				separator = "&";
			
			// Convert index into number. Index is 0 based. Number is 1 based.
			// Conversion is a usability measure
			int pageNumber = pageIndex+1;
			
			return url + separator + "Page=" + pageNumber.ToString();
		}
		
		
		private string AddSortQueryString(string url, string sortExpression)
		{
			string separator = "?";
			if (url.IndexOf("?") > -1)
				separator = "&";
			
			return url + separator + "Sort=" + sortExpression;
		}
		
		private string CreateDefaultNavigateUrl()
		{
			
			string url = HttpContext.Current.Request.Url.ToString();
			
			// If currently using a projection then the default path is to the same projection and the same action
			if (QueryStrings.Action != String.Empty && QueryStrings.Type != String.Empty)
				url = UrlCreator.Current.CreateUrl(QueryStrings.Action, QueryStrings.Type);
			
			// Remove the page query string. The new one gets added as needed.
			string pageKey = "Page";
			Regex pageRegex = new Regex("[?&]" + pageKey + "(?:=([^&]*))?", RegexOptions.IgnoreCase);
			
			url = pageRegex.Replace(url, "");
			
			// Remove the sort query string. The new one gets added as needed.
			string sortKey = "Sort";
			Regex sortRegex = new Regex("[?&]" + sortKey + "(?:=([^&]*))?", RegexOptions.IgnoreCase);
			
			url = sortRegex.Replace(url, "");
			
			return url;
		}

		private PlaceHolder CreatePagingPanel()
		{
			PlaceHolder panel = new PlaceHolder();
			
			
			panel.Visible = AllowPaging || AllowCustomPaging;
			
			if (AllowPaging)
			{
				panel.Controls.Add(new LiteralControl(Language.Pages + ":&nbsp;"));
	
				for (int i = 0; i < PageCount; i++)
				{
					if (i != CurrentPageIndex)
					{
						string url = CompileNavigateUrl(i, CurrentSort);
						
						HyperLink link = new HyperLink();
						link.Text = (i+1).ToString();
						
						link.NavigateUrl = url;
						panel.Controls.Add(link);
					}
					else
					{
						panel.Controls.Add(new LiteralControl("<b>" + (i+1) + "</b>"));
					}
	
					if (i < PageCount-1)
						panel.Controls.Add(new LiteralControl(" | "));
				}
			}
			
			return panel;
		}
		
		public string CompileNavigateUrl()
		{
			return CompileNavigateUrl(CurrentPageIndex, CurrentSort);
		}
		
		public string CompileNavigateUrl(int pageIndex, string sort)
		{
			
			string url = NavigateUrl;
			if (url == String.Empty)
				url = CreateDefaultNavigateUrl();
			
			url = AddPageQueryString(url, pageIndex);
			url = AddSortQueryString(url, sort);
			
			return url;
		}

		public override void DataBind()
		{

			using (LogGroup logGroup = LogGroup.Start("Binding IndexGrid control: " + ID, NLog.LogLevel.Debug))
			{
				ValidatePageIndex();
				
				base.DataBind ();
				
				if (!Page.IsPostBack)
					SelectSortItem();
				
				if (this.Parent == null)
					throw new InvalidOperationException("Parent == null");
				
				if (Items == null)
					throw new InvalidOperationException("Items == null");
				
				if (Columns == null)
					throw new InvalidOperationException("Columns == null");
			}
		}
		
		public void ValidatePageIndex()
		{
			int validPageIndex = GetValidPageIndex();
			
			if (validPageIndex != CurrentPageIndex)
			{
				CurrentPageIndex = validPageIndex;
				Page.Response.Redirect(CompileNavigateUrl());
			}
			
		}

		public int GetValidPageIndex()
		{
			if (VirtualItemCount == 0)
				return 0;
			
			long remainder = (VirtualItemCount % PageSize);
			
			int maximumPageIndex = (VirtualItemCount / PageSize)-1;
			
			// If there are some left over then add another page
			if (remainder > 0)
				maximumPageIndex ++;
			
			int pageIndex = CurrentPageIndex;
			
			if (pageIndex > maximumPageIndex)
			{
				pageIndex = maximumPageIndex;
			}
			
			return pageIndex;
		}
	}
}
