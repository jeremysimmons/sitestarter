using System;
using System.IO;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Data
{
	public class VersionUtilities
	{
		public static string GetVersionFileName(string pathVariation)
		{		
			string versionFile = "Version.number";
			if (pathVariation != null && pathVariation != String.Empty)
				versionFile = "Version." + pathVariation + ".number";
			
			return versionFile;
		}
		
		public static string GetVersionFileName()
		{
			if (!Config.IsInitialized)
				throw new InvalidOperationException("Configuration is not initialized. Use the other overload and pass the path variation.");
			
			string pathVariation = Config.Application.PathVariation;
		
			return GetVersionFileName(pathVariation);
		}
		
		public static Version GetLegacyVersion(string applicationPath)
		{			
			string versionFilePath = applicationPath + Path.DirectorySeparatorChar
				+ "App_Data" + Path.DirectorySeparatorChar
				+ "Legacy" + Path.DirectorySeparatorChar
				+ GetVersionFileName();
			
			if (!File.Exists(versionFilePath))
				return new Version("0.0.0.0");
			else
				return LoadVersionFromFile(versionFilePath);
		}
		
		public static Version GetImportVersion(string applicationPath)
		{			
			string versionFilePath = applicationPath + Path.DirectorySeparatorChar
				+ "App_Data" + Path.DirectorySeparatorChar
				+ "Import" + Path.DirectorySeparatorChar
				+ GetVersionFileName();
			
			if (!File.Exists(versionFilePath))
				return new Version("0.0.0.0");
			else
				return LoadVersionFromFile(versionFilePath);
		}
		
		public static Version GetCurrentVersion(string applicationPath)
		{
			string versionFilePath = applicationPath + Path.DirectorySeparatorChar
				+ "Version.number"; // Don't use path variation here
			
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
