using System;
using SoftwareMonkeys.SiteStarter.Data;
using System.IO;
using SoftwareMonkeys.SiteStarter.IO;

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
					if (DataAccess.IsInitialized)
						projectionsDirectoryPath = Configuration.Config.Application.PhysicalApplicationPath + Path.DirectorySeparatorChar + "Projections";
				}
				return projectionsDirectoryPath;
			}
			set { projectionsDirectoryPath = value; }
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
					if (DataAccess.IsInitialized)
						projectionsInfoDirectoryPath = DataAccess.Data.DataDirectoryPath + Path.DirectorySeparatorChar + "Projections";
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
			
			if (projection.Action == null)
				throw new ArgumentNullException("projection.Action", "No action has been set to the Action property.");
			
			string name = projection.TypeName + "-" + projection.Action + ".projection";
			
			return name;
		}
		
		/*/// <summary>
		/// Creates the file name for the provided projection.
		/// </summary>
		/// <param name="projection">The projection to create the file name for.</param>
		/// <returns>The full file name for the provided projection.</returns>
		public virtual string CreateProjectionFileName(ProjectionInfo projection)
		{			
			if (projection == null)
				throw new ArgumentNullException("projection");
			
			if (projection.Action == null)
				throw new ArgumentNullException("projection.Action", "No action has been set to the Action property.");
			
			string name = projection.TypeName + "-" + projection.Action + ".ascx";
			
			return name;
		}*/
		
		// TODO: Remove if not needed
		/*/// <summary>
		/// Creates the file name for the provided projection.
		/// </summary>
		/// <param name="projection">The projection to create the file name for.</param>
		/// <returns>The full file name for the provided entity.</returns>
		public string CreateFileName(IProjection projection)
		{
			return CreateFileName(new ProjectionInfo(projection));
		}*/
		
		
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
		/// Creates the full file path for the provided projection.
		/// </summary>
		/// <param name="projection">The projection to create the file path for.</param>
		/// <returns>The full file path for the provided projection.</returns>
		public string CreateProjectionFilePath(ProjectionInfo projection)
		{
			if (projection.ProjectionFilePath == null || projection.ProjectionFilePath == String.Empty)
				throw new ArgumentException("The projection.ProjectionFilePath property hasn't been set.");
			
			return FileMapper.MapApplicationRelativePath(projection.ProjectionFilePath);
		}
		
		// TODO: Remove if not needed
		/*
		/// <summary>
		/// Creates the full file path for the provided projection.
		/// </summary>
		/// <param name="projection">The projection to create the file path for.</param>
		/// <returns>The full file path for the provided projection.</returns>
		public string CreateFilePath(IProjection projection)
		{
			return CreateFilePath(new ProjectionInfo(projection));
		}*/
		
	}
}
