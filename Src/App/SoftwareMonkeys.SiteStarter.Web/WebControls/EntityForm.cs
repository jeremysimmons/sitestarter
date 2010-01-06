using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
	/// <summary>
	/// Renders an interactive form for an entity.
	/// </summary>
	public class EntityForm : Table
	{
		// private Table table = new Table();
		protected TableRow HeadingRow = new TableRow();
		protected TableCell HeadingCell = new TableCell();

		/*private EntityFormItemCollection items;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public EntityFormItemCollection Items
        {
            get { return items; }
            set { items = value; }
        }*/



		#region Properties
		/// <summary>
		/// Gets/sets the heading text to be displayed on the form.
		/// </summary>
		[Bindable(true)]
		[Browsable(true)]
		public string HeadingText
		{
			get
			{
				if (ViewState["HeadingText"] == null)
					ViewState["HeadingText"] = "&nbsp;";
				return (string)ViewState["HeadingText"];
			}
			set { ViewState["HeadingText"] = value;
				HeadingCell.Text = value;
			}
		}

		/// <summary>
		/// Gets/sets the heading CSS class to be displayed on the form.
		/// </summary>
		[Bindable(true)]
		[Browsable(true)]
		public string HeadingCssClass
		{
			get
			{
				if (ViewState["HeadingCssClass"] == null)
					ViewState["HeadingCssClass"] = "Heading2"; // TODO: Adjust default HeadingCssClass
				return (string)ViewState["HeadingCssClass"];
			}
			set { ViewState["HeadingCssClass"] = value; }
		}

		/// <summary>
		/// Gets/sets the CSS class of each item text label.
		/// </summary>
		[Bindable(true)]
		[Browsable(true)]
		public string ItemTextCssClass
		{
			get
			{
				if (ViewState["ItemTextCssClass"] == null)
					ViewState["ItemTextCssClass"] = "";
				return (string)ViewState["ItemTextCssClass"];
			}
			set { ViewState["ItemTextCssClass"] = value; }
		}

		/// <summary>
		/// Gets/sets the data source to bind with.
		/// </summary>
		[Bindable(true)]
		[Browsable(true)]
		public object DataSource
		{
			get
			{
				return ViewState["DataSource"];
			}
			set { ViewState["DataSource"] = value; }
		}
		#endregion

		/// <summary>
		/// Fired when a button is clicked.
		/// </summary>
		public event EntityFormEventHandler EntityCommand;

		public EntityForm()
		{
		}

		protected override void OnInit(EventArgs e)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Initializing the EntityForm control.", NLog.LogLevel.Debug))
			{
				//Controls.Add(table);
				//table.Width = Unit.Percentage(100);
				//if (!Page.IsPostBack)
				//{
				HeadingCell.Text = HeadingText;
				HeadingCell.ColumnSpan = 2;
				HeadingCell.CssClass = HeadingCssClass;
				HeadingRow.Cells.Add(HeadingCell);
				Rows.AddAt(0, HeadingRow);
				
				TableRow newButtonsRow = null;
				foreach (TableRow row in Rows)
				{
					AppLogger.Debug("Row type: " + row.GetType().ToString());

					if (row is EntityFormButtonsItem)
					{
						EntityFormButtonsItem item = (EntityFormButtonsItem)row;

						AppLogger.Debug("Binding item with field control ID: " + item.FieldControlID);
						
						newButtonsRow = CopyButtonsRow(item);
						
						foreach (Control control in item.Cells[1].Controls)
						{
							if (control is Button)
							{
								HandleEvents((Button)control);
							}
						}
						
						if (newButtonsRow != null)
						{
							foreach (Control control in newButtonsRow.Cells[0].Controls)
							{
								if (control is Button)
								{
									HandleEvents((Button)control);
								}
							}
						}
					}
					else if (row is EntityFormItem || row.GetType().BaseType == typeof(EntityFormItem))
					{
						EntityFormItem item = (EntityFormItem)row;

						AppLogger.Debug("Binding item with field control ID: " + item.FieldControlID);
						
						// If a custom CSS class hasn't been set on the object use the default one
						if (item.TextCssClass != String.Empty)
							item.TextCssClass = ItemTextCssClass;
						
						foreach (Control control in item.Cells[1].Controls)
						{
							if (control is Button)
							{
								// HandleEvents((Button)control);
							}
						}
						
						//Rows.Add(item);
					}
					else if (row is TableRow)
					{
						// Rows.Add(row);
					}
				}
				if (newButtonsRow != null)
					Rows.AddAt(0, newButtonsRow);
				
				//}
			}
			
			base.OnInit(e);
		}

		private TableRow CopyButtonsRow(EntityFormButtonsItem item)
		{
			/*   TableRow newRow = new TableRow();
            TableCell newCell = new TableCell();
            newRow.Cells.Add(newCell);
            newCell.ColumnSpan = 2;

            foreach (Control control in item.Cells[1].Controls)
            {
                if (control is Button)
                {
                    Button button = new Button();
                    button.Text = ((Button)control).Text;
                    button.CommandName = ((Button)control).CommandName;
                    button.CommandArgument = ((Button)control).CommandArgument;
                    button.Visible = ((Button)control).Visible;
                    newCell.Controls.Add(button);
                }
                else if (control is LiteralControl)
                {
                    LiteralControl literal = new LiteralControl(((LiteralControl)control).Text);
                    newCell.Controls.Add(literal);
                }
            }

            return newRow;*/
			return null;
		}

		public override void DataBind()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Binding the DataSource data to the values of the fields in the EntityForm.", NLog.LogLevel.Debug))
			{
				base.DataBind();

				foreach (TableRow row in Rows)
				{
					if (DataSource != null)
					{
						if (row is EntityFormItem)
						{
							EntityFormItem item = (EntityFormItem)row;
							string propertyName = ((EntityFormItem)item).PropertyName;
							
							if (item.AutoBind && item.PropertyName != String.Empty && item.PropertyName != null)
							{
								using (LogGroup logGroup2 = AppLogger.StartGroup("Property: " + propertyName, NLog.LogLevel.Debug))
								{
									object propertyValue = Reflector.GetPropertyValue(DataSource, propertyName);

									if (propertyValue == null)
										AppLogger.Debug("PropertyValue: [null]");
									else
										AppLogger.Debug("Property value: " + propertyValue.ToString());
									
									Control field = FindControl(item.FieldControlID);
									
									EntityFormHelper.SetFieldValue(field, propertyValue, item.ControlValuePropertyName, null);
								}
							}
						}
					}
				}
			}
		}

		public void ReverseBind(object obj)
		{
			DataSource = obj;
			ReverseBind();
		}

		// TODO: Remove
		/*public void ReverseBind()
        {
            foreach (TableRow row in this.Rows)
            {
                if (row is EntityFormItem)
                {
                    EntityFormItem item = (EntityFormItem)row;
                    PropertyInfo property = DataSource.GetType().GetProperty(((EntityFormItem)item).PropertyName);
                    if (property != null)
                    {
                        Control field = FindControl(item.FieldControlID);
                        // Skip label fields, they're not editable
                        if (field.GetType() != typeof(Label))
                        {
                            object value = EntityFormHelper.GetFieldValue(field, item.ControlValuePropertyName, property.PropertyType);
                            // Collection<Entity> types need to be cast back to collection of base Entity objects to work
                            // TODO: Check if needed
                       //     if (value is Collection<Entity>)
                      //      {
                      //          property.SetValue(DataSource, value), null);
                      //      }
                     //       else
                            property.SetValue(DataSource, value, null);
                        }
                    }
                }
            }
        }*/

		public void ReverseBind()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Transferring data from form fields to entity.", NLog.LogLevel.Debug))
			{
				foreach (TableRow row in this.Rows)
				{
					if (row is EntityFormItem)
					{
						EntityFormItem item = (EntityFormItem)row;
						//PropertyInfo property = Reflector.GetProperty(((EntityFormItem)item).PropertyName, DataSource);
						
						if (item.AutoBind && item.PropertyName != null && item.PropertyName != String.Empty)
						{
							AppLogger.Debug("Property name: " + item.PropertyName);
							//AppLogger.Debug("Property type: " + property.PropertyType.ToString());
							
							//if (property != null)
							//{
							//	AppLogger.Debug("Property found");
							
							Control field = FindControl(item.FieldControlID);
							// Skip label fields, they're not editable
							if (field.GetType() != typeof(Label))
							{
								Type propertyType = Reflector.GetPropertyType(DataSource, item.PropertyName);
								
								AppLogger.Debug("Property type: " + (propertyType == null ? "[null]" : propertyType.ToString()));
								
								object value = EntityFormHelper.GetFieldValue(field, item.ControlValuePropertyName, propertyType);
								
								AppLogger.Debug("Field value type: " + (value == null ? "[null]" : value.GetType().ToString()));
								AppLogger.Debug("Field value: " + (value == null ? "[null]" : value.ToString()));
														
								object castValue = EntityFormHelper.Convert(value, propertyType);
								
								Reflector.SetPropertyValue(DataSource, item.PropertyName, castValue);
							}
							//}
							//else
							//	AppLogger.Debug("Property not found.");
						}
					}
				}
			}
		}

		/* protected override void Render(HtmlTextWriter writer)
        {


            table.RenderControl(writer);
        }*/

		public void HandleEvents(Button button)
		{
			button.Click += new EventHandler(Button_Click);
		}

		private void Button_Click(object sender, EventArgs e)
		{
			RaiseEntityCommand(sender);
		}

		#region Event raisers
		protected virtual void RaiseEntityCommand(object sender)
		{
			if (EntityCommand != null)
			{
				if (sender is Button)
				{
					Button button = ((Button)sender);

					EntityCommand(sender, new EntityFormEventArgs(button.CommandName, button.CommandArgument));//button.CommandName, button.CommandArgument));
				}
				else
					throw new NotSupportedException(sender.GetType().Name + " type is not supported to raise EntityCommand event.");
			}
		}
		#endregion
	}

	#region Event types
	public delegate void EntityFormEventHandler(object sender, EntityFormEventArgs e);

	public class EntityFormEventArgs : EventArgs
	{
		private string commandName;
		public string CommandName
		{
			get { return commandName; }
		}

		private string commandArgument;
		public string CommandArgument
		{
			get { return commandArgument; }
		}

		public EntityFormEventArgs(string commandName, string commandArgument)
		{
			this.commandName = commandName;
			this.commandArgument = commandArgument;
		}
	}
	#endregion
}