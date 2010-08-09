using System;

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
	}
}
