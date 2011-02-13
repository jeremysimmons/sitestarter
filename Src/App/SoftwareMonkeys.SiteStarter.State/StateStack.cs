using System;

namespace SoftwareMonkeys.SiteStarter.State
{
	/// <summary>
	/// Holds a stack and persists it in state.
	/// </summary>
	public class StateStack<T> : StateCollection<T>
	{		
		/// <summary>
		/// Sets the scope and group key used by this collection.
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="groupKey"></param>
		public StateStack(StateScope scope, string groupKey) : base(scope, groupKey)
		{
		}
		
		/// <summary>
		/// Adds an item to the stack.
		/// </summary>
		/// <param name="value">The item to add to the stack.</param>
		public void Push(T value)
		{
			Add(value);
		}
		
		/// <summary>
		/// Removes the top item from the stack.
		/// </summary>
		public void Pop()
		{
			if (Count >= 1)
				Remove(this[Count-1]);
		}
		
	}
}
