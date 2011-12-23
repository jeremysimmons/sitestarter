using System;
using System.IO;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Used to load entities from file.
	/// </summary>
	public class EntityLoader
	{
		private EntityFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create file paths for entities.
		/// </summary>
		public EntityFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new EntityFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		public EntityInfo[] Entities;
		
		public EntityLoader()
		{
		}
		
		
		/// <summary>
		/// Loads all the entities found in the entities cache file.
		/// </summary>
		/// <returns>An array of the the entities found in the cache file.</returns>
		public EntityInfo[] LoadInfoFromFile()
		{
			return LoadInfoFromFile(false);
		}
		
		/// <summary>
		/// Loads all the entities found in the entities cache file.
		/// </summary>
		/// <param name="includeDisabled"></param>
		/// <returns>An array of the the entities found in the cache file.</returns>
		public EntityInfo[] LoadInfoFromFile(bool includeDisabled)
		{
			// Logging disabled to boost performance
			//using (LogGroup logGroup = LogGroup.StartDebug("Loading the entities from the XML file."))
			//{
			if (Entities == null)
			{
				List<EntityInfo> validEntities = new List<EntityInfo>();
				
				EntityInfo[] entities = new EntityInfo[]{};
				
				using (StreamReader reader = new StreamReader(File.OpenRead(FileNamer.EntitiesInfoFilePath)))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(EntityInfo[]));
					entities = (EntityInfo[])serializer.Deserialize(reader);
				}
				
				foreach (EntityInfo entity in entities)
					if (entity.Enabled || includeDisabled)
						validEntities.Add(entity);
				
				Entities = validEntities.ToArray();
			}
			//}
			return Entities;
		}
	}
}
