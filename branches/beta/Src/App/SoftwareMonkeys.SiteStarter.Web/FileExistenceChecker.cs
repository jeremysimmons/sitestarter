using System;
using System.IO;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.IO;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	///
	/// </summary>
	public class FileExistenceChecker
	{
		private string applicationPath = String.Empty;
		public string ApplicationPath
		{
			get {
				if (applicationPath == String.Empty)
					applicationPath = StateAccess.State.ApplicationPath;
				return applicationPath; }
			set { applicationPath = value; }
		}
		
		private IFileMapper fileMapper;
		public IFileMapper FileMapper
		{
			get {
				if (fileMapper == null)
					fileMapper = new FileMapper(ApplicationPath);
				return fileMapper; }
			set { fileMapper = value; }
		}
		
		public FileExistenceChecker()
		{
		}
		
		public virtual bool Exists(string relativeFilePath)
		{
			bool isReal = false;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Checking whether the file exists: " + relativeFilePath))
			{
				string physicalFile = FileMapper.MapPath(relativeFilePath);
				
				LogWriter.Debug("Physical file: " + physicalFile);
				
				isReal = File.Exists(physicalFile);
				
				LogWriter.Debug("Is real? " + isReal.ToString());
			}
			return isReal;
		}
	}
}
