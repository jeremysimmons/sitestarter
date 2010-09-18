using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Tests;
using System.IO;
using System.Xml;

namespace SoftwareMonkeys.SiteStarter.Diagnostics.Tests
{
	[TestFixture]
	public class LogThreaderTests : BaseDiagnosticsTestFixture
	{
		public string ApplicationPath
		{
			get { return TestUtilities.GetTestingPath(this); }
		}
		
		public LogThreaderTests()
		{
		}
		
		
		#region Tests
		[Test]
		public void Test_SplitThreads()
		{
			string applicationName = "MockApplication";
			
			string testLogDir = TestUtilities.GetTestDataPath(this, applicationName);

			
			string dateStamp = DateTime.Now.ToShortDateString().Replace("/", "-");
			
			LogThreader threader = new LogThreader(testLogDir, dateStamp);

			string testThreadsDirectoryPath = threader.ThreadsDirectoryPath;
			
			// Create a test log file
			string testLogFile = threader.LogFilePath;
			string testIndexFile = testLogDir + Path.DirectorySeparatorChar + dateStamp + Path.DirectorySeparatorChar + threader.ThreadsDirectoryName + Path.DirectorySeparatorChar + threader.ThreadsIndexFileName;
			
			if (!Directory.Exists(testThreadsDirectoryPath))
				Directory.CreateDirectory(testThreadsDirectoryPath);
			
			
			if (!Directory.Exists(Path.GetDirectoryName(testLogFile)))
				Directory.CreateDirectory(Path.GetDirectoryName(testLogFile));
			
			// Ensure the old files are cleared
			// TODO: Remove: This should no longer be necessary
			foreach (string file in Directory.GetFiles(testThreadsDirectoryPath))
				File.Delete(file);
			
			using (StreamWriter writer = new StreamWriter(File.Create(testLogFile)))
			{
				writer.Write(CreateExampleLog());
				
				writer.Close();
			}
			
			// Analyze the test log file
			threader.SplitThreads();
			
			
			
			// Check that the index file was created
			Assert.AreEqual(true, File.Exists(testIndexFile), "The index file wasn't found.");
			
			XmlDocument indexDoc = new XmlDocument();
			indexDoc.Load(testIndexFile);
			
			foreach (XmlNode node in indexDoc.DocumentElement.ChildNodes)
			{
				Assert.IsTrue(node.Attributes["ID"] != null, "The ID attribute of the thread node is null.");
				
				Guid id = new Guid(node.Attributes["ID"].Value);
				Assert.AreNotEqual(Guid.Empty, id, "The thread ID is Guid.Empty.");
				
				
				//Assert.IsTrue(node.Attributes["Title"] != null, "The title attribute of the thread node is null.");
				
				//string title = node.Attributes["Title"].Value;
				//Assert.AreNotEqual(String.Empty, title, "The thread title is String.Empty.");
				
				
			}
			
			
			
			
			/*string[] fileNames = Directory.GetFiles(testLogDir + @"\Detail");
			
			// Now read the creation time for each file
			DateTime[] creationTimes = new DateTime[fileNames.Length];
			for (int i=0; i < fileNames.Length; i++)
				creationTimes[i] = new FileInfo(fileNames[i]).CreationTime;
			
			// sort it
			Array.Sort(creationTimes,fileNames);
			
			Assert.AreEqual(5, fileNames.Length, "Incorrect number of thread files created.");
			
			string fileName0 = Path.GetFileName(fileNames[0]);
			string fileName1 = Path.GetFileName(fileNames[1]);
			string fileName2 = Path.GetFileName(fileNames[2]);
			string fileName3 = Path.GetFileName(fileNames[3]);
			
			Assert.AreEqual("ASP.global_asax.Application_Start.xml", fileName0, "The sub log file in position 0 has an invalid name.");
			Assert.AreEqual("SoftwareMonkeys.SiteStarter.Configuration.Config.Initialize.xml", fileName1, "The sub log file in position 1 has an invalid name.");
			Assert.AreEqual("ASP.global_asax.Application_Start.xml", fileName2, "The sub log file in position 2 has an invalid name.");
			Assert.AreEqual("ASP.global_asax.Application_Start.xml", fileName3, "The sub log file in position 3 has an invalid name.");*/
			
		}
		
		
		[Test]
		public void Test_CreateNewThread()
		{
			Guid threadID = Guid.NewGuid();
			
			LogThreader threader = new LogThreader(TestUtilities.GetTestingPath(this), DateTime.Now.ToShortDateString());
			
			XmlDocument doc = threader.CreateNewThread();
			
			Assert.IsNotNull(doc, "The thread document is null.");
		}
		
