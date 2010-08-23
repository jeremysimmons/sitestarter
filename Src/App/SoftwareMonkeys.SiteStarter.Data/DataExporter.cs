using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Provides an abstract base implementation for all data exporter adapters.
	/// </summary>
	public abstract class DataExporter : DataAdapter, IDataExporter
	{
		private string exportDirectoryPath;
		/// <summary>
		/// Gets/sets the path to the directory containing exported data.
		/// </summary>
		public virtual string ExportDirectoryPath {
			get {
				return exportDirectoryPath;
			}
			set { exportDirectoryPath = value; }
			
		}
		
		/// <summary>
		/// Configures the exported directory path.
		/// </summary>
		public DataExporter()
		{
			if (Configuration.Config.IsInitialized)
			{
				ExportDirectoryPath = Configuration.Config.Application.PhysicalApplicationPath + Path.DirectorySeparatorChar +
					"App_Data" + Path.DirectorySeparatorChar + "Exported";
			}
		}
		
		#region Primary functions
		/// <summary>
		/// Exports data to XML files in the specified directory.
		/// </summary>
		public virtual void ExportToXml()
		{
			foreach (string storeName in DataAccess.Data.GetDataStoreNames())
			{
				foreach (IEntity entity in DataAccess.Data.Stores[storeName].Indexer.GetEntities())
				{
					ExportEntity(entity);
				}
			}
		}
		
		// TODO: Remove if not needed
		/*/// <summary>
		/// Exports data of the specified types to XML files in the specified directory.
		/// </summary>
		/// <param name="directoryPath">The directory path to export the files to.</param>
		/// <param name="typeNames">The short names of the types to export.</param>
		public virtual void ExportToXml(string directoryPath, string[] typeNames)
		{
			foreach (string typeName in typeNames)
			{
				foreach (IEntity entity in DataAccess.Data.Stores[EntitiesUtilities.GetType(typeName)].Indexer.GetEntities())
				{
					ExportEntity(entity, directoryPath);
				}
			}
		}*/
			
			#endregion
			
			#region Utility functions
			/// <summary>
			/// Exports the provided entity to the specified exports directory.
			/// </summary>
			/// <param name="entity">The entity to export to file.</param>
			public void ExportEntity(IEntity entity)
		{
			string filePath = CreateEntityPath(entity);
			
			SerializeToFile(entity, filePath);
		}
		
		
		/// <summary>
		/// Serializes the provided entity to the provided file path.
		/// </summary>
		/// <param name="entity">The entity to serialize.</param>
		/// <param name="filePath">The path to save the serialized file to.</param>
		public void SerializeToFile(IEntity entity, string filePath)
		{
			if (!Directory.Exists(Path.GetDirectoryName(filePath)))
				Directory.CreateDirectory(Path.GetDirectoryName(filePath));
			
			XmlSerializer serializer = new XmlSerializer(entity.GetType());
			using (StreamWriter writer = File.CreateText(filePath))
			{
				serializer.Serialize(writer, entity);

				writer.Close();
			}
		}
		
		/// <summary>
		/// Creates the file name of the exported file for the provided entity in the specified export directory.
		/// </summary>
		/// <param name="entity">The entity being exported.</param>
		/// <returns>The full export path for the provided entity.</returns>
		public string CreateEntityPath(IEntity entity)
		{
			Guid id = entity.ID;

			string fullPath = String.Empty;
			if (EntitiesUtilities.IsReference(entity.GetType()))
			{
				fullPath = CreateReferenceExportPath((EntityIDReference)entity, ExportDirectoryPath);
			}
			else
			{
				fullPath = ExportDirectoryPath + Path.DirectorySeparatorChar +
					entity.GetType().FullName + Path.DirectorySeparatorChar
					+ id + ".xml";
			}
			
			return fullPath;
		}
		
		/// <summary>
		/// Creates the full unique file path to save the serialized entity reference to.
		/// </summary>
		/// <param name="reference">The reference to create the file path for.</param>
		/// <param name="sourceDirectory">The directory containing all exported data (each in their own sub-folder).</param>
		/// <returns>The full file path unique to the provided reference.</returns>
		public string CreateReferenceExportPath(EntityIDReference reference, string sourceDirectory)
		{
			return CreateReferenceExportPath(sourceDirectory, reference.Type1Name, reference.Type2Name, reference.Entity1ID, reference.Entity2ID, reference.Property1Name, reference.Property2Name);
		}

		/// <summary>
		/// Creates the full unique file path to save the serialized entity reference to.
		/// </summary>
		/// <returns>The full file path unique to the provided reference.</returns>
		public string CreateReferenceExportPath(string sourceDirectory, string type1Name, string type2Name, Guid entity1ID, Guid entity2ID, string property1Name, string property2Name)
		{
			string referenceFileName = String.Empty;
			using (LogGroup logGroup = AppLogger.StartGroup("Creating the path to the specified reference file.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Type 1 name: " + type1Name);
				AppLogger.Debug("Type 2 name: " + type2Name);
				AppLogger.Debug("Entity 1 ID: " + entity1ID);
				AppLogger.Debug("Entity 2 ID: " + entity2ID);
				AppLogger.Debug("Property 1 name: " + property1Name);
				AppLogger.Debug("Property 2 name: " + property2Name);

				// Sort the names alphabetically
				string[] typeNames = new String[] { type1Name, type2Name };
				Array.Sort(typeNames);

				// If the type name order changed then switch the other variables too
				if (type1Name != typeNames[0])
				{
					AppLogger.Debug("Switched order of entity type names.");

					type1Name = typeNames[0];
					type2Name = typeNames[1];

					// Switch the property names
					string y = property1Name;
					string x = property2Name;

					property1Name = x;
					property2Name = y;

					// Switch the entity names
					Guid a = entity1ID;
					Guid b = entity2ID;

					entity1ID = b;
					entity2ID = a;

					AppLogger.Debug("Type 1 name: " + type1Name);
					AppLogger.Debug("Type 2 name: " + type2Name);
					AppLogger.Debug("Entity 1 ID: " + entity1ID);
					AppLogger.Debug("Entity 2 ID: " + entity2ID);
					AppLogger.Debug("Property 1 name: " + property1Name);
					AppLogger.Debug("Property 2 name: " + property2Name);
				}

				string typeName = typeNames[0] + "-" + typeNames[1];

				referenceFileName = sourceDirectory + Path.DirectorySeparatorChar;
				if (Path.GetDirectoryName(sourceDirectory) != type1Name + "-" + type2Name)
					referenceFileName += typeName + Path.DirectorySeparatorChar;
				referenceFileName += entity1ID + "-" + property1Name + "---" + entity2ID + "-" + property2Name + ".xml";

				AppLogger.Debug("Reference file name: " + referenceFileName);
			}
			return referenceFileName;
		}
		#endregion
	}
}
