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
    public class EntityFormButtonsItem : EntityFormSpecificItem
    {
        /*private TextBox textBox = new TextBox();
        public TextBox TextBox
        {
            get { return textBox; }
            set { textBox = value; }
        }*/

        protected override void CreateFieldControls()
        {
           /* TextBox.ID = FieldControlID;
            FieldCell.Controls.Add(TextBox);*/
        }
    }
}
