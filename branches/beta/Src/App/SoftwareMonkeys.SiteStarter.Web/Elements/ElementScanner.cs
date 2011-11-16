using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Web.Elements
{
	/// <summary>
	/// Used for scanning assemblies to look for usable business elements.
	/// </summary>
	public class ElementScanner : BaseElementScanner
	{
		
		private ElementFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used for generating element file names and paths.
		/// </summary>
		public ElementFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new ElementFileNamer();
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
		/// <param name="includeTestElements"></param>
		/// <returns></returns>
		public bool ContainsElements(Assembly assembly, bool includeTestElements)
		{
			bool doesContain = false;
			
			object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyContainsElementsAttribute), true);
			if (attributes.Length > 0)
			{
				AssemblyContainsElementsAttribute attribute = (AssemblyContainsElementsAttribute)attributes[0];
				
				doesContain = includeTestElements // True if test elements are included, or
					|| !attribute.AreTestElements; // True if elements are NOT test elements
			}
			
			return doesContain;
		}
		
		/// <summary>
		/// Finds all the elements in the available assemblies.
		/// </summary>
		/// <param name="includeTestElements"></param>
		/// <returns>An array of info about the elements found.</returns>
		public override ElementInfo[] FindElements(bool includeTestElements)
		{
			List<ElementInfo> elements = new List<ElementInfo>();
			
			using (LogGroup logGroup = LogGroup.Start("Finding elements by scanning the attributes of the available types.", NLog.LogLevel.Debug))
			{
				foreach (string assemblyPath in AssemblyPaths)
				{
					Assembly assembly = Assembly.LoadFrom(assemblyPath);
					
					// Disabled because it bloats logs
					//LogWriter.Debug("Assembly: " + assembly.FullName);
					
					if (ContainsElements(assembly, includeTestElements))
					{
						LogWriter.Debug("Assembly does contain elements");
						
						try
						{
							foreach (Type type in assembly.GetTypes())
							{
								LogWriter.Debug("Checking type: " + type.FullName);
								
								if  (IsElement(type))
								{
									LogWriter.Debug("Found element type: " + type.ToString());
									
									ElementInfo elementInfo = new ElementInfo(type);
									
									LogWriter.Debug("Found match.");
									
									elements.Add(elementInfo);
								}
							}
						}
						catch (Exception ex)
						{
							LogWriter.Error("Error occurred while trying to scan for elements.");
							
							LogWriter.Error(ex);
						}
					}
					// Disabled because it bloats logs
					//else
					//	LogWriter.Debug("Assembly doesn't contain elements. Skipping.");
				}
			}
			
			return elements.ToArray();
		}
		
		/// <summary>
		/// Checks whether the provided type is a element.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool IsElement(Type type)
		{
			bool isElement = false;
			using (LogGroup logGroup = LogGroup.Start("Checks whether the provided type is a element.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Type: " + type.ToString());
				
				bool matchesInterface = false;
				bool isNotInterface = false;
				bool isNotAbstract = false;
				bool hasAttribute = false;
				
				matchesInterface = (type.GetInterface("IElement") != null);
				
				isNotInterface = !type.IsInterface;
				
				isNotAbstract = !type.IsAbstract;
				
				hasAttribute = type.GetCustomAttributes(typeof(ElementAttribute), true).Length > 0;

				LogWriter.Debug("Matches interface: " + matchesInterface);
				LogWriter.Debug("Is not element interface: " + isNotInterface);
				LogWriter.Debug("Has attribute: " + isNotAbstract);
				
				isElement = matchesInterface
					&& isNotInterface
					&& isNotAbstract
					&& hasAttribute;
				
				LogWriter.Debug("Is element: "+ isElement.ToString());
			}
			
			return isElement;
		}
	}
}
