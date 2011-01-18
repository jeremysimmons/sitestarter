using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Defines the interface of all data importer adapters which are used to import data from XML.
	/// </summary>
	public interface IDataImporter : IDataAdapter
	{
		/// <summary>
		/// Gets/sets the path to the directory containing the importable XML files.
		/// </summary>
		string ImportableDirectoryPath {get;set;}
		
		
		/// <summary>
		/// Gets/sets the path to the directory containing the XML files after they've been imported.
		/// </summary>
		string ImportedDirectoryPath {get;set;}
		
		/// <summary>
		/// Imports the data from the XML files found in the provided directory (each in their own type specific subdirectory).
		/// </summary>
		void ImportFromXml();
		
		/// <summary>
		/// Checks whether the provided entity is valid and can be imported.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		bool IsValid(IEntity entity);
		
		/// <summary>
		/// Retrieves the previous version (indicated in either the legacy or import directory).
		/// </summary>
		/// <returns></returns>
		Version GetPreviousVersion();
	}
}
