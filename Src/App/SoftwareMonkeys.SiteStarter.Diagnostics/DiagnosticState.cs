using System;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{
	/// <summary>
	/// Hold the diagnostic state collection.
	/// </summary>
	static public class DiagnosticState
	{
		static private StateStack<LogGroup> groupStack;
		/// <summary>
		/// Gets/sets the log state group stack.
		/// </summary>
		static public StateStack<LogGroup> GroupStack
		{
			get {
				if (groupStack == null)
					groupStack = new StateStack<LogGroup>(StateScope.Operation, "GroupStack");
				return groupStack; }
			set { groupStack = value; }
		}
	}
}
