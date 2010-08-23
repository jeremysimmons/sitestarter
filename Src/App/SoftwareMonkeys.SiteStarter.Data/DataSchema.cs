using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Provides a base implementation for all data schema adapters.
	/// </summary>
	public class DataSchema : DataAdapter, IDataSchema
	{
		private DataSchemaCommandCollection schemaCommands;
		/// <summary>
		/// Gets/sets the schema commands that need to be executed upon legacy data.
		/// </summary>
		public DataSchemaCommandCollection SchemaCommands
		{
			get {
				if (schemaCommands == null)
					schemaCommands = GetCommands();
				return schemaCommands; }
			set { schemaCommands = value; }
		}
		
		private string schemaCommandDirectoryPath;
		/// <summary>
		/// Gets/sets the path to the schema command directory that contains XML schema command files.
		/// </summary>
		public string SchemaCommandDirectoryPath
		{
			get {
				if (schemaCommandDirectoryPath == String.Empty)
					schemaCommandDirectoryPath = Configuration.Config.Application.PhysicalApplicationPath + Path.DirectorySeparatorChar + "App_Data" + Path.DirectorySeparatorChar + "Schema";
				return schemaCommandDirectoryPath; }
			set { schemaCommandDirectoryPath = value; }
		}
		
		private Version legacyVersion;
		/// <summary>
		/// Gets/sets the version of the legacy data.
		/// </summary>
		public Version LegacyVersion
		{
			get {
				if (legacyVersion == null)
					legacyVersion = GetLegacyVersion();
				return legacyVersion; }
			set { legacyVersion = value; }
		}
		
		private Version applicationVersion;
		/// <summary>
		/// Gets/sets the current version of the application.
		/// </summary>
		public Version ApplicationVersion
		{
			get {
				if (applicationVersion == null)
					applicationVersion = GetApplicationVersion();
				return applicationVersion; }
			set { applicationVersion = value; }
		}
		
		public DataSchema()
		{
		}
		
		private string schemaDirectory = "Schema";
		/// <summary>
		/// Gets/sets the directory containing the schema change files.
		/// </summary>
		public virtual string SchemaDirectory {
			get {
				return schemaDirectory;
			}
			set { schemaDirectory = value; }
		}
		
		/// <summary>
		/// Gets/sets a boolean flag indicating whether the schema is up to date with the application.
		/// </summary>
		public virtual bool IsUpToDate
		{
			get
			{
				return CheckIsUpToDate();
			}
		}
		
		/// <summary>
		/// Renews/updates the provided schema to match the latest version of the application, by executing the appropriate schema edit commands.
		/// </summary>
		/// <param name="document">The serialized entity as an XML document.</param>
		public void RenewSchema(XmlDocument document)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Renewing the schema of the provided XML document.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Total commands found: " + SchemaCommands.Count);
				
				foreach (DataSchemaCommand command in SchemaCommands)
				{
					if (command.AppliesTo(document))
						command.Execute(document);
				}
			}
		}
		
		/// <summary>
		/// Checks whether the legacy version matches the current application version, therefore indicating that data schema.
		/// </summary>
		/// <returns>A boolean flag indicating whether the schema is up to date with the current application.</returns>
		public bool CheckIsUpToDate()
		{
			// Load the commands
			DataSchemaCommandCollection foundCommands = GetCommands();
			
			
			// If no relevant commands were found then the data structure must be up to date
			return (foundCommands.Count == 0);
		}
		
		
		/// <summary>
		/// Retrieves the version of the data in the legacy folder.
		/// </summary>
		/// <returns>The version of the legacy data.</returns>
		public Version GetLegacyVersion()
		{
			return VersionUtilities.GetLegacyVersion(Configuration.Config.Application.PhysicalApplicationPath);
		}
		
		/// <summary>
		/// Retrieves the current version of the application.
		/// </summary>
		/// <returns>The current version of the application.</returns>
		public Version GetApplicationVersion()
		{
			return VersionUtilities.GetCurrentVersion(Configuration.Config.Application.PhysicalApplicationPath);
		}
		
		
		public string[] Messages {
			get {
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// Loads the schema commands from the provided directory and executes those that are required. Changes already applied once are not
		/// re-applied.
		/// </summary>
		/// <param name="schemaCommandDirectory">The directory containing the schema command files.</param>
		public void ApplyCommands(string schemaDirectory)
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Executes the requires commands from those provided. Changes already applied once are not
		/// re-applied.
		/// </summary>
		/// <param name="commands">The commands to apply.</param>
		public void ApplyCommands(DataSchemaCommandCollection commands)
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Renames the specified property on the specified type.
		/// </summary>
		/// <param name="type">The type containing the property to rename.</param>
		/// <param name="originalPropertyName">The name of the property to rename.</param>
		/// <param name="newPropertyName">The new name of the property.</param>
		public void RenameProperty(string type, string originalPropertyName, string newPropertyName)
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Renames the specified type.
		/// </summary>
		/// <param name="originalName">The name of the type to rename.</param>
		/// <param name="newName">The new name of the type.</param>
		public void RenameType(string originalName, string newName)
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Suspends the data access system so that the schema can be changed.
		/// </summary>
		/// <returns></returns>
		public string[] Suspend()
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Resumes the data access system so that the application can continue to run.
		/// </summary>
		public void Resume()
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Retrieves the schema commands that are applicable to legacy data (between legacy version and current application version).
		/// </summary>
		/// <returns>A collection of the commands that need to be applied the legacy data.</returns>
		public DataSchemaCommandCollection GetCommands()
		{
			DataSchemaCommandCollection collection = new DataSchemaCommandCollection();
			
			string schemaCommandDirectoryPath = SchemaCommandDirectoryPath;
			Version legacyVersion = LegacyVersion;
			Version currentVersion = ApplicationVersion;
			
			if (Directory.Exists(schemaCommandDirectoryPath))
			{
				foreach (string file in Directory.GetFiles(schemaCommandDirectoryPath))
				{
					Version commandVersion = ExtractVersionFromFileName(file);
					
					// If the version specified in the command file is greater than the legacy version and less than or equal to the current version (inclusive)
					if (commandVersion > legacyVersion &&
					    commandVersion <= currentVersion)
					{
						// Define the extra types that might be required during serialization
						Type[] extraTypes = new Type[]
						{
							typeof(RenameTypeCommand),
							typeof(RenamePropertyCommand)
						};
						
						XmlSerializer serializer = new XmlSerializer(typeof(DataSchemaCommandCollection), extraTypes);
						using (StreamReader reader = new StreamReader(File.OpenRead(file)))
						{
							DataSchemaCommandCollection c = (DataSchemaCommandCollection)serializer.Deserialize(reader);
							
							collection.AddRange(c);

							reader.Close();
						}
					}
				}
			}
			
			return collection;
		}
		
		/// <summary>
		/// Serializes and saves the provided data commands to file ready for execution.
		/// </summary>
		/// <param name="commands">The data schema command to serialize and save.</param>
		/// <param name="groupName">The name of the group/module that the commands apply to. (This is used in the file name.)</param>
		/// <param name="version">The version of data schema that the command corresponds with. (This is used in the file name.)</param>
		public void SaveCommands(DataSchemaCommandCollection commands, string groupName, Version version)
		{
			string path = Configuration.Config.Application.PhysicalApplicationPath + Path.DirectorySeparatorChar + SchemaDirectory;
			
			SaveCommands(path, commands, groupName, version);
		}
		
		/// <summary>
		/// Serializes and saves the provided data commands to file ready for execution.
		/// </summary>
		/// <param name="commandsDirectoryPath">The physical path to the schema commands directory.</param>
		/// <param name="commands">The data schema command to serialize and save.</param>
		/// <param name="groupName">The name of the group/module that the commands apply to. (This is used in the file name.)</param>
		/// <param name="version">The version of data schema that the command corresponds with. (This is used in the file name.)</param>
		public void SaveCommands(string commandsDirectoryPath, DataSchemaCommandCollection commands, string groupName, Version version)
		{
			
			string commandFile = CreateCommandFilePath(commandsDirectoryPath, groupName, version);
			
			if (!Directory.Exists(Path.GetDirectoryName(commandFile)))
				Directory.CreateDirectory(Path.GetDirectoryName(commandFile));
			
			// Define the extra types that might be required during serialization
			Type[] extraTypes = new Type[]
			{
				typeof(RenameTypeCommand),
				typeof(RenamePropertyCommand)
			};
			
			XmlSerializer serializer = new XmlSerializer(commands.GetType(), extraTypes);
			using (StreamWriter writer = File.CreateText(commandFile))
			{
				serializer.Serialize(writer, commands);

				writer.Close();
			}
		}
		
		/// <summary>
		/// Creates the full file path for a serialized collection of commands related to the specified group/module and specified version.
		/// </summary>
		/// <param name="commandsDirectoryPath">The physical path to the schema commands directory..</param>
		/// <param name="groupName">The name of the group/module that the commands apply to.</param>
		/// <param name="version">The version of the data schema that the commands correspond with.</param>
		/// <returns>The full file path for a serialized schema commands file.</returns>
		public string CreateCommandFilePath(string commandsDirectoryPath, string groupName, Version version)
		{
			string fileName = groupName + "--" + version.ToString().Replace(".","-") + ".schema";
			
			return commandsDirectoryPath + Path.DirectorySeparatorChar + fileName;
		}
		
		/// <summary>
		/// Extracts the version component from the name of a schema file.
		/// </summary>
		/// <param name="fileName">The name of the schema file to extract the version from.</param>
		/// <returns>The version extracted from the provided file name.</returns>
		public Version ExtractVersionFromFileName(string fileName)
		{
			string fixedFileName = Path.GetFileNameWithoutExtension(fileName);
			
			// Replace the two -- characters with a single | character so it can be easily split
			fixedFileName = fixedFileName.Replace("--", "|");
			
			// Split the name into the two parts; 0) group name and 1) version string
			string[] parts = fixedFileName.Split('|');
			
			if (parts.Length != 2)
				throw new ArgumentException("The provided schema file name is invalid: " + fileName);
			
			// Switch _ characters back to . characters in version
			string versionString = parts[1].Replace("-", ".");
			
			Version version = null;
			try
			{
				version = new Version(versionString);
			}
			catch(FormatException)
			{
				throw new ArgumentException("Cannot parse the version number from file name: " + fileName);
			}
			
			return version;
		}
		
		
	}
}
