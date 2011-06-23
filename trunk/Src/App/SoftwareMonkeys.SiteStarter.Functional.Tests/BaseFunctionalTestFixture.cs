using System;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Tests;
using SoftwareMonkeys.SiteStarter.Entities;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Web.Tests;
using System.IO;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Functional.Tests
{
	/// <summary>
	/// 
	/// </summary>
	public class BaseFunctionalTestFixture : BaseWebTestFixture
	{
		public BaseFunctionalTestFixture()
		{
		}
		
		[SetUp]
		public override void Start()
		{
			base.Start();
			
			InitializeFunctionalTesting();
		}
		
		[TearDown]
		public override void End()
		{
			DisposeFunctionalTesting();
			
			base.End();
		}
		
		public virtual void InitializeFunctionalTesting()
		{
			//ClearAppData();
		}
		
		public virtual void DisposeFunctionalTesting()
		{
			ReportFunctionalLogs();
			
			ClearAppData();
		}
		
		public void ClearAppData()
		{			
			string dataDirectory = GetWWWDirectory() + Path.DirectorySeparatorChar + "App_Data";
			
			ClearData(dataDirectory);
			ClearLogs(dataDirectory);
			ClearConfiguration(dataDirectory);
		}
		
		public void ClearData(string dataDirectory)
		{
			using (Batch batch = BatchState.StartBatch())
			{
				foreach (IEntity entity in DataAccess.Data.Indexer.GetEntities())
					DataAccess.Data.Deleter.Delete(entity);
			}
			
			// TODO: Clean up
			//DataAccess.Dispose(true, true);
			
			//foreach (string file in Directory.GetFiles(dataDirectory, "*.db4o"))
			//{
			//	File.Delete(file);
			//}
		}
		
		public void ClearLogs(string dataDirectory)
		{
			string logsDirectory = dataDirectory + Path.DirectorySeparatorChar + "Logs";
			
			LogWriter.Dispose();
			
			if (Directory.Exists(logsDirectory))
				Directory.Delete(logsDirectory, true);
		}
		
		public void ClearConfiguration(string dataDirectory)
		{
			foreach (string file in Directory.GetFiles(dataDirectory, "*.config"))
			{
				File.Delete(file);
			}
		}
		
		public string GetWWWDirectory()
		{
			string cd = Environment.CurrentDirectory;
			
			string reportDirectory = String.Empty;
			
			string subDir = "Src/App/WWW/";
			
			string stepUp = "/";
			
			// TODO: Clean up
			// If the current directory is within an assembly bin directory
			//if (cd.IndexOf(GetType().Assembly.GetName().Name.Replace(".", "-")) > -1)
			
			// If the current directory is within the /src/ directory
			if (cd.ToLower().IndexOf(Path.DirectorySeparatorChar + "src" + Path.DirectorySeparatorChar) > -1)
			{
				stepUp = "/../../../../../"; // Removing /Src/App/[namespace]/bin/[mode]/
			}
			// Otherwise it's in the general bin directory
			else
			{
				stepUp = "/../../"; // Removing /bin/[mode]/
			}
			
			reportDirectory = Path.GetFullPath(cd + stepUp + subDir);
			
			return reportDirectory;
		}
		
		public void ReportFunctionalLogs()
		{
			string inputDirectory = GetWWWDirectory().TrimEnd('\\') + Path.DirectorySeparatorChar
				+ "App_Data" + Path.DirectorySeparatorChar
				+ "Logs";
			
			string reportDirectory = GetLogReportDirectory();
			// TODO: Clean up
			//throw new Exception(inputDirectory + " - " + reportDirectory);
			LogReporter reporter = new LogReporter(inputDirectory, reportDirectory, "AppLog.xml");
			reporter.Report(GetType().Name + Path.DirectorySeparatorChar + TestName);
		}
	}
}
