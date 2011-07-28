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
			if (Contains(item))
				RemoveAt(IndexOf(item));
			
			Commit();
		}
		
		public new void RemoveAt(int index)
		{
			if (index > -1 && index < base.Count)
			{
				base.RemoveAt(index);
				
				Commit();
			}
		}
		
		/*/// <summary>
		/// Adds the provided item to the state collection.
		/// </summary>
		/// <param name="value">The value to add to the state collection.</param>
		public void Add(T value)
		{
			int lastPosition = Count-1; // The position of the last item
			
			int newPosition = lastPosition+1; // The position of the new item
			
			// The two lines above could be skipped but are there because it makes the logic in the code more readable
			
			SetStateValue(newPosition, value);
		}*/
			
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
		
		/*/// <summary>
		/// Retrieves the value of the state variable with the provided key, in the scope indicated by the Scope property, and in the group indicated by the GroupKey property.
		/// </summary>
		/// <param name="index">The index of the item.</param>
		/// <returns>The value in state corresponding with the specified key, scope, and group.</returns>
		public T GetStateValue(int index)
		{
			string fullKey = GetStateKey(groupKey, index);
			
			T value = default(T);
			
			switch (Scope)
			{
				case StateScope.Application:
					value = (T)StateAccess.State.GetApplication(fullKey);
					break;
				case StateScope.Session:
					value = (T)StateAccess.State.GetSession(fullKey);
					break;
				case StateScope.Operation:
					value = (T)StateAccess.State.GetRequest(fullKey);
					break;
			}
			
			return value;
		}*/
			
			/*/// <summary>
			/// Sets the provided value in the state to correspond with the provided key, in the scope indicated by the Scope property, and in the group indicated by the GroupKey property.
			/// </summary>
			/// <param name="index">The index of the item.</param>
			/// <param name="value">The value to save to state along with the provided key, prefixed by scope and group key.</param>
			public void SetStateValue(int index, T value)
		{
			string fullKey = GetStateKey(groupKey, index);
			
			if (value == null)
			{
				// Remove the key
				List<string> list = new List<string>(Keys);
				list.RemoveAt(index);
				
				Keys = list.ToArray();
			}
			
			switch (Scope)
			{
				case StateScope.Application:
					StateAccess.State.SetApplication(fullKey, value);
					break;
				case StateScope.Session:
					StateAccess.State.SetSession(fullKey, value);
					break;
				case StateScope.Operation:
					StateAccess.State.SetRequest(fullKey, value);
					break;
			}
		}*/
			
			/*
		/// <summary>
		/// Checks whether a state value exists at the specified index.
		/// </summary>
		/// <param name="index">The index of the item.</param>
		public bool StateValueExists(int index)
		{
			if (!StateAccess.IsInitialized || StateAccess.State == null)
				throw new InvalidOperationException("The state hasn't been initialized.");
			
			string fullKey = GetStateKey(groupKey, index);
			
			bool exists = false;
			
			switch (Scope)
			{
				case StateScope.Application:
					exists = StateAccess.State.ContainsApplication(fullKey) &&
						StateAccess.State.GetApplication(fullKey) != null;
					break;
				case StateScope.Session:
					exists = StateAccess.State.ContainsSession(fullKey) &&
						StateAccess.State.GetSession(fullKey) != null;
					break;
				case StateScope.Operation:
					exists = StateAccess.State.ContainsRequest(fullKey) &&
						StateAccess.State.GetRequest(fullKey) != null;
					break;
			}
			
			return exists;
		}*/
			
			/// <summary>
			/// Creates the key used to store the collection in state.
			/// </summary>
			/// <returns>The state key.</returns>
			public string GetStateKey()
		{
			string key  = groupKey;
			
			return key;
		}
		/*
		/// <summary>
		/// Counts the number of items in the collection.
		/// </summary>
		/// <returns>The total number of items in the collection.</returns>
		public int GetCount()
		{
			bool doContinue = true;
			int i = -1;
			int count = 0;
			
			do
			{
				i++;
				doContinue = StateValueExists(i);
			} while (doContinue);
			
			count = i;
			
			return count;
		}*/
		/*
		/// <summary>
		/// The total number of items in the collection.
		/// </summary>
		public int Count {
			get {
				return GetCount();
			}
		}*/
		
		public bool IsReadOnly {
			get {
				throw new NotImplementedException();
			}
		}
		/*
		public int IndexOf(T item)
		{
			for (int i = 0; i < Count; i ++)
			{
				if (item.Equals(this[i]))
					return i;
			}
			
			return -1;
		}
		
		public void Insert(int index, T item)
		{
			this[index] = item;
		}
		
		public void RemoveAt(int index)
		{
			List<string> list = new List<string>(Keys);
			list.RemoveAt(index);
			
			Keys = list.ToArray();
			
			SetStateValue(index, default(T));
			
		}
		
		public void Clear()
		{
			for (int i = 0; i < Count; i ++)
			{
				this[i] = default(T);
			}
		}
		
		public bool Contains(T item)
		{
			return IndexOf(item) > -1;
		}
		
		public void CopyTo(T[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}*/
		
		/*public bool Remove(T item)
		{
			int index = IndexOf(item);
			
			if (index > -1)
			{
				List<string> list = new List<string>(Keys);
				list.RemoveAt(index);
				
				Keys = list.ToArray();
				
				SetStateValue(index, default(T));
				
				return true;
			}
			else
				return false;
		}*/
		
		/*public IEnumerator<T> GetEnumerator()
		{
			for (int i = 0; i < Count; i++)
			{
				yield return this[i];
			}
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			for (int i = 0; i < Count; i++)
			{
				yield return this[i];
			}
		}*/
		
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
