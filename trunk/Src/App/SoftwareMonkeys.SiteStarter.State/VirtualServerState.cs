using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.State
{
	/// <summary>
	/// Provides easy access to information relating to the projects module.
	/// </summary>
	static public class VirtualServerState
	{
		/// <summary>
		/// Gets/sets a boolean value indicating whether a current project has been selected.
		/// </summary>
		static public bool VirtualServerSelected
		{
			get
			{
				return !String.IsNullOrEmpty(VirtualServerName)
					&& VirtualServerID != null && !String.IsNullOrEmpty(VirtualServerID) && VirtualServerID != Guid.Empty.ToString();
			}
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
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", MessageId = "Member")]
		static public string VirtualServerID
		{
			get {
				if (!StateAccess.State.ContainsSession("VirtualServerID"))
					StateAccess.State.SetSession("VirtualServerID", String.Empty);
				return (string)StateAccess.State.GetSession("VirtualServerID"); }
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
