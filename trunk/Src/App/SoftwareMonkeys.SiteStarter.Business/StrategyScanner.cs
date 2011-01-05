using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using SoftwareMonkeys.SiteStarter.Diagnostics;

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
			
			string binPath = Configuration.Config.Application.PhysicalApplicationPath
				+ Path.DirectorySeparatorChar + "bin";
			
			foreach (string file in Directory.GetFiles(binPath, "*.dll"))
			{
				list.Add(file);
			}
			
			return list.ToArray();
		}
		
		
		/// <summary>
		/// Finds all the strategies in the available assemblies.
		/// </summary>
		/// <returns>An array of info about the strategies found.</returns>
		public StrategyInfo[] FindStrategies()
		{
			List<StrategyInfo> strategies = new List<StrategyInfo>();
			
			//using (LogGroup logGroup = AppLogger.StartGroup("Finding strategies by scanning the attributes of the available type.", NLog.LogLevel.Debug))
			//{				
				foreach (string assemblyPath in AssemblyPaths)
				{
					Assembly assembly = Assembly.LoadFrom(assemblyPath);
					
					foreach (Type type in assembly.GetTypes())
					{
						if (IsStrategy(type))
						{
							//AppLogger.Debug("Found strategy type: " + type.ToString());
							
							StrategyInfo strategyInfo = new StrategyInfo(type);
							
							if (strategyInfo.TypeName != null && strategyInfo.TypeName != String.Empty
							    && strategyInfo.Action != null && strategyInfo.Action != String.Empty)
							{
								//AppLogger.Debug("Found match.");
								
								strategies.Add(strategyInfo);
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
			
			//using (LogGroup logGroup = AppLogger.StartGroup("Checks whether the provided type is a strategy.", NLog.LogLevel.Debug))
			//{
			//	AppLogger.Debug("Type: " + type.ToString());
				
				matchesInterface = typeof(IStrategy).IsAssignableFrom(type)
					|| type.GetInterface("IStrategy") != null;
				
				isNotInterface = !type.IsInterface;
				
				isNotAbstract = (!type.IsAbstract);
				
			//	AppLogger.Debug("Matches interface: " + matchesInterface);
			//	AppLogger.Debug("Is not strategy interface: " + isNotInterface);
			//}
			
			return matchesInterface
				&& isNotInterface
				&& isNotAbstract;
		}
	}
}
