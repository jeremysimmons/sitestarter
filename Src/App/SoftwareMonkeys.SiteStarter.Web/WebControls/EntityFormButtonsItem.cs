using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Web.UI;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
    /// <summary>
    /// Represents a buttons item in the entity form.
    /// </summary>
    public class EntityFormButtonsItem : EntityFormSpecificItem
    {        
    	public string ButtonCssClass
    	{
    		get {
    			if (ViewState["ButtonCssClass"] == null)
    				ViewState["ButtonCssClass"] = "FormButton";
    			return (string)ViewState["ButtonCssClass"]; }
    		set { ViewState["ButtonCssClass"] = value; }
    	}
    	
        public EntityFormButtonsItem()
        {
        	AutoBind = false;
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
				if (control is Button)
					((Button)control).CssClass = ButtonCssClass;
				
				ApplyCssClass(control.Controls);
			}
		}

		public void Add(Button button)
		{
			Cells[1].Controls.Add(button);
		}
    }
}
