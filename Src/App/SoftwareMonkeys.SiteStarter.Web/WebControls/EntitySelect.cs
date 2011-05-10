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
	public class EntitySelect : ListBox, IPostBackDataHandler
	{
		/// <summary>
		/// Gets/sets the name of the property to use for the value of the items.
		/// </summary>
		public string ValuePropertyName
		{
			get
			{
				if (ViewState["ValuePropertyName"] == null)
				{
					ViewState["ValuePropertyName"] = DataValueField = "ID";
				}
				return (string)ViewState["ValuePropertyName"];
			}
			set { ViewState["ValuePropertyName"] = value;
				DataValueField = value;
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
					ViewState["TextPropertyName"] = DataTextField = "Name";
				}
				return (string)ViewState["TextPropertyName"];
			}
			set { ViewState["TextPropertyName"] = value;
				DataTextField = value;
			}
		}
		
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
		/// Gets/sets the Entity data required for this control.
		/// </summary>
		[Browsable(false)]
		public new IEntity[] DataSource
		{
			get
			{
				if (base.DataSource == null)
				{
					RaiseDataLoading();
				}
				return (IEntity[])base.DataSource;
			}
			set { base.DataSource = value; }
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
				if (ViewState["SelectedEntityIDs"] == null)
				{
					ViewState["SelectedEntityIDs"] = new Guid[] { };
				}
				return (Guid[])ViewState["SelectedEntityIDs"];
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
				
				foreach (ListItem item in Items)
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
			this.DataValueField = "ID";
		}
		
		protected override void OnInit(EventArgs e)
		{
			CssClass = "Field";
			
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
	}

	/// <summary>
	/// Displays a dropdown list for the Entity to select a Entity.
	/// </summary>
	public class EntitySelect<E> : EntitySelect
		where E : IEntity
	{
		protected bool DataPosted = false;

		#region Properties
		/// <summary>
		/// Gets/sets the Entity data required for this control.
		/// </summary>
		[Browsable(false)]
		public new E[] DataSource
		{
			get
			{
				if (base.DataSource == null)
				{
					RaiseDataLoading();
				}
				return Collection<E>.ConvertAll(base.DataSource);
			}
			set { base.DataSource = Collection<IEntity>.ConvertAll(value); }
		}

		/*/// <summary>
		/// Gets/sets the Entity data required for this control.
		/// </summary>
		[Browsable(false)]
		public new object DataSource
		{
			get
			{
				//if (base.DataSource == null)
				//{
				//    RaiseDataLoading();
				//}
				return base.DataSource;
			}
			set { base.DataSource = value; }
		}*/

			// TODO: Check if necessary
			/// <summary>
			/// Gets/sets the selected Entity.
			/// </summary>
			[Browsable(false)]
			[Bindable(true)]
			public new Guid SelectedEntityID
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
				if (value == Guid.Empty)
					SelectedEntityIDs = new Guid[] { };
				else
					SelectedEntityIDs = new Guid[] { value };
			}
		}

		/// <summary>
		/// Gets/sets the selected entities.
		/// </summary>
		[Browsable(false)]
		[Bindable(true)]
		public Guid[] SelectedEntityIDs
		{
			get
			{
				return base.SelectedEntityIDs;
			}
			set
			{
				base.SelectedEntityIDs = value;
			}
		}
		
		/// <summary>
		/// Gets/sets the selected entity.
		/// </summary>
		[Browsable(false)]
		[Bindable(true)]
		public E SelectedEntity
		{
			get
			{
				return (E)base.SelectedEntity;
			}
			set
			{
				base.SelectedEntity = value;
			}
		}

		/// <summary>
		/// Gets/sets the selected entity.
		/// </summary>
		[Browsable(false)]
		[Bindable(true)]
		public new E[] SelectedEntities
		{
			get
			{
				return (E[])(new Collection<E>(base.SelectedEntities).ToArray(typeof(E)));
			}
			set
			{
				base.SelectedEntities = Collection<IEntity>.ConvertAll(value);
			}
		}

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
					ViewState["NoSelectionText"] = "-- Select " + typeof(E).Name + " --";
				return (string)ViewState["NoSelectionText"]; }
			set { ViewState["NoSelectionText"] = value; }
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
				// If the HideNoSelection property hasn't been specified then set it
				// based on the display mode
				if (ViewState["HideNoSelection"] == null)
				{
					// If display mode is multiple then set HideNoSelection to true
					if (value == ListSelectionMode.Multiple)
						ViewState["HideNoSelection"] = true;
					else
						ViewState["HideNoSelection"] = false;
				}
			}
		}

		#endregion

		/// <summary>
		/// Sets the default data settings of the dropdownlist.
		/// </summary>
		public EntitySelect()
		{
			this.DataTextField = TextPropertyName;
			this.DataValueField = "ID";
		}

		/// <summary>
		/// Binds and populates the control.
		/// </summary>
		public override void DataBind()
		{
			using (LogGroup logGroup = LogGroup.Start("Data binding the EntitySelect control.", NLog.LogLevel.Debug))
			{
				if (DataSource != null)
				{
					LogWriter.Debug("DataSource property != null");
					// Organise the data.
					Collection<E> data = new Collection<E>(DataSource);
					data.Sort(TextPropertyName, Entities.SortDirection.Ascending);
					DataSource = (E[])data.ToArray(typeof(E));

					if (!DataPosted)
					{
						LogWriter.Debug("DataPosted == false");
						
						// Start the base binding functionality
						base.DataBind();

						Populate();
					}
					else
						LogWriter.Debug("DataPosted == true");
				}
				else
					LogWriter.Debug("DataSource property == null");
				
				
				// Select the appropriate list item.
				SelectItems();
			}
		}

		#region IPostBackDataHandler Members

		protected override void RaisePostDataChangedEvent()
		{
			LogWriter.Debug("Raising PostDataChanged event.");
			
			OnSelectedIndexChanged(EventArgs.Empty);
		}

		protected override bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
		{
			using (LogGroup logGroup = LogGroup.Start("Loading post data.", NLog.LogLevel.Debug))
			{
				//if (DataSource == null || DataSource.Length == 0)
				//{
				//	if (DataSource == null)
				//		LogWriter.Debug("DataSource == null");
				//
				//	if (DataSource.Length == 0)
				//		LogWriter.Debug("DataSource.Length == 0");
				//
				//	LogWriter.Debug("Raising DataLoading event to reload DataSource.");
				
				RaiseDataLoading();
				//}
				
				LogWriter.Debug("Post data key: " + postDataKey);
				
				
				string postValue = GetPostValue(postDataKey, postCollection);
				
				if (postValue != null)
					LogWriter.Debug("Post value: " + postValue.ToString());
				else
					LogWriter.Debug("Post value: [null]");
				
				E[] entities = GetPostedEntities(postValue);
				
				bool posted = IsDataPosted(entities);
				
				LogWriter.Debug("Data posted: " + posted.ToString());

				if (posted)
				{
					LogWriter.Debug("Setting posted IDs to SelectedEntityIDs");
					
					if (entities != null)
						LogWriter.Debug("Posted entities #: " + entities.Length.ToString());
					else
						LogWriter.Debug("Posted entities: [null]");
					
					SelectedEntityIDs = Collection<E>.GetIDs(entities);
				}
				
				DataPosted = posted;
			}
			return DataPosted;
		}
		#endregion
		
		protected Type MakeGenericType()
		{
			Type innerType = null;
			
			if (EntityType == null)
				throw new Exception("EntityType isn't specified.");

			//if (EntityType == null)
			innerType = EntitiesUtilities.GetType(EntityType);
			//else
			//    innerType = EntityType;
			
			if (innerType == null)
				throw new Exception("Inner type not configured or can't be identified.");
			
			LogWriter.Debug("Inner type: " + innerType.ToString());

			Type genericType = typeof(Collection<>).MakeGenericType(new System.Type[] { innerType });
			
			if (genericType == null)
				throw new Exception("Generic type not configured or can't be identified.");
			
			return genericType;
		}
		
		protected string GetPostValue(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
		{
			string postValue = String.Empty;
			
			if (postCollection != null)
				postValue = postCollection[postDataKey];
			
			return postValue;
		}
		
		protected E[] GetPostedEntities(string postValue)
		{
			LogWriter.Debug("Post value: " + postValue);
			
			E[] entities = new E[] {};
			
			if (postValue != null)
			{
				Guid[] ids = GetIDsFromString(postValue);
				
				LogWriter.Debug("Post value: !null");
				
				entities = Collection<E>.GetByIDs(DataSource, ids);
			}
			
			return entities;
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
		
		protected bool IsDataPosted(E[] entities)
		{
			bool posted = false;
			
			int existingCount = 0;
			if (SelectedEntityIDs != null)
				existingCount = SelectedEntityIDs.Length;
			
			int newCount = 0;
			if (entities != null)
				newCount = entities.Length;
			
			// If the new count matches the existing count
			if (existingCount == newCount)
			{
				LogWriter.Debug("SelectedEntityIDs.Length == entities.Count");
				
				// Loop through the IDs and compare
				foreach (Guid id in Collection<E>.GetIDs(entities))
				{
					LogWriter.Debug("Checking whether ID '" + id.ToString() + "' is in SelectedEntityIDs.");
					
					if (SelectedEntityIDs == null
					    || Array.IndexOf(SelectedEntityIDs, id) == -1)
					{
						LogWriter.Debug("ID is not in SelectedEntityIDs");
						
						posted = true;
					}
				}
			} // Otherwise a different count means data was posted
			else
			{
				posted = true;
			}
			
			return posted;
		}

		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{

			// Name and ID
			writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
			writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);

			// Size and display mode
			if (DisplayMode == ListSelectionMode.Multiple)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Size, Rows.ToString());
				writer.AddAttribute(HtmlTextWriterAttribute.Multiple, "multiple");
			}

			// Enabled
			if (!Enabled)
				writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");

			// Style class
			writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);

			// Height and width
			if (Height.Value > 0)
				writer.AddStyleAttribute(HtmlTextWriterStyle.Height, Utilities.FormatUnit(Height));
			if (Width.Value > 0)
				writer.AddStyleAttribute(HtmlTextWriterStyle.Width, Utilities.FormatUnit(Width));

			// AutoPostBack of SelectedIndexChanged event
			if (AutoPostBack)
			{
				writer.AddAttribute("onchange", "__doPostBack('" + this.ClientID + "','')");
			}


			// Skip base implementation
			//base.AddAttributesToRender (writer);


			foreach (string key in Attributes.Keys)
			{
				// Make sure the size element isn't added when it's not wanted
				if (key.ToLower() != "size" || DisplayMode == ListSelectionMode.Multiple)
					writer.AddAttribute(key, Attributes[key]);
			}
		}

		public virtual void Populate()
		{
			using (LogGroup logGroup = LogGroup.Start("Populating the EntitySelect control.", NLog.LogLevel.Debug))
			{
				Items.Clear();

				// Add the first item.
				if (!HideNoSelection)
				{
					LogWriter.Debug("HideNoSelection == false");
					LogWriter.Debug("Inserting the 'No Selection' item.");
					this.Items.Insert(0, new ListItem(NoSelectionText, Guid.Empty.ToString()));
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
								
								Items.Add(new ListItem(GetText(entity), GetValue(entity)));
								
								existingIDs.Add(entity.ID);
							}
							else
								LogWriter.Debug("Entity already added so it was skipped.");
						}
					}
				}
			}
		}
		
		public string GetText(IEntity entity)
		{
			string text = String.Empty;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving the text representation of the provided entity."))
			{
				PropertyInfo property = entity.GetType().GetProperty(TextPropertyName);
				
				if (property == null)
				{
					LogWriter.Debug("Text property '" + TextPropertyName + "' NOT found.");
					throw new Exception("Can't find '" + TextPropertyName + "' property on type '" + typeof(E).Name + "' as specified by the TextPropertyName property on the '" + ID + "' EntitySelect control.");
				}
				else
					LogWriter.Debug("Text property '" + TextPropertyName + "' found.");
				
				object value = property.GetValue(entity, null);
				
				if (value == null)
					LogWriter.Debug("Text property == null.");
				else
					LogWriter.Debug("Text property != null.");
				
				text = (value == null
				        ? String.Empty
				        : value.ToString());
				
				LogWriter.Debug("String value: " + text);
			}
			return text;
		}
		
		public string GetValue(IEntity entity)
		{
			string stringValue = String.Empty;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving the value representation of the provided entity."))
			{
				PropertyInfo property = entity.GetType().GetProperty(ValuePropertyName);
				
				if (property == null)
				{
					LogWriter.Debug("Value property '" + ValuePropertyName + "' NOT found.");
					throw new Exception("Can't find '" + ValuePropertyName + "' property on type '" + typeof(E).Name + "' as specified by the ValuePropertyName property on the '" + ID + "' EntitySelect control.");
				}
				else
					LogWriter.Debug("Value property '" + ValuePropertyName + "' found.");
				
				object value = property.GetValue(entity, null);
				
				if (value == null)
					LogWriter.Debug("Value property == null.");
				else
					LogWriter.Debug("Value property != null.");
				
				stringValue = (value == null
				               ? String.Empty
				               : value.ToString());
				
				LogWriter.Debug("String value: " + stringValue);
			}
			
			return stringValue;
		}

		public void Populate(E[] entities)
		{
			DataSource = entities;
			Populate();
		}
	}

	
	public class EntitySelectControlBuilder : ControlBuilder {
		public override void Init(TemplateParser parser, ControlBuilder parentBuilder, Type type, string tagName, string id,
		                          IDictionary attribs) {

			string entityTypeName = (string)attribs["EntityType"];

			if (entityTypeName != null || entityTypeName != String.Empty)
			{
				Type entityType = EntitiesUtilities.GetType(entityTypeName);
				if (entityType == null)
				{
					throw new Exception(string.Format("The '{0}' type cannot be found or is invalid/incomplete.", entityTypeName));
				}
				Type controlType = typeof(EntitySelect<>).MakeGenericType(entityType);
				base.Init(parser, parentBuilder, controlType, tagName, id, attribs);
			}
			else
				throw new Exception("The EntityType property must be set to the type of Entity being displayed in the control.");
		}
	}
}
