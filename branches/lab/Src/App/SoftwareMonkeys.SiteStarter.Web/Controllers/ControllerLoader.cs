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
		
		private string controllersDirectoryPath;
		/// <summary>
		/// Gets/sets the path to the directory containing the controller files.
		/// </summary>
		public string ControllersDirectoryPath
		{
			get {
				if (controllersDirectoryPath == null || controllersDirectoryPath == String.Empty)
				{
					if (FileNamer == null)
						throw new InvalidOperationException("FileNamer is not set.");
					
					controllersDirectoryPath = FileNamer.ControllersDirectoryPath;
				}
				return controllersDirectoryPath; }
			set { controllersDirectoryPath = value; }
		}
		
		private string controllersInfoDirectoryPath;
		/// <summary>
		/// Gets/sets the path to the directory containing the controller info files.
		/// </summary>
		public string ControllersInfoDirectoryPath
		{
			get {
				if (controllersInfoDirectoryPath == null || controllersInfoDirectoryPath == String.Empty)
				{
					if (FileNamer == null)
						throw new InvalidOperationException("FileNamer is not set.");
					
					controllersInfoDirectoryPath = FileNamer.ControllersInfoDirectoryPath;
				}
				return controllersInfoDirectoryPath; }
			set { controllersInfoDirectoryPath = value; }
		}
		
		public ControllerLoader()
		{
		}
		
		/// <summary>
		/// Loads all the controllers found in the controllers directory.
		/// </summary>
		/// <returns>An array of the the controllers found in the directory.</returns>
		public ControllerInfo[] LoadFromDirectory()
		{
			List<ControllerInfo> controllers = new List<ControllerInfo>();
			
			using (LogGroup logGroup = LogGroup.Start("Loading the controllers from the XML files.", NLog.LogLevel.Debug))
			{
				foreach (string file in Directory.GetFiles(ControllersDirectoryPath))
				{
					LogWriter.Debug("File: " + file);
					
					controllers.Add(LoadFromFile(file));
				}
			}
			
			return controllers.ToArray();
		}
		
		/// <summary>
		/// Loads all the controllers found in the controllers directory.
		/// </summary>
		/// <returns>An array of the the controllers found in the directory.</returns>
		public ControllerInfo[] LoadInfoFromDirectory()
		{
			List<ControllerInfo> controllers = new List<ControllerInfo>();
			
			using (LogGroup logGroup = LogGroup.Start("Loading the controllers info from the XML files.", NLog.LogLevel.Debug))
			{
				foreach (string file in Directory.GetFiles(ControllersInfoDirectoryPath))
				{
					LogWriter.Debug("File: " + file);
					
					controllers.Add(LoadFromFile(file));
				}
			}
			
			return controllers.ToArray();
		}
		
		/// <summary>
		/// Loads the controller from the specified path.
		/// </summary>
		/// <param name="controllerPath">The full path to the controller to load.</param>
		/// <returns>The controller deserialized from the specified file path.</returns>
		public ControllerInfo LoadFromFile(string controllerPath)
		{
			ControllerInfo info = null;
			
			using (LogGroup logGroup = LogGroup.Start("Loading the controller from the specified path.", NLog.LogLevel.Debug))
			{
				if (!File.Exists(controllerPath))
					throw new ArgumentException("The specified file does not exist.");
				
				LogWriter.Debug("Path: " + controllerPath);
				
				
				using (StreamReader reader = new StreamReader(File.OpenRead(controllerPath)))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(ControllerInfo));
					
					info = (ControllerInfo)serializer.Deserialize(reader);
					
					reader.Close();
				}
			}
			
			return info;
		}
	}
}
