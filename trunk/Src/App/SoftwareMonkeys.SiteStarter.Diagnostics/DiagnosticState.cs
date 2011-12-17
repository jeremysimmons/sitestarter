using System;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{
	/// <summary>
	/// Hold the diagnostic state collection.
	/// </summary>
	static public class DiagnosticState
	{		
		/// <summary>
		/// Gets/sets the current group.
		/// </summary>
		static public LogGroup CurrentGroup
		{
			get {
				if (StateAccess.State.ContainsOperation("DiagnosticState_CurrentGroup"))
					return (LogGroup)StateAccess.State.GetOperation("DiagnosticState_CurrentGroup");
				return null; }
			set {
				if (value != null)
					StateAccess.State.SetOperation("DiagnosticState_CurrentGroup", value);
				else
					StateAccess.State.RemoveOperation("DiagnosticState_CurrentGroup");
				
			}
		}
		
		/// <summary>
		/// Gets/sets the ID of the current group.
		/// </summary>
		static public Guid CurrentGroupID
		{
			get
			{
				if (CurrentGroup != null)
					return CurrentGroup.ID;
				else
					return Guid.Empty;
			}
		}
		
		/// <summary>
		/// Adds the provided log group to the stack.
		/// </summary>
		/// <param name="group">The group to add to the stack.</param>
		static public void PushGroup(LogGroup group)
		{
			if (group == null)
				throw new ArgumentNullException("group");
			
			group.Parent = DiagnosticState.CurrentGroup;
			
			if (CurrentGroup != null)
				CurrentGroup.Append(group);
			
			CurrentGroup = group;
		}

		/// <summary>
		/// Pops the highest group out of the stack.
		/// </summary>
		static public void PopGroup()
		{
			if (CurrentGroup != null)
				CurrentGroup = CurrentGroup.Parent;
		}
		
		/// <summary>
		/// Disposes the diagnostic state. Pops all groups so they're written to the log, then disposes the log writer to unlock the file.
		/// </summary>
		static public void Dispose()
		{
			while (CurrentGroup != null)
				PopGroup();
			
			LogWriter.Dispose();
		}
	}
}
