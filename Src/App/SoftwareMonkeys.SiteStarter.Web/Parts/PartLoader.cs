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
		
		private string partsInfoDirectoryPath;
		/// <summary>
		/// Gets/sets the path to the directory containing the part info files.
		/// </summary>
		public string PartsInfoDirectoryPath
		{
			get {
				if (partsInfoDirectoryPath == null || partsInfoDirectoryPath == String.Empty)
				{
					if (FileNamer == null)
						throw new InvalidOperationException("FileNamer is not set.");
					
					partsInfoDirectoryPath = FileNamer.PartsInfoDirectoryPath;
				}
				return partsInfoDirectoryPath; }
			set { partsInfoDirectoryPath = value; }
		}
		
		public PartLoader()
		{
		}
		
		/// <summary>
		/// Loads all the parts found in the parts directory.
		/// </summary>
		/// <returns>An array of the the parts found in the directory.</returns>
		public PartInfo[] LoadFromDirectory()
		{
			List<PartInfo> parts = new List<PartInfo>();
			
			using (LogGroup logGroup = LogGroup.Start("Loading the parts from the XML files.", NLog.LogLevel.Debug))
			{
				foreach (string file in Directory.GetFiles(PartsDirectoryPath))
				{
					LogWriter.Debug("File: " + file);
					
					parts.Add(LoadFromFile(file));
				}
			}
			
			return parts.ToArray();
		}
		
		/// <summary>
		/// Loads all the enabled parts found in the parts directory.
		/// </summary>
		/// <returns>An array of the the parts found in the directory.</returns>
		public PartInfo[] LoadInfoFromDirectory()
		{
			return LoadInfoFromDirectory(false);
		}
		
		/// <summary>
		/// Loads all the parts found in the parts directory.
		/// </summary>
		/// <param name="includeDisabled">A value indicating whether or not to include disabled projections.</param>
		/// <returns>An array of the the parts found in the directory.</returns>
		public PartInfo[] LoadInfoFromDirectory(bool includeDisabled)
		{			
			List<PartInfo> parts = new List<PartInfo>();
			
			using (LogGroup logGroup = LogGroup.Start("Loading the parts info from the XML files.", NLog.LogLevel.Debug))
			{
				foreach (string file in Directory.GetFiles(PartsInfoDirectoryPath))
				{
					LogWriter.Debug("File: " + file);
					
					PartInfo part = LoadFromFile(file);
					if (includeDisabled || part.Enabled)
						parts.Add(part);
				}
			}
			
			return parts.ToArray();
		}
		
		/// <summary>
		/// Loads the part from the specified path.
		/// </summary>
		/// <param name="partPath">The full path to the part to load.</param>
		/// <returns>The part deserialized from the specified file path.</returns>
		public PartInfo LoadFromFile(string partPath)
		{
			PartInfo info = null;
			
			using (LogGroup logGroup = LogGroup.Start("Loading the part from the specified path.", NLog.LogLevel.Debug))
			{
				if (!File.Exists(partPath))
					throw new ArgumentException("The specified file does not exist.");
				
				LogWriter.Debug("Path: " + partPath);
				
				
				using (StreamReader reader = new StreamReader(File.OpenRead(partPath)))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(PartInfo));
					
					info = (PartInfo)serializer.Deserialize(reader);
					
					reader.Close();
				}
			}
			
			return info;
		}
	}
}
