using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using SoftwareMonkeys.SiteStarter.Diagnostics;

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
		/// Gets/sets the paths of the assemblies containing controllers.
		/// </summary>
		public string[] AssemblyPaths
		{
			get { return assemblyPaths; }
			set { assemblyPaths = value; }
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
		/// Finds all the controllers in the available assemblies.
		/// </summary>
		/// <returns>An array of info about the controllers found.</returns>
		public override ControllerInfo[] FindControllers()
		{
			List<ControllerInfo> controllers = new List<ControllerInfo>();
			
			using (LogGroup logGroup = AppLogger.StartGroup("Finding controllers by scanning the attributes of the available type.", NLog.LogLevel.Debug))
			{
				if (AssemblyPaths == null)
					AssemblyPaths = GetAssemblyPaths();
				
				foreach (string assemblyPath in AssemblyPaths)
				{
					Assembly assembly = Assembly.LoadFrom(assemblyPath);
					
					foreach (Type type in assembly.GetTypes())
					{
						if  (IsController(type))
						{
							AppLogger.Debug("Found controller type: " + type.ToString());
							
							ControllerInfo controllerInfo = new ControllerInfo(type);
							
							if (controllerInfo.TypeName != null && controllerInfo.TypeName != String.Empty
							    && controllerInfo.Action != null && controllerInfo.Action != String.Empty)
							{
								AppLogger.Debug("Found match.");
								
								controllers.Add(controllerInfo);
							}
						}
					}
				}
			}
			
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
			
			using (LogGroup logGroup = AppLogger.StartGroup("Checks whether the provided type is a controller.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Type: " + type.ToString());
				
				matchesInterface = (type.GetInterface("IController") != null);
				
				isNotInterface = !type.IsInterface;
				
				isNotAbstract = !type.IsAbstract;
				
				AppLogger.Debug("Matches interface: " + matchesInterface);
				AppLogger.Debug("Is not controller interface: " + isNotInterface);
			}
			
			return matchesInterface
				&& isNotInterface
				&& isNotAbstract;
		}
	}
}
