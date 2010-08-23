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
    public class EntityFormPasswordConfirmItem : EntityFormSpecificItem
    {
    	
    	public string TextBoxCssClass
    	{
    		get {
    			if (ViewState["TextBoxCssClass"] == null)
    				ViewState["TextBoxCssClass"] = "Field";
    			return (string)ViewState["TextBoxCssClass"]; }
    		set { ViewState["TextBoxCssClass"] = value; }
    	}
    	
        private TextBox textBox = new TextBox();
        public TextBox TextBox
        {
            get { return textBox; }
            set { textBox = value; }
        }

        private CompareValidator compareValidator = new CompareValidator();
        public CompareValidator CompareValidator
        {
            get { return compareValidator; }
            set { compareValidator = value; }
        }

        [Bindable(true)]
        [Browsable(true)]
        public string CompareTo
        {
            get { return (string)ViewState["CompareTo"]; }
            set { ViewState["CompareTo"] = value; }
        }

        [Bindable(true)]
        [Browsable(true)]
        public string CompareToErrorMessage
        {
            get { return (string)ViewState["CompareToErrorMessage"]; }
            set { ViewState["CompareToErrorMessage"] = value;
            CompareValidator.ErrorMessage = value;
            }
        }

        protected override void CreateFieldControls()
        {
            TextBox.ID = FieldControlID;
            TextBox.TextMode = TextBoxMode.Password;

            CompareValidator.ControlToCompare = FieldControlID;
            CompareValidator.ControlToValidate = CompareTo;
            CompareValidator.ErrorMessage = CompareToErrorMessage;
            CompareValidator.Text = "&laquo;";

            FieldCell.Controls.Add(TextBox);
            FieldCell.Controls.Add(new LiteralControl("&nbsp;"));
            FieldCell.Controls.Add(CompareValidator);
        }
        
		protected override void OnLoad(EventArgs e)
		{
			ApplyCssClass(Controls);
			
			base.OnLoad(e);
		}
        
		private void ApplyCssClass(ControlCollection controls)
		{
			foreach (Control control in controls)
			{
				if (control is TextBox)
					((TextBox)control).CssClass = TextBoxCssClass;
				
				ApplyCssClass(control.Controls);
			}
		}
    }
}
