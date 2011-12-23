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
		private string controllersInfoFilePath;
		/// <summary>
		/// Gets the path to the file containing the controller info.
		/// </summary>
		public virtual string ControllersInfoFilePath
		{
			get {
				if (controllersInfoFilePath == null || controllersInfoFilePath == String.Empty)
				{
					if (StateAccess.IsInitialized)
						controllersInfoFilePath = StateAccess.State.PhysicalApplicationPath
							+ Path.DirectorySeparatorChar + "App_Data"
							+ Path.DirectorySeparatorChar + "Controllers.xml";
				}
				return controllersInfoFilePath;
			}
			set { controllersInfoFilePath = value; }
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
	}
}
