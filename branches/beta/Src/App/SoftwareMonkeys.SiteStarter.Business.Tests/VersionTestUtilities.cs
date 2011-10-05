using System;
using System.IO;
using SoftwareMonkeys.SiteStarter.Tests;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Data;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// Description of VersionTestUtilities.
	/// </summary>
	public class VersionTestUtilities
	{
		static public void CreateDummyVersion(string directoryPath, string pathVariation)
		{	
			string versionFile = directoryPath + Path.DirectorySeparatorChar + VersionUtilities.GetVersionFileName(pathVariation);
			
			if (!Directory.Exists(Path.GetDirectoryName(versionFile)))
			    Directory.CreateDirectory(Path.GetDirectoryName(versionFile));
			
			using (StreamWriter writer = File.CreateText(versionFile))
			{
				writer.Write("1.0.0.0");
				writer.Close();
			}
		}
	}
}
