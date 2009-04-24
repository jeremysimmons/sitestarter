using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareMonkeys.SiteStarter.Configuration
{
    public interface IAppConfig
    {

	/// <summary>
	/// The universal ID of the current project.
	/// </summary>
	Guid UniversalProjectID { get; }

        // TODO: Clean up
        int SessionTimeout { get;set; }

        string ApplicationPath { get; set; }
		//string DatabaseLogPath { get; }
		//string ProjectsDirectory { get; }
		//string ProjectsDirectoryPath { get; }
        //string BackupDirectoryPath { get; }
        //string DataDirectoryPath { get; }
        //string DatabasePath { get; }

        string PhysicalPath { get;set; }

      /*  void AddDataServer(string name, IObjectServer server);
        IObjectServer GetDataServer(string name);
        void RemoveDataServer(string name);
        void RemoveDataServers();

        void AddDataStore(string name, IObjectContainer store);
        IObjectContainer GetDataStore(string name);
        void RemoveDataStore(string name);
        void RemoveDataStores();*/

        // TODO: Tidy up
       /* void AddLogWriter(string name, Db4oMessageWriter writer);
        Db4oMessageWriter GetLogWriter(string name);
        void RemoveLogWriter(string name);
        void RemoveLogWriters();*/


		//Guid ProjectID { get; }

		/*string MapPath(string path);

		string ApplicationUrl { get; }
		string ApplicationPath { get; }

		string CompanyName { get; }*/

		//ResourceManager TextManager { get; }

		/*string[] TraceData { get; }

		string[] TextFileExtensions { get; }
		string[] ImageFileExtensions { get; }
		string[] HtmlFileExtensions { get; }

		bool EnableEmailNotification { get; }
		string SmtpServer { get; }
		bool NotifyUserCreated { get; }
		bool NotifyBugReported { get; }

        string[] Modules { get;set; }

		bool EnableDemo { get; }*/

    }
}
