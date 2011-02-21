using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using SoftwareMonkeys.SiteStarter.Diagnostics;

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
					if (Configuration.Config.IsInitialized)
						binDirectoryPath = Configuration.Config.Application.PhysicalApplicationPath + Path.DirectorySeparatorChar + "bin";
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
		/// <returns>An array of info about the entities found.</returns>
		public EntityInfo[] FindEntities()
		{
			List<EntityInfo> entities = new List<EntityInfo>();
			
			//using (LogGroup logGroup = LogGroup.Start("Finding entities by scanning the attributes of the available type.", NLog.LogLevel.Debug))
			//{
				foreach (string assemblyPath in AssemblyPaths)
				{
					try
					{
						Assembly assembly = Assembly.LoadFrom(assemblyPath);
						
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
					catch(ReflectionTypeLoadException ex)
					{
						LogWriter.Error(ex.ToString());
					}
					
				}
			//}
			
			return entities.ToArray();
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
			
			//using (LogGroup logGroup = LogGroup.Start("Checks whether the provided type is a entity.", NLog.LogLevel.Debug))
			//{
			//	LogWriter.Debug("Type: " + type.ToString());
				
				matchesInterface = typeof(IEntity).IsAssignableFrom(type)
					|| type.GetInterface("IEntity") != null
					|| type.Name == "IEntity";
				
				
			//	LogWriter.Debug("Matches interface: " + matchesInterface);
			//}
			
			return matchesInterface;
		}
	}
}
