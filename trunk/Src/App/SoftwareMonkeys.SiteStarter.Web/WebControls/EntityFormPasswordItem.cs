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
    public class EntityFormPasswordItem : EntityFormSpecificItem
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

        protected override void CreateFieldControls()
        {
            TextBox.ID = FieldControlID;
            TextBox.TextMode = TextBoxMode.Password;

            FieldCell.Controls.Add(TextBox);

            base.CreateChildControls();
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
