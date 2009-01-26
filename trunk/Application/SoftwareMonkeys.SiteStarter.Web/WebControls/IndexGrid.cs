using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using SoftwareMonkeys.SiteStarter.Entities;
using System.ComponentModel;
using System.Collections;
using SoftwareMonkeys.SiteStarter.Web.Properties;

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
            if (Sort != null)
            {
                if (CurrentSort != null && CurrentSort.Trim().Length > 0)
                    Sort.SelectedIndex = Sort.Items.IndexOf(Sort.Items.FindByValue(CurrentSort));
                else
                {
                    Sort.SelectedIndex = Sort.Items.IndexOf(Sort.Items.FindByValue(DefaultSort));
                }
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
        public string DefaultSort
        {
            get { return (string)ViewState["DefaultSort"]; }
            set
            {
                ViewState["DefaultSort"] = value;
                SelectSortItem();
            }
        }

        /// <summary>
        /// Gets/sets the default sort expression to use when displaying the index.
        /// </summary>
        public string CurrentSort
        {
            get {
                if (ViewState["CurrentSort"] == null || (string)ViewState["CurrentSort"] == String.Empty)
                    ViewState["CurrentSort"] = DefaultSort;
                return (string)ViewState["CurrentSort"]; }
            set
            {
                ViewState["CurrentSort"] = value;
                if (Sort.SelectedValue != value)
                    SelectSortItem();
            }
        }
        #endregion

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
        public new BaseEntity[] DataSource
        {
            get
            {return (BaseEntity[])base.DataSource;
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
        
        /// <summary>
        /// Gets/sets the text displayed when the data is empty.
        /// </summary>
        public string EmptyDataText // TODO: Finish adding support for this property
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
            if (CurrentSort != String.Empty)
                Sort.SelectedIndex = Sort.Items.IndexOf(Sort.Items.FindByValue(CurrentSort));
            else
                Sort.SelectedIndex = Sort.Items.IndexOf(Sort.Items.FindByValue(DefaultSort));

			//Sort.Items.Add(new ListItem("------ " + TextHelper.Get("Sort") + " ------", ""));
			
			CustomHolder = new PlaceHolder();
			CustomHolder.ID = "CustomHolder";

            Page.RegisterStartupScript("IndexUtil", "<script language='javascript' src='/Scripts/IndexUtil.js'></script>");

			base.OnInit(e);
		}

        protected override void OnLoad(EventArgs e)
        {

            if (!Page.IsPostBack && (DataSource == null || DataSource.Length == 0))
            {
                TableCell cell = new TableCell();
                cell.Controls.Add(new LiteralControl("<i>[" + EmptyDataText + "]</i>"));

                DataGridItem row = new DataGridItem(Items.Count, 0, ListItemType.Item);
                row.Cells.Add(cell);

                row.Visible = DataSource == null || DataSource.Length == 0;

                if (Controls.Count > 0)
                    ((Table)Controls[0]).Rows.Add(row);
            }

            base.OnLoad(e);
        }

        protected override void DataBind(bool raiseOnDataBinding)
        {
            
            base.DataBind(raiseOnDataBinding);

        }

        void Sort_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentSort = Sort.SelectedValue;
            RaiseSortChanged();
        }

		protected override void OnItemCreated(DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Pager)
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
			else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				e.Item.Attributes.Add("onmouseover", "this.className='ListItemOver';");
				e.Item.Attributes.Add("onmouseout", "this.className='ListItem';");
				e.Item.Attributes.Add("onclick", (EnableExpansion ? "ToggleExpansion('" + ClientID + "', " + e.Item.ItemIndex + ")" : String.Empty));
			}
			else if (e.Item.ItemType == ListItemType.Header)
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
				table.ID = "HeaderTable";
				table.Width = Unit.Percentage(100);
				table.CssClass = "Heading2";
				table.Rows.Add(new TableRow());

				e.Item.Cells[0].Controls.Add(table);

				// Create cell 1
				TableCell cell1 = new TableCell();
				cell1.Width = Unit.Percentage(80);
				cell1.ID = "HeaderCell";
				cell1.Text = HeaderText;
				//cell1.CssClass = "";

				// Create cell 2
				TableCell cell2 = new TableCell();
				cell2.ID = "CustomCell";
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
			}
			else if (e.Item.ItemType == ListItemType.Footer)
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

			base.OnItemCreated(e);
		}

		protected override void OnPageIndexChanged(DataGridPageChangedEventArgs e)
		{
			CurrentPageIndex = e.NewPageIndex;
			
			base.OnPageIndexChanged (e);
		}

		protected override void OnPreRender(EventArgs e)
		{
            if (DataSource == null || DataSource.Length == 0)
            {
                // TODO: Add an item with a message saying there's no records
            }

			if (DataSource == null || DataSource.Length == 0)
			{
				ShowFooter = true;
			}

			if (PageCount <= 1)
				PagerStyle.Visible = false;

			/*foreach (DataGridItem item in this.Items)
			{
				item.EnableViewState = false;
			}*/

			if (EnableExpansion)
			{
				if (!Page.IsClientScriptBlockRegistered("SmartGrid"))
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

					Page.RegisterClientScriptBlock("SmartGrid", script);
				}
			}

			base.OnPreRender(e);
		}

		private TableCell CreatePagingCell()
		{
			TableCell cell = new TableCell();
			
			cell.Controls.Add(new LiteralControl(Language.Pages + "&nbsp;"));

			for (int i = 0; i < PageCount; i++)
			{
				if (i != CurrentPageIndex)
				{
					LinkButton button = new LinkButton();
					button.Text = (i+1).ToString();
					button.CommandName = "Page";
					button.CommandArgument = (i+1).ToString();
					cell.Controls.Add(button);
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

		public override void DataBind()
		{
			SelectSortItem();

			base.DataBind ();
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
