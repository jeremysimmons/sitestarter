using System;
using System.Web.UI;
using System.IO;
using SoftwareMonkeys.SiteStarter.State;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	/// <summary>
	/// Used to extract projection info from a file path.
	/// </summary>
	public class ProjectionInfoExtractor
	{		
		private ControlLoader controlLoader;
		/// <summary>
		/// Gets/sets component used to load user controls.
		/// </summary>
		public ControlLoader ControlLoader
		{
			get {
				if (controlLoader == null)
				{
						throw new InvalidOperationException("ControlLoader has not been initialized, and cannot initialize automatically because the Page property is null.");
				}
				return controlLoader; }
			set { controlLoader = value; }
		}
		
		public ProjectionInfoExtractor(ControlLoader controlLoader)
		{
			ControlLoader = controlLoader;
		}
		
		/// <summary>
		/// Extracts the projection infos from the provided file path.
		/// </summary>
		/// <param name="filePath">The full path to the projection (.ascx) file.</param>
		/// <returns></returns>
		public ProjectionInfo[] ExtractProjectionInfo(string filePath)
		{
			string shortName = Path.GetFileNameWithoutExtension(filePath);
			string extension = Path.GetExtension(filePath).Trim('.');
			
			string[] actions = ExtractActions(shortName);
			
			string typeName = ExtractType(shortName);
			
			List<ProjectionInfo> projections = new List<ProjectionInfo>();
			
			string relativeFilePath = filePath.Replace(StateAccess.State.PhysicalApplicationPath, "")
				.Replace(@"\", "/");
			
			BaseProjection p = null;
			try
			{
				p = (BaseProjection)ControlLoader.LoadControl(filePath);
			}
			catch (Exception ex)
			{
				throw new Exception("Unable to load projection: " + filePath, ex);
			}
			
			if (p == null)
				throw new ArgumentException("Cannot find projection file at path: " + filePath);
			
			// Ensure that the menu properties have been set
			p.InitializeMenu();
			
			string menuTitle = p.MenuTitle;
			string menuCategory = p.MenuCategory;
			bool showOnMenu = p.ShowOnMenu;
			
			// If the projection is type and action based
			if (actions.Length > 0)
			{
				foreach (string action in actions)
				{
					ProjectionInfo info = new ProjectionInfo();
					info.Action = action;
					info.TypeName = typeName;
					info.Name = info.TypeName + "-" + info.Action;
					info.ProjectionFilePath = relativeFilePath;
					info.Format = GetFormatFromFileName(filePath);
					
					info.MenuTitle = menuTitle;
					info.MenuCategory = menuCategory;
					info.ShowOnMenu = showOnMenu;
					
					projections.Add(info);
				}
			}
			// Otherwise it's just a simple name based projection
			else
			{
				ProjectionInfo info = new ProjectionInfo();
				
				info.Name = shortName;
				info.ProjectionFilePath = relativeFilePath;
				info.Format = GetFormatFromFileName(filePath);
				
				info.MenuTitle = menuTitle;
				info.MenuCategory = menuCategory;
				info.ShowOnMenu = showOnMenu;
				
				projections.Add(info);
			}
			
			return projections.ToArray();
		}
		
		
		/// <summary>
		/// Retrieves the format of the projection with the provided file name.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public ProjectionFormat GetFormatFromFileName(string fileName)
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
		/// Converts a sub extension such as 'xml' from '*.xml.ascx' into a projection format such as ProjectionFormat.Xml.
		/// </summary>
		/// <param name="subExtension"></param>
		/// <returns></returns>
		private ProjectionFormat ConvertSubExtensionToFormat(string subExtension)
		{
			switch (subExtension.Trim('.').ToLower())
			{
				case "xml":
					return ProjectionFormat.Xml;
				case "xslt":
					return ProjectionFormat.Xslt;
				case "html":
				default:
					return ProjectionFormat.Html;
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
			
			List<string> actions = new List<string>();
			
			if (parts.Length > 1)
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
			
			if (parts.Length > 1)
			{
				return parts[0];
			}
			else
				return String.Empty;
		}
	}
}
