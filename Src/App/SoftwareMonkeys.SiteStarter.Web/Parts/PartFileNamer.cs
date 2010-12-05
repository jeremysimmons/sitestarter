using System;
using SoftwareMonkeys.SiteStarter.Data;
using System.IO;
using SoftwareMonkeys.SiteStarter.IO;

namespace SoftwareMonkeys.SiteStarter.Web.Parts
{
	/// <summary>
	/// Used to create file names/paths for projection components.
	/// </summary>
	public class PartFileNamer
	{
		private string projectionsDirectoryPath;
		/// <summary>
		/// Gets the path to the directory containing the projection files.
		/// </summary>
		public virtual string PartsDirectoryPath
		{
			get {
				if (projectionsDirectoryPath == null || projectionsDirectoryPath == String.Empty)
				{
					if (DataAccess.IsInitialized)
						projectionsDirectoryPath = Configuration.Config.Application.PhysicalApplicationPath + Path.DirectorySeparatorChar + "Parts";
				}
				return projectionsDirectoryPath;
			}
			set { projectionsDirectoryPath = value; }
		}
		
		private string projectionsInfoDirectoryPath;
		/// <summary>
		/// Gets the path to the directory containing serialized projection information.
		/// </summary>
		public virtual string PartsInfoDirectoryPath
		{
			get {
				if (projectionsInfoDirectoryPath == null || projectionsInfoDirectoryPath == String.Empty)
				{
					if (DataAccess.IsInitialized)
						projectionsInfoDirectoryPath = DataAccess.Data.DataDirectoryPath + Path.DirectorySeparatorChar + "Parts";
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
		
		public PartFileNamer()
		{
		}
		
		/// <summary>
		/// Creates the file name for the serialized info for the provided projection.
		/// </summary>
		/// <param name="projection">The projection to create the file name for.</param>
		/// <returns>The full file name for the serialized info for the provided projection.</returns>
		public virtual string CreateInfoFileName(PartInfo projection)
		{			
			if (projection == null)
				throw new ArgumentNullException("projection");
			
			if (projection.Action == null)
				throw new ArgumentNullException("projection.Action", "No action has been set to the Action property.");
			
			string name = projection.TypeName + "-" + projection.Action + "." + projection.Format.ToString().ToLower() + ".projection";
			
			return name;
		}
		
		/*/// <summary>
		/// Creates the file name for the provided projection.
		/// </summary>
		/// <param name="projection">The projection to create the file name for.</param>
		/// <returns>The full file name for the provided projection.</returns>
		public virtual string CreatePartFileName(PartInfo projection)
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
		public string CreateFileName(IPart projection)
		{
			return CreateFileName(new PartInfo(projection));
		}*/
		
		
		/// <summary>
		/// Creates the full file path for the serialized info for the provided projection.
		/// </summary>
		/// <param name="projection">The projection to create the file path for.</param>
		/// <returns>The full file path for the serialized info for the provided projection.</returns>
		public string CreateInfoFilePath(PartInfo projection)
		{
			return PartsInfoDirectoryPath + Path.DirectorySeparatorChar + CreateInfoFileName(projection);
		}
		
		
		/// <summary>
		/// Creates the full file path for the provided projection.
		/// </summary>
		/// <param name="projection">The projection to create the file path for.</param>
		/// <returns>The full file path for the provided projection.</returns>
		public string CreatePartFilePath(PartInfo projection)
		{
			if (projection.PartFilePath == null || projection.PartFilePath == String.Empty)
				throw new ArgumentException("The projection.PartFilePath property hasn't been set.");
			
			return FileMapper.MapApplicationRelativePath(projection.PartFilePath);
		}
		
		// TODO: Remove if not needed
		/*
		/// <summary>
		/// Creates the full file path for the provided projection.
		/// </summary>
		/// <param name="projection">The projection to create the file path for.</param>
		/// <returns>The full file path for the provided projection.</returns>
		public string CreateFilePath(IPart projection)
		{
			return CreateFilePath(new PartInfo(projection));
		}*/
		
	}
}
