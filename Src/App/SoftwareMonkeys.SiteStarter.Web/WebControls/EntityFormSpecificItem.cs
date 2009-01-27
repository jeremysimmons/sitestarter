using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Web.UI.WebControls;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
    public abstract class EntityFormSpecificItem : EntityFormItem
    {
        /// <summary>
        /// Calls the CreateFieldControls function.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            CreateFieldControls();
        }

        /// <summary>
        /// Overridden by derived classes to add the field controls to the item.
        /// </summary>
        protected virtual void CreateFieldControls()
        {

        }
    }
}
