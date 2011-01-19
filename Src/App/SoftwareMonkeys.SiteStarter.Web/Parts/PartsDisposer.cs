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
		
		
		/*static private BasePartScanner[] defaultScanners;
		/// <summary>
		/// Gets/sets the part scanners used to find available parts in the existing assemblies.
		/// </summary>
		static public BasePartScanner[] DefaultScanners
		{
			get {
				if (defaultScanners == null)
				{
					if (StateAccess.State.ContainsApplication(DefaultScannersKey))
						defaultScanners = (BasePartScanner[])StateAccess.State.GetApplication(DefaultScannersKey);
					
					defaultScanners = new BasePartScanner[]
					{
						new PartScanner()
					};
				}
				return defaultScanners; }
			set { defaultScanners = value;
				StateAccess.State.SetApplication(DefaultScannersKey, defaultScanners);
			}
		}*/
		
		
		private BasePartScanner[] scanners;
		/// <summary>
		/// Gets/sets the part scanners used to find available parts in the existing assemblies.
		/// </summary>
		public BasePartScanner[] Scanners
		{
			get {
				if (scanners == null)
				{
					scanners = PartsInitializer.DefaultScanners;
					
					// Make sure each of the default scanners uses the right page
					foreach (BasePartScanner scanner in scanners)
						scanner.Page = Page;
					/*scanners = new BasePartScanner[]
					{
						new PartScanner(Page)
					};*/
				}
				return scanners; }
			set { scanners = value; }
		}
		
		public Page Page;
		
		public PartsDisposer()
		{
		}
		
		public PartsDisposer(Page page)
		{
			Page = page;
			Scanners = new BasePartScanner[]
			{
				new PartScanner(Page)
			};
		}
		
		public PartsDisposer(Page page, params BasePartScanner[] scanners)
		{
			Page = page;
			Scanners = scanners;
		}
		
		/// <summary>
		/// Disposes the parts found by the scanner.
		/// </summary>
		public void Dispose()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Disposing the parts.", NLog.LogLevel.Debug))
			{
				PartInfo[] parts = new PartInfo[]{};
				
				foreach (PartScanner scanner in Scanners)
				{
					Dispose(scanner.FindParts());
				}
				
			}
		}
		
		/// <summary>
		/// Disposes the provided parts.
		/// </summary>
		public void Dispose(PartInfo[] parts)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Disposing the parts.", NLog.LogLevel.Debug))
			{
				foreach (PartInfo part in parts)
				{
					PartState.Parts.Remove(
						PartState.Parts[part.Key]
					);
					
					DeleteInfo(part);
				}
			}
		}
		
		/// <summary>
		/// Deletes the part info file.
		/// </summary>
		/// <param name="info"></param>
		public void DeleteInfo(PartInfo info)
		{
			string path = FileNamer.CreateInfoFilePath(info);
			
			File.Delete(path);
		}
	}
}
