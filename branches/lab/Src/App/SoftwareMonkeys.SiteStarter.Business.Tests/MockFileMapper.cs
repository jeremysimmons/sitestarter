using System;
using System.Web;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.IO;
using SoftwareMonkeys.SiteStarter.Tests;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// Maps relative file resource paths to full physical paths.
	/// </summary>
	public class MockFileMapper : IFileMapper
	{		
		BaseTestFixture ExecutingTestFixture;
		
		private string applicationPath = String.Empty;
		/// <summary>
		/// Gets/sets the server relative application path.
		/// </summary>
		public string ApplicationPath
		{
			get { return applicationPath; }
			set {applicationPath = value; }
		}
		
		private string physicalRoot = String.Empty;
		public string PhysicalRoot
		{
			get { return physicalRoot; }
			set { physicalRoot = value; }
		}
		
		public MockFileMapper(BaseTestFixture executingTestFixture)
		{
			ExecutingTestFixture = executingTestFixture;
			PhysicalRoot = TestUtilities.GetTestingPath(ExecutingTestFixture);
		}
		
		public MockFileMapper(BaseTestFixture executingTestFixture, string physicalRoot)
		{
			ExecutingTestFixture = executingTestFixture;
			PhysicalRoot = physicalRoot;
		}
		
		
		public MockFileMapper(BaseTestFixture executingTestFixture, string physicalRoot, string applicationPath)
		{
			ExecutingTestFixture = executingTestFixture;
			PhysicalRoot = physicalRoot;
			this.ApplicationPath = applicationPath;
		}
		
		
		/// <summary>
		/// Maps the provided server relative path without using the HttpContext.Current.Server.MapPath function.
		/// </summary>
		/// <param name="relativeUrl"></param>
		/// <returns></returns>
		public string MapPath(string relativeUrl)
		{			
			string path = PhysicalRoot + Path.DirectorySeparatorChar + relativeUrl.Replace("/", @"\").Trim('\\');
			
			return path;
		}
		
		/// <summary>
		/// Maps the provided application relative path without using the HttpContext.Current.Server.MapPath function.
		/// </summary>
		/// <param name="applicationRelativePath"></param>
		/// <returns></returns>
		public string MapApplicationRelativePath(string applicationRelativePath)
		{
			if (applicationRelativePath == null || applicationRelativePath == String.Empty)
				throw new ArgumentNullException("applicationRelativePath");
			
			if (ApplicationPath == null || ApplicationPath == String.Empty)
				throw new InvalidOperationException("The ApplicationPath property has not been set on the MockFileMapper.");
			
			string path = PhysicalRoot + Path.DirectorySeparatorChar + ApplicationPath.Replace("/", @"\").Trim('\\') + Path.DirectorySeparatorChar + applicationRelativePath.Replace("/", @"\").Trim('\\');
			
			return path;
		}
	}
}
