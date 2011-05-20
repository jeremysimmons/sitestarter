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
	/// Used for scanning assemblies to look for usable business parts.
	/// </summary>
	public class PartScanner : BasePartScanner
	{
		private PartFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used for generating part file names and paths.
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
			get
			{
				if (controlLoader == null)
				{
					if (Page != null)
						controlLoader = new ControlLoader(Page);
					else
						throw new InvalidOperationException("ControlLoader has not been initialized, and cannot initialize automatically because the Page property is null.");
				}
				return controlLoader;
			}
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
		/// Finds all the parts in the available assemblies.
		/// </summary>
		/// <param name="page"></param>
		/// <returns>An array of info about the parts found.</returns>
		public override PartInfo[] FindParts()
		{
			List<PartInfo> parts = new List<PartInfo>();
			
			using (LogGroup logGroup = LogGroup.Start("Finding parts by scanning the attributes of the available type.", NLog.LogLevel.Debug))
			{
				if (Directory.Exists(FileNamer.PartsDirectoryPath))
				{
					foreach (string file in Directory.GetFiles(FileNamer.PartsDirectoryPath))
					{
						if (IsPart(file))
						{
							foreach (PartInfo info in ExtractPartInfo(file))
							{
								parts.Add(info);
							}
						}
					}
				}
			}
			
			return parts.ToArray();
		}
		
		/// <summary>
		/// Extracts the part infos from the provided file path.
		/// </summary>
		/// <param name="filePath">The full path to the part (.ascx) file.</param>
		/// <returns></returns>
		public PartInfo[] ExtractPartInfo(string filePath)
		{
			string shortName = Path.GetFileNameWithoutExtension(filePath);
			string extension = Path.GetExtension(filePath).Trim('.');
			
			string[] actions = ExtractActions(shortName);
			
			string typeName = ExtractType(shortName);
			
			List<PartInfo> parts = new List<PartInfo>();
			
			string relativeFilePath = filePath.Replace(Configuration.Config.Application.PhysicalApplicationPath, "")
				.Replace(@"\", "/")
				.Trim('/');
			
			
			foreach (string action in actions)
			{
				PartInfo info = new PartInfo();
				info.Action = action;
				info.TypeName = typeName;
				info.PartFilePath = relativeFilePath;
				
				BasePart p = (BasePart)ControlLoader.LoadControl(filePath);
				
				if (p == null)
					throw new ArgumentException("Cannot find part file at path: " + filePath);
				
				// Ensure that the menu properties have been set
				p.InitializeMenu();
				
				info.MenuTitle = p.MenuTitle;
				info.MenuCategory = p.MenuCategory;
				
				parts.Add(info);
			}
			
			return parts.ToArray();
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
			
			List<string> actions = new List<string>();
			
			// If no dash (-) is found then the full file name (without extension) is the action
			if (parts.Length == 1)
			{
				actions.Add(parts[0]);
			}
			else
			{
				for (int i = 1; i < parts.Length; i++)
				{
					actions.Add(parts[i]);
				}
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
			
			// If no dash (-) is found in the file name then the type is String.Empty
			if (parts.Length <= 1)
				return String.Empty;
			else
				return parts[0];
		}
		
		/// <summary>
		/// Checks whether the file at the specified location is a part file.
		/// </summary>
		/// <param name="fileName">The full name and path of the file to check.</param>
		/// <returns></returns>
		public bool IsPart(string fileName)
		{
			string ext = Path.GetExtension(fileName);
			string shortFileName = Path.GetFileNameWithoutExtension(fileName);
			
			// File extension
			if (ext.ToLower() == ".ascx")
				return true;
			
			
			return false;
		}
		
	}
}
