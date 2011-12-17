using System;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.State
{
	/// <summary>
	/// Persists data in application, session, or request scope state.
	/// </summary>
	public class StateCollection<T> : System.Collections.Generic.List<T>
	{
		
		private StateScope scope;
		/// <summary>
		/// Gets/sets the scope of the state being managed by the collection.
		/// </summary>
		public StateScope Scope
		{
			get { return scope; }
			set { scope = value; }
		}
		
		private string groupKey;
		/// <summary>
		/// Gets/sets the key/prefix of the group/feature/component that the state data belongs to.
		/// </summary>
		public string GroupKey
		{
			get { return groupKey; }
			set { groupKey = value; }
		}
		
		/// <summary>
		/// Gets/sets the state variable at the specified index.
		/// </summary>
		public new T this[int index]
		{
			get { return base[index]; }
			set {
				if (value != null)
					base[index] = value;
				else
					RemoveAt(index);
				
				Commit();
			}
		}
		
		public new void Add(T item)
		{
			base.Add(item);
			
			Commit();
		}
		
		public new void Remove(T item)
		{
			if (item != null && Contains(item))
			{
				base.Remove(item);

				Commit();
			}
		}
		
		public new void RemoveAt(int index)
		{
			if (index > -1 && index < base.Count)
			{
				base.RemoveAt(index);
				
				Commit();
			}
		}
		/// <summary>
		/// Sets the scope and group key used by this collection.
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="groupKey"></param>
		public StateCollection(StateScope scope, string groupKey)
		{
			Scope = scope;
			GroupKey = groupKey;
		}
		
		/// <summary>
		/// Creates the key used to store the collection in state.
		/// </summary>
		/// <returns>The state key.</returns>
		public string GetStateKey()
		{
			string key  = groupKey;
			
			return key;
		}
		
		public bool IsReadOnly {
			get {
				throw new NotImplementedException();
			}
		}
		
		public new T[] ToArray()
		{
			List<T> list = new List<T> ();
			for (int i = 0; i < Count; i++)
			{
				list.Add(this[i]);
			}
			return list.ToArray();
		}
		
		
		public void Commit()
		{
			string key = GroupKey;
			
			switch (Scope)
			{
				case StateScope.Application:
					StateAccess.State.SetApplication(key, this);
					break;
				case StateScope.Session:
					StateAccess.State.SetSession(key, this);
					break;
				case StateScope.Operation:
					StateAccess.State.SetOperation(key, this);
					break;
			}
		}
		
		static public StateCollection<T> Current(StateScope scope, string group)
		{
			StateCollection<T> collection = null;
			switch (scope)
			{
				case StateScope.Application:
					collection = (StateAccess.State.ContainsApplication(group)
					              ? (StateCollection<T>)StateAccess.State.GetApplication(group)
					              :  new StateCollection<T>(scope, group));
					break;
				case StateScope.Session:
					collection = (StateAccess.State.ContainsSession(group)
					              ? (StateCollection<T>)StateAccess.State.GetSession(group)
					              :  new StateCollection<T>(scope, group));
					break;
				case StateScope.Operation:
					collection = (StateAccess.State.ContainsOperation(group)
					              ? (StateCollection<T>)StateAccess.State.GetOperation(group)
					              :  new StateCollection<T>(scope, group));
					break;
			}
			return collection;
		}
	}
}
