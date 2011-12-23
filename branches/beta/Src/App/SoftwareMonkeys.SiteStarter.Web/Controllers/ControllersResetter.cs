using System;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// Used to reset the controllers so they can be re-scanned and re-intialized.
	/// </summary>
	public class ControllersResetter
	{
		public ControllersResetter()
		{
		}
		
		public void Reset()
		{
			// Dispose the in-memory state
			new ControllersDisposer().Dispose();
			
			string path = new ControllerFileNamer().ControllersInfoFilePath;
			
			File.Delete(path);
			
			// Now the controllers can be re-scanned and re-initialized
		}
	}
}
