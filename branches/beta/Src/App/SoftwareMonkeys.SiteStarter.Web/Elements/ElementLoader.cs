using System;
using System.IO;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Elements
{
	/// <summary>
	/// Used to load elements (a type of user element) for display on a page.
	/// </summary>
	public class ElementLoader
	{
		private ElementFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create file paths for elements.
		/// </summary>
		public ElementFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new ElementFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		private string elementsDirectoryPath;
		/// <summary>
		/// Gets/sets the path to the directory containing the element files.
		/// </summary>
		public string ElementsDirectoryPath
		{
			get {
				if (elementsDirectoryPath == null || elementsDirectoryPath == String.Empty)
				{
					if (FileNamer == null)
						throw new InvalidOperationException("FileNamer is not set.");
					
					elementsDirectoryPath = FileNamer.ElementsDirectoryPath;
				}
				return elementsDirectoryPath; }
			set { elementsDirectoryPath = value; }
		}
		
		private string elementsInfoDirectoryPath;
		/// <summary>
		/// Gets/sets the path to the directory containing the element info files.
		/// </summary>
		public string ElementsInfoDirectoryPath
		{
			get {
				if (elementsInfoDirectoryPath == null || elementsInfoDirectoryPath == String.Empty)
				{
					if (FileNamer == null)
						throw new InvalidOperationException("FileNamer is not set.");
					
					elementsInfoDirectoryPath = FileNamer.ElementsInfoDirectoryPath;
				}
				return elementsInfoDirectoryPath; }
			set { elementsInfoDirectoryPath = value; }
		}
		
		public ElementLoader()
		{
		}
		
		/// <summary>
		/// Loads all the elements found in the elements directory.
		/// </summary>
		/// <returns>An array of the the elements found in the directory.</returns>
		public ElementInfo[] LoadFromDirectory()
		{
			List<ElementInfo> elements = new List<ElementInfo>();
			
			using (LogGroup logGroup = LogGroup.Start("Loading the elements from the XML files.", NLog.LogLevel.Debug))
			{
				foreach (string file in Directory.GetFiles(ElementsDirectoryPath))
				{
					LogWriter.Debug("File: " + file);
					
					elements.Add(LoadFromFile(file));
				}
			}
			
			return elements.ToArray();
		}
		
		/// <summary>
		/// Loads all the elements found in the elements directory.
		/// </summary>
		/// <returns>An array of the the elements found in the directory.</returns>
		public ElementInfo[] LoadInfoFromDirectory()
		{
			List<ElementInfo> elements = new List<ElementInfo>();
			
			using (LogGroup logGroup = LogGroup.Start("Loading the elements info from the XML files.", NLog.LogLevel.Debug))
			{
				foreach (string file in Directory.GetFiles(ElementsInfoDirectoryPath))
				{
					LogWriter.Debug("File: " + file);
					
					elements.Add(LoadFromFile(file));
				}
			}
			
			return elements.ToArray();
		}
		
		/// <summary>
		/// Loads the element from the specified path.
		/// </summary>
		/// <param name="elementPath">The full path to the element to load.</param>
		/// <returns>The element deserialized from the specified file path.</returns>
		public ElementInfo LoadFromFile(string elementPath)
		{
			ElementInfo info = null;
			
			// Disabled logging to boost performance
			//using (LogGroup logGroup = LogGroup.Start("Loading the element from the specified path.", NLog.LogLevel.Debug))
			//{
				if (!File.Exists(elementPath))
					throw new ArgumentException("The specified file does not exist.");
				
			//	LogWriter.Debug("Path: " + elementPath);
				
				
				using (StreamReader reader = new StreamReader(File.OpenRead(elementPath)))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(ElementInfo));
					
					info = (ElementInfo)serializer.Deserialize(reader);
					
					reader.Close();
				}
			//}
			
			return info;
		}
	}
}
