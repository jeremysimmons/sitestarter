using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Used for scanning assemblies to look for usable business entities.
	/// </summary>
	public class EntityScanner
	{
		private string[] assemblyPaths;
		/// <summary>
		/// Gets/sets the paths of the assemblies containing entities.
		/// </summary>
		public string[] AssemblyPaths
		{
			get {
				if (assemblyPaths == null || assemblyPaths.Length == 0)
					assemblyPaths = GetAssemblyPaths();
				return assemblyPaths; }
			set { assemblyPaths = value; }
		}
		
		private string binDirectoryPath = String.Empty;
		/// <summary>
		/// Gets/sets the path to the directory containing the assemblies.
		/// </summary>
		public string BinDirectoryPath
		{
			get {
				if (binDirectoryPath == String.Empty)
				{
					if (StateAccess.IsInitialized)
						binDirectoryPath = StateAccess.State.PhysicalApplicationPath + Path.DirectorySeparatorChar + "bin";
				}
				return binDirectoryPath; }
			set { binDirectoryPath = value; }
		}
		
		public EntityScanner()
		{
		}
		
		/// <summary>
		/// Retrieves a list of all available assembly paths.
		/// </summary>
		/// <returns>An array of full file paths to the available assemblies.</returns>
		public string[] GetAssemblyPaths()
		{
			List<string> list = new List<string>();
			
			string binPath = BinDirectoryPath;
			
			if (binPath != String.Empty)
			{
				foreach (string file in Directory.GetFiles(binPath, "*.dll"))
				{
					list.Add(file);
				}
			}
			
			return list.ToArray();
		}
		
		
		/// <summary>
		/// Finds all the entities in the available assemblies.
		/// </summary>
		/// <param name="includeTestEntities">A flag indicating whether to find test/mock entities as well.</param>
		/// <returns>An array of info about the entities found.</returns>
		public EntityInfo[] FindEntities(bool includeTestEntities)
		{
			List<EntityInfo> entities = new List<EntityInfo>();
			
			//using (LogGroup logGroup = LogGroup.StartDebug("Finding entities by scanning the attributes of the available type."))
			//{
				foreach (string assemblyPath in AssemblyPaths)
				{
					try
					{
						Assembly assembly = Assembly.LoadFrom(assemblyPath);
						
						if (ContainsEntities(assembly, includeTestEntities))
						{
							foreach (Type type in assembly.GetTypes())
							{
								if (IsEntity(type))
								{
									//LogWriter.Debug("Found entity type: " + type.ToString());
									
									EntityInfo entityInfo = new EntityInfo(type);
									
									if (entityInfo.TypeName != null && entityInfo.TypeName != String.Empty)
									{
										//LogWriter.Debug("Found match.");
										
										entities.Add(entityInfo);
									}
								}
							}
						}
					}
					catch(ReflectionTypeLoadException ex)
					{
						LogWriter.Error("An error occurred when scanning for entities...");
						
						LogWriter.Error(ex.ToString());
					}
					
				}
			//}
			
			return entities.ToArray();
		}
		
		/// <summary>
		/// Checks whether the provided assembly contains entity types by looking for AssemblyContainsEntitiesAttribute.
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public bool ContainsEntities(Assembly assembly)
		{
			return ContainsEntities(assembly, false);
		}
		
		/// <summary>
		/// Checks whether the provided assembly contains entity types by looking for AssemblyContainsEntitiesAttribute.
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="includeTestEntities">A flag indicating whether to include test/mock entities as well.</param>
		/// <returns></returns>
		public bool ContainsEntities(Assembly assembly, bool includeTestEntities)
		{
			bool doesContainEntities = false;
			
			object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyContainsEntitiesAttribute), true);
			if (attributes.Length > 0)
			{
				AssemblyContainsEntitiesAttribute attribute = (AssemblyContainsEntitiesAttribute)attributes[0];
				
				doesContainEntities = includeTestEntities // True if test entities are included, or
					|| !attribute.AreTestEntities; // True if entities are NOT test entities
			}
			
			return doesContainEntities;
		}
		
		/// <summary>
		/// Checks whether the provided type is a entity.
		/// Note: Matches true for base entities and interfaces as well.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool IsEntity(Type type)
		{
			bool matchesInterface = false;
			//bool isNotInterface = false;
			//bool isNotBaseEntity = false;
			bool hasAttribute = false;
			
			//using (LogGroup logGroup = LogGroup.Start("Checks whether the provided type is a entity.", NLog.LogLevel.Debug))
			//{
			//	LogWriter.Debug("Type: " + type.ToString());
			
			
			// TODO: This function is a performance hot spot. The Type.GetInterface function is the issue.
			// See if performance can be improved, possibly by removing...
			// type.GetInterface("IEntity") != null
			// ...and relying on...
			// typeof(IEntity).IsAssignableFrom(type)
			
			matchesInterface = typeof(IEntity).IsAssignableFrom(type)
				|| type.GetInterface("IEntity") != null
				|| type.Name == "IEntity";
			
			hasAttribute = type.GetCustomAttributes(typeof(EntityAttribute), true).Length > 0;
			
			//	LogWriter.Debug("Matches interface: " + matchesInterface);
			//}
			
			return matchesInterface
				&& hasAttribute;
		}
	}
}
