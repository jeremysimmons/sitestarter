using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace SoftwareMonkeys.SiteStarter.State
{
    /// <summary>
    /// Provides easy access to information relating to the projects module.
    /// </summary>
    public class VirtualServerState
    {
        /// <summary>
        /// Gets/sets a boolean value indicating whether a current project has been selected.
        /// </summary>
        static public bool VirtualServerSelected
        {
            get { return VirtualServerName != null && VirtualServerName != String.Empty
            	&& VirtualServerID != null && VirtualServerID != String.Empty && VirtualServerID != Guid.Empty.ToString();}
        }

        /// <summary>
        /// Gets/sets the name of the current server.
        /// </summary>
        static public string VirtualServerName
        {
            get { return (string)StateAccess.State.GetSession("VirtualServerName"); }
            set { StateAccess.State.SetSession("VirtualServerName", value); }
        }

        /// <summary>
        /// Gets/sets the ID of the current server.
        /// </summary>
        static public string VirtualServerID
        {
            get { return (string)StateAccess.State.GetSession("VirtualServerID"); }
            set { StateAccess.State.SetSession("VirtualServerID", value); }
        }
        
        static public void SuspendVirtualServerState()
        {
        	string name = VirtualServerName;
        	string id = VirtualServerID;
        	
        	StateAccess.State.SetSession("Suspended_VirtualServerName", name);
        	StateAccess.State.SetSession("Suspended_VirtualServerID", id);
        	
        	VirtualServerName = String.Empty;
        	VirtualServerID = String.Empty;
        }
        
        static public void RestoreVirtualServerState()
        {
    		string name = (string)StateAccess.State.GetSession("Suspended_VirtualServerName");
    		string id = (string)StateAccess.State.GetSession("Suspended_VirtualServerID");
        	
        	StateAccess.State.SetSession("VirtualServerName", name);
        	StateAccess.State.SetSession("VirtualServerID", id);
        	
        	
        	StateAccess.State.SetSession("Suspended_VirtualServerName", String.Empty);
        	StateAccess.State.SetSession("Suspended_VirtualServerID", String.Empty);
        }
        
        static public void Switch(string name, Guid id)
        {
        	VirtualServerName = name;
        	VirtualServerID = id.ToString();
        }
    }
}
