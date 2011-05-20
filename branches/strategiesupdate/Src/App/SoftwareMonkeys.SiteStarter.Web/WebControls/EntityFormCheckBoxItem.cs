using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Web.UI;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
    /// <summary>
    /// Represents a item containing a CheckBox in the entity form.
    /// </summary>
    public class EntityFormCheckBoxItem : EntityFormSpecificItem
    {
        private CheckBox checkBox = new CheckBox();
        public CheckBox CheckBox
        {
            get { return checkBox; }
            set { checkBox = value; }
        }

        protected override void CreateFieldControls()
        {
            CheckBox.ID = FieldControlID;
            FieldCell.Controls.Add(CheckBox);
        }
    }
}
