using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Web.UI;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
    /// <summary>
    /// An item for entering keywords.
    /// </summary>
    public class EntityFormKeywordsItem : EntityFormSpecificItem
    {
        private KeywordsControl keywordsControl = new KeywordsControl();
        public KeywordsControl KeywordsControl
        {
            get { return keywordsControl; }
            set { keywordsControl = value; }
        }

        protected override void CreateFieldControls()
        {
            KeywordsControl.ID = FieldControlID;
            FieldCell.Controls.Add(KeywordsControl);
            ControlValuePropertyName = "Keywords";
            KeywordsControl.Width = Unit.Pixel(400);
        }
    }
}
