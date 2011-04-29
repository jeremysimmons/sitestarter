using System;
using SoftwareMonkeys.SiteStarter.Data;
using System.IO;
using SoftwareMonkeys.SiteStarter.IO;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// Used to create file names/paths for controller components.
	/// </summary>
	public class ControllerFileNamer
	{
		private string controllersDirectoryPath;
		/// <summary>
		/// Gets the path to the directory containing the controller files.
		/// </summary>
		public virtual string ControllersDirectoryPath
		{
			get {
				if (controllersDirectoryPath == null || controllersDirectoryPath == String.Empty)
				{
					if (DataAccess.IsInitialized)
						controllersDirectoryPath = StateAccess.State.PhysicalApplicationPath + Path.DirectorySeparatorChar + "Controllers";
				}
				return controllersDirectoryPath;
			}
			set { controllersDirectoryPath = value; }
		}
		
		private string controllersInfoDirectoryPath;
		/// <summary>
		/// Gets the path to the directory containing serialized controller information.
		/// </summary>
		public virtual string ControllersInfoDirectoryPath
		{
			get {
				if (controllersInfoDirectoryPath == null || controllersInfoDirectoryPath == String.Empty)
				{
					if (DataAccess.IsInitialized)
						controllersInfoDirectoryPath =  StateAccess.State.PhysicalApplicationPath + Path.DirectorySeparatorChar + "App_Data" + Path.DirectorySeparatorChar + "Controllers";
				}
				return controllersInfoDirectoryPath;
			}
			set { controllersInfoDirectoryPath = value; }
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
		
		public ControllerFileNamer()
		{
		}
		
		/// <summary>
		/// Creates the file name for the serialized info for the provided controller.
		/// </summary>
		/// <param name="controller">The controller to create the file name for.</param>
		/// <returns>The full file name for the serialized info for the provided controller.</returns>
		public virtual string CreateInfoFileName(ControllerInfo controller)
		{			
			if (controller == null)
				throw new ArgumentNullException("controller");
			
			if (controller.Action == null)
				throw new ArgumentNullException("controller.Action", "No action has been set to the Action property.");
			
			string name = controller.TypeName + "-" + controller.Action + ".controller";
			
			return name;
		}
		
		/*/// <summary>
		/// Creates the file name for the provided controller.
		/// </summary>
		/// <param name="controller">The controller to create the file name for.</param>
		/// <returns>The full file name for the provided controller.</returns>
		public virtual string CreateControllerFileName(ControllerInfo controller)
		{			
			if (controller == null)
				throw new ArgumentNullException("controller");
			
			if (controller.Action == null)
				throw new ArgumentNullException("controller.Action", "No action has been set to the Action property.");
			
			string name = controller.TypeName + "-" + controller.Action + ".ascx";
			
			return name;
		}*/
		
		// TODO: Remove if not needed
		/*/// <summary>
		/// Creates the file name for the provided controller.
		/// </summary>
		/// <param name="controller">The controller to create the file name for.</param>
		/// <returns>The full file name for the provided entity.</returns>
		public string CreateFileName(IController controller)
		{
			return CreateFileName(new ControllerInfo(controller));
		}*/
		
		
		/// <summary>
		/// Creates the full file path for the serialized info for the provided controller.
		/// </summary>
		/// <param name="controller">The controller to create the file path for.</param>
		/// <returns>The full file path for the serialized info for the provided controller.</returns>
		public string CreateInfoFilePath(ControllerInfo controller)
		{
			return ControllersInfoDirectoryPath + Path.DirectorySeparatorChar + CreateInfoFileName(controller);
		}
		
		// TODO: Remove if not needed
		/*
		/// <summary>
		/// Creates the full file path for the provided controller.
		/// </summary>
		/// <param name="controller">The controller to create the file path for.</param>
		/// <returns>The full file path for the provided controller.</returns>
		public string CreateFilePath(IController controller)
		{
			return CreateFilePath(new ControllerInfo(controller));
		}*/
		
	}
}
