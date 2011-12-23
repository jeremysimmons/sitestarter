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
		/// Saves the provided controllers info to file.
		/// </summary>
		/// <param name="controllers">An array of the controllers to save to file.</param>
		public void SaveInfoToFile(ControllerInfo[] controllers)
		{
			// Logging disabled to boost performance
			//using (LogGroup logGroup = LogGroup.StartDebug("Saving the provided controllers to XML file."))
			//{
			string path = FileNamer.ControllersInfoFilePath;
			
			//LogWriter.Debug("Path : " + path);
			
			if (!Directory.Exists(Path.GetDirectoryName(path)))
				Directory.CreateDirectory(Path.GetDirectoryName(path));
			
			using (StreamWriter writer = File.CreateText(path))
			{
				XmlSerializer serializer = new XmlSerializer(controllers.GetType());
				serializer.Serialize(writer, controllers);
				writer.Close();
			}
			//}
		}
	}
}
