using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Data
{
    /// <summary>
    /// Provides functionality for data access throughout the application.
    /// </summary>
    public class DataAccess
    {
        /// <summary>
        /// Gets a flag indicating whether the data provider has been initialized
        /// </summary>
        static public bool IsInitialized
        {
            get { return State.StateAccess.State.GetApplication("DataAccess.Data") != null; }
        }

        //static private DataProvider data;
        /// <summary>
        /// Gets the data provider for the current context.
        /// </summary>
        static public DataProvider Data
        {
            get
            {
                if (State.StateAccess.State.GetApplication("DataAccess.Data") == null)
                    throw new InvalidOperationException("The data access provider has not been intialized so the application cannot run.");
                return (DataProvider)State.StateAccess.State.GetApplication("DataAccess.Data");
            }
            set { State.StateAccess.State.SetApplication("DataAccess.Data", value); }
        }

        /// <summary>
        /// Initializes the data access provider.
        /// </summary>
        static public void Initialize()
        {
            // Initialization for data provider is automatic
        }

        /// <summary>
        /// Disposes and clears all data access objects.
        /// </summary>
        static public void Dispose(bool fullDisposal)
        {
            if (IsInitialized)
            {
                Data.Dispose(fullDisposal);
                State.StateAccess.State.SetApplication("DataAccess.Data", null);
            }
        }

        /// <summary>
        /// Gets the specified type from within the executing assembly.
        /// </summary>
        static public Type GetType(string typeName)
        {
            return Type.GetType(typeName);
        }
    }
}