		[Test]
		public void Test_SaveThread()
		{
			
			//string testLogFile = ApplicationPath + @"\App_Data\Testing\Log.xml";
			//string testIndexFile = ApplicationPath + @"\App_Data\Testing\Detail\Index.xml";
			string rootDir = ApplicationPath + @"\App_Data\Testing\";
			
			Guid threadID = Guid.NewGuid();
			//string threadTitle = "Test Thread";
			
			XmlDocument threadDoc = new XmlDocument();
			threadDoc.AppendChild(threadDoc.CreateElement("Test"));
			
			string dateStamp = DateTime.Now.ToShortDateString().Replace("/", "-");
			
			LogThreader threader = new LogThreader(rootDir, dateStamp);
			
			threader.SaveThread(threadDoc, threadID);
			
			string threadFile = threader.ThreadsDirectoryPath + Path.DirectorySeparatorChar + threadID + ".xml";
			
			Assert.IsTrue(File.Exists(threadFile), "The thread document wasn't saved.");
		}
		
		
		[Test]
		public void Test_AddThreadToIndex()
		{
			
			//string testLogFile = ApplicationPath + @"\App_Data\Testing\Log.xml";
			//string testIndexFile = ApplicationPath + @"\App_Data\Testing\Detail\Index.xml";
			string rootDir = Path.Combine(ApplicationPath, @"App_Data\Testing");
			
			Guid threadID = Guid.NewGuid();
			string title = "Test Title";
			//string threadTitle = "Test Thread";
			
			XmlDocument indexDoc = new XmlDocument();
			indexDoc.AppendChild(indexDoc.CreateElement("Index"));
			
			LogThreader threader = new LogThreader(rootDir, DateTime.Now.ToShortDateString());
			
			threader.AddThreadToIndex(indexDoc, threadID, title);
			
			Assert.AreEqual(1, indexDoc.DocumentElement.ChildNodes.Count, "Invalid number of threads found.");
			
			XmlNode node = indexDoc.DocumentElement.ChildNodes[0];
			
			Assert.AreEqual(threadID.ToString(), node.Attributes["ID"].Value, "The thread doesn't have the expected ID.");
			Assert.AreEqual(title, node.Attributes["Title"].Value, "The thread doesn't have the expected title.");
		}
		#endregion
		
