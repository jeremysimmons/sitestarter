using System;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Web.Elements
{
	/// <summary>
	/// Used to reset the elements so they can be re-scanned and re-intialized.
	/// </summary>
	public class ElementsResetter
	{
		public ElementsResetter()
		{
		}
		
		public void Reset()
		{
			// Dispose the in-memory state
			new ElementsDisposer().Dispose();
			
			string path = new ElementFileNamer().ElementsInfoDirectoryPath;
			
			// Delete element info (not the actual elements)
			foreach (string file in Directory.GetFiles(path))
			{
				File.Delete(file);
			}
			
			// Now the elements can be re-scanned and re-initialized
		}
	}
}
