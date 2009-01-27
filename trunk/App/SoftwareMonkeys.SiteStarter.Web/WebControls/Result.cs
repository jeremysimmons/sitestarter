using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SoftwareMonkeys.SiteStarter.Web.WebControls
{
    /// <summary>
    /// Displays the result of an action.
    /// </summary>
    public class Result : Panel
    {
        /// <summary>
        /// Gets/sets a value indicating whether the result is an error.
        /// </summary>
        public bool Error
        {
            get
            {
                if (HttpContext.Current.Session["Result_Error"] == null)
                    return false;
                return (bool)HttpContext.Current.Session["Result_Error"];
            }
        }

        /// <summary>
        /// Gets/sets the text result.
        /// </summary>
        public string Text
        {
            get
            {
                if (HttpContext.Current.Session["Result_Text"] == null)
                    HttpContext.Current.Session["Result_Text"] = String.Empty;
                return (string)HttpContext.Current.Session["Result_Text"];
            }
            set { HttpContext.Current.Session["Result_Text"] = value; }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            // Only display the control if it is marked as visible and a message is available
            if (Visible && Text != String.Empty)
            {
                // Use the correct CSS class depending on whether it is showing an error
                if (Error)
                    CssClass = "Error";
                else
                    CssClass = "Message";

                // Add the text to the control
                Controls.Add(new LiteralControl(Text));

                // Render the control
                base.Render(writer);

                // Reset the text because it has been displayed now
                HttpContext.Current.Session["Result_Text"] = String.Empty;
                HttpContext.Current.Session["Result_Error"] = false;
            }
        }

        #region Static functions
        /// <summary>
        /// Displays the provided text on the next Result control that is rendered on a page.
        /// </summary>
        /// <param name="text">The text to display on the Result control.</param>
        public static void Display(string text)
        {
            HttpContext.Current.Session["Result_Text"] = text;
            HttpContext.Current.Session["Result_Error"] = false;
        }

        /// <summary>
        /// Displays the provided error on the next Result control that is rendered on a page.
        /// </summary>
        /// <param name="error">The error to display on the Result control.</param>
        public static void DisplayError(string error)
        {
            HttpContext.Current.Session["Result_Text"] = error;
            HttpContext.Current.Session["Result_Error"] = true;
        }
        #endregion
    }
}