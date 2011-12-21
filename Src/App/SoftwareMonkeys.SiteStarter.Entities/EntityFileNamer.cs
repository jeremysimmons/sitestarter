using System;
using System.IO;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Used to create file names/paths for entity components.
	/// </summary>
	public class EntityFileNamer
	{
		private string entitiesInfoFilePath;
		/// <summary>
		/// Gets the path to the file containing the entity info.
		/// </summary>
		public virtual string EntitiesInfoFilePath
		{
			get {
				if (entitiesInfoFilePath == null || entitiesInfoFilePath == String.Empty)
				{
					if (StateAccess.IsInitialized)
						entitiesInfoFilePath = StateAccess.State.PhysicalApplicationPath
							+ Path.DirectorySeparatorChar + "App_Data"
							+ Path.DirectorySeparatorChar + "Entities.xml";
				}
				return entitiesInfoFilePath;
			}
			set { entitiesInfoFilePath = value; }
		}
		
		public EntityFileNamer()
		{
		}
	}
}
