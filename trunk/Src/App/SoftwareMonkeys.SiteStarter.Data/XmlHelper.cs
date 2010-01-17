using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using SoftwareMonkeys.SiteStarter.Entities;
//using Db4objects.Db4o.Query;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Diagnostics;

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
		static public object[] LoadObjects(string directory, int batchCount)
		{
			List<object> list = new List<object>();

			int i = 0;

			if (Directory.Exists(directory))
			{
				foreach (string file in Directory.GetFiles(directory, "*.xml"))
				{
					if (i >= batchCount)
						break;
					else
					{
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
							
							string typeName = doc.DocumentElement.Name;

							
							Type type = null;
							
							if (typeName == "EntityIDReference")
								type = typeof(EntityIDReference);
							else if (typeName == "EntityReference")
								type = typeof(EntityReference);
							else
								type = EntitiesUtilities.GetType(typeName);
							
							
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

						object existing = null;
						if (obj != null && obj is BaseEntity)
						{
							// TODO: Remove if not needed
							//					 Db4oDataStore store = (Db4oDataStore)DataAccess.Data.Stores[obj.GetType()];
							//		IQuery query = store.Query(obj.GetType());
							//                    query.Descend("id").Constrain(((Entity)obj).ID);
							//                    existing = store.GetEntity(query.Execute());

							existing = DataAccess.Data.Stores[DataUtilities.GetDataStoreName((IEntity)obj)].GetEntityByTypeAndProperty(obj.GetType(), "ID", ((BaseEntity)obj).ID);
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
			using (LogGroup logGroup = AppLogger.StartGroup("Importing all objects from the provided directory.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Directory: " + directoryPath);
				
				string[] types = XmlHelper.GetObjectTypes(directoryPath);
				
				foreach (string directory in Directory.GetDirectories(directoryPath))
				{
					ImportAllObjects(directory);
				}

				//foreach (string type in types)
				//{
				object[] objects = LoadObjects(directoryPath, 99999999);

				foreach (object obj in objects)
				{
					using (LogGroup logGroup2 = AppLogger.StartGroup("Importing object", NLog.LogLevel.Debug))
					{
						AppLogger.Debug("Object type: " + obj.GetType().ToString());
						
						//throw new Exception(obj.GetType().ToString());
						if (typeof(IEntity).IsAssignableFrom(obj.GetType()))
						{
							AppLogger.Debug("Object is IEntity");
							//throw new Exception(obj.GetType().ToString());
							IEntity entity = (IEntity)obj;
							DataAccess.Data.Save(entity);
							
							MarkAsImported(directoryPath, entity);
						}
						else
						{
							AppLogger.Debug("Object is not IEntity");
						}
					}
				}
				
				//}
			}
		}

		/// <summary>
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
		
		static public void MarkAsImported(string directory, IEntity entity)
		{
			string fileName = String.Empty;
			string toFileName = String.Empty;
			
			string typeName = Path.GetFileName(directory);
			
			if (entity is EntityIDReference
			    || entity is EntityReference)
			{
				string[] typeNames = new string[] {((EntityIDReference)entity).Type1Name,
					((EntityIDReference)entity).Type2Name};
				
				Array.Sort(typeNames);
				
				string[] ids = new string[] {((EntityIDReference)entity).Entity1ID.ToString(),
					((EntityIDReference)entity).Entity2ID.ToString()};
				
				Array.Sort(ids);
				
				fileName = Path.Combine(directory, ids[0] + "---" + ids[1] + ".xml");
			}
			else
			{
				fileName = Path.Combine(directory, entity.ID.ToString() + ".xml");
				
			}
			
			toFileName = Path.Combine(
				Path.Combine(
					Path.Combine(
						Path.GetDirectoryName(Path.GetDirectoryName(directory)),
						"Imported"),
					typeName),
				
				Path.GetFileName(fileName));
			
			if (!Directory.Exists(Path.GetDirectoryName(toFileName)))
				Directory.CreateDirectory(Path.GetDirectoryName(toFileName));
			
			//throw new Exception(fileName + " | " + toFileName);
			
			if (File.Exists(toFileName))
				File.Delete(toFileName);
			File.Move(fileName, toFileName);
		}
	}
}
