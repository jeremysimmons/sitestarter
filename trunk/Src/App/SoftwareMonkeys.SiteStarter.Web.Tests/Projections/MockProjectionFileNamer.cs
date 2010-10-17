using System;
using SoftwareMonkeys.SiteStarter.Web.Projections;
using SoftwareMonkeys.SiteStarter.Tests;

namespace SoftwareMonkeys.SiteStarter.Web.Tests.Projections
{
	/// <summary>
	/// A mock projection file namer for use during testing.
	/// </summary>
	public class MockProjectionFileNamer : ProjectionFileNamer
	{
		public virtual string ProjectionsDirectoryPath
		{
			get {
				if (projectionsDirectoryPath == null || projectionsDirectoryPath == String.Empty)
				{
					if (DataAccess.IsInitialized)
						projectionsDirectoryPath = TestUtilities.GetTestApplicationPath.T + Path.DirectorySeparatorChar + "Projections";
				}
				return projectionsDirectoryPath;
			}
			set { projectionsDirectoryPath = value; }
		}
		
		public MockProjectionFileNamer()
		{
		}
	}
}
