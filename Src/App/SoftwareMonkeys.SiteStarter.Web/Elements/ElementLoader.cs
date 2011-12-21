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
		
		public ElementInfo[] Elements;
		
		public ElementLoader()
		{
		}
		
		/// <summary>
		/// Loads all the reactions found in the reactions directory.
		/// </summary>
		/// <returns>An array of the the reactions found.</returns>
		public ElementInfo[] LoadInfoFromFile()
		{
			return LoadInfoFromFile(false);
		}
		
		/// <summary>
		/// Loads all the elements found in the elements file.
		/// </summary>
		/// <param name="includeDisabled"></param>
		/// <returns>An array of the the elements found.</returns>
		public ElementInfo[] LoadInfoFromFile(bool includeDisabled)
		{
			// Logging disabled to boost performance
			//using (LogGroup logGroup = LogGroup.StartDebug("Loading the elements from the XML file."))
			//{
			if (Elements == null)
			{
				List<ElementInfo> validElements = new List<ElementInfo>();
				
				ElementInfo[] elements = new ElementInfo[]{};
				
				using (StreamReader reader = new StreamReader(File.OpenRead(FileNamer.ElementsInfoFilePath)))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(ElementInfo[]));
					elements = (ElementInfo[])serializer.Deserialize(reader);
				}
				
				foreach (ElementInfo element in elements)
					if (element.Enabled || includeDisabled)
						validElements.Add(element);
				
				Elements = validElements.ToArray();
			}
			//}
			return Elements;
		}
	}
}
