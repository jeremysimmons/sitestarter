using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Xml.XPath;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Data
{
    /// <summary>
    /// Description of UpdateUtilities.
    /// </summary>
    public class XmlEntitySchema
    {
       /* public string ReferenceTypes
        {
            get { return GetReferenceTypes(); }
        }*/

        static public string[] GetReferenceTypes(NameValueCollection referenceMappings)
        {
            if (referenceMappings == null)
                throw new ArgumentNullException("referenceMappings");

            List<string> list = new List<string>();

            using (LogGroup logGroup = AppLogger.StartGroup("Retrieving reference types", NLog.LogLevel.Debug))
            {

                foreach (string key in referenceMappings)
                {
                    AppLogger.Debug("Item value: " + referenceMappings[key]);

                    string[] parts = referenceMappings[key].Split('-');

                    if (parts.Length >= 2)
                    {
                        string[] parts1 = parts[0].Split('.');
                        string[] parts2 = parts[1].Split('.');

                        if (parts1.Length > 0 && parts2.Length > 0)
                        {
                            string[] nameParts = new string[] { parts1[0], parts2[0] };

                            Array.Sort(nameParts);

                            string name = nameParts[0] + "-" + nameParts[1];

                            AppLogger.Debug("Adding " + name);

                            if (!list.Contains(name))
                                list.Add(name);
                        }
                    }
                }
            }

            return list.ToArray();
        }

        private string sourceDirectory;
        /// <summary>
        /// 
        /// </summary>
        public string SourceDirectory
        {
            get { return sourceDirectory; }
            set { sourceDirectory = value; }
        }

        private string convertedDirectory;
        /// <summary>
        /// 
        /// </summary>
        public string ConvertedDirectory
        {
            get { return convertedDirectory; }
            set { convertedDirectory = value; }
        }


        private string importedDirectory;
        /// <summary>
        /// 
        /// </summary>
        public string ImportedDirectory
        {
            get { return importedDirectory; }
            set { importedDirectory = value; }
        }

        private NameValueCollection referenceMappings;
        public NameValueCollection ReferenceMappings
        {
            get { return referenceMappings; }
        }

        public XmlEntitySchema(string sourceDirectory, string convertedDirectory, string importedDirectory)
        {
            SourceDirectory = sourceDirectory;
            ConvertedDirectory = convertedDirectory;
            ImportedDirectory = importedDirectory;
        }


        public XmlEntitySchema(string applicationPath)
        {
            SourceDirectory = GetImportsDirectory(applicationPath);
            ConvertedDirectory = GetConvertedDirectory(applicationPath);
            ImportedDirectory = GetImportedDirectory(applicationPath);
        }

        /*public void ExecuteConversion()
        {
        }*/

        static public string GetImportsDirectory(string applicationPath)
        {
            string dir = string.Empty;

            using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the imports/legacy directory.", NLog.LogLevel.Debug))
            {
                dir = Path.Combine(applicationPath, @"App_Data" + Path.DirectorySeparatorChar + "Legacy");

                AppLogger.Debug("Dir: " + dir);
            }

            return dir;
        }


        static public string GetImportedDirectory(string applicationPath)
        {
            string dir = string.Empty;

            using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the imported directory.", NLog.LogLevel.Debug))
            {
                dir = Path.Combine(applicationPath, @"App_Data" + Path.DirectorySeparatorChar + "Imported");

                AppLogger.Debug("Dir: " + dir);
            }

            return dir;
        }


        static public string GetConvertedDirectory(string applicationPath)
        {
            string dir = string.Empty;

            using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the converted directory.", NLog.LogLevel.Debug))
            {
                dir = Path.Combine(applicationPath, @"App_Data" + Path.DirectorySeparatorChar + "LegacyConverted");

                AppLogger.Debug("Dir: " + dir);
            }

            return dir;
        }


        public void ConvertReferences(NameValueCollection referenceMappings)
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Converting the references in the specified directory.", NLog.LogLevel.Debug))
            {
                AppLogger.Debug(referenceMappings.Count + " mappings found.");

                ConvertReferences(SourceDirectory,
                                  ConvertedDirectory,
                                  referenceMappings);
            }

        }

        public void ConvertReferences(string importsDirectory, string convertedDirectory, NameValueCollection referenceMappings)
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Converting the references in the specified directory to another specified directory.", NLog.LogLevel.Debug))
            {
                AppLogger.Debug("Imports directory: " + importsDirectory);
                AppLogger.Debug("Converted directory: " + convertedDirectory);

                if (Directory.Exists(importsDirectory))
                {
                    if (!Directory.Exists(convertedDirectory))
                        Directory.CreateDirectory(convertedDirectory);

                    string[] files = Directory.GetFiles(importsDirectory);
                    if (files.Length == 0)
                        AppLogger.Debug("No files found in imports directory. Skipped.");
                    else
                    {
                        foreach (string file in files)
                        {
                            using (LogGroup logGroup2 = AppLogger.StartGroup("Converting a file.", NLog.LogLevel.Debug))
                            {
                                AppLogger.Debug("File: " + file);

                                ConvertFileReferences(file, convertedDirectory, referenceMappings);
                            }
                        }
                    }

                    string[] directories = Directory.GetDirectories(importsDirectory);
                    if (directories.Length == 0)
                        AppLogger.Debug("No sub directories found.");
                    else
                    {
                        foreach (string directory in directories)
                        {
                            using (LogGroup logGroup3 = AppLogger.StartGroup("Stepping into a directory.", NLog.LogLevel.Debug))
                            {
                                AppLogger.Debug("Directory: " + directory);

                                //if (directory.IndexOf("Converted") == -1)
                                //{
                                //throw new Exception(Path.GetFileName(directory));
                                string subConvertedDirectory = convertedDirectory + Path.DirectorySeparatorChar + Path.GetFileName(directory);

                                //	throw new Exception(subImportsDirectory);

                                AppLogger.Debug("subConvertedDirectory: " + subConvertedDirectory);

                                ConvertReferences(directory, subConvertedDirectory, referenceMappings);
                                //}
                            }
                        }
                    }

                }
                else
                    AppLogger.Debug("Imports directory not found. Skipped.");
            }
        }

        public void ConvertFileReferences(string fileName, string convertedDirectory, NameValueCollection referenceMappings)
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Converting references from old version to new version in specified file.", NLog.LogLevel.Debug))
            {
                AppLogger.Debug("fileName = " + fileName);
                AppLogger.Debug("convertedDirectory = " + convertedDirectory);

                if (referenceMappings == null)
                    throw new ArgumentNullException("referenceMappings");

                AppLogger.Debug("# reference mappings: " + referenceMappings.Count.ToString());

                //throw new Exception(convertedDirectory); 
                string newFileName = convertedDirectory + Path.DirectorySeparatorChar + Path.GetFileName(fileName);

                AppLogger.Debug("newFileName = " + newFileName);

                if (!Directory.Exists(convertedDirectory))
                    Directory.CreateDirectory(convertedDirectory);

                XmlDocument doc = null;
                using (FileStream stream = File.OpenRead(fileName))
                {
                    doc = new XmlDocument();
                    doc.Load(stream);

                    AppLogger.Debug("Loaded stream");

                    stream.Close();
                }


                Dictionary<string, XmlDocument> data = ConvertFileReferences(doc, convertedDirectory, referenceMappings);
                foreach (string file in data.Keys)
                {
                    AppLogger.Debug("Saving new reference file: " + file);

                    XmlDocument d = data[file];

                    string dir = Path.GetDirectoryName(file);
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    d.Save(file);
                }

                // Delete the original so it doesn't get converted twice
                //File.Delete(fileName);
                // Testing without it

            }
        }

        public Dictionary<string, XmlDocument> ConvertFileReferences(XmlDocument doc, string convertedDirectory, NameValueCollection referenceMappings)
        {
            Dictionary<string, XmlDocument> newData = new Dictionary<string, XmlDocument>();

            using (LogGroup logGroup = AppLogger.StartGroup("Converting references from old version to new version in specified document.", NLog.LogLevel.Debug))
            {

                if (!Directory.Exists(convertedDirectory))
                    Directory.CreateDirectory(convertedDirectory);

                XmlDocument convertedDoc;

                try
                {
                    convertedDoc = (XmlDocument)doc.Clone();

                    string shortName = doc.DocumentElement.Name;

                    AppLogger.Debug("shortName = " + shortName);

                    NameValueCollection mappingsFor = GetReferenceMappingsFor(referenceMappings, shortName);

                    Guid id = Guid.Empty;

                    foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                    {

                        if (node.Name == "ID")
                            id = new Guid(node.InnerText);

                        AppLogger.Debug("ID: " + id.ToString());

                        foreach (string key in mappingsFor)
                        {

                            string[] keyParts = key.Split('.');
                            //throw new Exception(keyParts[1]);
                            if (node.Name.ToLower() == keyParts[1].ToLower())
                            {
                                // Remove obsolete property
                                XmlNode convertedNode = convertedDoc.SelectSingleNode(shortName + "/" + node.Name);
                                convertedDoc.DocumentElement.RemoveChild(convertedNode);

                                // Create the references from the node
                                foreach (XmlNode subNode in node.ChildNodes)
                                {

                                    Guid referencedEntityID = new Guid(subNode.InnerText);

                                    CreateReferences(newData, convertedDirectory, mappingsFor[key], id, referencedEntityID, subNode, mappingsFor);
                                }
                            }
                        }
                    }

                    string newFileName = Path.Combine(convertedDirectory, id.ToString() + ".xml");

                    convertedDoc.Save(newFileName);


                }
                catch (XmlException ex)
                {
                    throw ex;
                    //Ignore
                }
            }

            return newData;
        }

        public void CreateReferences(Dictionary<String, XmlDocument> data, string convertedDirectory, string keyValue, Guid entityID, Guid entity2ID, XmlNode subNode, NameValueCollection mappingsFor)
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Creating a reference for the import process.", NLog.LogLevel.Debug))
            {

                string[] newMappingParts = keyValue.Split('-');

                string property1Name = String.Empty;
                string property2Name = String.Empty;

                string type1Name = String.Empty;
                string type2Name = String.Empty;

                string[] typeNames = new String[] { };

                if (newMappingParts[0] != String.Empty)
                {
                    string[] parts1 = newMappingParts[0].Split('.');

                    type1Name = parts1[0];
                    if (parts1.Length > 1)
                        property1Name = parts1[1];
                }

                if (newMappingParts[1] != String.Empty)
                {
                    string[] parts2 = newMappingParts[1].Split('.');

                    type2Name = parts2[0];
                    if (parts2.Length > 1)
                        property2Name = parts2[1];
                }

                string referenceFileName = CreateReferenceFilePath(Path.GetDirectoryName(convertedDirectory),
                    type1Name, type2Name, entityID, entity2ID, property1Name, property2Name);

                AppLogger.Debug("referenceFileName = " + referenceFileName);

                data.Add(referenceFileName, CreateReference(
                                type1Name, property1Name, entityID,
                                type2Name, property2Name, entity2ID));


            }
        }

        static public string CreateReferenceFilePath(string sourceDirectory, EntityIDReference reference)
        {
            return CreateReferenceFilePath(sourceDirectory, reference.Type1Name, reference.Type2Name, reference.Entity1ID, reference.Entity2ID, reference.Property1Name, reference.Property2Name);
        }

        static public string CreateReferenceFilePath(string sourceDirectory, string type1Name, string type2Name, Guid entity1ID, Guid entity2ID, string property1Name, string property2Name)
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
                    //DataUtilities.GetDataStoreName(typeNames);

                referenceFileName = sourceDirectory + Path.DirectorySeparatorChar;
                if (Path.GetDirectoryName(sourceDirectory) != type1Name + "-" + type2Name)
                    referenceFileName += typeName + Path.DirectorySeparatorChar;
                referenceFileName += entity1ID + "-" + property1Name + "---" + entity2ID + "-" + property2Name + ".xml";

                AppLogger.Debug("Reference file name: " + referenceFileName);
            }
            return referenceFileName;
        }

        /*private void ImportData()
        {
        }*/

        /*private void ConvertUserFile(XmlDocument doc, string newFileName)
        {
			
			
            CreateReference(Path.GetDirectoryName(Path.GetDirectoryName(newFileName)),
                            "User", "Roles", Guid.NewGuid(),
                            "UserRole", "Users", Guid.NewGuid());
        }*/

        public XmlDocument CreateReference(string type1, string property1, Guid id1,
                                           string type2, string property2, Guid id2)
        {

            EntityReference reference = new EntityReference();
            using (LogGroup logGroup = AppLogger.StartGroup("Creating a single reference.", NLog.LogLevel.Debug))
            {
                AppLogger.Debug("type1: " + type1);
                AppLogger.Debug("property1: " + property1);
                AppLogger.Debug("id1: " + id1.ToString());

                AppLogger.Debug("type2: " + type2);
                AppLogger.Debug("property2: " + property2);
                AppLogger.Debug("id2: " + id2.ToString());

                reference.ID = Guid.NewGuid();
                reference.Type1Name = type1;
                reference.Property1Name = property1;
                reference.Entity1ID = id1;
                reference.Type2Name = type2;
                reference.Property2Name = property2;
                reference.Entity2ID = id2;

               //string[] typeNames = new string[] { type1, type2 };

                //string[] ids = new string[] { id1.ToString(), id2.ToString() };

                //Array.Sort(typeNames);
                //Array.Sort(ids);

            }
            //throw new Exception(referenceFileName);
            return SerializeToDocument(reference);
        }


        public XmlDocument SerializeToDocument(IEntity entity)
        {
            XmlDocument doc = new XmlDocument();

            using (LogGroup logGroup = AppLogger.StartGroup("Serializing the specified entity to XmlDocument.", NLog.LogLevel.Debug))
            {
                XPathNavigator nav = doc.CreateNavigator();

                using (XmlWriter writer = nav.AppendChild())
                {
                    XmlSerializer serializer = new XmlSerializer(entity.GetType());
                    serializer.Serialize(writer, entity);
                }
            }

            return doc;
        }


        public NameValueCollection GetReferenceMappingsFor(NameValueCollection mappings, string typeName)
        {

            NameValueCollection mappingsFor = new NameValueCollection();

            foreach (string key in mappings)
            {
                string[] keyParts = key.Split('.');

                if (keyParts[0] == typeName)
                {
                    mappingsFor.Add(key, mappings[key]);
                }
            }
            return mappingsFor;
        }

        static public string GenerateExplanation(NameValueCollection referenceMappings)
        {
            StringBuilder builder = new StringBuilder();

            foreach (string key in referenceMappings.Keys)
            {
                string[] parts = referenceMappings[key].Split('-');
                builder.Append("Changed " + key + " to " + parts[0]);
                builder.Append("<br/>");
                builder.Append("Created reference objects between " + parts[0] + " and " + parts[1]);
                //builder.Append("Changed " + key + " to " + parts[1]);
                builder.Append("<br/>");
            }

            return builder.ToString();
        }

        public void Finished()
        {
            foreach (string file in Directory.GetFiles(ConvertedDirectory))
            {
                File.Delete(file);
                
            }
        }
    }
}
