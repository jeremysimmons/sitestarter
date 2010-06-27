using System;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Description of VersionUtilities.
	/// </summary>
	public class VersionUtilities
	{
		public static Version GetLegacyVersion(string applicationPath)
		{
			string versionFilePath = applicationPath + Path.DirectorySeparatorChar
				+ "App_Data" + Path.DirectorySeparatorChar
				+ "Legacy" + Path.DirectorySeparatorChar
				+ "Version.Number";
			
			if (!File.Exists(versionFilePath))
				return new Version("0.0.0.0");
			else
				return LoadVersionFromFile(versionFilePath);
		}
		
		public static Version GetCurrentVersion(string applicationPath)
		{
			string versionFilePath = applicationPath + Path.DirectorySeparatorChar
				+ "Version.Number";
			
			return LoadVersionFromFile(versionFilePath);
		}
		
		public static Version LoadVersionFromFile(string path)
		{
			Version version;
			
			using (StreamReader reader = new StreamReader(File.OpenRead(path)))
			{
				version = new Version(reader.ReadLine());
				
				reader.Close();
			}
			
			return version;
		}
	}
}
