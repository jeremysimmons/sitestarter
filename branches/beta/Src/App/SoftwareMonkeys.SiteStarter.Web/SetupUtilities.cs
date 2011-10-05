using System;
using System.Web;
using System.IO;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	///
	/// </summary>
	public class SetupUtilities
	{
		
		/// <summary>
		/// Gets a value indicating whether the /App_Data/Import/ directory exists.
		/// </summary>
		static public bool RequiresImport
		{
			get
			{
				string path = StateAccess.State.PhysicalApplicationPath + Path.DirectorySeparatorChar +
					"App_Data" + Path.DirectorySeparatorChar + "Import";
				
				return Directory.Exists(path);
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether the /App_Data/Legacy/ directory exists.
		/// </summary>
		static public bool RequiresRestore
		{
			get
			{
				string path = StateAccess.State.PhysicalApplicationPath + Path.DirectorySeparatorChar +
					"App_Data" + Path.DirectorySeparatorChar + "Legacy";
				
				return Directory.Exists(path);
			}
		}
	}
}
