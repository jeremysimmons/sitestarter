using System;
using System.Diagnostics;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Tests;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.State.Tests;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Reflection;
using System.IO;
using NLog;

namespace SoftwareMonkeys.SiteStarter.Diagnostics.Tests
{
	/// <summary>
	/// Provides a base implementation for all diagnostics test fixtures.
	/// </summary>
	public class BaseDiagnosticsTestFixture : BaseStateTestFixture
	{
		public LogGroup FixtureLogGroup = null;
		public bool EnableTestLogging = true;
		
		[SetUp]
		public override void Start()
		{
			base.Start();
			
			InitializeMockDiagnostics();
		}
		
		/// <summary>
		/// Ends a test by disposing the mock test environment and deleting mock data.
		/// </summary>
		[TearDown]
		public override void End()
		{
			DisposeMockDiagnostics();
			
			base.End();
			
		}
		
		
		/// <summary>
		/// Initializes the mock diagnostic system.
		/// </summary>
		public virtual void InitializeMockDiagnostics()
		{
			if (EnableTestLogging)
			{
				// Create a log group for the test
				FixtureLogGroup = LogGroup.Start("Starting test '" + TestName + "'.", LogLevel.Info);
				
				// Enable debug logging during testing
				LogSettingsManager.Current.Defaults["Debug"] = new ModeDetector().IsDebug;
			}
		}
		
		/// <summary>
		/// Disposes the mock diagnostic system.
		/// </summary>
		public virtual void DisposeMockDiagnostics()
		{
			if (EnableTestLogging)
			{
				FixtureLogGroup.Dispose();
				FixtureLogGroup = null;
			}
			
			DiagnosticState.Dispose();
			
			ReportLogs();
		}
		
		public void ReportLogs()
		{
			string reportDirectory = GetLogReportDirectory();
			
			// TODO: Clean up
			//if (StateAccess.IsInitialized)
			//{
			new LogReporter(reportDirectory).Report(GetType().Name + Path.DirectorySeparatorChar + TestName);
			//}
		}
		
		public string GetLogReportDirectory()
		{
			string cd = Environment.CurrentDirectory;
			
			string reportDirectory = String.Empty;
			
			string version = Assembly.GetExecutingAssembly().GetName().Version.ToString().Replace(".", "-");
			
			string subDir = "TestResults/Logs/" + version;
			
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
			
			// Add the namespace to the path
			reportDirectory = reportDirectory + Path.DirectorySeparatorChar + ShortenNamespace(GetType().Namespace).Replace(".", "-");
			
			return reportDirectory;
		}
		
		/// <summary>
		/// Shortens the provided namespace by removing the first (company) and second (project) parts.
		/// </summary>
		/// <param name="original"></param>
		/// <returns></returns>
		public string ShortenNamespace(string original)
		{
			string output = original;
			
			string[] parts = original.Split('.');
			
			// If the namespace has more than 2 parts remove the first two (company and project)
			if (parts.Length > 2)
				output = original.Replace(parts[0] + "." // Company name
				                          + parts[1] + ".", // Project name
				                          "");
			
			return output;
		}
		
		protected string LoadLogContent(string logPath)
		{
			string content = String.Empty;
			if (File.Exists(logPath))
			{
				using (StreamReader reader = new StreamReader(File.OpenRead(logPath)))
				{
					content = reader.ReadToEnd();
				}
			}
			return content;
		}
	}
}
