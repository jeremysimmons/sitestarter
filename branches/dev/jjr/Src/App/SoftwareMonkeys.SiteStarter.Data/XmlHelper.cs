using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using SoftwareMonkeys.SiteStarter.Entities;
//using Db4objects.Db4o.Query;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Data
{
    public class XmlHelper
    {
        #region Import functions
        /// <summary>
        /// Load all the objects (from XML) from the specified directory.
        /// </summary>
        /// <param name="objectType">The fully qualified name of the type to retrieve.</param>
        /// <param name="batchCount"></param>
        /// <returns>An array of the loaded objects.</returns>
        static public object[] LoadObjects(string directory, string objectType, int batchCount)
        {
            List<object> list = new List<object>();

            int i = 0;

            if (Directory.Exists(Path.Combine(directory, objectType)))
            {
                foreach (string file in Directory.GetFiles(Path.Combine(directory, objectType), "*.xml"))
                {
                    if (i >= batchCount)
                        break;
                    else
                    {
                        object obj = null;
                        try
                        {
                            // Skip all Guid objects. They don't need to be imported
                            if (objectType != "System.Guid")
                            {

				Type type = GetType(objectType);
				if (type == null)
					throw new ArgumentException("The specified object type '" + objectType + "' could not be found.");

                                XmlSerializer serializer = new XmlSerializer(type);
                                using (StreamReader reader = new StreamReader(file))
                                {
                                    obj = serializer.Deserialize(reader);
                                    reader.Close();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.ToString().ToLower().IndexOf("root element is missing") > -1)
                                throw new Exception("Root element is missing in file '" + file + "'");
                            // TODO: Reverse. Currently ignores exception
                            //else
                            //    throw ex;
                            
                        }

                        object existing = null;
                        if (obj != null && obj is BaseEntity)
                        {
				// TODO: Remove if not needed
                           /* Db4oDataStore store = (Db4oDataStore)DataAccess.Data.Stores[obj.GetType()];
				IQuery query = store.Query(obj.GetType());
                            query.Descend("id").Constrain(((Entity)obj).ID);
                            existing = store.GetEntity(query.Execute());*/

                            existing = DataAccess.Data.GetEntity(obj.GetType(), "id", ((BaseEntity)obj).ID);
                        }

                        if (obj != null && existing == null)
                        {
                            list.Add(obj);
                            i++;
                        }
                    }
                }

            }
            return (object[])list.ToArray();
        }

        /// <summary>
        /// Retrieves the names of all the types that can be imported from the specified directory.
        /// </summary>
        /// <param name="directoryPath">The path of the directory to check for loadable objects.</param>
        /// <returns>An array of the names of loadable objects found.</returns>
        public static string[] GetObjectTypes(string directoryPath)
        {
            List<string> list = new List<string>();

            foreach (string directory in Directory.GetDirectories(directoryPath))
            {
                list.Add(Path.GetFileName(directory));
            }

            return (string[])list.ToArray();
        }

        /// <summary>
        /// Imports all the objects from the specified directory into the data store.
        /// </summary>
        /// <param name="directoryPath">The path of the directory to load the data from.</param>
        static public void ImportAllObjects(string directoryPath)
        {
            string[] types = XmlHelper.GetObjectTypes(directoryPath);

            foreach (string type in types)
            {
                object[] objects = LoadObjects(directoryPath, type, 99999999);

                foreach (object obj in objects)
                {
                    if (obj is BaseEntity)
                    {
                        BaseEntity entity = (BaseEntity)obj;
                        DataAccess.Data.Stores[entity.GetType()].Save(entity);
                    }
                }
            }
        }

        /// <summary>
        /// Imports all the objects of the specified type from the specified directory into the data store.
        /// </summary>
        /// <param name="directoryPath">The path of the directory to load the data from.</param>
        static public void ImportAll(string type, string directoryPath)
        {
            // TODO: Make it so the type doesn't need to be converted to string
            object[] objects = LoadObjects(directoryPath, type, 99999999);

            foreach (object obj in objects)
            {
                if (obj is BaseEntity)
                {
                    BaseEntity entity = (BaseEntity)obj;
                    DataAccess.Data.Stores[entity.GetType()].Save(entity);
                }
            }

        }
        #endregion

       /// <summary>
        /// Retrieves the type as specified by the provided string.
        /// </summary>
        /// <param name="type">The fully qualified name of the type to retrieve.</param>
        /// <returns>The type specified by the provided string.</returns>
        static public Type GetType(string type)
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
    }
}
