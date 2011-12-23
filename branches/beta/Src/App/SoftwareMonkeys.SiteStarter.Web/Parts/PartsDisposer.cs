using System;
using System.Web.UI;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Web.Parts
{
	/// <summary>
	/// Used to remove/dispose parts from the system.
	/// </summary>
	public class PartsDisposer
	{
		
		private PartFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create part file names/paths.
		/// </summary>
		public PartFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new PartFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		
		public PartsDisposer()
		{
		}
		
		/// <summary>
		/// Disposes the parts found by the scanner.
		/// </summary>
		public void Dispose()
		{
			using (LogGroup logGroup = LogGroup.Start("Disposing the parts.", NLog.LogLevel.Debug))
			{
				Dispose(PartState.Parts.ToArray());
			}
		}
		
		/// <summary>
		/// Disposes the provided parts.
		/// </summary>
		public void Dispose(PartInfo[] parts)
		{
			using (LogGroup logGroup = LogGroup.Start("Disposing the parts.", NLog.LogLevel.Debug))
			{
				foreach (PartInfo part in parts)
				{
					PartState.Parts.Remove(
						PartState.Parts[part.Key]
					);
				}
			}
		}
	}
}
