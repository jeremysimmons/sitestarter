using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Data
{
    public class XmlEntityManager
    {
        private string importablesDirectory;
        public string ImportablesDirectory
        {
            get { return importablesDirectory; }
            set { importablesDirectory = value; }

        }

        private string importedDirectory;
        public string ImportedDirectory
        {
            get { return importedDirectory; }
            set { importedDirectory = value; }
        }

        public XmlEntityManager(string importablesDirectory, string importedDirectory)
        {
            this.ImportablesDirectory = importablesDirectory;
            this.ImportedDirectory = importedDirectory;
        }

        public XmlEntityManager(string importablesDirectory)
        {
            this.ImportablesDirectory = importablesDirectory;
        }


        #region Import functions
        /// <summary>
        /// Load all the objects (from XML) from the specified directory.
        /// </summary>
        /// <param name="batchCount"></param>
        /// <returns>An array of the loaded objects.</returns>
        public object[] LoadObjects(int batchCount)
        {
            string directory = ImportablesDirectory;//Path.Combine(ImportablesDirectory, typeName);
            List<object> list = new List<object>();

            using (LogGroup logGroup = AppLogger.StartGroup("Loading objects from the specified directory.", NLog.LogLevel.Debug))
            {
                AppLogger.Debug("Directory: " + directory);

                int i = 0;

                if (Directory.Exists(directory))
                {
                    AppLogger.Debug("Directory exists");

                    foreach (string file in Directory.GetFiles(directory, "*.xml"))
                    {
                        AppLogger.Debug("Source xml file: " + file);
                        //if (i >= batchCount)
                        //	break;
                        //else
                        //{
                        object obj = null;
                        try
                        {
                            // Skip all Guid objects. They don't need to be imported
                            //if (objectType != "System.Guid")
                            //{

                            //	Type type = GetType(objectType);
                            //	if (type == null)
                            //		throw new ArgumentException("The specified object type '" + objectType + "' could not be found.");

                            XmlDocument doc = new XmlDocument();
                            doc.Load(file);

                            string shortTypeName = doc.DocumentElement.Name;


                            Type type = GetTypeFromShortType(shortTypeName);


                            AppLogger.Debug("Loading type: " + type.ToString());


                            XmlSerializer serializer = new XmlSerializer(type);
                            using (StreamReader reader = new StreamReader(file))
                            {
                                obj = serializer.Deserialize(reader);
                                reader.Close();
                            }
                            //}
                        }
                        catch (Exception ex)
                        {
                            if (ex.ToString().ToLower().IndexOf("root element is missing") > -1)
                                throw new Exception("Root element is missing in file '" + file + "'");
                            // TODO: Reverse. Currently ignores exception
                            else
                                throw ex;

                        }

                        if (obj == null)
                            AppLogger.Debug("Couldn't deserialize object.");
                        else
                        {
                            if (obj is IEntity)
                                AppLogger.Debug("obj is IEntity");
                            else
                                AppLogger.Debug("obj is not IEntity");

                            /*// Check if an equivalent object exists
                            bool existing = false;
                            if (EntitiesUtilities.IsEntity(obj.GetType()))
                            {
                                // TODO: Remove if not needed
                                //					 Db4oDataStore store = (Db4oDataStore)DataAccess.Data.Stores[obj.GetType()];
                                //		IQuery query = store.Query(obj.GetType());
                                //                    query.Descend("id").Constrain(((Entity)obj).ID);
                                //                    existing = store.GetEntity(query.Execute());

                                existing = DataAccess.Data.IsStored((IEntity)obj);
                                //existing = DataAccess.Data.Stores[DataUtilities.GetDataStoreName((IEntity)obj)].GetEntityByTypeAndProperty(obj.GetType(), "ID", ((BaseEntity)obj).ID);
                            }

                            AppLogger.Debug("Entity already stored: " + existing.ToString());

                            // If no existing object is found, but the loaded object is not null
                            if (!existing)
                            {*/
                                AppLogger.Debug("Adding object to list: " + obj.GetType().ToString());
                                list.Add(obj);
                                i++;
                            //}
                        }
                        //}
                    }

                }
            }
            return (object[])list.ToArray();
        }

        private Type GetTypeFromShortType(string shortTypeName)
        {
            
            Type type = null;
            using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the type from the provided short type name.", NLog.LogLevel.Debug))
            {
                AppLogger.Debug("Short type name: " + shortTypeName);

                if (shortTypeName == "EntityIDReference")
                    type = typeof(EntityIDReference);
                else if (shortTypeName == "EntityReference")
                    type = typeof(EntityReference);
                else
                    type = EntitiesUtilities.GetType(shortTypeName);

                if (type != null)
                    AppLogger.Debug("Full type: " + type.ToString());
                else
                    AppLogger.Debug("Full type: [null]");
            }
            return type;
        }



        /// <summary>
        /// Imports all the objects from the specified directory into the data store.
        /// </summary>
        public void ImportAll()
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Importing all objects in the working directory.", NLog.LogLevel.Debug))
            {
                // TODO: Make it so the type doesn't need to be converted to string
                object[] objects = LoadObjects(99999999);

                if (objects == null)
                    AppLogger.Debug("objects == null");
                else
                    AppLogger.Debug("# objects found: " + objects.Length);

                foreach (object obj in objects)
                {
                    if (EntitiesUtilities.IsEntity(obj.GetType())
                        || EntitiesUtilities.IsReference(obj.GetType()))
                    {
                        AppLogger.Debug("Entity type: " + obj.GetType().ToString());

                        if (!DataAccess.Data.IsStored((IEntity)obj))
                        {
                            AppLogger.Debug("New entity. Importing.");
                            BaseEntity entity = (BaseEntity)obj;
                            DataAccess.Data.Saver.Save(entity);
                        }
                        else
                        {
                            AppLogger.Debug("Entity already exists in store. Skipping.");
                        }

                        MarkAsImported((BaseEntity)obj);
                    }
                }
            }
        }
        #endregion

        public void MarkAsImported(IEntity entity)
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Marking the provided entity as imported by moving it to the imported directory.", NLog.LogLevel.Debug))
            {
                if (entity == null)
                    throw new ArgumentNullException("entity", "The provided entity cannot be null.");

                AppLogger.Debug("Entity type: " + entity.GetType());

                string fileName = String.Empty;
                string toFileName = String.Empty;


                string typeName = entity.GetType().ToString();

                if (EntitiesUtilities.IsReference(entity.GetType()))
                {
                    AppLogger.Debug("Entity provided is a reference entity.");

                    string[] typeNames = new string[] {((EntityIDReference)entity).Type1Name,
					((EntityIDReference)entity).Type2Name};


                    string[] propertyNames = new string[] {((EntityIDReference)entity).Property1Name,
					((EntityIDReference)entity).Property2Name};

                    Guid[] ids = new Guid[] {((EntityIDReference)entity).Entity1ID,
					((EntityIDReference)entity).Entity2ID};

                    fileName = XmlEntitySchemaEditor.CreateReferenceFilePath(Path.GetDirectoryName(ImportablesDirectory), typeNames[0], typeNames[1], ids[0], ids[1], propertyNames[0], propertyNames[1]);
                    toFileName = XmlEntitySchemaEditor.CreateReferenceFilePath(Path.GetDirectoryName(ImportedDirectory), typeNames[0], typeNames[1], ids[0], ids[1], propertyNames[0], propertyNames[1]);
                }
                else
                {
                    AppLogger.Debug("Entity provided is a standard entity (not a reference object).");

                    fileName = Path.Combine(ImportablesDirectory, entity.ID.ToString() + ".xml");
                    toFileName = Path.Combine(ImportedDirectory,
                        Path.GetFileName(fileName));
                }

                if (ImportedDirectory != String.Empty)
                {
                    AppLogger.Debug("Imported directory specified. Continuing.");
                    

                    AppLogger.Debug("From path: " + fileName);
                    AppLogger.Debug("To path: " + toFileName);

                    if (!Directory.Exists(Path.GetDirectoryName(toFileName)))
                        Directory.CreateDirectory(Path.GetDirectoryName(toFileName));


                    if (File.Exists(fileName))
                    {
                        if (File.Exists(toFileName))
                            File.Delete(toFileName);

                        File.Move(fileName, toFileName);

                        AppLogger.Debug("Moved");
                    }
                    else
                    {
                        AppLogger.Debug("File not found. Not moved.");
                    }
                }
                else
                {

                    AppLogger.Debug("Imported directory not specified. Skipping move.");
                }
            }
        }

        /*/// <summary>
		/// Imports all the objects of the specified type from the specified directory into the data store.
		/// </summary>
		/// <param name="directoryPath">The path of the directory to load the data from.</param>
		static public void ImportAll(string type, string directoryPath)
		{
			// TODO: Make it so the type doesn't need to be converted to string
			object[] objects = LoadObjects(directoryPath, 99999999);

			foreach (object obj in objects)
			{
				if (obj is BaseEntity)
				{
                    if (!DataAccess.Data.IsStored((BaseEntity)obj))
                    {
                        BaseEntity entity = (BaseEntity)obj;
                        DataAccess.Data.Saver.Save(entity);


                        MarkAsImported(directoryPath, (BaseEntity)obj);
                    }
				}
			}

		}*/

        /// <summary>
        /// Retrieves the type as specified by the provided string.
        /// </summary>
        /// <param name="type">The fully qualified name of the type to retrieve.</param>
        /// <returns>The type specified by the provided string.</returns>
        public Type GetShortType(string type)
        {
            if (type.IndexOf(".Modules.") > -1)
            {
                string[] parts = type.Split('.');
                string ns = String.Empty;
                // Get the name of the assembly containing the specified type
                for (int i = 0; i < parts.Length - 2; i++)
                {
                    ns += parts[i] + ".";
                }
                ns = ns.TrimEnd('.');
                string typeName = parts[parts.Length - 1];
                Assembly assembly = Assembly.Load(ns);
                if (assembly == null)
                    throw new Exception("The assembly '" + ns + "' could not be found.");
                return assembly.GetType(type, true, true);
                //return Type.GetType(typeName, true, true);
            }
            else
            {

                Assembly assembly = Assembly.GetExecutingAssembly();

                return assembly.GetType(type, true, true);
            }
        }


        static public void ImportTypeFromDirectory(string type, string importablesDirectory)
        {
            ImportTypeFromDirectory(type, importablesDirectory, string.Empty);
        }

        static public void ImportTypeFromDirectory(string type, string importablesDirectory, string importedDirectory)
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Importing the specified type from the specified directory.", NLog.LogLevel.Debug))
            {
                AppLogger.Debug("Importables directory: " + importablesDirectory);
                AppLogger.Debug("Type: " + type);

                // If it's a short name and NOT a reference name
                if (type.IndexOf(".") == -1
                    && type.IndexOf("-") == -1)
                {
                    type = EntitiesUtilities.GetType(type).ToString();
                    AppLogger.Debug("Short type provided. Switching to full type name.");
                    AppLogger.Debug("Type: " + type);
                }

                string fullPath = Path.Combine(importablesDirectory, type);
                string importedFullPath = Path.Combine(importedDirectory, type);

                if (Directory.Exists(fullPath))
                {
                    AppLogger.Debug("Type directory found: " + fullPath);

                    XmlEntityManager manager = new XmlEntityManager(fullPath, importedFullPath);
                    manager.ImportAll();
                }
                else
                {
                    AppLogger.Debug("Type directory not found: " + fullPath);
                }
            }
        }

        static public void ImportAllFromDirectory(string importablesDirectory)
        {
            ImportAllFromDirectory(importablesDirectory, String.Empty);
        }

        static public void ImportAllFromDirectory(string importablesDirectory, string importedDirectory)
        {

            using (LogGroup logGroup = AppLogger.StartGroup("Importing all types from the specified directory.", NLog.LogLevel.Debug))
            {
                AppLogger.Debug("Importables directory: " + importablesDirectory);

                foreach (string type in GetImportableTypes(importablesDirectory))
                {
                    AppLogger.Debug("Found type: " + type);

                    string fullPath = Path.Combine(importablesDirectory, type);
                    string importedFullPath = Path.Combine(importedDirectory, type);

                    if (Directory.Exists(fullPath))
                    {
                        AppLogger.Debug("Directory found: " + fullPath);

                        XmlEntityManager manager = new XmlEntityManager(fullPath, importedFullPath);
                        manager.ImportAll();
                    }
                    else
                    {
                        AppLogger.Debug("Directory not found: " + fullPath);
                    }
                }

                /*foreach (string referenceType in GetReferenceTypes(importablesDirectory))
                {
                    AppLogger.Debug("Found reference type: " + referenceType);

                    string fullPath = Path.Combine(importablesDirectory, referenceType);
                    string importedFullPath = Path.Combine(importedDirectory, referenceType);

                    if (Directory.Exists(fullPath))
                    {
                        AppLogger.Debug("Directory found: " + fullPath);

                        XmlEntityManager manager = new XmlEntityManager(fullPath, importedFullPath);
                        manager.ImportAll();
                    }
                    else
                    {
                        AppLogger.Debug("Directory not found: " + fullPath);
                    }
                }*/

            }
        }



        static public string[] GetImportableTypes(string directoryPath)
        {
            List<string> list = new List<string>();
            using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the importable types found.", NLog.LogLevel.Debug))
            {
                string[] entityTypes = GetEntityTypes(directoryPath);
                list.AddRange(entityTypes);
                AppLogger.Debug("# entity types: " + entityTypes.Length);
                string[] referenceTypes = GetReferenceTypes(directoryPath);
                list.AddRange(referenceTypes);
                AppLogger.Debug("# reference types: " + referenceTypes.Length);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Retrieves the names of all the types that can be imported from the specified directory.
        /// </summary>
        /// <param name="directoryPath">The path of the directory to check for loadable objects.</param>
        /// <returns>An array of the names of loadable objects found.</returns>
        static public string[] GetEntityTypes(string directoryPath)
        {
            List<string> list = new List<string>();

            using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the types of entities found.", NLog.LogLevel.Debug))
            {
                AppLogger.Debug("Path: " + directoryPath);
                if (Directory.Exists(directoryPath))
                {
                    foreach (string directory in Directory.GetDirectories(directoryPath))
                    {
                        string lastPart = Path.GetFileName(directory);

                        if (lastPart.IndexOf("-") == -1)
                        {
                            AppLogger.Debug("Adding: "+ lastPart);
                            list.Add(lastPart);
                        }
                    }
                }
            }
        

            return (string[])list.ToArray();
        }

        /// <summary>
        /// Retrieves the names of all the types that can be imported from the specified directory.
        /// </summary>
        /// <param name="directoryPath">The path of the directory to check for loadable objects.</param>
        /// <returns>An array of the names of loadable objects found.</returns>
        static public string[] GetReferenceTypes(string directoryPath)
        {
            List<string> list = new List<string>();

            using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the types of references found.", NLog.LogLevel.Debug))
            {
                AppLogger.Debug("Path: " + directoryPath);

                if (Directory.Exists(directoryPath))
                {
                    foreach (string directory in Directory.GetDirectories(directoryPath))
                    {
                        string lastPart = Path.GetFileName(directory);

                        if (lastPart.IndexOf("-") > -1)
                        {
                            AppLogger.Debug("Adding: " + lastPart);
                            list.Add(lastPart);
                        }
                    }
                }

                AppLogger.Debug("# types found: " + list.Count.ToString());
            }

            return (string[])list.ToArray();
        }

        static public void SaveToFile(IEntity entity, string directory)
        {
            XmlEntityManager manager = new XmlEntityManager(directory);
            manager.SaveToFile(entity);
        }

        public void SaveToFile(IEntity entity)
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Saving the provided entity to file.", NLog.LogLevel.Debug))
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                AppLogger.Debug("Entity type: " + entity.GetType().ToString());


                string fullPath = String.Empty;
                if (EntitiesUtilities.IsReference(entity.GetType()))
                    fullPath = XmlEntitySchemaEditor.CreateReferenceFilePath(ImportablesDirectory, (EntityIDReference)entity);
                else
                    fullPath = Path.Combine(this.ImportablesDirectory, entity.ID.ToString() + ".xml");

                if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                AppLogger.Debug("Full path: " + fullPath);

                EntityReferenceCollection references = EntitiesUtilities.GetReferences(entity);

                if (references != null)
                {
                    AppLogger.Debug("# references: " + references.Count.ToString());
                }
                else
                {
                    AppLogger.Debug("references == null");
                }

                using (Stream stream = File.Create(fullPath))
                {
                    XmlSerializer serializer = new XmlSerializer(entity.GetType());
                    serializer.Serialize(stream, entity);

                    stream.Close();
                }

                foreach (EntityReference reference in references)
                {
                    SaveReferenceToFile(reference);
                }
            }
        }

        public void SaveReferenceToFile(EntityReference reference)
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Saving reference to file.", NLog.LogLevel.Debug))
            {
                string upPath = Path.GetDirectoryName(this.ImportablesDirectory);

                AppLogger.Debug("Parent directory: " + upPath);

                string fullPath = XmlEntitySchemaEditor.CreateReferenceFilePath(upPath, reference);

                AppLogger.Debug("Full path: " + fullPath);

                SerializeToFile(reference, fullPath);
            }

        }

        static public void SerializeToFile(IEntity entity, string fileName)
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Serializing entity to file.", NLog.LogLevel.Debug))
            {
                AppLogger.Debug("File name: " + fileName);

                string directoryPath = Path.GetDirectoryName(fileName);

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                using (FileStream stream = File.Create(fileName))
                {
                    XmlSerializer serializer = new XmlSerializer(entity.GetType());
                    serializer.Serialize(stream, entity);
                    stream.Close();
                }
            }
        }
    }
}
