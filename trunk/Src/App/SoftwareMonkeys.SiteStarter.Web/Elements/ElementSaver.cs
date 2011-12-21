using System;
using System.IO;
using SoftwareMonkeys.SiteStarter.Data;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Elements
{
	/// <summary>
	/// Used to save element components to file.
	/// </summary>
	public class ElementSaver
	{		
		private ElementFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create file names/paths.
		/// </summary>
		public ElementFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new ElementFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		public ElementSaver()
		{
		}
		
		/// <summary>
		/// Saves the provided elements to file.
		/// </summary>
		/// <param name="elements">An array of the elements to save to file.</param>
		public void SaveInfoToFile(ElementInfo[] elements)
		{
			// Logging disabled to boost performance
			//using (LogGroup logGroup = LogGroup.StartDebug("Saving the provided elements to XML file."))
			//{
			string path = FileNamer.ElementsInfoFilePath;
			
			//LogWriter.Debug("Path : " + path);
			
			if (!Directory.Exists(Path.GetDirectoryName(path)))
				Directory.CreateDirectory(Path.GetDirectoryName(path));
			
			using (StreamWriter writer = File.CreateText(path))
			{
				XmlSerializer serializer = new XmlSerializer(elements.GetType());
				serializer.Serialize(writer, elements);
				writer.Close();
			}
			//}
		}
	}
}
