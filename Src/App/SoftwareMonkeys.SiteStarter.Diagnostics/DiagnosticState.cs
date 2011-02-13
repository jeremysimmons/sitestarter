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
				if (StateAccess.State.ContainsRequest("AppLogger_CurrentGroup"))
					return (LogGroup)StateAccess.State.GetRequest("AppLogger_CurrentGroup");
				return null; }
			set {
				if (value != null)
					StateAccess.State.SetRequest("AppLogger_CurrentGroup", value);
				else
					StateAccess.State.RemoveRequest("AppLogger_CurrentGroup");
				
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
				LogGroup parentGroup = CurrentGroup.Parent;
				
				GroupStack.Pop();

				CurrentGroup = parentGroup;
			}
		}
	}
}
