using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web;

namespace SoftwareMonkeys.SiteStarter.Web
{
    public class OperationManager
    {
        /// <summary>
        /// Gets/sets the name of the operation currently under way.
        /// </summary>
        static public string CurrentOperation
        {
            get
            {
                if (HttpContext.Current.Session["CurrentOperation"] == null)
                    HttpContext.Current.Session["CurrentOperation"] = String.Empty;
                return (string)HttpContext.Current.Session["CurrentOperation"];
            }
            set { HttpContext.Current.Session["CurrentOperation"] = value; }
        }

        /// <summary>
        /// Starts an operation with the provided name and window title.
        /// </summary>
        /// <param name="operation">The name of the operation.</param>
        static public void StartOperation(string operation)
        {
            CurrentOperation = operation;
        }

        /// <summary>
        /// Starts an operation with the provided name and window title.
        /// </summary>
        /// <param name="operation">The name of the operation.</param>
        /// <param name="view">The view to display.</param>
        static public void StartOperation(string operation, View view)
        {
            CurrentOperation = operation;
            ((MultiView)view.Parent).ActiveViewIndex = ((MultiView)view.Parent).Views.IndexOf(view);
        }
    }
}
