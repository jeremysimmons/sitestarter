using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;
using System.Reflection;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
	[ControlBuilder(typeof(EntitySelectControlBuilder))]
	public class EntitySelect : WebControl
	{
		public Panel MenuPanel;
		public Panel ExpansionPanel;
		public PlaceHolder ContentPanel;
		
		public PlaceHolder CollapsedPanel;
		
		public CheckBoxList CheckBoxesList;
		public RadioButtonList RadioButtonsList;
		public DropDownList DropDownList;
		
		public PlaceHolder CheckBoxesPanel;
		public PlaceHolder RadioButtonsPanel;
		public PlaceHolder DropDownPanel;
		
		
		private string entityType = typeof(SoftwareMonkeys.SiteStarter.Entities.IEntity).FullName;
		/// <summary>
		/// Gets/sets the type of entity being displayed in the list.
		/// </summary>
		public virtual string EntityType
		{
			get
			{
				//  if (entityType != null)
				//      return entityType.FullName;
				return entityType;
			}
			set
			{
				entityType = value;
				// Reset the entity type object if it's been made obsolete by the entity type string.
				//    if (!entityType.FullName.Equals(entityTypeString))
				//      entityType = null;
			}
		}
		
		/// <summary>
		/// Gets/sets the display mode of the DropDownList.
		/// </summary>
		public ListSelectionMode DisplayMode
		{
			get
			{
				if (ViewState["DisplayMode"] == null)
					ViewState["DisplayMode"] = ListSelectionMode.Single;
				return (ListSelectionMode)ViewState["DisplayMode"]; }
			set { ViewState["DisplayMode"] = value;
				
			}
		}
		
		/// <summary>
		/// Gets/sets a flag indicating whether to automatically post back after the selection has changed.
		/// </summary>
		public bool AutoPostBack
		{
			get
			{
				if (ViewState["AutoPostBack"] == null)
					ViewState["AutoPostBack"] = false;
				return (bool)ViewState["AutoPostBack"]; }
			set { ViewState["AutoPostBack"] = value;
			}
		}
		
		/// <summary>
		/// Gets/sets the selection mode.
		/// </summary>
		public ListSelectionMode SelectionMode
		{
			get
			{
				if (ViewState["SelectionMode"] == null)
					ViewState["SelectionMode"] = ListSelectionMode.Single;
				return (ListSelectionMode)ViewState["SelectionMode"]; }
			set { ViewState["SelectionMode"] = value;
				// TODO: Check if needed
				// If the HideNoSelection property hasn't been specified then set it
				// based on the selection mode
				if (ViewState["HideNoSelection"] == null)
				{
					// If selection mode is multiple then set HideNoSelection to true
					if (value == ListSelectionMode.Multiple)
						ViewState["HideNoSelection"] = true;
					else
						ViewState["HideNoSelection"] = false;
				}
			}
		}
		
		/// <summary>
		/// Gets/sets a flag indicating whether or not to collapse the field if it's empty..
		/// </summary>
		public bool CollapseEmpty
		{
			get
			{
				if (ViewState["CollapseEmpty"] == null)
					ViewState["CollapseEmpty"] = false;
				return (bool)ViewState["CollapseEmpty"]; }
			set { ViewState["CollapseEmpty"] = value;
			}
		}

		private IEntity[] dataSource;
		/// <summary>
		/// Gets/sets the Entity data required for this control.
		/// </summary>
		[Browsable(false)]
		public IEntity[] DataSource
		{
			get
			{
				if (dataSource == null)
				{
					RaiseDataLoading();
				}
				return (IEntity[])dataSource;
			}
			set { dataSource = value; }
		}

		/// <summary>
		/// Gets/sets the ID of the selected Entity.
		/// </summary>
		[Browsable(false)]
		[Bindable(true)]
		public IEntity SelectedEntity
		{
			get
			{
				if (SelectedEntities != null && SelectedEntities.Length > 0)
					return SelectedEntities[0];
				else
					return null;
			}
			set
			{
				SelectedEntities = new IEntity[] { value };
			}
		}

		private IEntity[] selectedEntities;
		/// <summary>
		/// Gets/sets the selected entities.
		/// </summary>
		[Browsable(false)]
		[Bindable(true)]
		public IEntity[] SelectedEntities
		{
			get
			{
				if (selectedEntities == null)
				{
					if (DataSource != null && GetDataSourceLength() > 0)
						selectedEntities = Collection<IEntity>.GetByIDs(DataSource, SelectedEntityIDs);
					else
						selectedEntities = new IEntity[]{};
				}
				return selectedEntities;
			}
			set
			{
				selectedEntities = value;
				SelectedEntityIDs = Collection<IEntity>.GetIDs(value);
			}
		}

		/// <summary>
		/// Gets/sets the ID of the selected Entity.
		/// </summary>
		[Browsable(false)]
		[Bindable(true)]
		public Guid SelectedEntityID
		{
			get
			{
				if (SelectedEntityIDs != null && SelectedEntityIDs.Length > 0)
					return SelectedEntityIDs[0];
				else
					return Guid.Empty;
			}
			set
			{
				SelectedEntityIDs = new Guid[] { value };
			}
		}

		/// <summary>
		/// Gets/sets the IDs of the selected entities.
		/// </summary>
		[Browsable(false)]
		[Bindable(true)]
		public Guid[] SelectedEntityIDs
		{
			get
			{				
				Guid[] ids = new Guid[]{};
				
				if (DropDownList != null)
				{
					ids = GetIDsFromString(DropDownList.SelectedValue);
				}
				else if (RadioButtonsList != null)
					ids = GetIDsFromString(RadioButtonsList.SelectedValue);
				else
				{
					ids = GetIDsFromCheckBox();
				}
				return ids;
			}
			set
			{
				using (LogGroup logGroup = LogGroup.Start("Setting the SelectedEntityIDs property of the EntitySelect with ID " + ID + ".", NLog.LogLevel.Debug))
				{
					if (value != null)
						LogWriter.Debug("#: " + value.Length.ToString());
					else
						LogWriter.Debug("#: " + 0);
					ViewState["SelectedEntityIDs"] = value;
					SelectItems();
				}
			}
		}
		
		public Guid[] GetIDsFromCheckBox()
		{
			List<Guid> ids = new List<Guid>();
			
			foreach (ListItem item in CheckBoxesList.Items)
			{
				if (item.Selected)
				{
					ids.Add(new Guid(item.Value));
				}
			}
			
			return ids.ToArray();
		}
		
		// TODO: Check if needed
		/// <summary>
		/// Gets/sets a value determining whether to hide the no selection option.
		/// </summary>
		public bool HideNoSelection
		{
			get
			{
				if (ViewState["HideNoSelection"] == null)
					ViewState["HideNoSelection"] = false;
				return (bool)ViewState["HideNoSelection"]; }
			set { ViewState["HideNoSelection"] = value; }
		}

		/// <summary>
		/// Gets/sets the text displayed on the no selection option.
		/// </summary>
		public string NoSelectionText
		{
			get
			{
				if (ViewState["NoSelectionText"] == null || (String)ViewState["NoSelectionText"] == String.Empty)
					ViewState["NoSelectionText"] = "-- Select " + DynamicLanguage.GetText(EntityState.GetType(EntityType).Name) + " --";
				return (string)ViewState["NoSelectionText"]; }
			set { ViewState["NoSelectionText"] = value; }
		}
		
		/// <summary>
		/// Gets/sets the text displayed when there's no data.
		/// </summary>
		public string NoDataText
		{
			get
			{
				if (ViewState["NoDataText"] == null || (String)ViewState["NoDataText"] == String.Empty)
					ViewState["NoDataText"] = Properties.Language.NoData;
				return (string)ViewState["NoDataText"]; }
			set { ViewState["NoDataText"] = value;
			}
		}
		
		/// <summary>
		/// Gets/sets the text displayed when there's no data.
		/// </summary>
		public string CollapsedText
		{
			get
			{
				if (ViewState["CollapsedText"] == null || (String)ViewState["CollapsedText"] == String.Empty)
				{
					if (DataSource == null || DataSource.Length == 0)
						ViewState["CollapsedText"] = NoDataText;
					else
						ViewState["CollapsedText"] = "...";
				}
				return (string)ViewState["CollapsedText"]; }
			set { ViewState["CollapsedText"] = value;
				if (CollapsedPanel != null)
					((Label)CollapsedPanel.Controls[0]).Text = value;
			}
		}

		
		/// <summary>
		/// Gets/sets the name of the property to use for the text of the items.
		/// </summary>
		public string TextPropertyName
		{
			get
			{
				if (ViewState["TextPropertyName"] == null)
				{
					ViewState["TextPropertyName"] = "Name";
				}
				return (string)ViewState["TextPropertyName"];
			}
			set { ViewState["TextPropertyName"] = value;
			}
		}
		
		/// <summary>
		/// Gets/sets the name of the property to use for the value of the items.
		/// </summary>
		public string ValuePropertyName
		{
			get
			{
				if (ViewState["ValuePropertyName"] == null)
				{
					ViewState["ValuePropertyName"] = "ID";
				}
				return (string)ViewState["ValuePropertyName"];
			}
			set { ViewState["ValuePropertyName"] = value;
			}
		}
		
		/// <summary>
		/// Gets/sets the text displayed when there's no data.
		/// </summary>
		public bool UseDropDownForSingle
		{
			get
			{
				if (ViewState["UseDropDownForSingle"] == null)
					ViewState["UseDropDownForSingle"] = true;
				return (bool)ViewState["UseDropDownForSingle"]; }
			set { ViewState["UseDropDownForSingle"] = value; }
		}

		protected void InitializeControls()
		{
			CreateExpansionPanel();
			CreateContentPanel();
			
			if (SelectionMode == ListSelectionMode.Multiple)
			{
				CreateCheckBoxesPanel();
			}
			if (SelectionMode == ListSelectionMode.Single)
			{
				if (UseDropDownForSingle)
				{
					CreateDropDownPanel();
				}
				else
				{
					CreateRadioButtonsPanel();
				}
			}
			
			CreateCollapsedPanel();
			CreateMenuPanel();
			
			RegisterExpansionScript();
		}
		
		protected void RegisterExpansionScript()
		{
			string script = @"
				<script language='javascript'>
				var expansionInterval = 100;
				var minimumExpansionHeight = 10;
				
				function GrowEntitySelect(clientID)
				{
					var ctrl = GetControl(clientID);
					var height = ctrl.style.height;

					var newHeight = AddToUnit(height, expansionInterval);

					ctrl.style.height = newHeight;
				}
				
				function ShrinkEntitySelect(clientID)
				{
					var ctrl = GetControl(clientID);
					var height = ctrl.style.height;
					
					var newHeight = AddToUnit(height, -expansionInterval);

					if (GetUnitValue(newHeight) < minimumExpansionHeight)
					{
						CollapseEntitySelect(clientID);
					}

					ctrl.style.height = newHeight;
				}
				
				function CollapseEntitySelect(clientID)
				{
					var ctrl = GetControl(clientID);
					var height = ctrl.style.height;
					
					//var collapsedCtrl = GetControl(clientID + '_Collapsed');

					//ctrl.style.display = '';
					//collapsedCtrl.style.display = 'inline';
				}
				
				function GetControl(clientID)
				{
					var ctrl = document.getElementById(clientID);
					
					if (ctrl == null)
						alert('Error: No field found with ID \'' + clientID + '\'');
				
					return ctrl;
				}
				
				function GetUnitValue(value)
				{
					if (value.indexOf('px') > -1)
						return parseInt(value.replace('px', ''));
					else
						return value;
				}
			
				function AddToUnit(value, addition)
				{
					if (value == null || value == '')
						value = '50px';
						
					var intValue = GetUnitValue(value);
					intValue = intValue + addition;
					return intValue + 'px';
				}
				</script>";
			
			string key = "EntitySelectScript";
			
			if (!Page.ClientScript.IsClientScriptBlockRegistered(key))
				Page.ClientScript.RegisterClientScriptBlock(GetType(), key, script);
		}
		
		protected void CreateMenuPanel()
		{
			if (SelectionMode == ListSelectionMode.Multiple || !UseDropDownForSingle)
			{
				MenuPanel = new Panel();
				Controls.Add(MenuPanel);
				MenuPanel.CssClass = "EntitySelectMenu";
				MenuPanel.Width = Width;
				
				HyperLink expandLink = new HyperLink();
				expandLink.Text = Properties.Language.Grow;
				expandLink.NavigateUrl = "javascript:GrowEntitySelect('" + ExpansionPanel.ClientID + "');";
				
				HyperLink shrinkLink = new HyperLink();
				shrinkLink.Text = Properties.Language.Shrink;
				shrinkLink.NavigateUrl = "javascript:ShrinkEntitySelect('" + ExpansionPanel.ClientID + "');";
				
				MenuPanel.Controls.Add(expandLink);
				MenuPanel.Controls.Add(new LiteralControl(" - "));
				MenuPanel.Controls.Add(shrinkLink);
			}
			
		}
		
		protected void CreateExpansionPanel()
		{
			ExpansionPanel = new Panel();
			Controls.Add(ExpansionPanel);
			
			ExpansionPanel.Width = Width;
			ExpansionPanel.Height = Height;
			ExpansionPanel.ID = ID + "_Expander";
			ExpansionPanel.CssClass = "EntitySelectExpander";
		}
		
		protected void CreateCheckBoxesPanel()
		{
			
			CheckBoxesPanel = new PlaceHolder();
			ContentPanel.Controls.Add(CheckBoxesPanel);
			
			CheckBoxesList = new CheckBoxList();
			CheckBoxesList.ID = ID + "_CheckBoxes";
			CheckBoxesList.AutoPostBack = AutoPostBack;
			CheckBoxesList.DataSource = DataSource;
			CheckBoxesPanel.Controls.Add(CheckBoxesList);
		}
		
		protected void CreateContentPanel()
		{
			ContentPanel = new PlaceHolder();
			ExpansionPanel.Controls.Add(ContentPanel);
		}
		
		protected void CreateRadioButtonsPanel()
		{
			
			RadioButtonsPanel = new PlaceHolder();
			Controls.Add(RadioButtonsPanel);
			
			RadioButtonsList = new RadioButtonList();
			RadioButtonsList.ID = ID + "_RadioButtons";
			RadioButtonsList.AutoPostBack = AutoPostBack;
			RadioButtonsList.DataSource = DataSource;
			RadioButtonsPanel.Controls.Add(RadioButtonsList);
			
			
		}
		
		protected void CreateDropDownPanel()
		{
			DropDownPanel = new PlaceHolder();
			Controls.Add(DropDownPanel);
			
			DropDownList = new DropDownList();
			DropDownList.ID = ID + "_DropDown";
			DropDownList.DataValueField = ValuePropertyName;
			DropDownList.DataTextField = TextPropertyName;
			DropDownList.Width = Width;
			DropDownList.CssClass = CssClass;
			DropDownList.AutoPostBack = AutoPostBack;
			DropDownList.DataSource = DataSource;
			DropDownPanel.Controls.Add(DropDownList);
			
			
		}
		
		protected void CreateCollapsedPanel()
		{
			CollapsedPanel = new PlaceHolder();
			CollapsedPanel.ID = ID + "_Collapsed";
			
			// Move it to the expansion panel
			Controls.Add(CollapsedPanel);
			
			Label textControl = new Label();
			textControl.Text = CollapsedText;
			textControl.CssClass = "EntitySelectCollapsed";
			
			
			CollapsedPanel.Controls.Add(textControl);
			
		}
		
		/// <summary>
		/// Selects the appropriate items depending on the SelectedEntityIDs.
		/// </summary>
		protected void SelectItems()
		{
			using (LogGroup logGroup = LogGroup.Start("Selecting the items that match the info specified.", NLog.LogLevel.Debug))
			{
				Guid[] selectedEntityIDs = new Guid[] {};
				
				if (ViewState["SelectedEntityIDs"] != null)
				{
					LogWriter.Debug("ViewState[\"SelectedEntityIDs\"] != null");
					
					selectedEntityIDs = (Guid[])ViewState["SelectedEntityIDs"];
				}
				else
					LogWriter.Debug("ViewState[\"SelectedEntityIDs\"] == null");
				
				
				LogWriter.Debug("# of selected entities: " + selectedEntityIDs.Length.ToString());
				
				foreach (ListItem item in GetItems())
				{
					using (LogGroup logGroup2 = LogGroup.Start("Selecting/deselecting item.", NLog.LogLevel.Debug))
					{
						Guid id = Guid.Empty;
						
						if (GuidValidator.IsValidGuid(item.Value))
							id = GuidValidator.ParseGuid(item.Value);
						
						LogWriter.Debug("item.Text: " + item.Text);
						LogWriter.Debug("item.Value: " + item.Value);
						item.Selected = (Array.IndexOf(selectedEntityIDs, id) > -1);
						LogWriter.Debug("item.Selected: " + item.Selected.ToString());
					}
				}
			}
		}
		
		/// <summary>
		/// Retrieves the items in the visible list (either check box or radio button list).
		/// </summary>
		/// <returns></returns>
		public ListItemCollection GetItems()
		{
			// If the controls are not null then they're in use
			if (CheckBoxesList != null)
				return CheckBoxesList.Items;
			else if (RadioButtonsList != null)
				return RadioButtonsList.Items;
			else if (DropDownList != null)
				return DropDownList.Items;
			
			return new ListItemCollection();
		}
		
		/// <summary>
		/// Clears the items in the visible list (either check box list or radio button list).
		/// </summary>
		public void ClearItems()
		{
			if (SelectionMode == ListSelectionMode.Multiple)
				CheckBoxesList.Items.Clear();
			else if (SelectionMode == ListSelectionMode.Multiple)
				RadioButtonsList.Items.Clear();
		}
		
		/// <summary>
		/// Inserts an item into the visible list (either check box list or radio button list).
		/// </summary>
		public void InsertItem(int index, ListItem item)
		{
			if (SelectionMode == ListSelectionMode.Multiple)
				CheckBoxesList.Items.Insert(index, item);
			else if (SelectionMode == ListSelectionMode.Multiple)
				RadioButtonsList.Items.Insert(index, item);
		}
		
		/// <summary>
		/// Inserts an item onto the visible list (either check box list or radio button list).
		/// </summary>
		public void AddItem(ListItem item)
		{
			
			if (SelectionMode == ListSelectionMode.Multiple)
				CheckBoxesList.Items.Add(item);
			else if (SelectionMode == ListSelectionMode.Multiple)
				RadioButtonsList.Items.Add(item);
		}
		
		public override void DataBind()
		{
			base.DataBind();
			
			ConfigureNoDataMessage();
			SetDefaultHeight();
			
			CascadeSettings();
			
			ConfigureVisibility();
			
			InsertNoSelectionItem();
		}
		
		public void InsertNoSelectionItem()
		{
			if (DropDownList != null &&
			   !HideNoSelection)
			{
				ListItem noSelectionItem = new ListItem(NoSelectionText, Guid.Empty.ToString());
				DropDownList.Items.Insert(0, noSelectionItem);
			}
		}
		
		public void ConfigureVisibility()
		{
			if (MenuPanel != null)
				MenuPanel.Visible = (DataSource != null && DataSource.Length > 0);
			
			if (ExpansionPanel != null)
				ExpansionPanel.Visible = (DataSource != null && DataSource.Length > 0)
					&& (SelectionMode == ListSelectionMode.Multiple || !UseDropDownForSingle);
			
			if (DropDownPanel != null)
				DropDownPanel.Visible = SelectionMode == ListSelectionMode.Single
					|| !CollapseEmpty;
			
			CollapsedPanel.Visible = (DataSource == null || DataSource.Length == 0)
				&& (CollapseEmpty || SelectionMode == ListSelectionMode.Multiple);
			
			ContentPanel.Visible = (DataSource != null && DataSource.Length > 0)
				|| !CollapseEmpty;
		}
		
		public void CascadeSettings()
		{
			ExpansionPanel.Height = Height;
		}
		
		public void ConfigureNoDataMessage()
		{
			
			if (DataSource == null || DataSource.Length == 0)
				CollapsedText = NoDataText;
		}
		
		public void SetDefaultHeight()
		{
			int minimumHeight = 15;
			int maximumHeight = 200;
			int itemHeight = 30;
			int height = 0;
			
			if (DataSource == null || DataSource.Length == 0)
				height = minimumHeight;
			else
			{
				if (SelectionMode == ListSelectionMode.Multiple)
					height = DataSource.Length * itemHeight;
				else
					height = itemHeight;
				
				if (height > maximumHeight)
					height = maximumHeight;
			}
			
			Height = Unit.Pixel(height);
		}

		
		protected Guid[] GetIDsFromString(string value)
		{
			List<Guid> list = new List<Guid>();
			
			foreach (string stringID in value.Split(','))
			{
				if (GuidValidator.IsValidGuid(stringID.Trim()))
				{
					Guid id = GuidValidator.ParseGuid(stringID.Trim());
					list.Add(id);
				}
			}
			
			return list.ToArray();
		}
		
		
		#region Events
		/// <summary>
		/// Raised when data is being loaded.
		/// </summary>
		public event EventHandler DataLoading;

		/// <summary>
		/// Called to raise the DataLoading event.
		/// </summary>
		protected void RaiseDataLoading()
		{
			if (DataLoading != null)
				DataLoading(this, EventArgs.Empty);
		}
		#endregion
		
		
		/// <summary>
		/// Sets the default data settings of the dropdownlist.
		/// </summary>
		public EntitySelect()
		{
		}
		
		protected override void OnInit(EventArgs e)
		{
			CssClass = "Field";
			
			InitializeControls();
			
			base.OnInit(e);
			
		}


		protected int GetDataSourceLength()
		{
			if (DataSource is Array)
				return ((Array)DataSource).Length;
			else
				throw new NotSupportedException("Invalid type: " + DataSource.GetType().ToString());
		}

		protected override void OnLoad(EventArgs e)
		{
			// This is called just-in-time in DataSource_get
			//RaiseDataLoading();

			base.OnLoad(e);
		}
		
		public virtual void Populate()
		{
			using (LogGroup logGroup = LogGroup.Start("Populating the EntitySelect control.", NLog.LogLevel.Debug))
			{
				ClearItems();

				// Add the first item.
				if (!HideNoSelection)
				{
					LogWriter.Debug("HideNoSelection == false");
					LogWriter.Debug("Inserting the 'No Selection' item.");
					InsertItem(0, new ListItem(NoSelectionText, Guid.Empty.ToString()));
				}
				else
				{
					LogWriter.Debug("HideNoSelection == true");
					LogWriter.Debug("NOT inserting the 'No Selection' item.");
				}

				using (LogGroup logGroup2 = LogGroup.Start("Looping through DataSource entities.", NLog.LogLevel.Debug))
				{
					// TODO: Check if code is necessary
					// Add the rest of the items
					foreach (IEntity entity in (IEnumerable)DataSource)
					{
						using (LogGroup logGroup3 = LogGroup.Start("Adding entity to control.", NLog.LogLevel.Debug))
						{
							LogWriter.Debug("Entity type: " + entity.GetType().ToString());
							LogWriter.Debug("Entity ID: " + entity.ID.ToString());
							
							// Create a list of each entity ID that gets added.
							ArrayList existingIDs = new ArrayList();
							
							// Check if the entity has already been added
							if (!existingIDs.Contains(entity.ID))
							{
								LogWriter.Debug("Adding entity.");
								
								PropertyInfo property = entity.GetType().GetProperty(TextPropertyName);
								
								if (property == null)
									throw new ArgumentException("Can't find '" + TextPropertyName + "' text property on type '" + entity.GetType().ToString() + "'.");
								
								object value = property.GetValue(entity, null);
								
								string stringValue = (value == null
								                      ? String.Empty
								                      : value.ToString());
								
								LogWriter.Debug("Text: " + stringValue);
								
								AddItem(new ListItem(stringValue, entity.ID.ToString()));
								
								existingIDs.Add(entity.ID);
							}
							else
								LogWriter.Debug("Entity already added so it was skipped.");
						}
					}
				}
			}
		}

		public void Populate(IEntity[] entities)
		{
			DataSource = entities;
			Populate();
		}
	}

}
