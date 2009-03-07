using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using NLog;

namespace SoftwareMonkeys.SiteStarter.Web
{
	public class LogWriter : System.Diagnostics.TraceListener
	{
		
		public LogWriter()
		{
			
		}
		
		public override void Write(string value)
		{
// TODO: Change this to just Tracer.Write
            AppLogger.Debug(value, Reflector.GetCallingMethod());
		}
		
		public override void WriteLine(string value)
		{
            AppLogger.Debug(value, Reflector.GetCallingMethod());
		}
		
		
	}
}