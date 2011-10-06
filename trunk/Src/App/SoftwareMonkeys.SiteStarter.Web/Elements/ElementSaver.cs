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
		private string elementsDirectoryPath;
		/// <summary>
		/// Gets the full path to the directory containing element mappings.
		/// </summary>
		public string ElementsDirectoryPath
		{
			get {
				if (elementsDirectoryPath == null || elementsDirectoryPath == String.Empty)
				{
					if (FileNamer != null)
						elementsDirectoryPath = FileNamer.ElementsDirectoryPath;
					
				}
				return elementsDirectoryPath;
			}
			set { elementsDirectoryPath = value; }
		}
		
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
		/// Saves the provided element info to the elements info directory.
		/// </summary>
		/// <param name="element">The element info to save to file.</param>
		public void SaveInfoToFile(ElementInfo element)
		{
			using (LogGroup logGroup = LogGroup.Start("Saving the provided element to file.", NLog.LogLevel.Debug))
			{
				string path = FileNamer.CreateInfoFilePath(element);
				
				LogWriter.Debug("Path : " + path);
				
				if (!Directory.Exists(Path.GetDirectoryName(path)))
					Directory.CreateDirectory(Path.GetDirectoryName(path));
				
				using (StreamWriter writer = File.CreateText(path))
				{
					XmlSerializer serializer = new XmlSerializer(element.GetType());
					serializer.Serialize(writer, element);
					writer.Close();
				}
			}
		}
		
		/// <summary>
		/// Saves the provided elements info to file.
		/// </summary>
		/// <param name="elements">An array of the elements to save to file.</param>
		public void SaveInfoToFile(ElementInfo[] elements)
		{
			using (LogGroup logGroup = LogGroup.Start("Saving the provided elements to XML files.", NLog.LogLevel.Debug))
			{
				foreach (ElementInfo element in elements)
				{
					SaveInfoToFile(element);
				}
			}
		}
	}
}
