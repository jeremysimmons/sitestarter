using System;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Web.Parts
{
	/// <summary>
	/// Used to reset the parts so they can be re-scanned and re-intialized.
	/// </summary>
	public class PartsResetter
	{
		public PartsResetter()
		{
		}
		
		public void Reset()
		{
			// Dispose the in-memory state
			new PartsDisposer().Dispose();
			
			string file = new PartFileNamer().PartsInfoFilePath;
			
			// Delete part info (not the actual parts)
			File.Delete(file);
			
			// Now the parts can be re-scanned and re-initialized
		}
	}
}
