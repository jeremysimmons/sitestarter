using System;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{
	/// <summary>
	/// Used to detect the current build mode.
	/// </summary>
	public class ModeDetector
	{
		/// <summary>
		/// Gets a value indicating whether the assembly was built in debug mode.
		/// </summary>
		public virtual bool IsDebug
		{
			get {
				bool isDebug = false;

				#if (DEBUG)
				isDebug = true;
				#else
				isDebug = false;
				#endif
				
				return isDebug;
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether the assembly was built in release mode.
		/// </summary>
		public bool IsRelease
		{
			get { return !IsDebug; }
		}
	}
}
