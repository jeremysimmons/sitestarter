using System;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to reset the reactions so they can be re-scanned and re-intialized.
	/// </summary>
	public class ReactionsResetter
	{
		public ReactionsResetter()
		{
		}
		
		public void Reset()
		{
			// Dispose the in-memory state
			new ReactionsDisposer().Dispose();
			
			string path = new ReactionFileNamer().ReactionsInfoDirectoryPath;
			
			// Delete reaction info (not the actual reactions)
			foreach (string file in Directory.GetFiles(path))
			{
				File.Delete(file);
			}
			
			// Now the reactions can be re-scanned and re-initialized
		}
	}
}
