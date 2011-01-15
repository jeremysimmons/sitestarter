using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Web.UI;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
    /// <summary>
    /// Represents an item in the entity form.
    /// </summary>
    public class EntityFormItem : TableRow
    {
        protected TableCell LabelCell = new TableCell();
        protected TableCell FieldCell = new TableCell();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new TableCellCollection Cells
        {
            get { return base.Cells; }
            set { }
        }

        protected RequiredFieldValidator ReqVal;

        private ITemplate fieldTemplate;
       // [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate FieldTemplate
        {
            get { return fieldTemplate; }
            set { fieldTemplate = value; }
        }

        /// <summary>
        /// The text shown as the item's text label.
        /// </summary>
        [Bindable(true)]
        [Browsable(true)]
        public string Text
        {
            get
            {
                if (ViewState["Text"] == null)
                    ViewState["Text"] = "[no text]";
                return (string)ViewState["Text"];
            }
            set
            {
                ViewState["Text"] = value;
                if (IsRequired)
                    LabelCell.Text = "<span class='Required'>*</span>&nbsp;" + value;
                else
                    LabelCell.Text = "&nbsp;&nbsp;&nbsp;" + value;
            }
        }
        
        private bool autoBind = true;
        /// <summary>
        /// Gets/sets a flag indicating whether the field controls should automatically be populated.
        /// </summary>
        public bool AutoBind
        {
        	get { return autoBind; }
        	set { autoBind = value; }
        }

        /// <summary>
        /// The ID of the control that will be involved in the two way binding.
        /// </summary>
        [Bindable(true)]
        [Browsable(true)]
        public string FieldControlID
        {
            get
            {
                if (ViewState["FieldControlID"] == null || (string)ViewState["FieldControlID"] == String.Empty)
                    ViewState["FieldControlID"] = PropertyName;
                return (string)ViewState["FieldControlID"];
            }
            set
            {
                ViewState["FieldControlID"] = value;
                if (ReqVal != null)
                    ReqVal.ControlToValidate = value;
            }
        }

        /// <summary>
        /// The name of the property on the data source to bind with.
        /// </summary>
        [Bindable(true)]
        [Browsable(true)]
        public string PropertyName
        {
            get
            {
                if (ViewState["PropertyName"] == null)
                    ViewState["PropertyName"] = "";
                return (string)ViewState["PropertyName"];
            }
            set
            {
                if (FieldControlID == String.Empty || FieldControlID != (string)ViewState["PropertyName"])
                    FieldControlID = value;
                ViewState["PropertyName"] = value;
                
            }
        }

        /// <summary>
        /// The name of the property on the data source to bind with (for use on custom control types).
        /// </summary>
        [Bindable(true)]
        [Browsable(true)]
        public string ControlValuePropertyName
        {
            get
            {
                if (ViewState["ControlValuePropertyName"] == null)
                    ViewState["ControlValuePropertyName"] = "";
                return (string)ViewState["ControlValuePropertyName"];
            }
            set
            {
                ViewState["ControlValuePropertyName"] = value;

            }
        }
        /// <summary>
        /// The CSS class used on the item's text label.
        /// </summary>
        [Bindable(true)]
        [Browsable(true)]
        public string TextCssClass
        {
            get
            {
                if (ViewState["TextCssClass"] == null)
                    ViewState["TextCssClass"] = "";
                return (string)ViewState["TextCssClass"];
            }
            set { ViewState["TextCssClass"] = value;
            LabelCell.CssClass = value;
            }
        }

        /// <summary>
        /// Gets/sets a value indicating whether or not the field is required.
        /// </summary>
        [Bindable(true)]
        [Browsable(true)]
        public bool IsRequired
        {
            get
            {
                if (ViewState["IsRequired"] == null)
                    ViewState["IsRequired"] = false;
                return (bool)ViewState["IsRequired"];
            }
            set
            {
                ViewState["IsRequired"] = value;
                if (Text != null)
                {
                    if (value)
                        Text = Text.Replace("&nbsp;&nbsp;&nbsp;", "<span class='Required'>*</span>&nbsp;");
                    else
                        Text = Text.Replace("<span class='Required'>*</span>&nbsp;", "&nbsp;&nbsp;&nbsp;");
                }
                if (ReqVal != null)
                    ReqVal.Enabled = value;
            }
        }

        /// <summary>
        /// Gets/sets the error displayed if the field is required but no value is entered or selected.
        /// </summary>
        [Bindable(true)]
        [Browsable(true)]
        public string RequiredErrorMessage
        {
            get
            {
                if (ViewState["RequiredErrorMessage"] == null)
                    ViewState["RequiredErrorMessage"] = FieldControlID + " required.";
                return (string)ViewState["RequiredErrorMessage"];
            }
            set
            {
                ViewState["RequiredErrorMessage"] = value;
            }
        }
       
        /// <summary>
        /// Gets/sets a boolean value indicating whether to hide the label cell.
        /// </summary>
        public bool HideLabelCell
        {
            get {
                if (ViewState["HideLabelCell"] == null)
                    ViewState["HideLabelCell"] = false;
                return (bool)ViewState["HideLabelCell"]; }
            set { ViewState["HideLabelCell"] = value;
            LabelCell.Visible = !value;
            if (value)
                FieldCell.ColumnSpan = 2;
            else
                FieldCell.ColumnSpan = 1;
            }
        }

        //private ControlCollection Controls

        protected override void OnInit(EventArgs e)
        {
            //Controls.Add(row);

            Cells.Add(LabelCell);
            Cells.Add(FieldCell);

            if (FieldTemplate != null)
                FieldTemplate.InstantiateIn(FieldCell);

            /*foreach (Control control in Controls)
            {
                fieldCell.Controls.Add(control);
            }*/

            ReqVal = new RequiredFieldValidator();

            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (IsRequired)
            {
                ReqVal.ControlToValidate = FieldControlID;
                //ReqVal.ErrorMessage = RequiredErrorMessage;

                ReqVal.Text = " &laquo; ";
                ReqVal.ID = FieldControlID + "ReqVal";
                FieldCell.Controls.Add(ReqVal);
            }

            base.OnLoad(e);
        }

        /*public void AddToTable(Table table)
        {
            table.Rows.Add(row);
        }*/

        /*protected override void Render(HtmlTextWriter writer)
        {
            row.RenderControl(writer);
        }*/

        public override void DataBind()
        {
            base.DataBind();

            if (IsRequired)
            {
                ReqVal.ControlToValidate = FieldControlID == String.Empty ? PropertyName : FieldControlID;
                ReqVal.ErrorMessage = RequiredErrorMessage;
            }
        }
    }
}
