using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Web.UI;
using System.Web;

namespace SoftwareMonkeys.SiteStarter.Web.Parts
{
	/// <summary>
	/// Used for scanning assemblies to look for usable business projections.
	/// </summary>
	public class PartScanner : BasePartScanner
	{
		// TODO: Tidy up
		
		/*private string projectionsDirectoryName;
		/// <summary>
		/// Gets/sets the name of the directory containing the projections.
		/// </summary>
		public string PartsDirectoryName
		{
			get { return projectionsDirectoryName; }
			set { projectionsDirectoryName = value; }
		}
		
		/// <summary>
		/// Gets/sets the full path of the directory containing the projections.
		/// </summary>
		public string PartsDirectoryPath
		{
			get { return projectionsDirectoryName; }
			set { projectionsDirectoryName = value; }
		}*/
		
		private PartFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used for generating projection file names and paths.
		/// </summary>
		public PartFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new PartFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		private ControlLoader controlLoader;
		/// <summary>
		/// Gets/sets component used to load user controls.
		/// </summary>
		public ControlLoader ControlLoader
		{
			get {
				if (controlLoader == null)
					throw new InvalidOperationException("ControlLoader has not been initialized. Pass a Page component as a parameter to the PartScanner constructor and it will automatically initialized the ControlLoader.");
				return controlLoader; }
			set { controlLoader = value; }
		}
		
		public PartScanner()
		{
		}
		
		public PartScanner(Page page)
		{
			ControlLoader = new ControlLoader(page);
		}
		
		/// <summary>
		/// Finds all the projections in the available assemblies.
		/// </summary>
		/// <param name="page"></param>
		/// <returns>An array of info about the projections found.</returns>
		public override PartInfo[] FindParts()
		{
			List<PartInfo> projections = new List<PartInfo>();
			
			using (LogGroup logGroup = AppLogger.StartGroup("Finding projections by scanning the attributes of the available type.", NLog.LogLevel.Debug))
			{
				foreach (string file in Directory.GetFiles(FileNamer.PartsDirectoryPath))
				{
					if (IsPart(file))
					{
						foreach (PartInfo info in ExtractPartInfo(file))
						{
							projections.Add(info);
						}
					}
				}
			}
			
			return projections.ToArray();
		}
		
		/// <summary>
		/// Extracts the projection infos from the provided file path.
		/// </summary>
		/// <param name="filePath">The full path to the projection (.ascx) file.</param>
		/// <returns></returns>
		public PartInfo[] ExtractPartInfo(string filePath)
		{
			string shortName = Path.GetFileNameWithoutExtension(filePath);
			string extension = Path.GetExtension(filePath).Trim('.');
			
			string[] actions = ExtractActions(shortName);
			
			string typeName = ExtractType(shortName);
			
			List<PartInfo> projections = new List<PartInfo>();
			
			string relativeFilePath = filePath.Replace(Configuration.Config.Application.PhysicalApplicationPath, "")
				.Replace(@"\", "/")
				.Trim('/');
			
			
			foreach (string action in actions)
			{
				PartInfo info = new PartInfo();
				info.Action = action;
				info.TypeName = typeName;
				info.PartFilePath = relativeFilePath;
				info.Format = GetFormatFromFileName(filePath);
				
				BasePart p = (BasePart)ControlLoader.LoadControl(filePath);
				
				if (p == null)
					throw new ArgumentException("Cannot find projection file at path: " + filePath);
				
				// Ensure that the menu properties have been set
				p.InitializeMenu();
				
				info.MenuTitle = p.MenuTitle;
				info.MenuCategory = p.MenuCategory;
				info.ShowOnMenu = p.ShowOnMenu;
				//info.Format = ConvertSubExtensionToFormat(subExtension);
				
				projections.Add(info);
			}
			
			return projections.ToArray();
		}
		
		/// <summary>
		/// Retrieves the format of the projection with the provided file name.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public PartFormat GetFormatFromFileName(string fileName)
		{
			string shortFileName = Path.GetFileName(fileName);
			string extension = shortFileName.Substring
				(
					shortFileName.IndexOf('.'),
					shortFileName.Length - shortFileName.IndexOf('.')
				);
			
			extension = extension.Trim('.');
			
			string subExtension = extension;
			
			if (extension.IndexOf(".") > -1)
			{
				subExtension = extension.Substring(0, extension.IndexOf("."));
			}
			
			return ConvertSubExtensionToFormat(subExtension);
		}
		
		/// <summary>
		/// Converts a sub extension such as 'xml' from '*.xml.ascx' into a projection format such as PartFormat.Xml.
		/// </summary>
		/// <param name="subExtension"></param>
		/// <returns></returns>
		private PartFormat ConvertSubExtensionToFormat(string subExtension)
		{
			switch (subExtension.Trim('.').ToLower())
			{
				case "xml":
					return PartFormat.Xml;
				case "xslt":
					return PartFormat.Xslt;
				case "html":
				default:
					return PartFormat.Html;
			}
		}
		
		/// <summary>
		/// Extracts the actions from the provide file name.
		/// </summary>
		/// <param name="shortFileName">The name of the file to extract the actions from.</param>
		/// <returns>The actions extracted from the file name.</returns>
		private string[] ExtractActions(string shortFileName)
		{
			if (shortFileName.IndexOf(".") > -1 || shortFileName.IndexOf(":") > -1)
				shortFileName = Path.GetFileNameWithoutExtension(shortFileName);
			
			string[] parts = shortFileName.Split('-');
			
			if (parts.Length <= 1)
				throw new ArgumentException("The provided short file name is invalid: " + shortFileName);
			
			List<string> actions = new List<string>();
			
			for (int i = 1; i < parts.Length; i++)
			{
				actions.Add(parts[i]);
			}
			
			return actions.ToArray();
		}
		
		/// <summary>
		/// Extracts the type name from the provided file name.
		/// </summary>
		/// <param name="shortFileName">The name of the file to extract the name from.</param>
		/// <returns>The action extracted from the file name.</returns>
		private string ExtractType(string shortFileName)
		{
			if (shortFileName.IndexOf(".") > -1 || shortFileName.IndexOf(":") > -1)
				shortFileName = Path.GetFileNameWithoutExtension(shortFileName);
			
			string[] parts = shortFileName.Split('-');
			
			if (parts.Length <= 1)
				throw new ArgumentException("The provided short file name is invalid: " + shortFileName);
			
			return parts[0];
		}
		
		/// <summary>
		/// Checks whether the file at the specified location is a projection file.
		/// </summary>
		/// <param name="fileName">The full name and path of the file to check.</param>
		/// <returns></returns>
		public bool IsPart(string fileName)
		{
			string ext = Path.GetExtension(fileName);
			string shortFileName = Path.GetFileNameWithoutExtension(fileName);
			
			// - in file name
			if (shortFileName.IndexOf('-') == -1)
				return false;
			
			// File extension
			if (ext.ToLower() == ".ascx")
				return true;
			
			// File extension
			if (ext.ToLower() == ".xslt")
				return true;
			
			// File extension
			if (ext.ToLower() == ".html")
				return true;
			
			
			return false;
		}
		
	}
}
