using System;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Defines the interface of all data backup adapters
	/// </summary>
	public interface IDataExporter : IDataAdapter
	{		
		/// <summary>
		/// Gets/sets the path of the export directory where data is serialized to before being zipped or refactored.
		/// </summary>
		string ExportDirectoryPath {get;set;}
		
		/// <summary>
		/// Exports data to XML files in the specified directory.
		/// </summary>
		void ExportToXml();
	}
}
