using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SoftwareMonkeys.SiteStarter.Web
{
    /// <summary>
    /// Defines the base of all pages.
    /// </summary>
    public class BasePage : UserControl
    {
        /// <summary>
        /// Gets/sets the title displayed in the window.
        /// </summary>
        public string WindowTitle
        {
            get
            {
                if (Context == null)
                    return String.Empty;
                if (ViewState["WindowTitle"] == null)
                    ViewState["WindowTitle"] = "Ghost Protest";
                return (string)ViewState["WindowTitle"];
            }
            set { ViewState["WindowTitle"] = value; }
        }
/*
        #region Operations
        /// <summary>
        /// Gets/sets the name of the operation currently under way.
        /// </summary>
        protected string CurrentOperation
        {
            get
            {
                if (ViewState["CurrentOperation"] == null)
                    ViewState["CurrentOperation"] = String.Empty;
                return (string)ViewState["CurrentOperation"];
            }
            set { ViewState["CurrentOperation"] = value; }
        }

        /// <summary>
        /// Starts an operation with the provided name and window title.
        /// </summary>
        /// <param name="operation">The name of the operation.</param>
        /// <param name="windowTitle">The title displayed in the window.</param>
        public void StartOperation(string operation, string windowTitle)
        {
            CurrentOperation = operation;
            WindowTitle = windowTitle;
        }

        /// <summary>
        /// Starts an operation with the provided name and window title.
        /// </summary>
        /// <param name="operation">The name of the operation.</param>
        /// <param name="windowTitle">The title displayed in the window.</param>
        /// <param name="panel"></param>
        public void StartOperation(string operation, string windowTitle, View view)
        {
            CurrentOperation = operation;
            WindowTitle = windowTitle;
            ((MultiView)view.Parent).ActiveViewIndex = ((MultiView)view.Parent).Views.IndexOf(view);
        }
        #endregion*/
    }
}
