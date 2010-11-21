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
		
		private string entitiesDirectoryPath;
		/// <summary>
		/// Gets/sets the path to the directory containing the entity files.
		/// </summary>
		public string EntitiesDirectoryPath
		{
			get {
				if (entitiesDirectoryPath == null || entitiesDirectoryPath == String.Empty)
				{
					if (FileNamer == null)
						throw new InvalidOperationException("FileNamer is not set.");
					
					entitiesDirectoryPath = FileNamer.EntitiesDirectoryPath;
				}
				return entitiesDirectoryPath; }
			set { entitiesDirectoryPath = value; }
		}
		
		public EntityLoader()
		{
		}
		
		/// <summary>
		/// Loads all the entities found in the entities directory.
		/// </summary>
		/// <returns>An array of the the entities found in the directory.</returns>
		public EntityInfo[] LoadFromDirectory()
		{
			List<EntityInfo> entities = new List<EntityInfo>();
			
			using (LogGroup logGroup = AppLogger.StartGroup("Loading the entities from the XML files.", NLog.LogLevel.Debug))
			{
				foreach (string file in Directory.GetFiles(EntitiesDirectoryPath))
				{
					AppLogger.Debug("File: " + file);
					
					entities.Add(LoadFromFile(file));
				}
			}
			
			return entities.ToArray();
		}
		
		/// <summary>
		/// Loads the entity from the specified path.
		/// </summary>
		/// <param name="entityPath">The full path to the entity to load.</param>
		/// <returns>The entity deserialized from the specified file path.</returns>
		public EntityInfo LoadFromFile(string entityPath)
		{
			EntityInfo info = null;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Loading the entity from the specified path.", NLog.LogLevel.Debug))
			{
				if (!File.Exists(entityPath))
					throw new ArgumentException("The specified file does not exist.");
				
				AppLogger.Debug("Path: " + entityPath);
				
				
				using (StreamReader reader = new StreamReader(File.OpenRead(entityPath)))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(EntityInfo));
					
					info = (EntityInfo)serializer.Deserialize(reader);
					
					reader.Close();
				}
			}
			
			return info;
		}
	}
}
