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
		public event EventHandler SortChanged;

		protected void RaiseSortChanged()
		{
			if (SortChanged != null)
				SortChanged(this, EventArgs.Empty);
		}

		/// <summary>
		/// Selects the appropriate sort item.
		/// </summary>
		private void SelectSortItem()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Selecting the current sort item.", NLog.LogLevel.Debug))
			{
				if (Sort != null)
				{
					AppLogger.Debug("Current sort: " + CurrentSort);
					AppLogger.Debug("Default sort: " + DefaultSort);
					if (CurrentSort != null && CurrentSort.Trim().Length > 0)
					{
						AppLogger.Debug("Using CurrentSort");
						Sort.SelectedIndex = Sort.Items.IndexOf(Sort.Items.FindByValue(CurrentSort));
					}
					else
					{
						AppLogger.Debug("Using DefaultSort");
						Sort.SelectedIndex = Sort.Items.IndexOf(Sort.Items.FindByValue(DefaultSort));
					}
				}
				else
					AppLogger.Debug("Sort control is null");
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
		/// Gets/sets a value indicating whether to enable expanding items.
		/// </summary>
		[Bindable(true)]
		[Browsable(true)]
		public bool EnableExpansion
		{
			get
			{
				if (ViewState["EnableExpansion"] == null)
					ViewState["EnableExpansion"] = false;
				return (bool)ViewState["EnableExpansion"];
			}
			set
			{
				ViewState["EnableExpansion"] = value;
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
		public string NoDataText // TODO: Finish adding support for this property
		{
			get
			{
				if (ViewState["EmptyDataText"] == null)
					ViewState["EmptyDataText"] = String.Empty;
				return (string)ViewState["EmptyDataText"];
			}
			set { ViewState["EmptyDataText"] = value; }
		}

		protected override void OnInit(EventArgs e)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Initializing IndexGrid control: " + ID, NLog.LogLevel.Debug))
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
					if (CurrentSort != String.Empty)
						Sort.SelectedIndex = Sort.Items.IndexOf(Sort.Items.FindByValue(CurrentSort));
					else
						Sort.SelectedIndex = Sort.Items.IndexOf(Sort.Items.FindByValue(DefaultSort));
				}

				AppLogger.Debug("Default sort: " + DefaultSort);
				AppLogger.Debug("Current sort: " + CurrentSort);
				
				//Sort.Items.Add(new ListItem("------ " + TextHelper.Get("Sort") + " ------", ""));
				
				CustomHolder = new PlaceHolder();
				CustomHolder.ID = "CustomHolder";
				
				Page.ClientScript.RegisterStartupScript(this.GetType(), "IndexUtil", "<script language='javascript' src='/Scripts/IndexUtil.js'></script>");
				
				
			}
			
			base.OnInit(e);

		}

		protected override void OnLoad(EventArgs e)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Initializing IndexGrid control: " + ID, NLog.LogLevel.Debug))
			{

				
				base.OnLoad(e);
				
				// If no sort items were added then hide the sort list
				if (Sort.Items.Count == 0)
					Sort.Visible = false;
			}
		}

		protected override void DataBind(bool raiseOnDataBinding)
		{
			
			base.DataBind(raiseOnDataBinding);

		}

		void Sort_SelectedIndexChanged(object sender, EventArgs e)
		{
			HttpContext.Current.Response.Redirect(CompileNavigateUrl(CurrentPageIndex, Sort.SelectedValue));
			//CurrentSort = Sort.SelectedValue;
			//RaiseSortChanged();
		}

		protected override void OnItemCreated(DataGridItemEventArgs e)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Customizing a grid item upon creation (ItemCreated event).", NLog.LogLevel.Debug))
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
			using (LogGroup logGroup = AppLogger.StartGroup("Customizing a header item.", NLog.LogLevel.Debug))
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

				Table table = CreateHeaderTable();
				
				e.Item.Cells[0].Controls.Add(table);
				e.Item.CssClass = "Heading2";
			}
		}
		
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
		}

		protected void CustomizeFooter(DataGridItemEventArgs e)
		{
			
			using (LogGroup logGroup = AppLogger.StartGroup("Customizing a footer item.", NLog.LogLevel.Debug))
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

				// Add the table
				Table table = new Table();
				table.ID = "FooterTable";
				table.Width = Unit.Percentage(100);
				table.Rows.Add(new TableRow());

				e.Item.Cells[0].Controls.Add(table);

				
				
				// Create cell 1
				TableCell cell1 = CreatePagingCell();
				cell1.Width = Unit.Percentage(50);
				cell1.HorizontalAlign = HorizontalAlign.Right;
				cell1.CssClass = "PagingContainer";
				cell1.ID = "PagingCell2";
				cell1.Wrap = false;
				cell1.Visible = AllowPaging && DataSource != null && DataSource.Length > 0;

				table.Rows[0].Cells.Add(cell1);
				
			}
		}
		
		protected void CustomizePager(DataGridItemEventArgs e)
		{
			
			using (LogGroup logGroup = AppLogger.StartGroup("Customizing a pager item.", NLog.LogLevel.Debug))
			{
				e.Item.Cells[0].Controls.Clear();

				/*if (!IsFirstPage)
				{
					LinkButton firstButton = new LinkButton();
					firstButton.Text = "|&laquo;&laquo;";
					firstButton.CommandName = "Page";
					firstButton.CommandArgument = "1";
					firstButton.CausesValidation = false;
					e.Item.Cells[0].Controls.Add(firstButton);

					e.Item.Cells[0].Controls.Add(new LiteralControl(" | "));

					LinkButton prevButton = new LinkButton();
					prevButton.Text = "&laquo;";
					prevButton.CommandName = "Page";
					prevButton.CommandArgument = (CurrentPageIndex).ToString();
					prevButton.CausesValidation = false;
					e.Item.Cells[0].Controls.Add(prevButton);

					e.Item.Cells[0].Controls.Add(new LiteralControl(" | "));
				}*/
				
				

				e.Item.Cells.RemoveAt(0);
			}
		}
		
		protected void CustomizeItem(DataGridItemEventArgs e)
		{
			
			using (LogGroup logGroup = AppLogger.StartGroup("Customizing a general or alternating item.", NLog.LogLevel.Debug))
			{
				e.Item.Attributes.Add("onmouseover", "this.className='ListItemOver';");
				e.Item.Attributes.Add("onmouseout", "this.className='ListItem';");
				e.Item.Attributes.Add("onclick", (EnableExpansion ? "ToggleExpansion('" + ClientID + "', " + e.Item.ItemIndex + ")" : String.Empty));
			}
		}

		protected override void OnPageIndexChanged(DataGridPageChangedEventArgs e)
		{
			CurrentPageIndex = e.NewPageIndex;
			
			base.OnPageIndexChanged (e);
		}
		
		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			
			//int numRows = base.CreateChildControls(dataSource, dataBinding);

			
			/*//create table
			Table table = new Table();
			table.ID = this.ID;

			//create a new header row
			GridViewRow row = base.CreateRow(-1, -1, DataControlRowType.Header, DataControlRowState.Normal);

			//convert the exisiting columns into an array and initialize
			DataControlField[] fields = new DataControlField[this.Columns.Count];
			this.Columns.CopyTo(fields, 0);
			this.InitializeRow(row, fields);
			table.Rows.Add(row);

			//create the empty row
			row = new GridViewRow(-1, -1, DataControlRowType.DataRow, DataControlRowState.Normal);
			TableCell cell = new TableCell();
			cell.ColumnSpan = this.Columns.Count;
			cell.Width = Unit.Percentage(100);
			cell.Controls.Add(new LiteralControl(EmptyTableRowText));
			row.Cells.Add(cell);
			table.Rows.Add(row);

			this.Controls.Add(table);*/
				

		}
		
		
		private void InitializeNoDataText()
		{
			Controls.Clear();
			
			AppLogger.Debug("DataSource is empty. Adding [NoDataText] to control.");
			
			Table table = new Table();
			
			Controls.Add(table);
			
			// Header row
			DataGridItem headerRow = new DataGridItem(Items.Count, 0, ListItemType.Header);
			table.Rows.Add(headerRow);
			
			// Header cell
			headerRow.Cells.Add(new TableCell());
			headerRow.Cells[0].Controls.Add(CreateHeaderTable());
			headerRow.CssClass = "Heading2";
			
			// Text cell
			TableCell textCell = new TableCell();
			textCell.Controls.Add(new LiteralControl("<i>" + NoDataText + "</i>"));
			
			DataGridItem textRow = new DataGridItem(Items.Count, 0, ListItemType.Item);
			textRow.Cells.Add(textCell);
			
			table.Rows.Add(textRow);
			
			//textRow.Visible = (DataSource == null || DataSource.Length == 0);
			
			//Table noDataTable = new Table();
			//outerTable.Rows[0].Cells[0].Controls.Add(noDataTable);
			
			//TableCell headerCell = new TableCell();
			//headerCell.Controls.Add(new LiteralControl(HeaderText));
			
			//headerRow.Cells[0]
			//headerRow.Cells.Add(headerCell);
			
			
			//noDataTable.Rows.Add(headerRow);
			//noDataTable.Rows.Add(textRow);
			
			
			
			/*//create a new header row
			DataGridItem row = base.CreateItem(-1, -1, ListItemType.Header);

			//convert the exisiting columns into an array and initialize
			DataGridColumn[] fields = new DataGridColumn[this.Columns.Count];
			this.Columns.CopyTo(fields, 0);
			this.InitializeItem(row, fields);
			noDataTable.Rows.Add(row);

			//create the empty row
			row = base.CreateItem(-1, -1, ListItemType.Header);
			TableCell cell = new TableCell();
			cell.ColumnSpan = this.Columns.Count;
			cell.Width = Unit.Percentage(100);
			cell.Controls.Add(new LiteralControl(NoDataText));
			row.Cells.Add(cell);
			noDataTable.Rows.Add(row);*/

	}

	protected override void OnPreRender(EventArgs e)
	{
		if (DataSource == null || DataSource.Length == 0)
		{
			InitializeNoDataText();
		}
		else
		{
			AppLogger.Debug("DataSource is not empty.");
			AppLogger.Debug("DataSource.Length: " + DataSource.Length);
		}
		
		if (DataSource == null || DataSource.Length == 0)
		{
			ShowFooter = true;
		}

		if (PageCount <= 1)
			PagerStyle.Visible = false;
		

		if (EnableExpansion)
		{
			if (!Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(), "SmartGrid"))
			{
				/*string script = @"<script language='JavaScript' defer>
						function ExpandGridItem(gridID, itemIndex)
						{
							for (var i = 0; i < " + Columns.Count + 5 + @"; i++)
							{
								var itemID = gridID + '__ctl' + itemIndex + '_Expanded_' + i;
								if (document.getElementById(itemID))
								{
									document.getElementById(itemID).style.display = '';
								}
							}
						}

						function CollapseGridItem(gridID, itemIndex)
						{
							for (var i = 0; i < " + Columns.Count + 5 + @"; i++)
							{
								var itemID = gridID + '__ctl' + itemIndex + '_Expanded_' + i;
								if (document.getElementById(itemID))
								{
									document.getElementById(itemID).style.display = 'none';
								}
							}
						}
				</script>";*/

				string script = @"<script language='JavaScript'>
						function ToggleExpansion(gridID, itemIndex)
						{
							for (var i = 0; i <= " + Columns.Count + @"; i++)
							{
								var itemID = gridID + '__ctl' + itemIndex + '_Expanded_' + i;

								if (document.getElementById(itemID))
								{

									if (document.getElementById(itemID).style.display == 'none')
										document.getElementById(itemID).style.display = '';
									else
										document.getElementById(itemID).style.display = 'none';
								}
							}
						}

						function InitToggle()
						{
							for (var x = 0; x <= " + (DataSource != null ? DataSource.Length : 0).ToString() + @"; x++)
							{
								ToggleExpansion('" + ClientID + @"', x);
							}
						}
						</script>";

				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SmartGrid", script);
			}
		}

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

	private TableCell CreatePagingCell()
	{
		TableCell cell = new TableCell();
		
		cell.Controls.Add(new LiteralControl(Language.Pages + "&nbsp;"));

		for (int i = 0; i < PageCount; i++)
		{
			if (i != CurrentPageIndex)
			{
				
				string url = CompileNavigateUrl(i, CurrentSort);
				
				HyperLink link = new HyperLink();
				link.Text = (i+1).ToString();
				//button.CommandName = "Page";
				//button.CommandArgument = (i+1).ToString();
				link.NavigateUrl = url;
				cell.Controls.Add(link);
			}
			else
			{
				cell.Controls.Add(new LiteralControl("<b>" + (i+1) + "</b>"));
			}

			if (i < PageCount-1)
				cell.Controls.Add(new LiteralControl(" | "));
		}

		/*if (!IsLastPage)
				{
					e.Item.Cells[0].Controls.Add(new LiteralControl(" | "));

					LinkButton nextButton = new LinkButton();
					nextButton.Text = "&raquo;";
					nextButton.CommandName = "Page";
					nextButton.CommandArgument = (CurrentPageIndex + 2).ToString();
					nextButton.CausesValidation = false;
					e.Item.Cells[0].Controls.Add(nextButton);

					e.Item.Cells[0].Controls.Add(new LiteralControl(" | "));

					LinkButton lastButton = new LinkButton();
					lastButton.Text = "&raquo;&raquo;|";
					lastButton.CommandName = "Page";
					lastButton.CommandArgument = PageCount.ToString();
					lastButton.CausesValidation = false;
					e.Item.Cells[0].Controls.Add(lastButton);
				}*/

		/*if (Items.Count == 0)
			{
				for (int i = 0; i < e.Item.Cells[0].Controls.Count; i++)
				{
					Control control = e.Item.Cells[0].Controls[i];
					//control.ID = Guid.NewGuid().ToString().Substring(0, 5);
					//e.Item.Cells[0].Controls.Remove(control);
					PagingCell1.Controls.Add(control);
					//i--;
				}
			}
			else
			{
				for (int i = 0; i < e.Item.Cells[0].Controls.Count; i++)
				{
					Control control = e.Item.Cells[0].Controls[i];
					//control.ID = Guid.NewGuid().ToString().Substring(0, 5);
					//e.Item.Cells[0].Controls.Remove(control);
					PagingCell2.Controls.Add(control);
					//i--;
				}
			}*/

		return cell;
	}
	
	private string CompileNavigateUrl(int pageIndex, string sort)
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

		using (LogGroup logGroup = AppLogger.StartGroup("Binding IndexGrid control: " + ID, NLog.LogLevel.Debug))
		{
			
			base.DataBind ();
			
			//if (DataSource == null)
			//	AppLogger.Debug("DataSource == null. Skipping bind.");
			//else
			//{

			SelectSortItem();
			
			if (Page == null)
				throw new InvalidOperationException("Page == null");
			
			if (Items == null)
				throw new InvalidOperationException("Items == null");
			
			if (Columns == null)
				throw new InvalidOperationException("Columns == null");

			
			//}

		}
	}


	#region Expansion functions
	/// <summary>
	/// Gets the ID to use for an expanding div.
	/// </summary>
	/// <param name="item"></param>
	/// <returns></returns>
	public string GetExpandedClientID(DataGridItem item, int cellID)
	{
		string id = ClientID + "__ctl" + item.ItemIndex + "_Expanded_" + cellID;
		// TODO: Remove expanded count
		//ExpandedCount++;
		return id;
	}
	#endregion
	

	protected override void Render(HtmlTextWriter writer)
	{
		base.Render (writer);

		if (EnableExpansion)
		{
			writer.Write(
				@"<script language='JavaScript' defer>

				InitToggle();

				</script>");
		}
	}

}
}
