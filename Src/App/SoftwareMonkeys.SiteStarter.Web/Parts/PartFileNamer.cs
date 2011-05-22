using System;
using SoftwareMonkeys.SiteStarter.Data;
using System.IO;
using SoftwareMonkeys.SiteStarter.IO;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Web.Parts
{
	/// <summary>
	/// Used to create file names/paths for part components.
	/// </summary>
	public class PartFileNamer
	{
		private string partsDirectoryPath;
		/// <summary>
		/// Gets the path to the directory containing the part files.
		/// </summary>
		public virtual string PartsDirectoryPath
		{
			get {
				if (partsDirectoryPath == null || partsDirectoryPath == String.Empty)
				{
					if (StateAccess.IsInitialized)
						partsDirectoryPath = StateAccess.State.PhysicalApplicationPath
							+ Path.DirectorySeparatorChar + "Parts";
				}
				return partsDirectoryPath;
			}
			set { partsDirectoryPath = value; }
		}
		
		private string partsInfoDirectoryPath;
		/// <summary>
		/// Gets the path to the directory containing serialized part information.
		/// </summary>
		public virtual string PartsInfoDirectoryPath
		{
			get {
				if (partsInfoDirectoryPath == null || partsInfoDirectoryPath == String.Empty)
				{
					if (StateAccess.IsInitialized)
						partsInfoDirectoryPath = StateAccess.State.PhysicalApplicationPath
							+ Path.DirectorySeparatorChar + "App_Data"
							+ Path.DirectorySeparatorChar + "Parts";
				}
				return partsInfoDirectoryPath;
			}
			set { partsInfoDirectoryPath = value; }
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
		/// Creates the file name for the serialized info for the provided part.
		/// </summary>
		/// <param name="part">The part to create the file name for.</param>
		/// <returns>The full file name for the serialized info for the provided part.</returns>
		public virtual string CreateInfoFileName(PartInfo part)
		{			
			if (part == null)
				throw new ArgumentNullException("part");
			
			if (part.Action == null)
				throw new ArgumentNullException("part.Action", "No action has been set to the Action property.");
			
			string name = String.Empty;
			
			if (part.TypeName != String.Empty)
				name += part.TypeName + "-";
			
			name += part.Action + ".part";
			
			return name;
		}
		
		/*/// <summary>
		/// Creates the file name for the provided part.
		/// </summary>
		/// <param name="part">The part to create the file name for.</param>
		/// <returns>The full file name for the provided part.</returns>
		public virtual string CreatePartFileName(PartInfo part)
		{			
			if (part == null)
				throw new ArgumentNullException("part");
			
			if (part.Action == null)
				throw new ArgumentNullException("part.Action", "No action has been set to the Action property.");
			
			string name = part.TypeName + "-" + part.Action + ".ascx";
			
			return name;
		}*/
		
		// TODO: Remove if not needed
		/*/// <summary>
		/// Creates the file name for the provided part.
		/// </summary>
		/// <param name="part">The part to create the file name for.</param>
		/// <returns>The full file name for the provided entity.</returns>
		public string CreateFileName(IPart part)
		{
			return CreateFileName(new PartInfo(part));
		}*/
		
		
		/// <summary>
		/// Creates the full file path for the serialized info for the provided part.
		/// </summary>
		/// <param name="part">The part to create the file path for.</param>
		/// <returns>The full file path for the serialized info for the provided part.</returns>
		public string CreateInfoFilePath(PartInfo part)
		{
			return PartsInfoDirectoryPath + Path.DirectorySeparatorChar + CreateInfoFileName(part);
		}
		
		
		/// <summary>
		/// Creates the full file path for the provided part.
		/// </summary>
		/// <param name="part">The part to create the file path for.</param>
		/// <returns>The full file path for the provided part.</returns>
		public string CreatePartFilePath(PartInfo part)
		{
			if (part.PartFilePath == null || part.PartFilePath == String.Empty)
				throw new ArgumentException("The part.PartFilePath property hasn't been set.");
			
			return FileMapper.MapApplicationRelativePath(part.PartFilePath);
		}
		
		// TODO: Remove if not needed
		/*
		/// <summary>
		/// Creates the full file path for the provided part.
		/// </summary>
		/// <param name="part">The part to create the file path for.</param>
		/// <returns>The full file path for the provided part.</returns>
		public string CreateFilePath(IPart part)
		{
			return CreateFilePath(new PartInfo(part));
		}*/
		
	}
}
