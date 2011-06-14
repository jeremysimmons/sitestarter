using System;
using System.ComponentModel;
using System.Web;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// Used to temporarily increase the timeout of a script for certain time consuming operations. Reverts back to the standard timeout when the component disposes.
	/// </summary>
	public class TimeoutExtender : Component
	{
		public int OriginalTimeout = 0;
		
		public TimeoutExtender(int timeoutInSeconds)
		{
			Initialize(timeoutInSeconds);
		}
		
		/// <summary>
		/// Sets the timeout to the provided number of seconds.
		/// </summary>
		/// <param name="timeoutInSeconds"></param>
		protected virtual void Initialize(int timeoutInSeconds)
		{
			OriginalTimeout = HttpContext.Current.Server.ScriptTimeout;
			
			int millisecondsInSecond = 1000; // 1000 milliseconds per second

			HttpContext.Current.Server.ScriptTimeout = timeoutInSeconds * millisecondsInSecond;
		}
		
		/// <summary>
		/// Reverts the timeout back to the default setting.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			HttpContext.Current.Server.ScriptTimeout = OriginalTimeout;
			
			base.Dispose(disposing);
		}
		
		static public TimeoutExtender NewMinutes(int timeoutInMinutes)
		{
			return NewSeconds(timeoutInMinutes*60);
		}
		
		static public TimeoutExtender NewSeconds(int timeoutInSeconds)
		{
			return new TimeoutExtender(timeoutInSeconds);
		}
	}
}
