using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareMonkeys.SiteStarter.State
{
    sealed public class StateAccess
    {
        private static StateProvider state;
        /// <summary>
        /// Gets/sets the current StateProvider instance.
        /// </summary>
        public static StateProvider State
        {
            get { return state; }
            set { state = value; }
        }

        static public bool IsInitialized
        {
            get { return state != null; }
        }
        
        private StateAccess()
        {}
    }
}
