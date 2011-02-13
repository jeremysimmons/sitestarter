using System;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{
	/// <summary>
	/// Description of CorruptLogException.
	/// </summary>
	public class CorruptLogException : Exception
	{
		public CorruptLogException(LogGroup group) : base("The log has been corrupted while in memory.\n"
		                                                  + "Group ID: " + group.ID.ToString() + "\n"
		                                                  + "Parent ID: " + group.ParentID.ToString() + "\n"
		                                                  + "Summary: " + group.Summary + "\n"
		                                                  + "Current group ID: " + DiagnosticState.CurrentGroupID.ToString() + "\n"
		                                                  + "Current group depth: " + DiagnosticState.GroupStack.Count.ToString())
		{
			
		}
	}
}
