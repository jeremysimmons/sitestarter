using System;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Used to create file names/paths for entity components.
	/// </summary>
	public class EntityFileNamer
	{
		private string entitiesDirectoryPath;
		/// <summary>
		/// Gets the path to the directory containing serialized entity component information.
		/// </summary>
		public string EntitiesDirectoryPath
		{
			get {
				if (entitiesDirectoryPath == null || entitiesDirectoryPath == String.Empty)
				{
					if (Configuration.Config.IsInitialized)
						entitiesDirectoryPath = Configuration.Config.Application.PhysicalApplicationPath
							+ Path.DirectorySeparatorChar + "App_Data"
							+ Path.DirectorySeparatorChar + "Entities";
				}
				return entitiesDirectoryPath;
			}
			set { entitiesDirectoryPath = value; }
		}
		
		public EntityFileNamer()
		{
		}
		
		/// <summary>
		/// Creates the file name for the provided entity.
		/// </summary>
		/// <param name="entity">The entity to create the file name for.</param>
		/// <returns>The full file name for the provided entity.</returns>
		public string CreateFileName(EntityInfo entity)
		{			
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			string name = entity.TypeName + ".entity";
			
			return name;
		}
		
		/// <summary>
		/// Creates the file name for the provided entity.
		/// </summary>
		/// <param name="entity">The entity to create the file name for.</param>
		/// <returns>The full file name for the provided entity.</returns>
		public string CreateFileName(IEntity entity)
		{
			return CreateFileName(new EntityInfo(entity));
		}
		
		
		/// <summary>
		/// Creates the full file path for the provided entity.
		/// </summary>
		/// <param name="entity">The entity to create the file path for.</param>
		/// <returns>The full file path for the provided entity.</returns>
		public string CreateFilePath(EntityInfo entity)
		{
			return EntitiesDirectoryPath + Path.DirectorySeparatorChar + CreateFileName(entity);
		}
		
		/// <summary>
		/// Creates the full file path for the provided entity.
		/// </summary>
		/// <param name="entity">The entity to create the file path for.</param>
		/// <returns>The full file path for the provided entity.</returns>
		public string CreateFilePath(IEntity entity)
		{
			return CreateFilePath(new EntityInfo(entity));
		}
		
	}
}
