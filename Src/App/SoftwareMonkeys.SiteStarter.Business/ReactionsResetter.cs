using System;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to reset the reactions so they can be re-scanned and re-intialized.
	/// </summary>
	public class ReactionsResetter
	{
		private ReactionFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create strategy file names/paths.
		/// </summary>
		public ReactionFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new ReactionFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		public ReactionsResetter()
		{
		}
		
		public void Reset()
		{
			// Dispose the in-memory state
			new ReactionsDisposer().Dispose();
			
			// Delete reaction info (not the actual reactions)
			File.Delete(FileNamer.ReactionsInfoFilePath);
			
			// Now the reactions can be re-scanned and re-initialized
		}
	}
}
