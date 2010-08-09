using System;
using System.ComponentModel;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Provides functions for analyzing zip files.
	/// </summary>
	public class ZipAnalyzer : Component
	{
		private string zipFilePath;
		/// <summary>
		/// Gets/sets the full path to the zip file.
		/// </summary>
		public string ZipFilePath
		{
			get { return zipFilePath; }
			set { zipFilePath = value; }
		}
		
		/// <summary>
		/// Sets the path to the zip file.
		/// </summary>
		/// <param name="zipFile">The full path to the zip file being analyzed.</param>
		public ZipAnalyzer(string zipFilePath)
		{
			ZipFilePath = zipFilePath;
		}
		
		/// <summary>
		/// Counts the file in the zip.
		/// </summary>
		/// <returns>The total number of files found.</returns>
		public long CountFiles()
		{
			long count = 0;
			
			ZipFile zf = null;
			
			try
			{
				FileStream fs = File.OpenRead(ZipFilePath);
				zf = new ZipFile(fs);
				
				count = zf.Count;
			}
			finally
			{
				if (zf != null)
				{
					zf.IsStreamOwner = true;
					zf.Close();
				}
			}
			
			return count;
		}
	}
}
