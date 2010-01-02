using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;
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

		/* private Type entityType;
        /// <summary>
        /// Gets/sets the type of entity being displayed in the list.
        /// </summary>
        public virtual Type EntityType
        {
            get
            {
                return entityType;
            }
            set { entityType = value; }
        }*/

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

		/*/// <summary>
		/// Gets/sets the Entity data required for this control.
		/// </summary>
		[Browsable(false)]
		public new object DataSource
		{
			get
			{
				return base.DataSource;
			}
			set { base.DataSource = value; }
		}*/

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
						selectedEntities = (IEntity[])Data.DataAccess.Data.GetEntities(EntitiesUtilities.GetType(EntityType), SelectedEntityIDs);
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
				using (LogGroup logGroup = AppLogger.StartGroup("Setting the SelectedEntityIDs property of the EntitySelect with ID " + ID + ".", NLog.LogLevel.Debug))
				{
					if (value != null)
						AppLogger.Debug("#: " + value.Length.ToString());
					else
						AppLogger.Debug("#: " + 0);
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
			using (LogGroup logGroup = AppLogger.StartGroup("Selecting the items that match the info specified.", NLog.LogLevel.Debug))
			{
				Guid[] selectedEntityIDs = new Guid[] {};
				
				if (ViewState["SelectedEntityIDs"] != null)
				{
					AppLogger.Debug("ViewState[\"SelectedEntityIDs\"] != null");
					
					selectedEntityIDs = (Guid[])ViewState["SelectedEntityIDs"];
				}
				else
					AppLogger.Debug("ViewState[\"SelectedEntityIDs\"] == null");
				
				
				AppLogger.Debug("# of selected entities: " + selectedEntityIDs.Length.ToString());
				
				foreach (ListItem item in Items)
				{
					using (LogGroup logGroup2 = AppLogger.StartGroup("Selecting/deselecting item.", NLog.LogLevel.Debug))
					{
						AppLogger.Debug("item.Text: " + item.Text);
						AppLogger.Debug("item.Value: " + item.Value);
						item.Selected = (Array.IndexOf(selectedEntityIDs, new Guid(item.Value)) > -1);
						AppLogger.Debug("item.Selected: " + item.Selected.ToString());
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
			set { ViewState["DisplayMode"] = value; }
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
					ViewState["ValuePropertyName"] = DataTextField = "Name";
				}
				return (string)ViewState["ValuePropertyName"];
			}
			set { ViewState["ValuePropertyName"] = value;
				DataTextField = value;
			}
		}
		#endregion

		/// <summary>
		/// Sets the default data settings of the dropdownlist.
		/// </summary>
		public EntitySelect()
		{
			this.DataTextField = ValuePropertyName;
			this.DataValueField = "ID";
		}

		/// <summary>
		/// Binds and populates the control.
		/// </summary>
		public override void DataBind()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Data binding the EntitySelect control.", NLog.LogLevel.Debug))
			{
				if (DataSource != null)
				{
					AppLogger.Debug("DataSource property != null");
					// Organise the data.
					Collection<E> data = new Collection<E>(DataSource);
					data.Sort(ValuePropertyName, Entities.SortDirection.Ascending);
					DataSource = (E[])data.ToArray(typeof(E));

					if (!DataPosted)
					{
						AppLogger.Debug("DataPosted == false");
						
						// Start the base binding functionality
						base.DataBind();

						Populate();
					}
					else
						AppLogger.Debug("DataPosted == true");
				}
				else
					AppLogger.Debug("DataSource property == null");
				
				
				// Select the appropriate list item.
				SelectItems();
			}
		}

		#region IPostBackDataHandler Members

		protected void RaisePostDataChangedEvent()
		{
			AppLogger.Debug("Raising PostDataChanged event.");
			
			OnSelectedIndexChanged(EventArgs.Empty);
		}

		protected override bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Loading post data.", NLog.LogLevel.Debug))
			{
				//if (DataSource == null || DataSource.Length == 0)
				//{
				//	if (DataSource == null)
				//		AppLogger.Debug("DataSource == null");
				//	
				//	if (DataSource.Length == 0)
				//		AppLogger.Debug("DataSource.Length == 0");
				//	
				//	AppLogger.Debug("Raising DataLoading event to reload DataSource.");
					
					RaiseDataLoading();
				//}
				
				AppLogger.Debug("Post data key: " + postDataKey);
				
				
				string postValue = GetPostValue(postDataKey, postCollection);
				
				E[] entities = GetPostedEntities(postValue);
				
				bool posted = IsDataPosted(entities);
				
				AppLogger.Debug("Data posted: " + posted.ToString());

				if (posted)
				{
					AppLogger.Debug("Setting posted IDs to SelectedEntityIDs");
					
					AppLogger.Debug("Posted entities #: " + entities.Length.ToString());
					
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
			innerType = EntityFactory.GetType(EntityType);
			//else
			//    innerType = EntityType;
			
			if (innerType == null)
				throw new Exception("Inner type not configured or can't be identified.");
			
			AppLogger.Debug("Inner type: " + innerType.ToString());

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
			AppLogger.Debug("Post value: " + postValue);
			
			Type genericType = MakeGenericType();
			
			AppLogger.Debug("Generic type: " + genericType.ToString());
			
			Collection<E> entities = (Collection<E>)Activator.CreateInstance(genericType);
			
			
			if (entities == null)
				throw new Exception("Generic collection for type '" + typeof(E).ToString() + "' cannot be instantiated.");
			
			
			if (postValue != null)
			{
				AppLogger.Debug("Post value: !null");
				
				foreach (string stringID in postValue.Split(','))
				{
					AppLogger.Debug("Post contains ID: " + stringID);
					
					Guid id = new Guid(stringID);
					
					if (id != Guid.Empty)
					{
						
						E entity = default(E);
						
						// TODO: Check if needed. Was throwing errors so it's been removed to simplify code.
						// ////!May incur performance hit though by always reloading entities from DB instead of DataSource property
						//if (DataSource != null)
						//{
						if (DataSource == null)
							throw new Exception("DataSource == null");
						
						//AppLogger.Debug("DataSource != null");
						
						AppLogger.Debug("Getting entity from DataSource");
						
						entity = (E)Collection<E>.GetByID((E[])DataSource, id);
						
						if (entity == null)
							throw new Exception("Entity could not be retrieved from DataSource property.");
						
						AppLogger.Debug("Found entity from post ID: " + entity.GetType().ToString());
						//}
						//else
						//{
						//	AppLogger.Debug("DataSource == null");
						
						//	AppLogger.Debug("Getting entity from EntityFactory");
						
						//	entity = (E)EntityFactory.GetEntity<E>(id);
						
						//	AppLogger.Debug("Found entity from post ID: " + typeof(E).ToString());
						//}
						
						AppLogger.Debug("Adding entity to list.");
						entities.Add(entity);
					}
				}
			}
			
			return entities.ToArray();
		}
		
		protected bool IsDataPosted(E[] entities)
		{
			bool posted = false;
			
			int existingCount = 0;
			if (SelectedEntityIDs != null)
				existingCount = SelectedEntityIDs.Length;
			
			// If the new count matches the existing count
			if (existingCount == entities.Length)
			{
				AppLogger.Debug("SelectedEntityIDs.Length == entities.Count");
				
				// Loop through the IDs and compare
				foreach (Guid id in Collection<E>.GetIDs(entities))
				{
					AppLogger.Debug("Checking whether ID '" + id.ToString() + "' is in SelectedEntityIDs.");
					
					if (SelectedEntityIDs == null
					    || Array.IndexOf(SelectedEntityIDs, id) == -1)
					{
						AppLogger.Debug("ID is not in SelectedEntityIDs");
						
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
			using (LogGroup logGroup = AppLogger.StartGroup("Populating the EntitySelect control.", NLog.LogLevel.Debug))
			{
				Items.Clear();

				// Add the first item.
				if (!HideNoSelection)
				{
					AppLogger.Debug("HideNoSelection == false");
					AppLogger.Debug("Inserting the 'No Selection' item.");
					this.Items.Insert(0, new ListItem(NoSelectionText, Guid.Empty.ToString()));
				}
				else
				{
					AppLogger.Debug("HideNoSelection == true");
					AppLogger.Debug("NOT inserting the 'No Selection' item.");
				}

				using (LogGroup logGroup2 = AppLogger.StartGroup("Looping through DataSource entities.", NLog.LogLevel.Debug))
				{
					// TODO: Check if code is necessary
					// Add the rest of the items
					foreach (IEntity entity in (IEnumerable)DataSource)
					{
						using (LogGroup logGroup3 = AppLogger.StartGroup("Adding entity to control.", NLog.LogLevel.Debug))
						{
							AppLogger.Debug("Entity type: " + entity.GetType().ToString());
							AppLogger.Debug("Entity ID: " + entity.ID.ToString());
							
							// Create a list of each entity ID that gets added.
							ArrayList existingIDs = new ArrayList();
							
							// Check if the entity has already been added
							if (!existingIDs.Contains(entity.ID))
							{
								AppLogger.Debug("Adding entity.");
								
								PropertyInfo property = entity.GetType().GetProperty(ValuePropertyName);
								
								if (property == null)
									AppLogger.Debug("Value property '" + ValuePropertyName + "' NOT found.");
								else
									AppLogger.Debug("Value property '" + ValuePropertyName + "' found.");
								
								object value = property.GetValue(entity, null);
								
								if (value == null)
									AppLogger.Debug("Value property == null.");
								else
									AppLogger.Debug("Value property != null.");
								
								string stringValue = (value == null
								                      ? String.Empty
								                      : value.ToString());
								
								AppLogger.Debug("String value: " + stringValue);
								
								Items.Add(new ListItem(stringValue, entity.ID.ToString()));
								
								existingIDs.Add(entity.ID);
							}
							else
								AppLogger.Debug("Entity already added so it was skipped.");
						}
					}
				}
			}
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
				Type entityType = EntityFactory.GetType(entityTypeName);
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
