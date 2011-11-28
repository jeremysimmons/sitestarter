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
			get {
				if (CurrentGroup != null)
					return CurrentGroup.ID;
				else
					return Guid.Empty;
			}
		}
		
		/// <summary>
		/// Gets/sets the indent of the current log entries.
		/// </summary>
		public static int GroupIndent
		{
			get
			{
				if (GroupStack == null || GroupStack.Count == 0)
					return 0;
				else
					return GroupStack.Count;
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
			
			// TODO: Check if needed. Should be obsolete.
			// The ParentID property being Guid.Empty should indicate that it's a thread title
			if (GroupStack.Count == 0)
				group.IsThreadTitle = true;
			
			GroupStack.Push(group);
		}

		/// <summary>
		/// Pops the highest group out of the stack.
		/// </summary>
		static public void PopGroup()
		{
			if (GroupStack.Count > 0)
			{
				LogGroup parentGroup = null;

				if (CurrentGroup != null)
					parentGroup = CurrentGroup.Parent;

				try
				{
					GroupStack.Pop();
				}
				catch (ArgumentOutOfRangeException ex)
				{
					LogWriter.Error(ex);
					// Skip argument out of range exception
					// Should be rare and only happen if the application gets unloaded before the groups are all popped out of the list.
					// For some reason checking the count before popping doesn't work
				}

				CurrentGroup = parentGroup;
			}
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
