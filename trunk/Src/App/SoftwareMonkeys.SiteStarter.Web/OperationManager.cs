using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.State;

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
                return (string)StateAccess.State.GetSession("CurrentOperation");
            }
            set
            {
                StateAccess.State.SetSession("CurrentOperation", value);
            }
        }

        /// <summary>
        /// Gets/sets the name of the previous operation.
        /// </summary>
        static public string PreviousOperation
        {
            get
            {
                return (string)StateAccess.State.GetSession("PreviousOperation");
            }
            set
            {
                StateAccess.State.SetSession("PreviousOperation", value);
            }
        }


        /// <summary>
        /// Starts an operation with the provided name and window title.
        /// </summary>
        /// <param name="operation">The name of the operation.</param>
        static public void StartOperation(string operation)
        {
        	StartOperation(operation, null);
        }

        /// <summary>
        /// Starts an operation with the provided name and window title.
        /// </summary>
        /// <param name="operation">The name of the operation.</param>
        /// <param name="view">The view to display.</param>
        static public void StartOperation(string operation, View view)
        {
            PreviousOperation = CurrentOperation;

            CurrentOperation = operation;
            if (view != null)
            	((MultiView)view.Parent).ActiveViewIndex = ((MultiView)view.Parent).Views.IndexOf(view);
        }
        
        static public void StartOperation(ICommandInfo command, View view)
        {
        	StartOperation(command.Action + command.TypeName, view);
        }
    }
}
