﻿using System;
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
			using (LogGroup logGroup = LogGroup.StartDebug("Exporting data to XML."))
			{
				foreach (string storeName in DataAccess.Data.GetDataStoreNames())
				{
					using (LogGroup logGroup2 = LogGroup.StartDebug("Data store: " + storeName))
					{
						IEntity[] entities = DataAccess.Data.Stores[storeName].Indexer.GetEntities();
						
						LogWriter.Debug("Entities found: " + entities.Length.ToString());
						
						foreach (IEntity entity in entities)
						{
							ExportEntity(entity);
						}
					}
				}
			}
		}
		
		#endregion
		
		#region Utility functions
		/// <summary>
		/// Exports the provided entity to the specified exports directory.
		/// </summary>
		/// <param name="entity">The entity to export to file.</param>
		public void ExportEntity(IEntity entity)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Exporting '" + entity.ShortTypeName + "' entity."))
			{
				string filePath = CreateEntityPath(entity);
			
				LogWriter.Debug("File path: " + filePath);
				
				SerializeToFile(entity, filePath);	
			}
		}
		
		
		/// <summary>
		/// Serializes the provided entity to the provided file path. Note: Deactivates the provided entity by stripping off the referenced properties to avoid circular reference error.
		/// </summary>
		/// <param name="entity">The entity to serialize.</param>
		/// <param name="filePath">The path to save the serialized file to.</param>
		public void SerializeToFile(IEntity entity, string filePath)
		{
			entity.Deactivate();
			
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
			return new EntityFileNamer(entity, ExportDirectoryPath).CreateFilePath();
		}
		
		/// <summary>
		/// Creates the full unique file path to save the serialized entity reference to.
		/// </summary>
		/// <param name="reference">The reference to create the file path for.</param>
		/// <param name="baseDirectory">The base directory containing the entities XML files.</param>
		/// <returns>The full file path unique to the provided reference.</returns>
		public string CreateReferenceExportPath(EntityReference reference, string baseDirectory)
		{
			return new EntityFileNamer(reference, baseDirectory).CreateFilePath();
		}
		#endregion
	}
}
