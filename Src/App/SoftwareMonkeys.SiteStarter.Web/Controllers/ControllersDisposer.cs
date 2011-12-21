using System;
using System.Web.UI;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// Used to remove/dispose controllers from the system.
	/// </summary>
	public class ControllersDisposer
	{
		
		private ControllerFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create controller file names/paths.
		/// </summary>
		public ControllerFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new ControllerFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		public Page Page;
		
		public ControllersDisposer()
		{
		}
		
		/// <summary>
		/// Disposes the controllers found by the scanner.
		/// </summary>
		public void Dispose()
		{
			using (LogGroup logGroup = LogGroup.Start("Disposing the controllers.", NLog.LogLevel.Debug))
			{
				ControllerInfo[] controllers = new ControllerInfo[]{};
				
				Dispose(ControllerState.Controllers.ToArray());		
			}
		}
		
		/// <summary>
		/// Disposes the provided controllers.
		/// </summary>
		public void Dispose(ControllerInfo[] controllers)
		{
			using (LogGroup logGroup = LogGroup.Start("Disposing the controllers.", NLog.LogLevel.Debug))
			{
				foreach (ControllerInfo controller in controllers)
				{
					ControllerState.Controllers.Remove(
						ControllerState.Controllers[controller.Key]
					);
				}
				
				DeleteInfo();
			}
		}
		
		/// <summary>
		/// Deletes the controller info file.
		/// </summary>
		public void DeleteInfo()
		{
			string path = FileNamer.ControllersInfoFilePath;
			
			File.Delete(path);
		}
	}
}
