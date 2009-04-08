using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;
using System.Reflection;
using System.Xml.Serialization;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
    [ControlBuilder(typeof(EntitySelectControlBuilder))]
    public class EntitySelect : ListBox, IPostBackDataHandler
    {
        private string entityType = typeof(SoftwareMonkeys.SiteStarter.Entities.BaseEntity).FullName;
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
        public new BaseEntity[] DataSource
        {
            get
            {
                if (base.DataSource == null)
                {
                    RaiseDataLoading();
                }
                return (BaseEntity[])base.DataSource;
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
        public BaseEntity SelectedEntity
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
                SelectedEntities = new BaseEntity[] { value };
            }
        }

        private BaseEntity[] selectedEntities;
        /// <summary>
        /// Gets/sets the selected entities.
        /// </summary>
        [Browsable(false)]
        [Bindable(true)]
        public BaseEntity[] SelectedEntities
        {
            get
            {
                if (selectedEntities == null)
                {
                    if (DataSource != null && GetDataSourceLength() > 0)
                        selectedEntities = EntityFactory.GetEntities(DataSource.GetType().GetElementType(), SelectedEntityIDs);
                }
                return selectedEntities;
            }
            set
            {
                selectedEntities = value;
                SelectedEntityIDs = Collection<BaseEntity>.GetIDs(value);
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
                ViewState["SelectedEntityIDs"] = value;
                SelectItems();
            }
        }

        /// <summary>
        /// Selects the appropriate items depending on the SelectedEntityIDs.
        /// </summary>
        protected void SelectItems()
        {
            if (ViewState["SelectedEntityIDs"] != null)
            {
                if (((Guid[])ViewState["SelectedEntityIDs"]).Length > 0)
                {
                    foreach (ListItem item in Items)
                    {
                        item.Selected = (Array.IndexOf((Guid[])ViewState["SelectedEntityIDs"], new Guid(item.Value)) > -1);
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
        where E : BaseEntity
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
		                return (E[])base.DataSource;
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
        public E[] SelectedEntities
        {
            get
            {
                return (E[])(new Collection<E>(base.SelectedEntities).ToArray(typeof(E)));
            }
            set
            {
                base.SelectedEntities = value;
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
            if (DataSource != null)
            {
                // Organise the data.
                Collection<BaseEntity> data = new Collection<BaseEntity>(DataSource);
                data.Sort(ValuePropertyName, Entities.SortDirection.Ascending);
                DataSource = (E[])data.ToArray(typeof(E));

                if (!DataPosted)
                {
                    // Start the base binding functionality
                    base.DataBind();

                    Populate();

                    // Select the appropriate list item.
                    SelectItems();
                }
            }
        }

		#region IPostBackDataHandler Members

		protected void RaisePostDataChangedEvent()
		{
            OnSelectedIndexChanged(EventArgs.Empty);
		}

        protected override bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            string postValue = postCollection[postDataKey];
            Type innerType = null;

            //if (EntityType == null)
                innerType = EntityFactory.GetType(EntityType);
            //else
            //    innerType = EntityType;

            Type genericType = typeof(Collection<>).MakeGenericType(new System.Type[] { innerType });
            Collection<E> entities = (Collection<E>)Activator.CreateInstance(genericType);
            if (postValue != null)
            {
                foreach (string stringID in postValue.Split(','))
                {
                    Guid id = new Guid(stringID);
                    E entity = null;
                    if (DataSource != null)
                        entity = (E)Collection<E>.GetByID((E[])DataSource, id);
                    else
                        entity = (E)EntityFactory.GetEntity(typeof(E), id);
                    entities.Add(entity);
                }
            }
            bool posted = false;
            if (SelectedEntityIDs.Length == entities.Count)
            {
                foreach (Guid id in entities.GetIDs())
                {
                    if (Array.IndexOf(SelectedEntityIDs, id) == -1)
                        posted = true;
                }
            }
            else
                posted = true;

            if (posted)
                SelectedEntityIDs = entities.GetIDs();
            return DataPosted = posted;
        }
		#endregion

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
            Items.Clear();

            // Add the first item.
            if (!HideNoSelection)
                this.Items.Insert(0, new ListItem(NoSelectionText, Guid.Empty.ToString()));

            // TODO: Check if code is necessary
            // Add the rest of the items
            foreach (BaseEntity entity in (IEnumerable)DataSource)
            {
                ArrayList existingIDs = new ArrayList();
                if (!existingIDs.Contains(entity.ID))
                {
                    PropertyInfo property = entity.GetType().GetProperty(ValuePropertyName);
                    object value = property.GetValue(entity, null);
                    Items.Add(new ListItem(value == null ? String.Empty : value.ToString(), entity.ID.ToString()));
                    existingIDs.Add(entity.ID);
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
