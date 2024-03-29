﻿using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// Used for scanning assemblies to look for usable business controllers.
	/// </summary>
	public class ControllerScanner : BaseControllerScanner
	{
		
		private ControllerFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used for generating controller file names and paths.
		/// </summary>
		public ControllerFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new ControllerFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		
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
		/// <param name="includeTestControllers"></param>
		/// <returns></returns>
		public bool ContainsControllers(Assembly assembly, bool includeTestControllers)
		{
			bool doesContain = false;
			
			object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyContainsControllersAttribute), true);
			if (attributes.Length > 0)
			{
				AssemblyContainsControllersAttribute attribute = (AssemblyContainsControllersAttribute)attributes[0];
				
				doesContain = includeTestControllers // True if test controllers are included, or
					|| !attribute.AreTestControllers; // True if controllers are NOT test controllers
			}
			
			return doesContain;
		}
		
		/// <summary>
		/// Finds all the controllers in the available assemblies.
		/// </summary>
		/// <param name="includeTestControllers"></param>
		/// <returns>An array of info about the controllers found.</returns>
		public override ControllerInfo[] FindControllers(bool includeTestControllers)
		{
			List<ControllerInfo> controllers = new List<ControllerInfo>();
			
			//using (LogGroup logGroup = LogGroup.Start("Finding controllers by scanning the attributes of the available type.", NLog.LogLevel.Debug))
			//{
			foreach (string assemblyPath in AssemblyPaths)
			{
				Assembly assembly = Assembly.LoadFrom(assemblyPath);
				
				if (ContainsControllers(assembly, includeTestControllers))
				{
					try
					{
						foreach (Type type in assembly.GetTypes())
						{
							if  (IsController(type))
							{
								//LogWriter.Debug("Found controller type: " + type.ToString());
								
								ControllerInfo controllerInfo = new ControllerInfo(type);
								
								if (controllerInfo.TypeName != null && controllerInfo.TypeName != String.Empty
								    && controllerInfo.Action != null && controllerInfo.Action != String.Empty)
								{
									//LogWriter.Debug("Found match.");
									
									controllers.Add(controllerInfo);
								}
							}
						}
					}
					catch (Exception ex)
					{
						LogWriter.Error("Error occurred while trying to scan for controllers.");
						
						LogWriter.Error(ex);
					}
				}
			}
			//}
			
			return controllers.ToArray();
		}
		
		/// <summary>
		/// Checks whether the provided type is a controller.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool IsController(Type type)
		{
			bool matchesInterface = false;
			bool isNotInterface = false;
			bool isNotAbstract = false;
			bool hasAttribute = false;
			
			//using (LogGroup logGroup = LogGroup.Start("Checks whether the provided type is a controller.", NLog.LogLevel.Debug))
			//{
			//	LogWriter.Debug("Type: " + type.ToString());
			
			matchesInterface = (type.GetInterface("IController") != null);
			
			isNotInterface = !type.IsInterface;
			
			isNotAbstract = !type.IsAbstract;
			
			hasAttribute = type.GetCustomAttributes(typeof(ControllerAttribute), true).Length > 0;
			
			//
			//	LogWriter.Debug("Matches interface: " + matchesInterface);
			//	LogWriter.Debug("Is not controller interface: " + isNotInterface);
			//}
			
			return matchesInterface
				&& isNotInterface
				&& isNotAbstract
				&& hasAttribute;
		}
	}
}
