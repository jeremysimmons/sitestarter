using System;
using System.IO;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Used to save entity components to file.
	/// </summary>
	public class EntitySaver
	{
		private string entitiesDirectoryPath;
		/// <summary>
		/// Gets the full path to the directory containing entity mappings.
		/// </summary>
		public string EntitiesDirectoryPath
		{
			get {
				if (entitiesDirectoryPath == null || entitiesDirectoryPath == String.Empty)
				{
					if (FileNamer != null)
						entitiesDirectoryPath = FileNamer.EntitiesInfoDirectoryPath;
					
				}
				return entitiesDirectoryPath;
			}
			set { entitiesDirectoryPath = value; }
		}
		
		private EntityFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create file names/paths.
		/// </summary>
		public EntityFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new EntityFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		public EntitySaver()
		{
		}
		
		/// <summary>
		/// Saves the provided entity to the entities mappings directory.
		/// </summary>
		/// <param name="entity">The entity to save to file.</param>
		public void SaveToFile(EntityInfo entity)
		{
			using (LogGroup logGroup = LogGroup.Start("Saving the provided entity to file.", NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				string path = FileNamer.CreateInfoFilePath(entity);
				
				LogWriter.Debug("Path : " + path);
				
				if (!Directory.Exists(Path.GetDirectoryName(path)))
					Directory.CreateDirectory(Path.GetDirectoryName(path));
				
				using (StreamWriter writer = File.CreateText(path))
				{
					XmlSerializer serializer = new XmlSerializer(entity.GetType());
					serializer.Serialize(writer, entity);
					writer.Close();
				}
			}
		}
		
		/// <summary>
		/// Saves the provided entities to file.
		/// </summary>
		/// <param name="entities">An array of the entities to save to file.</param>
		public void SaveToFile(EntityInfo[] entities)
		{
			using (LogGroup logGroup = LogGroup.Start("Saving the provided entities to XML files.", NLog.LogLevel.Debug))
			{
				foreach (EntityInfo entity in entities)
				{
					if (entity != null)
						SaveToFile(entity);
				}
			}
		}
	}
}
