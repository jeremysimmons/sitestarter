using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used for scanning assemblies to look for usable business strategies.
	/// </summary>
	public class StrategyScanner
	{
		private string[] assemblyPaths;
		/// <summary>
		/// Gets/sets the paths of the assemblies containing strategies.
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
		
		public StrategyScanner()
		{
		}
		
		/// <summary>
		/// Retrieves a list of all available assembly paths.
		/// </summary>
		/// <returns>An array of full file paths to the available assemblies.</returns>
		public string[] GetAssemblyPaths()
		{
			List<string> list = new List<string>();
			
			foreach (string file in Directory.GetFiles(BinDirectoryPath, "*.dll"))
			{
				list.Add(file);
			}
			
			return list.ToArray();
		}
		
		/// <summary>
		/// Checks whether the provided assembly contains strategy types by looking for AssemblyContainsStrategiesAttribute.
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public bool ContainsStrategies(Assembly assembly)
		{
			bool doesContainStrategies = false;
			
			doesContainStrategies = assembly.GetCustomAttributes(typeof(AssemblyContainsStrategiesAttribute), true).Length > 0;
			
			return doesContainStrategies;
		}
		
		/// <summary>
		/// Finds all the strategies in the available assemblies.
		/// </summary>
		/// <returns>An array of info about the strategies found.</returns>
		public StrategyInfo[] FindStrategies()
		{
			List<StrategyInfo> strategies = new List<StrategyInfo>();
			
			//using (LogGroup logGroup = LogGroup.Start("Finding strategies by scanning the attributes of the available type.", NLog.LogLevel.Debug))
			//{
			foreach (string assemblyPath in AssemblyPaths)
			{
				Assembly assembly = Assembly.LoadFrom(assemblyPath);
				
				if (ContainsStrategies(assembly))
				{
					Type[] types = new Type[] {};
					
					try
					{
						types = assembly.GetTypes();
					}
					catch (ReflectionTypeLoadException ex)
					{
						LogWriter.Error("An error occurred when scanning for busines strategies...");
						
						LogWriter.Error(ex);
					}
					
					foreach (Type type in types)
					{
						if (IsStrategy(type))
						{
							//LogWriter.Debug("Found strategy type: " + type.ToString());
							
							foreach (StrategyInfo strategyInfo in  StrategyInfo.ExtractInfo(type))
							{
								if (strategyInfo.TypeName != null && strategyInfo.TypeName != String.Empty
								    && strategyInfo.Action != null && strategyInfo.Action != String.Empty)
								{
									//LogWriter.Debug("Found match.");
									
									//LogWriter.Debug("Type name: " + strategyInfo.TypeName);
									//LogWriter.Debug("Action: " + strategyInfo.Action);
									
									strategies.Add(strategyInfo);
								}
							}
						}
					}
				}
			}
			//}
			
			return strategies.ToArray();
		}
		
		/// <summary>
		/// Checks whether the provided type is a strategy.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool IsStrategy(Type type)
		{
			bool matchesInterface = false;
			bool isNotInterface = false;
			bool isNotAbstract = false;
			bool hasAttribute = false;
			
			//using (LogGroup logGroup = LogGroup.Start("Checks whether the provided type is a strategy.", NLog.LogLevel.Debug))
			//{
			//	LogWriter.Debug("Type: " + type.ToString());
			
			matchesInterface = typeof(IStrategy).IsAssignableFrom(type)
				|| type.GetInterface("IStrategy") != null;
			
			isNotInterface = !type.IsInterface;
			
			isNotAbstract = (!type.IsAbstract);
			
			hasAttribute = type.GetCustomAttributes(typeof(StrategyAttribute), true).Length > 0;
			
			//	LogWriter.Debug("Matches interface: " + matchesInterface);
			//	LogWriter.Debug("Is not strategy interface: " + isNotInterface);
			//}
			
			return matchesInterface
				&& isNotInterface
				&& isNotAbstract
				&& hasAttribute;
		}
	}
}