		#region Utilities functions
		public string CreateExampleLog()
		{
			return @"<?xml version='1.0'?>
<?xml-stylesheet type='text/xsl' href='../../../LogTemplate.xsl'?>
<Log>
<Entry>
<GroupID>cc2105da-abb4-4220-a35c-e58c3f0d2099</GroupID>
<LogLevel>Debug</LogLevel>
<Timestamp>3/11/2009 5:39:17 PM</Timestamp>
<Indent>0</Indent>
<Component>ASP.global_asax</Component>
<Method>Application_Start</Method>
<Data>Start... - Initializes the state management, config, modules, and data.</Data>
<StackTrace>CreateStackTrace at offset 111 in file:line:column &lt;filename unknown&gt;:0:0

FormatLogEntry at offset 707 in file:line:column &lt;filename unknown&gt;:0:0

Debug at offset 82 in file:line:column &lt;filename unknown&gt;:0:0

Debug at offset 39 in file:line:column &lt;filename unknown&gt;:0:0

Write at offset 328 in file:line:column &lt;filename unknown&gt;:0:0

StartLevel1 at offset 98 in file:line:column &lt;filename unknown&gt;:0:0

Start at offset 62 in file:line:column &lt;filename unknown&gt;:0:0

StartGroup at offset 85 in file:line:column &lt;filename unknown&gt;:0:0

StartGroup at offset 67 in file:line:column &lt;filename unknown&gt;:0:0

Application_Start at offset 102 in file:line:column &lt;filename unknown&gt;:0:0

_InvokeMethodFast at offset 0 in file:line:column &lt;filename unknown&gt;:0:0

InvokeMethodFast at offset 72 in file:line:column &lt;filename unknown&gt;:0:0

Invoke at offset 262 in file:line:column &lt;filename unknown&gt;:0:0

Invoke at offset 30 in file:line:column &lt;filename unknown&gt;:0:0

ProcessSpecialRequest at offset 272 in file:line:column &lt;filename unknown&gt;:0:0

FireApplicationOnStart at offset 90 in file:line:column &lt;filename unknown&gt;:0:0

EnsureAppStartCalled at offset 137 in file:line:column &lt;filename unknown&gt;:0:0

GetApplicationInstance at offset 93 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestInternal at offset 290 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestNoDemand at offset 99 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 284 in file:line:column &lt;filename unknown&gt;:0:0

</StackTrace>
</Entry>


<Entry>
<GroupID>79f0bcff-8a5f-4f92-aed8-936f7ce217e4</GroupID>
<LogLevel>Debug</LogLevel>
<Timestamp>3/11/2009 5:39:17 PM</Timestamp>
<Indent>1</Indent>
<Component>SoftwareMonkeys.SiteStarter.Web.State.StateProviderManager</Component>
<Method>Initialize</Method>
<Data>Start... - Initializes the state provider manager to hold all application data while in memory.</Data>
<StackTrace>CreateStackTrace at offset 111 in file:line:column &lt;filename unknown&gt;:0:0

FormatLogEntry at offset 707 in file:line:column &lt;filename unknown&gt;:0:0

Debug at offset 82 in file:line:column &lt;filename unknown&gt;:0:0

Debug at offset 39 in file:line:column &lt;filename unknown&gt;:0:0

Write at offset 328 in file:line:column &lt;filename unknown&gt;:0:0

StartLevel1 at offset 98 in file:line:column &lt;filename unknown&gt;:0:0

Start at offset 62 in file:line:column &lt;filename unknown&gt;:0:0

StartGroup at offset 85 in file:line:column &lt;filename unknown&gt;:0:0

StartGroup at offset 65 in file:line:column &lt;filename unknown&gt;:0:0

Initialize at offset 114 in file:line:column &lt;filename unknown&gt;:0:0

.cctor at offset 67 in file:line:column &lt;filename unknown&gt;:0:0

Initialize at offset 0 in file:line:column &lt;filename unknown&gt;:0:0

Initialize at offset 93 in file:line:column &lt;filename unknown&gt;:0:0

Application_Start at offset 121 in file:line:column &lt;filename unknown&gt;:0:0

_InvokeMethodFast at offset 0 in file:line:column &lt;filename unknown&gt;:0:0

InvokeMethodFast at offset 72 in file:line:column &lt;filename unknown&gt;:0:0

Invoke at offset 262 in file:line:column &lt;filename unknown&gt;:0:0

Invoke at offset 30 in file:line:column &lt;filename unknown&gt;:0:0

ProcessSpecialRequest at offset 272 in file:line:column &lt;filename unknown&gt;:0:0

FireApplicationOnStart at offset 90 in file:line:column &lt;filename unknown&gt;:0:0

EnsureAppStartCalled at offset 137 in file:line:column &lt;filename unknown&gt;:0:0

GetApplicationInstance at offset 93 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestInternal at offset 290 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestNoDemand at offset 99 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 284 in file:line:column &lt;filename unknown&gt;:0:0

</StackTrace>
</Entry>


<Entry>
<GroupID>79f0bcff-8a5f-4f92-aed8-936f7ce217e4</GroupID>
<LogLevel>Debug</LogLevel>
<Timestamp>3/11/2009 5:39:17 PM</Timestamp>
<Indent>1</Indent>
<Component>SoftwareMonkeys.SiteStarter.Web.State.StateProviderManager</Component>
<Method>Initialize</Method>
<Data>...End</Data>
<StackTrace>CreateStackTrace at offset 111 in file:line:column &lt;filename unknown&gt;:0:0

FormatLogEntry at offset 707 in file:line:column &lt;filename unknown&gt;:0:0

Debug at offset 82 in file:line:column &lt;filename unknown&gt;:0:0

Debug at offset 39 in file:line:column &lt;filename unknown&gt;:0:0

Write at offset 328 in file:line:column &lt;filename unknown&gt;:0:0

EndLevel1 at offset 70 in file:line:column &lt;filename unknown&gt;:0:0

End at offset 85 in file:line:column &lt;filename unknown&gt;:0:0

Dispose at offset 40 in file:line:column &lt;filename unknown&gt;:0:0

Initialize at offset 869 in file:line:column &lt;filename unknown&gt;:0:0

.cctor at offset 67 in file:line:column &lt;filename unknown&gt;:0:0

Initialize at offset 0 in file:line:column &lt;filename unknown&gt;:0:0

Initialize at offset 93 in file:line:column &lt;filename unknown&gt;:0:0

Application_Start at offset 121 in file:line:column &lt;filename unknown&gt;:0:0

_InvokeMethodFast at offset 0 in file:line:column &lt;filename unknown&gt;:0:0

InvokeMethodFast at offset 72 in file:line:column &lt;filename unknown&gt;:0:0

Invoke at offset 262 in file:line:column &lt;filename unknown&gt;:0:0

Invoke at offset 30 in file:line:column &lt;filename unknown&gt;:0:0

ProcessSpecialRequest at offset 272 in file:line:column &lt;filename unknown&gt;:0:0

FireApplicationOnStart at offset 90 in file:line:column &lt;filename unknown&gt;:0:0

EnsureAppStartCalled at offset 137 in file:line:column &lt;filename unknown&gt;:0:0

GetApplicationInstance at offset 93 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestInternal at offset 290 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestNoDemand at offset 99 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 284 in file:line:column &lt;filename unknown&gt;:0:0

</StackTrace>
</Entry>


<Entry>
<GroupID>0d47f426-5b11-4d0a-8293-5aa28cfa1963</GroupID>
<LogLevel>Debug</LogLevel>
<Timestamp>3/11/2009 5:39:17 PM</Timestamp>
<Indent>1</Indent>
<Component>SoftwareMonkeys.SiteStarter.Configuration.Config</Component>
<Method>Initialize</Method>
<Data>Start... - Initializes the application configuration settings.</Data>
<StackTrace>CreateStackTrace at offset 111 in file:line:column &lt;filename unknown&gt;:0:0

FormatLogEntry at offset 707 in file:line:column &lt;filename unknown&gt;:0:0

Debug at offset 82 in file:line:column &lt;filename unknown&gt;:0:0

Debug at offset 39 in file:line:column &lt;filename unknown&gt;:0:0

Write at offset 328 in file:line:column &lt;filename unknown&gt;:0:0

StartLevel1 at offset 98 in file:line:column &lt;filename unknown&gt;:0:0

Start at offset 62 in file:line:column &lt;filename unknown&gt;:0:0

StartGroup at offset 85 in file:line:column &lt;filename unknown&gt;:0:0

StartGroup at offset 67 in file:line:column &lt;filename unknown&gt;:0:0

Initialize at offset 98 in file:line:column &lt;filename unknown&gt;:0:0

Initialize at offset 213 in file:line:column &lt;filename unknown&gt;:0:0

Application_Start at offset 121 in file:line:column &lt;filename unknown&gt;:0:0

_InvokeMethodFast at offset 0 in file:line:column &lt;filename unknown&gt;:0:0

InvokeMethodFast at offset 72 in file:line:column &lt;filename unknown&gt;:0:0

Invoke at offset 262 in file:line:column &lt;filename unknown&gt;:0:0

Invoke at offset 30 in file:line:column &lt;filename unknown&gt;:0:0

ProcessSpecialRequest at offset 272 in file:line:column &lt;filename unknown&gt;:0:0

FireApplicationOnStart at offset 90 in file:line:column &lt;filename unknown&gt;:0:0

EnsureAppStartCalled at offset 137 in file:line:column &lt;filename unknown&gt;:0:0

GetApplicationInstance at offset 93 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestInternal at offset 290 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestNoDemand at offset 99 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 284 in file:line:column &lt;filename unknown&gt;:0:0

</StackTrace>
</Entry>


<Entry>
<GroupID>0d47f426-5b11-4d0a-8293-5aa28cfa1963</GroupID>
<LogLevel>Debug</LogLevel>
<Timestamp>3/11/2009 5:39:17 PM</Timestamp>
<Indent>2</Indent>
<Component>SoftwareMonkeys.SiteStarter.Configuration.Config</Component>
<Method>Initialize</Method>
<Data>Looking for configs in: C:\Inetpub\wwwroot\SoftwareMonkeys\SiteStarter\Src\App\WWW</Data>
<StackTrace>CreateStackTrace at offset 111 in file:line:column &lt;filename unknown&gt;:0:0

FormatLogEntry at offset 707 in file:line:column &lt;filename unknown&gt;:0:0

Debug at offset 82 in file:line:column &lt;filename unknown&gt;:0:0

Debug at offset 54 in file:line:column &lt;filename unknown&gt;:0:0

Initialize at offset 134 in file:line:column &lt;filename unknown&gt;:0:0

Initialize at offset 213 in file:line:column &lt;filename unknown&gt;:0:0

Application_Start at offset 121 in file:line:column &lt;filename unknown&gt;:0:0

_InvokeMethodFast at offset 0 in file:line:column &lt;filename unknown&gt;:0:0

InvokeMethodFast at offset 72 in file:line:column &lt;filename unknown&gt;:0:0

Invoke at offset 262 in file:line:column &lt;filename unknown&gt;:0:0

Invoke at offset 30 in file:line:column &lt;filename unknown&gt;:0:0

ProcessSpecialRequest at offset 272 in file:line:column &lt;filename unknown&gt;:0:0

FireApplicationOnStart at offset 90 in file:line:column &lt;filename unknown&gt;:0:0

EnsureAppStartCalled at offset 137 in file:line:column &lt;filename unknown&gt;:0:0

GetApplicationInstance at offset 93 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestInternal at offset 290 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestNoDemand at offset 99 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 284 in file:line:column &lt;filename unknown&gt;:0:0

</StackTrace>
</Entry>


<Entry>
<GroupID>bae6878d-b6d0-403c-a156-a4b94ebad536</GroupID>
<LogLevel>Debug</LogLevel>
<Timestamp>1/12/2009 3:47:46 PM</Timestamp>
<Indent>3</Indent>
<Component>SoftwareMonkeys.SiteStarter.Data.DataUtilities</Component>
<Method>GetType</Method>
<Data>4 settings found for this mapping item.</Data>
<StackTrace>CreateStackTrace at offset 111 in file:line:column &lt;filename unknown&gt;:0:0

FormatLogEntry at offset 707 in file:line:column &lt;filename unknown&gt;:0:0

Debug at offset 82 in file:line:column &lt;filename unknown&gt;:0:0

Debug at offset 54 in file:line:column &lt;filename unknown&gt;:0:0

GetType at offset 759 in file:line:column &lt;filename unknown&gt;:0:0

GetDataStoreName at offset 297 in file:line:column &lt;filename unknown&gt;:0:0

GetDataStoreName at offset 115 in file:line:column &lt;filename unknown&gt;:0:0

get_Item at offset 58 in file:line:column &lt;filename unknown&gt;:0:0

GetEntity&lt;T&gt; at offset 113 in file:line:column &lt;filename unknown&gt;:0:0

GetUserRoleByName at offset 132 in file:line:column &lt;filename unknown&gt;:0:0

AddUsersToRoles&lt;U,R&gt; at offset 673 in file:line:column &lt;filename unknown&gt;:0:0

AddUsersToRoles at offset 103 in file:line:column &lt;filename unknown&gt;:0:0

AddUserToRole at offset 192 in file:line:column &lt;filename unknown&gt;:0:0

Page_Load at offset 1704 in file:line:column &lt;filename unknown&gt;:0:0

EventArgFunctionCaller at offset 15 in file:line:column &lt;filename unknown&gt;:0:0

Callback at offset 36 in file:line:column &lt;filename unknown&gt;:0:0

OnLoad at offset 100 in file:line:column &lt;filename unknown&gt;:0:0

LoadRecursive at offset 51 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestMain at offset 628 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 132 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 81 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestWithNoAssert at offset 22 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 50 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 38 in file:line:column &lt;filename unknown&gt;:0:0

System.Web.HttpApplication.IExecutionStep.Execute at offset 182 in file:line:column &lt;filename unknown&gt;:0:0

ExecuteStep at offset 76 in file:line:column &lt;filename unknown&gt;:0:0

ResumeSteps at offset 307 in file:line:column &lt;filename unknown&gt;:0:0

System.Web.IHttpAsyncHandler.BeginProcessRequest at offset 124 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestInternal at offset 380 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestNoDemand at offset 99 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 284 in file:line:column &lt;filename unknown&gt;:0:0

</StackTrace>
</Entry>


<Entry>
<GroupID>bae6878d-b6d0-403c-a156-a4b94ebad536</GroupID>
<LogLevel>Debug</LogLevel>
<Timestamp>1/12/2009 3:47:46 PM</Timestamp>
<Indent>3</Indent>
<Component>SoftwareMonkeys.SiteStarter.Data.DataUtilities</Component>
<Method>GetType</Method>
<Data>No alias. This is the actual type.</Data>
<StackTrace>CreateStackTrace at offset 111 in file:line:column &lt;filename unknown&gt;:0:0

FormatLogEntry at offset 707 in file:line:column &lt;filename unknown&gt;:0:0

Debug at offset 82 in file:line:column &lt;filename unknown&gt;:0:0

Debug at offset 54 in file:line:column &lt;filename unknown&gt;:0:0

GetType at offset 833 in file:line:column &lt;filename unknown&gt;:0:0

GetDataStoreName at offset 297 in file:line:column &lt;filename unknown&gt;:0:0

GetDataStoreName at offset 115 in file:line:column &lt;filename unknown&gt;:0:0

get_Item at offset 58 in file:line:column &lt;filename unknown&gt;:0:0

GetEntity&lt;T&gt; at offset 113 in file:line:column &lt;filename unknown&gt;:0:0

GetUserRoleByName at offset 132 in file:line:column &lt;filename unknown&gt;:0:0

AddUsersToRoles&lt;U,R&gt; at offset 673 in file:line:column &lt;filename unknown&gt;:0:0

AddUsersToRoles at offset 103 in file:line:column &lt;filename unknown&gt;:0:0

AddUserToRole at offset 192 in file:line:column &lt;filename unknown&gt;:0:0

Page_Load at offset 1704 in file:line:column &lt;filename unknown&gt;:0:0

EventArgFunctionCaller at offset 15 in file:line:column &lt;filename unknown&gt;:0:0

Callback at offset 36 in file:line:column &lt;filename unknown&gt;:0:0

OnLoad at offset 100 in file:line:column &lt;filename unknown&gt;:0:0

LoadRecursive at offset 51 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestMain at offset 628 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 132 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 81 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestWithNoAssert at offset 22 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 50 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 38 in file:line:column &lt;filename unknown&gt;:0:0

System.Web.HttpApplication.IExecutionStep.Execute at offset 182 in file:line:column &lt;filename unknown&gt;:0:0

ExecuteStep at offset 76 in file:line:column &lt;filename unknown&gt;:0:0

ResumeSteps at offset 307 in file:line:column &lt;filename unknown&gt;:0:0

System.Web.IHttpAsyncHandler.BeginProcessRequest at offset 124 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestInternal at offset 380 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestNoDemand at offset 99 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 284 in file:line:column &lt;filename unknown&gt;:0:0

</StackTrace>
</Entry>


<Entry>
<GroupID>bae6878d-b6d0-403c-a156-a4b94ebad536</GroupID>
<LogLevel>Debug</LogLevel>
<Timestamp>1/12/2009 3:47:46 PM</Timestamp>
<Indent>3</Indent>
<Component>SoftwareMonkeys.SiteStarter.Data.DataUtilities</Component>
<Method>GetType</Method>
<Data>Returning type: SoftwareMonkeys.SiteStarter.Entities.UserRole</Data>
<StackTrace>CreateStackTrace at offset 111 in file:line:column &lt;filename unknown&gt;:0:0

FormatLogEntry at offset 707 in file:line:column &lt;filename unknown&gt;:0:0

Debug at offset 82 in file:line:column &lt;filename unknown&gt;:0:0

Debug at offset 54 in file:line:column &lt;filename unknown&gt;:0:0

GetType at offset 1419 in file:line:column &lt;filename unknown&gt;:0:0

GetDataStoreName at offset 297 in file:line:column &lt;filename unknown&gt;:0:0

GetDataStoreName at offset 115 in file:line:column &lt;filename unknown&gt;:0:0

get_Item at offset 58 in file:line:column &lt;filename unknown&gt;:0:0

GetEntity&lt;T&gt; at offset 113 in file:line:column &lt;filename unknown&gt;:0:0

GetUserRoleByName at offset 132 in file:line:column &lt;filename unknown&gt;:0:0

AddUsersToRoles&lt;U,R&gt; at offset 673 in file:line:column &lt;filename unknown&gt;:0:0

AddUsersToRoles at offset 103 in file:line:column &lt;filename unknown&gt;:0:0

AddUserToRole at offset 192 in file:line:column &lt;filename unknown&gt;:0:0

Page_Load at offset 1704 in file:line:column &lt;filename unknown&gt;:0:0

EventArgFunctionCaller at offset 15 in file:line:column &lt;filename unknown&gt;:0:0

Callback at offset 36 in file:line:column &lt;filename unknown&gt;:0:0

OnLoad at offset 100 in file:line:column &lt;filename unknown&gt;:0:0

LoadRecursive at offset 51 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestMain at offset 628 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 132 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 81 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestWithNoAssert at offset 22 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 50 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 38 in file:line:column &lt;filename unknown&gt;:0:0

System.Web.HttpApplication.IExecutionStep.Execute at offset 182 in file:line:column &lt;filename unknown&gt;:0:0

ExecuteStep at offset 76 in file:line:column &lt;filename unknown&gt;:0:0

ResumeSteps at offset 307 in file:line:column &lt;filename unknown&gt;:0:0

System.Web.IHttpAsyncHandler.BeginProcessRequest at offset 124 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestInternal at offset 380 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestNoDemand at offset 99 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 284 in file:line:column &lt;filename unknown&gt;:0:0

</StackTrace>
</Entry>


<Entry>
<GroupID>fdc155c3-2835-43b6-bed4-f490ccbc62be</GroupID>
<LogLevel>Debug</LogLevel>
<Timestamp>1/12/2009 3:48:08 PM</Timestamp>
<Indent>0</Indent>
<Component>SoftwareMonkeys.SiteStarter.Business.TestExecutor</Component>
<Method>RunTests</Method>
<Data>Running unit tests.</Data>
<StackTrace>CreateStackTrace at offset 111 in file:line:column &lt;filename unknown&gt;:0:0

FormatLogEntry at offset 707 in file:line:column &lt;filename unknown&gt;:0:0

Debug at offset 82 in file:line:column &lt;filename unknown&gt;:0:0

Debug at offset 39 in file:line:column &lt;filename unknown&gt;:0:0

Write at offset 328 in file:line:column &lt;filename unknown&gt;:0:0

StartLevel1 at offset 75 in file:line:column &lt;filename unknown&gt;:0:0

Start at offset 62 in file:line:column &lt;filename unknown&gt;:0:0

StartGroup at offset 85 in file:line:column &lt;filename unknown&gt;:0:0

StartGroup at offset 67 in file:line:column &lt;filename unknown&gt;:0:0

RunTests at offset 140 in file:line:column &lt;filename unknown&gt;:0:0

RunTests at offset 37 in file:line:column &lt;filename unknown&gt;:0:0

OnLoad at offset 674 in file:line:column &lt;filename unknown&gt;:0:0

LoadRecursive at offset 51 in file:line:column &lt;filename unknown&gt;:0:0

LoadRecursive at offset 142 in file:line:column &lt;filename unknown&gt;:0:0

LoadRecursive at offset 142 in file:line:column &lt;filename unknown&gt;:0:0

LoadRecursive at offset 142 in file:line:column &lt;filename unknown&gt;:0:0

LoadRecursive at offset 142 in file:line:column &lt;filename unknown&gt;:0:0

LoadRecursive at offset 142 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestMain at offset 628 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 132 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 81 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestWithNoAssert at offset 22 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 50 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 38 in file:line:column &lt;filename unknown&gt;:0:0

System.Web.HttpApplication.IExecutionStep.Execute at offset 182 in file:line:column &lt;filename unknown&gt;:0:0

ExecuteStep at offset 76 in file:line:column &lt;filename unknown&gt;:0:0

ResumeSteps at offset 307 in file:line:column &lt;filename unknown&gt;:0:0

System.Web.IHttpAsyncHandler.BeginProcessRequest at offset 124 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestInternal at offset 380 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestNoDemand at offset 99 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 284 in file:line:column &lt;filename unknown&gt;:0:0

</StackTrace>
</Entry>


<Entry>
<GroupID>fdc155c3-2835-43b6-bed4-f490ccbc62be</GroupID>
<LogLevel>Debug</LogLevel>
<Timestamp>1/12/2009 3:48:08 PM</Timestamp>
<Indent>1</Indent>
<Component>SoftwareMonkeys.SiteStarter.Business.TestExecutor</Component>
<Method>RunTests</Method>
<Data>Test fixture found: NUnit.Framework.TestCase</Data>
<StackTrace>CreateStackTrace at offset 111 in file:line:column &lt;filename unknown&gt;:0:0

FormatLogEntry at offset 707 in file:line:column &lt;filename unknown&gt;:0:0

Debug at offset 82 in file:line:column &lt;filename unknown&gt;:0:0

Debug at offset 54 in file:line:column &lt;filename unknown&gt;:0:0

RunTests at offset 461 in file:line:column &lt;filename unknown&gt;:0:0

RunTests at offset 37 in file:line:column &lt;filename unknown&gt;:0:0

OnLoad at offset 674 in file:line:column &lt;filename unknown&gt;:0:0

LoadRecursive at offset 51 in file:line:column &lt;filename unknown&gt;:0:0

LoadRecursive at offset 142 in file:line:column &lt;filename unknown&gt;:0:0

LoadRecursive at offset 142 in file:line:column &lt;filename unknown&gt;:0:0

LoadRecursive at offset 142 in file:line:column &lt;filename unknown&gt;:0:0

LoadRecursive at offset 142 in file:line:column &lt;filename unknown&gt;:0:0

LoadRecursive at offset 142 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestMain at offset 628 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 132 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 81 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestWithNoAssert at offset 22 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 50 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 38 in file:line:column &lt;filename unknown&gt;:0:0

System.Web.HttpApplication.IExecutionStep.Execute at offset 182 in file:line:column &lt;filename unknown&gt;:0:0

ExecuteStep at offset 76 in file:line:column &lt;filename unknown&gt;:0:0

ResumeSteps at offset 307 in file:line:column &lt;filename unknown&gt;:0:0

System.Web.IHttpAsyncHandler.BeginProcessRequest at offset 124 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestInternal at offset 380 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequestNoDemand at offset 99 in file:line:column &lt;filename unknown&gt;:0:0

ProcessRequest at offset 284 in file:line:column &lt;filename unknown&gt;:0:0

</StackTrace>
</Entry>
";
			
			#endregion
		}
	}
}