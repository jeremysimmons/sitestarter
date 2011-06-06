using System;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Used to create file names/paths for entity components.
	/// </summary>
	public class EntityFileNamer
	{
		private string entitiesInfoDirectoryPath;
		/// <summary>
		/// Gets the path to the directory containing serialized entity component information.
		/// </summary>
		public string EntitiesInfoDirectoryPath
		{
			get {
				if (entitiesInfoDirectoryPath == null || entitiesInfoDirectoryPath == String.Empty)
				{
					if (Configuration.Config.IsInitialized)
					{
						entitiesInfoDirectoryPath = Configuration.Config.Application.PhysicalApplicationPath
							+ Path.DirectorySeparatorChar + "App_Data"
							+ Path.DirectorySeparatorChar + "Entities";
					}
				}
				return entitiesInfoDirectoryPath;
			}
			set { entitiesInfoDirectoryPath = value; }
		}
		
		public EntityFileNamer()
		{
		}
		
		/// <summary>
		/// Creates the file name for the provided entity.
		/// </summary>
		/// <param name="entity">The entity to create the file name for.</param>
		/// <returns>The full file name for the provided entity.</returns>
		public string CreateInfoFileName(EntityInfo entity)
		{			
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			string name = entity.TypeName + ".entity";
			
			return name;
		}
		
		/// <summary>
		/// Creates the full file path for the provided entity.
		/// </summary>
		/// <param name="entity">The entity to create the file path for.</param>
		/// <returns>The full file path for the provided entity.</returns>
		public string CreateInfoFilePath(EntityInfo entity)
		{
			string path = EntitiesInfoDirectoryPath + Path.DirectorySeparatorChar + CreateInfoFileName(entity);
			
			return path;
		}
		
	}
}
