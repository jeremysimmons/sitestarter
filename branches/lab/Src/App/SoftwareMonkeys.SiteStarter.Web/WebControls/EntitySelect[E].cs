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
		#endregion

		/// <summary>
		/// Empty constructor.
		/// </summary>
		public EntitySelect()
		{
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
					data.Sort(ValuePropertyName, Entities.SortDirection.Ascending);
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

		// TODO: Check if needed. Should be obsolete.
		/*protected override void RaisePostDataChangedEvent()
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
		}*/
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
				// TODO: Clean up
				//writer.AddAttribute(HtmlTextWriterAttribute.Size, Rows.ToString());
				//writer.AddAttribute(HtmlTextWriterAttribute.Multiple, "multiple");
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
