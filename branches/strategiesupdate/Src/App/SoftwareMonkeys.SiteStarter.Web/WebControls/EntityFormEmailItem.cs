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
    public class EntityFormEmailItem : EntityFormSpecificItem
    {
        private TextBox textBox = new TextBox();
        public TextBox TextBox
        {
            get { return textBox; }
            set { textBox = value; }
        }

        [Bindable(true)]
        [Browsable(true)]
        public string InvalidEmailError
        {
            get { return (string)ViewState["InvalidEmailError"]; }
            set { ViewState["InvalidEmailError"] = value; }
        }

        protected override void CreateFieldControls()
        {
            TextBox.ID = FieldControlID;

            RegularExpressionValidator regExpVal = new RegularExpressionValidator();
            regExpVal.ValidationExpression = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            regExpVal.ControlToValidate = FieldControlID;
            regExpVal.ErrorMessage = InvalidEmailError;
            regExpVal.Text = " &laquo; ";

            FieldCell.Controls.Add(TextBox);
            FieldCell.Controls.Add(regExpVal);
        }
    }
}
