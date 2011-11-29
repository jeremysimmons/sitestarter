using System;
using System.Web;
using SoftwareMonkeys.SiteStarter.Data;
using System.IO;
using SoftwareMonkeys.SiteStarter.IO;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	/// <summary>
	/// Used to create file names/paths for projection components.
	/// </summary>
	public class ProjectionFileNamer
	{
		private string projectionsDirectoryPath;
		/// <summary>
		/// Gets the path to the directory containing the projection files.
		/// </summary>
		public virtual string ProjectionsDirectoryPath
		{
			get {
				if (projectionsDirectoryPath == null || projectionsDirectoryPath == String.Empty)
				{
					if (StateAccess.IsInitialized)
						projectionsDirectoryPath = StateAccess.State.PhysicalApplicationPath
							+ Path.DirectorySeparatorChar + "Projections";
				}
				return projectionsDirectoryPath;
			}
			set { projectionsDirectoryPath = value; }
		}
		
		private string projectionTemplatesDirectoryPath;
		/// <summary>
		/// Gets the path to the directory containing the projection template files.
		/// </summary>
		public virtual string ProjectionTemplatesDirectoryPath
		{
			get {
				if (projectionTemplatesDirectoryPath == null || projectionTemplatesDirectoryPath == String.Empty)
				{
					if (StateAccess.IsInitialized)
						projectionTemplatesDirectoryPath = StateAccess.State.PhysicalApplicationPath
							+ Path.DirectorySeparatorChar + "ProjectionTemplates";
				}
				return projectionTemplatesDirectoryPath;
			}
			set { projectionTemplatesDirectoryPath = value; }
		}
		
		private string projectionsInfoDirectoryPath;
		/// <summary>
		/// Gets the path to the directory containing serialized projection information.
		/// </summary>
		public virtual string ProjectionsInfoDirectoryPath
		{
			get {
				if (projectionsInfoDirectoryPath == null || projectionsInfoDirectoryPath == String.Empty)
				{
					if (StateAccess.IsInitialized)
					{
						projectionsInfoDirectoryPath = StateAccess.State.PhysicalApplicationPath
							+ Path.DirectorySeparatorChar + "App_Data"
							+ Path.DirectorySeparatorChar + "Projections";
					}
				}
				return projectionsInfoDirectoryPath;
			}
			set { projectionsInfoDirectoryPath = value; }
		}
		
		private IFileMapper fileMapper;
		/// <summary>
		/// Gets/sets the file mapper used for mapping the full path of relative files.
		/// </summary>
		public IFileMapper FileMapper
		{
			get {
				if (fileMapper == null)
					fileMapper = new FileMapper();
				return fileMapper; }
			set { fileMapper = value; }
		}
		
		public ProjectionFileNamer()
		{
		}
		
		/// <summary>
		/// Creates the file name for the serialized info for the provided projection.
		/// </summary>
		/// <param name="projection">The projection to create the file name for.</param>
		/// <returns>The full file name for the serialized info for the provided projection.</returns>
		public virtual string CreateInfoFileName(ProjectionInfo projection)
		{			
			if (projection == null)
				throw new ArgumentNullException("projection");
			
			string name = projection.Name;
			
			if (projection.TypeName != String.Empty && projection.Action != String.Empty)
			{
				name = projection.TypeName + "-" + projection.Action;
			}
			
			name = name + "." + projection.Format.ToString().ToLower() + ".projection";
			
			return name;
		}
		
		/// <summary>
		/// Creates the file name for the serialized info for the specified projection.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="typeName"></param>
		/// <param name="format"></param>
		/// <returns>The full file name for the serialized info for the provided projection.</returns>
		public virtual string CreateInfoFileName(string action, string typeName, ProjectionFormat format)
		{			
			string name = String.Empty;
			
			if (typeName != String.Empty && action != String.Empty)
			{
				name = typeName + "-" + action;
			}
			
			name = name + "." + format.ToString().ToLower() + ".projection";
			
			return name;
		}
		
		/// <summary>
		/// Creates the full file path for the serialized info for the provided projection.
		/// </summary>
		/// <param name="projection">The projection to create the file path for.</param>
		/// <returns>The full file path for the serialized info for the provided projection.</returns>
		public string CreateInfoFilePath(ProjectionInfo projection)
		{
			return ProjectionsInfoDirectoryPath + Path.DirectorySeparatorChar + CreateInfoFileName(projection);
		}
		
		/// <summary>
		/// Creates the full file path for the serialized info for the specified projection.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="typeName"></param>
		/// <param name="format"></param>
		/// <returns>The full file path for the serialized info for the provided projection.</returns>
		public string CreateInfoFilePath(string action, string typeName, ProjectionFormat format)
		{
			return ProjectionsInfoDirectoryPath + Path.DirectorySeparatorChar + CreateInfoFileName(action, typeName, format);
		}
		
		/// <summary>
		/// Creates the full file path for the provided projection.
		/// </summary>
		/// <param name="projection">The projection to create the file path for.</param>
		/// <returns>The full file path for the provided projection.</returns>
		public string CreateProjectionFilePath(ProjectionInfo projection)
		{
			if (projection == null)
				throw new ArgumentNullException("projection");
			
			if (projection.ProjectionFilePath == null || projection.ProjectionFilePath == String.Empty)
				throw new ArgumentException("The projection.ProjectionFilePath property hasn't been set.");
			
			return FileMapper.MapApplicationRelativePath(projection.ProjectionFilePath);
		}
		
		
		/// <summary>
		/// Creates the application relative file path for the specified projection.
		/// </summary>
		/// <param name="name">The name of the projection to create the file path for.</param>
		/// <returns>The application relative file path of the projection.</returns>
		public string CreateRelativeProjectionFilePath(string name)
		{
			string basePath  = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath);
			
			string directoryName = ProjectionsDirectoryPath.Replace(basePath, "").Replace(@"\", "/");
			
			return CreateRelativeProjectionFilePath(name, directoryName);
		}
		
		/// <summary>
		/// Creates the application relative file path for the specified projection.
		/// </summary>
		/// <param name="name">The name of the projection to create the file path for.</param>
		/// <param name="directory">The directory containing the projection (relative to the application).</param>
		/// <returns>The application relative file path of the projection.</returns>
		public string CreateRelativeProjectionFilePath(string name, string directory)
		{
			if (name.ToLower().IndexOf("ascx") == -1)
				name = name + ".ascx";
			
			string path = "/" + directory.Trim('/') + "/" + name;
			
			return path;
		}
		
		/// <summary>
		/// Creates the application relative file path for the specified projection template.
		/// </summary>
		/// <param name="name">The name of the projection template to create the file path for.</param>
		/// <returns>The application relative file path of the projection template.</returns>
		public string CreateRelativeProjectionTemplateFilePath(string name)
		{
			string basePath  = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath);
			
			string directoryName = ProjectionTemplatesDirectoryPath.Replace(basePath, "").Replace(@"\", "/");
			
			return CreateRelativeProjectionFilePath(name, directoryName);
		}
		
		/// <summary>
		/// Creates the full file path for the specified projection template.
		/// </summary>
		/// <param name="name">The name of the projection template to create the file path for.</param>
		/// <returns>The full file path of the projection template.</returns>
		public string CreateProjectionTemplateFilePath(string name)
		{
			if (name.ToLower().IndexOf("ascx") == -1)
				name = name + ".ascx";
			
			return ProjectionTemplatesDirectoryPath.TrimEnd('\\') + @"\" + name;
		}
		
	}
}
