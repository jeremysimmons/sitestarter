using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Web.UI;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
    /// <summary>
    /// Represents an item in the entity form with a text label on it.
    /// </summary>
    public class EntityFormLabelItem : EntityFormSpecificItem
    {
        private Label label = new Label();
        public Label Label
        {
            get { return label; }
            set { label = value; }
        }

       /* [Bindable(true)]
        [Browsable(true)]
        public string Text
        {
            get { return label.Text; }
            set { label.Text = value; }
        }*/

        protected override void CreateFieldControls()
        {
            Label.ID = FieldControlID;
            FieldCell.Controls.Add(Label);
        }
    }
}
