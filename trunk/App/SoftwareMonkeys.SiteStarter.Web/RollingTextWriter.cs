using System;
using System.Diagnostics;
using System.IO;
using System.Web;

namespace SoftwareMonkeys.SiteStarter.Web
{
	public class RollingTextWriter: System.Diagnostics.TraceListener
	{
		string _fileName;
		System.DateTime _currentDate;
		System.IO.StreamWriter _traceWriter;
		
		public RollingTextWriter(string fileName)
		{
			// Pass in the path of the logfile (ie. C:\Logs\MyAppLog.log)
			// The logfile will actually be created with a yyyymmdd format appended to the filename
			_fileName = fileName;
			_traceWriter = new StreamWriter(generateFilename(), true);
		}
		
		public override void Write(string value)
		{
			checkRollover();
			_traceWriter.Write(value);
		}
		
		public override void WriteLine(string value)
		{
			checkRollover();
			_traceWriter.WriteLine(value);
		}
		
		private string generateFilename()
		{
			if (_fileName.IndexOf(":") == -1)
				_fileName = HttpContext.Current.Server.MapPath(Path.Combine(HttpContext.Current.Request.ApplicationPath, _fileName));

			_currentDate = System.DateTime.Today;
			
			return Path.Combine(Path.GetDirectoryName(_fileName),
			Path.GetFileNameWithoutExtension(_fileName) + "_" +
			_currentDate.ToString("yyyymmdd") + Path.GetExtension(_fileName));
		}
		
		private void checkRollover()
		{
			// If the date has changed, close the current stream and create a new file for today’s date
			if (_currentDate.CompareTo(System.DateTime.Today) != 0)
			{
			_traceWriter.Close();
			_traceWriter = new StreamWriter(generateFilename(), true);
			}
		}
		
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
			_traceWriter.Close();
			}
		}
	}
}