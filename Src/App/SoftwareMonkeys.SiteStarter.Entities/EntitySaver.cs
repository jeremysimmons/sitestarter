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
		/// Saves the provided entities to file.
		/// </summary>
		/// <param name="entities">An array of the entities to save to file.</param>
		public void SaveToFile(EntityInfo[] entities)
		{
			// Logging disabled to boost performance
			//using (LogGroup logGroup = LogGroup.StartDebug("Saving the provided entities to XML file."))
			//{
			string path = FileNamer.EntitiesInfoFilePath;
			
			//LogWriter.Debug("Path : " + path);
			
			if (!Directory.Exists(Path.GetDirectoryName(path)))
				Directory.CreateDirectory(Path.GetDirectoryName(path));
			
			using (StreamWriter writer = File.CreateText(path))
			{
				XmlSerializer serializer = new XmlSerializer(entities.GetType());
				serializer.Serialize(writer, entities);
				writer.Close();
			}
			//}
		}
	}
}
