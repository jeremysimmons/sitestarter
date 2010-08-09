using System;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Zips up all the files in a directory and saves the zip to a specific location.
	/// </summary>
	public class DirectoryZipper
	{
		private int totalFilesZipped;
		/// <summary>
		/// Gets/sets the total number of files that were zipped.
		/// </summary>
		public int TotalFilesZipped
		{
			get { return totalFilesZipped; }
			set { totalFilesZipped = value; }
		}
		
		private string directoryPath;
		/// <summary>
		/// Gets/sets the path to the directory being zipped.
		/// </summary>
		public string DirectoryPath
		{
			get { return directoryPath; }
			set { directoryPath = value; }
		}
		
		/// <summary>
		/// Sets the path to the directory being zipped.
		/// </summary>
		/// <param name="directoryPath">The path to the directory being zipped.</param>
		public DirectoryZipper(string directoryPath)
		{
			DirectoryPath = directoryPath;
		}
		
		/// <summary>
		/// Zips the files in the specifed directory and outputs the zip file to the specified location.
		/// </summary>
		/// <param name="zipFilePath">The full path to the output zip file.</param>
		/// <returns>The total number of files added to the zip file.</returns>
		public int ZipToFile(string zipFilePath)
		{
			int total = 0;
			
			if (Directory.Exists(DirectoryPath))
			{
				if (!Directory.Exists(Path.GetDirectoryName(zipFilePath)))
				{
					Directory.CreateDirectory(Path.GetDirectoryName(zipFilePath));
				}


				// Create the zip file
				//Crc32 crc = new Crc32();
				ZipOutputStream zipFile = new ZipOutputStream(File.Create(zipFilePath));
				zipFile.UseZip64 = UseZip64.Off;
				zipFile.SetLevel(9);

				total += ZipFromPath(zipFile, DirectoryPath);

				// Close the writer
				zipFile.Finish();
				zipFile.Close();
			}
			
			return total;
		}

		/// <summary>
		/// Zips the files in the specified folder to the provided zip output file.
		/// </summary>
		/// <param name="zipFile">The output stream to add the file to.</param>
		/// <param name="directoryPath">The path of the directory containing the files to add to the zip output stream.</param>
		/// <returns>The total number of files zipped.</returns>
		private int ZipFromPath(ZipOutputStream zipFile, string directoryPath)
		{
			int total = 0;
			
			foreach (string filePath in Directory.GetFiles(directoryPath))
			{
				string[] parts = Path.GetDirectoryName(filePath).Split('\\');
				string newFileName = filePath.Replace(DirectoryPath, String.Empty);
				
				try
				{
					FileStream stream = File.OpenRead(filePath);

					byte[] buffer = new byte[stream.Length];
					stream.Read(buffer, 0, buffer.Length);
					ZipEntry entry = new ZipEntry(newFileName);

					entry.DateTime = DateTime.Now;

					entry.Size = stream.Length;
					stream.Close();

					//crc.Reset();
					//crc.Update(buffer);

					//entry.Crc  = crc.Value;

					zipFile.PutNextEntry(entry);

					zipFile.Write(buffer, 0, buffer.Length);

					total++;
				}
				catch (FileNotFoundException ex)
				{
					throw ex;
					// Just ignore files that have been moved or deleted. They'll get removed from the list next time the release is edited.
				}
				catch (Exception ex)
				{
					throw ex;
					//Error error = new Error(ex);
					//error.IsLocal = true;
					//My.ErrorEngine.SaveError(error);
				}
			}

			foreach (string directory in Directory.GetDirectories(directoryPath))
			{
				total += ZipFromPath(zipFile, directory);
			}
			
			return total;
		}

	}
}
