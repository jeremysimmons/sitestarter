using System;
using System.Xml;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Defines the interface of all data schema editor components
	/// </summary>
	public interface IDataSchema
	{
		/// <summary>
		/// Gets/sets the path to the schema command directory that contains XML schema command files.
		/// </summary>
		string SchemaCommandDirectoryPath {get;set;}
			
		/// <summary>
		/// Gets/sets the version of the legacy data.
		/// </summary>
		Version LegacyVersion {get;set;}
		
		/// <summary>
		/// Gets/sets the current version of the application.
		/// </summary>
		Version ApplicationVersion {get;set;}
		
		/// <summary>
		/// Gets/sets the directory containing the schema change files.
		/// </summary>
		string SchemaDirectory {get;set;}
				
		/// <summary>
		/// Gets/sets a boolean flag indicating whether the schema is up to date with the application.
		/// </summary>
		bool IsUpToDate {get;}
		
		/// <summary>
		/// Checks whether the legacy version matches the current application version, therefore indicating that data schema.
		/// </summary>
		/// <returns>A boolean flag indicating whether the schema is up to date with the current application.</returns>
		bool CheckIsUpToDate();
		
		/// <summary>
		/// Retrieves the version of the data in the legacy folder.
		/// </summary>
		/// <returns>The version of the legacy data.</returns>
		Version GetLegacyVersion();
		
		/// <summary>
		/// Retrieves the current version of the application.
		/// </summary>
		/// <returns>The current version of the application.</returns>
		Version GetApplicationVersion();
		
		//string[] Messages {get;}
		
		/// <summary>
		/// Loads the schema commands from the provided directory and executes those that are required. Changes already applied once are not
		/// re-applied.
		/// </summary>
		/// <param name="schemaDirectory">The directory containing the schema change files.</param>
		void ApplyCommands(string schemaDirectory);
		
		/// <summary>
		/// Loads the schema commands from the provided directory and executes those that are required. Changes already applied once are not
		/// re-applied.
		/// </summary>
		/// <param name="schemaDirectory">The directory containing the schema change files.</param>
		void ApplyCommands(DataSchemaCommandCollection commands);
		
		/// <summary>
		/// Renames the specified property on the specified type.
		/// </summary>
		/// <param name="type">The type containing the property to rename.</param>
		/// <param name="originalPropertyName">The name of the property to rename.</param>
		/// <param name="newPropertyName">The new name of the property.</param>
		void RenameProperty(string type, string originalPropertyName, string newPropertyName);
		
		/// <summary>
		/// Renames the specified type.
		/// </summary>
		/// <param name="originalName">The name of the type to rename.</param>
		/// <param name="newName">The new name of the type.</param>
		void RenameType(string originalName, string newName);
		
		/// <summary>
		/// Suspends the data access system so that the schema can be changed.
		/// </summary>
		/// <returns></returns>
		string[] Suspend();
		
		/// <summary>
		/// Resumes the data access system so that the application can continue to run.
		/// </summary>
		void Resume();
		
		/// <summary>
		/// Retrieves the schema commands applicable to the legacy data (between the legacy version and current application version).
		/// </summary>
		/// <returns>A collection of the comands that need to be applied to legacy data.</returns>
		DataSchemaCommandCollection GetCommands();
		
		/// <summary>
		/// Serializes and saves the provided collection of data commands to file ready for execution.
		/// </summary>
		/// <param name="commands">The data schema commands to serialize and save.</param>
		/// <param name="groupName">The name of the group/module that the commands apply to. (This is used in the file name.)</param>
		/// <param name="version">The version of data schema that the command corresponds with. (This is used in the file name.)</param>
		void SaveCommands(DataSchemaCommandCollection commands, string groupName, Version version);
		
		/// <summary>
		/// Serializes and saves the provided collection of data commands to file ready for execution.
		/// </summary>
		/// <param name="schemaCommandDirectoryPath">The path to the directory containing the schema command files.</param>
		/// <param name="commands">The data schema commands to serialize and save.</param>
		/// <param name="groupName">The name of the group/module that the commands apply to. (This is used in the file name.)</param>
		/// <param name="version">The version of data schema that the command corresponds with. (This is used in the file name.)</param>
		void SaveCommands(string schemaCommandDirectoryPath, DataSchemaCommandCollection commands, string groupName, Version version);
		
		
		/// <summary>
		/// Renews/updates the provided schema to match the latest version of the application, by executing the appropriate schema edit commands.
		/// </summary>
		/// <param name="document">The serialized entity as an XML document.</param>
		void RenewSchema(XmlDocument document);
	}
}
