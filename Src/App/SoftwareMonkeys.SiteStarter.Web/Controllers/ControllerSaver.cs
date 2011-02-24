using System;
using System.IO;
using SoftwareMonkeys.SiteStarter.Data;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// Used to save controller components to file.
	/// </summary>
	public class ControllerSaver
	{
		private string controllersDirectoryPath;
		/// <summary>
		/// Gets the full path to the directory containing controller mappings.
		/// </summary>
		public string ControllersDirectoryPath
		{
			get {
				if (controllersDirectoryPath == null || controllersDirectoryPath == String.Empty)
				{
					if (FileNamer != null)
						controllersDirectoryPath = FileNamer.ControllersDirectoryPath;
					
				}
				return controllersDirectoryPath;
			}
			set { controllersDirectoryPath = value; }
		}
		
		private ControllerFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create file names/paths.
		/// </summary>
		public ControllerFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new ControllerFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		public ControllerSaver()
		{
		}
		
		/// <summary>
		/// Saves the provided controller info to the controllers info directory.
		/// </summary>
		/// <param name="controller">The controller info to save to file.</param>
		public void SaveInfoToFile(ControllerInfo controller)
		{
			using (LogGroup logGroup = LogGroup.Start("Saving the provided controller to file.", NLog.LogLevel.Debug))
			{
				string path = FileNamer.CreateInfoFilePath(controller);
				
				LogWriter.Debug("Path : " + path);
				
				if (!Directory.Exists(Path.GetDirectoryName(path)))
					Directory.CreateDirectory(Path.GetDirectoryName(path));
				
				using (StreamWriter writer = File.CreateText(path))
				{
					XmlSerializer serializer = new XmlSerializer(controller.GetType());
					serializer.Serialize(writer, controller);
					writer.Close();
				}
			}
		}
		
		/// <summary>
		/// Saves the provided controllers info to file.
		/// </summary>
		/// <param name="controllers">An array of the controllers to save to file.</param>
		public void SaveInfoToFile(ControllerInfo[] controllers)
		{
			using (LogGroup logGroup = LogGroup.Start("Saving the provided controllers to XML files.", NLog.LogLevel.Debug))
			{
				foreach (ControllerInfo controller in controllers)
				{
					SaveInfoToFile(controller);
				}
			}
		}
	}
}
