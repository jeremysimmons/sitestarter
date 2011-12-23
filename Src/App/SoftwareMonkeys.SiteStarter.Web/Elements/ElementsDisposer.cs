using System;
using System.Web.UI;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Web.Elements
{
	/// <summary>
	/// Used to remove/dispose elements from the system.
	/// </summary>
	public class ElementsDisposer
	{
		
		private ElementFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create element file names/paths.
		/// </summary>
		public ElementFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new ElementFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		public Page Page;
		
		public ElementsDisposer()
		{
		}
		
		/// <summary>
		/// Disposes the elements found by the scanner.
		/// </summary>
		public void Dispose()
		{
			using (LogGroup logGroup = LogGroup.Start("Disposing the elements.", NLog.LogLevel.Debug))
			{
				if (ElementState.IsInitialized)
				{
					Dispose(ElementState.Elements);
				}
			}
		}
		
		/// <summary>
		/// Disposes the provided elements.
		/// </summary>
		public void Dispose(ElementStateCollection elements)
		{
			using (LogGroup logGroup = LogGroup.Start("Disposing the provided elements.", NLog.LogLevel.Debug))
			{				
				elements.Clear();
			}
		}
	}
}
