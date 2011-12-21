using System;
using System.IO;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Parts
{
	/// <summary>
	/// Used to load parts (a type of user control) for display on a page.
	/// </summary>
	public class PartLoader
	{
		private PartFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create file paths for parts.
		/// </summary>
		public PartFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new PartFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		private string partsDirectoryPath;
		/// <summary>
		/// Gets/sets the path to the directory containing the part files.
		/// </summary>
		public string PartsDirectoryPath
		{
			get {
				if (partsDirectoryPath == null || partsDirectoryPath == String.Empty)
				{
					if (FileNamer == null)
						throw new InvalidOperationException("FileNamer is not set.");
					
					partsDirectoryPath = FileNamer.PartsDirectoryPath;
				}
				return partsDirectoryPath; }
			set { partsDirectoryPath = value; }
		}
		
		protected PartInfo[] Parts = null;
		
		public PartLoader()
		{
		}
		
		/// <summary>
		/// Loads all the enabled parts found in the parts file.
		/// </summary>
		/// <returns>An array of the the parts found.</returns>
		public PartInfo[] LoadInfoFromFile()
		{
			return LoadInfoFromFile(false);
		}
		
		/// <summary>
		/// Loads all the parts found in the parts file.
		/// </summary>
		/// <param name="includeDisabled">A value indicating whether or not to include disabled parts.</param>
		/// <returns>An array of the the parts found.</returns>
		public PartInfo[] LoadInfoFromFile(bool includeDisabled)
		{
			// Logging disabled to boost performance
			//using (LogGroup logGroup = LogGroup.StartDebug("Loading the parts from the XML file."))
			//{
			if (Parts == null)
			{
				List<PartInfo> validParts = new List<PartInfo>();
				
				PartInfo[] parts = new PartInfo[]{};
				
				using (StreamReader reader = new StreamReader(File.OpenRead(FileNamer.PartsInfoFilePath)))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(PartInfo[]));
					parts = (PartInfo[])serializer.Deserialize(reader);
				}
				
				foreach (PartInfo part in parts)
					if (part.Enabled || includeDisabled)
						validParts.Add(part);
				
				Parts = validParts.ToArray();
			}
			//}
			return Parts;
		}
		
		/// <summary>
		/// Loads the part from the specified path.
		/// </summary>
		/// <param name="partPath">The full path to the part to load.</param>
		/// <returns>The part deserialized from the specified file path.</returns>
		public PartInfo LoadFromFile(string partPath)
		{
			PartInfo info = null;
			
			//using (LogGroup logGroup = LogGroup.Start("Loading the part from the specified path.", NLog.LogLevel.Debug))
			//{
			if (!File.Exists(partPath))
				throw new ArgumentException("The specified file does not exist.");
			
			//	LogWriter.Debug("Path: " + partPath);
			
			
			using (StreamReader reader = new StreamReader(File.OpenRead(partPath)))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(PartInfo));
				
				info = (PartInfo)serializer.Deserialize(reader);
				
				reader.Close();
			}
			//}
			
			return info;
		}
	}
}
