using System;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Used to reset the entities so they can be re-scanned and re-intialized.
	/// </summary>
	public class EntitiesResetter
	{
		public EntitiesResetter()
		{
		}
		
		public void Reset()
		{
			// Dispose the in-memory state
			new EntitiesDisposer().Dispose();
			
			string path = new EntityFileNamer().EntitiesInfoDirectoryPath;
			
			// Delete entity info (not the actual entities)
			foreach (string file in Directory.GetFiles(path))
			{
				File.Delete(file);
			}
			
			// Now the entities can be re-scanned and re-initialized
		}
	}
}
