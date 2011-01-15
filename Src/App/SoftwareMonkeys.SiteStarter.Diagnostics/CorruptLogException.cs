using System;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{
	/// <summary>
	/// Description of CorruptLogException.
	/// </summary>
	public class CorruptLogException : Exception
	{
		public CorruptLogException(LogGroup group) : base("The log has been corrupted while in memory.\nGroup ID: " + group.ID.ToString() + "\nSummary: " + group.Summary + "\nCurrent group ID: " + DiagnosticState.CurrentGroupID.ToString())
		{
			
		}
	}
}
