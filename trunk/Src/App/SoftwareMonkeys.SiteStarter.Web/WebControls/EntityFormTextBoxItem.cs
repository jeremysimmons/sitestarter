using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Web.UI;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
    /// <summary>
    /// Displays a form item with a text box.
    /// </summary>
    public class EntityFormTextBoxItem : EntityFormSpecificItem
    {        	
        private TextBox textBox = new TextBox();
        public TextBox TextBox
        {
            get { return textBox; }
            set { textBox = value; }
        }

        protected override void CreateFieldControls()
        {
            TextBox.ID = FieldControlID;
            FieldCell.Controls.Add(TextBox);
        }
    }
}
