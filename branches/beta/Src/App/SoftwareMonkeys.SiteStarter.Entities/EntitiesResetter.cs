using System;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Used to reset the entities so they can be re-scanned and re-intialized.
	/// </summary>
	public class EntitiesResetter
	{
		private EntityFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create entity file names/paths.
		/// </summary>
		public EntityFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new EntityFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		public EntitiesResetter()
		{
		}
		
		public void Reset()
		{
			// Dispose the in-memory state
			new EntitiesDisposer().Dispose();
			
			// Delete entity info (not the actual entities)
			File.Delete(FileNamer.EntitiesInfoFilePath);
			
			// Now the entities can be re-scanned and re-initialized
		}
	}
}
