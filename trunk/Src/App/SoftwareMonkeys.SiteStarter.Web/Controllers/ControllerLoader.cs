using System;
using System.IO;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// Used to load controllers (a type of user control) for display on a page.
	/// </summary>
	public class ControllerLoader
	{
		private ControllerFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create file paths for controllers.
		/// </summary>
		public ControllerFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new ControllerFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		protected ControllerInfo[] Controllers;
		
		public ControllerLoader()
		{
		}
		
		/// <summary>
		/// Loads all the controllers found in the controllers directory.
		/// </summary>
		/// <returns>An array of the the controllers found in the directory.</returns>
		public ControllerInfo[] LoadInfoFromDirectory()
		{
			return LoadInfoFromDirectory(false);
		}
		
		/// <summary>
		/// Loads all the controllers found in the controllers directory.
		/// </summary>
		/// <param name="includeDisabled"></param>
		/// <returns>An array of the the controllers found in the directory.</returns>
		public ControllerInfo[] LoadInfoFromDirectory(bool includeDisabled)
		{
			// Logging disabled to boost performance
			//using (LogGroup logGroup = LogGroup.StartDebug("Loading the controllers from the XML file."))
			//{
			if (Controllers == null)
			{
				List<ControllerInfo> validControllers = new List<ControllerInfo>();
				
				ControllerInfo[] controllers = new ControllerInfo[]{};
				
				using (StreamReader reader = new StreamReader(File.OpenRead(FileNamer.ControllersInfoFilePath)))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(ControllerInfo[]));
					controllers = (ControllerInfo[])serializer.Deserialize(reader);
				}
				
				foreach (ControllerInfo controller in controllers)
					if (controller.Enabled || includeDisabled)
						validControllers.Add(controller);
				
				Controllers = validControllers.ToArray();
			}
			//}
			return Controllers;
		}
		
		/// <summary>
		/// Loads the controller from the specified path.
		/// </summary>
		/// <param name="controllerPath">The full path to the controller to load.</param>
		/// <returns>The controller deserialized from the specified file path.</returns>
		public ControllerInfo LoadFromFile(string controllerPath)
		{
			ControllerInfo info = null;
			
			// Disabled logging to boost performance
			//using (LogGroup logGroup = LogGroup.Start("Loading the controller from the specified path.", NLog.LogLevel.Debug))
			//{
				if (!File.Exists(controllerPath))
					throw new ArgumentException("The specified file does not exist.");
				
			//	LogWriter.Debug("Path: " + controllerPath);
				
				
				using (StreamReader reader = new StreamReader(File.OpenRead(controllerPath)))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(ControllerInfo));
					
					info = (ControllerInfo)serializer.Deserialize(reader);
					
					reader.Close();
				}
			//}
			
			return info;
		}
	}
}
