using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.State
{
	/// <summary>
	/// Persists data in application, session, or request scope state.
	/// </summary>
	public class StateNameValueCollection<T> : NameObjectCollectionBase, IEnumerable<T>
	{
		private StateCollection<string> keys;
		/// <summary>
		/// Gets/sets
		/// </summary>
		new public StateCollection<string> Keys
		{
			get {
				if (keys == null)
				{
					keys = StateCollection<string>.Current(StateScope.Application, GroupKey + "_Keys");
				}
				return keys; }
			set { keys = value; }
		}
		
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
			set { groupKey = value;
				Keys.GroupKey = value + "_Keys";
			}
		}
		
		/// <summary>
		/// Gets/sets the state variable with the specified key.
		/// </summary>
		public T this[string key]
		{
			get { return GetStateValue(key); }
			set { SetStateValue(key, value); }
		}
		
		/// <summary>
		/// Gets/sets the state variable with the specified key.
		/// </summary>
		public T this[int index]
		{
			get { return this[Keys[index]]; }
			set { this[Keys[index]] = value; }
		}
		
		/// <summary>
		/// Sets the scope used by this collection.
		/// </summary>
		/// <param name="scope"></param>
		public StateNameValueCollection(StateScope scope)
		{
			Scope = scope;
		}
		
		/// <summary>
		/// Sets the scope and group key used by this collection.
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="groupKey"></param>
		public StateNameValueCollection(StateScope scope, string groupKey)
		{
			Scope = scope;
			GroupKey = groupKey;
		}
		
		/// <summary>
		/// Counts the number of items in the collection.
		/// </summary>
		/// <returns>The total number of items in the collection.</returns>
		public int GetCount()
		{
			/*int i = 0;
			int count = 0;
			
			foreach (string key in StateAccess.State.GetKeys(Scope))
			{
				// If it starts with the group key
				//if (key.IndexOf(GroupKey + "_") > -1)
				//{
					i++;
				//}
			}
			
			count = i;
			
			return count;*/
			
			return Keys.Count;
		}
		
		
		/// <summary>
		/// The total number of items in the collection.
		/// </summary>
		public override int Count {
			get {
				return GetCount();
			}
		}
		
		/// <summary>
		/// Checks whether a state value exists at the specified index.
		/// </summary>
		/// <param name="key">The key of the item.</param>
		public bool StateValueExists(string key)
		{
			if (!StateAccess.IsInitialized || StateAccess.State == null)
				throw new InvalidOperationException("The state hasn't been initialized.");
			
			string fullKey = GetStateKey(groupKey, key);
			
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
					exists = StateAccess.State.ContainsOperation(fullKey) &&
						StateAccess.State.GetOperation(fullKey) != null;
					break;
			}
			
			return exists;
		}
		
		/// <summary>
		/// Retrieves the value of the state variable with the provided key, in the scope indicated by the Scope property, and in the group indicated by the GroupKey property.
		/// </summary>
		/// <param name="key">The key to retrieve the value for.</param>
		/// <returns>The value in state corresponding with the specified key, scope, and group.</returns>
		public T GetStateValue(string key)
		{
			string fullKey = GetStateKey(groupKey, key);
			
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
					value = (T)StateAccess.State.GetOperation(fullKey);
					break;
			}
			
			return value;
		}
		
		/// <summary>
		/// Removes the value with the provided key.
		/// </summary>
		/// <param name="key">The key of the value to remove.</param>
		public void RemoveStateValue(string key)
		{
			string fullKey = GetStateKey(groupKey, key);
			
			// Remove the key
			if (Keys.Contains(key))
				Keys.Remove(key);
			
			
			switch (Scope)
			{
				case StateScope.Application:
					StateAccess.State.RemoveApplication(fullKey);
					break;
				case StateScope.Session:
					StateAccess.State.RemoveSession(fullKey);
					break;
				case StateScope.Operation:
					StateAccess.State.RemoveOperation(fullKey);
					break;
			}
		}
		
		/// <summary>
		/// Sets the provided value in the state to correspond with the provided key, in the scope indicated by the Scope property, and in the group indicated by the GroupKey property.
		/// </summary>
		/// <param name="key">The key to assign to the provided value.</param>
		/// <param name="value">The value to save to state along with the provided key, prefixed by scope and group key.</param>
		public void SetStateValue(string key, T value)
		{
			string fullKey = GetStateKey(groupKey, key);
			
			switch (Scope)
			{
				case StateScope.Application:
					StateAccess.State.SetApplication(fullKey, value);
					break;
				case StateScope.Session:
					StateAccess.State.SetSession(fullKey, value);
					break;
				case StateScope.Operation:
					StateAccess.State.SetOperation(fullKey, value);
					break;
			}
			
			if (value == null)
			{
				// Remove the key
				Keys.Remove(key);
				
				RemoveStateValue(key);
			}
			else
			{
				if (!Keys.Contains(key))
					Keys.Add(key);
			}
		}
		
		/// <summary>
		/// Retrieves the full key used to store a state variable prefixed by the provided group.
		/// </summary>
		/// <param name="groupKey">The key of the group used as a prefix.</param>
		/// <param name="key">The key of the state entry.</param>
		/// <returns>The full key; a combination of groupKey and key with an underscore _ between them.</returns>
		public string GetStateKey(string groupKey, string key)
		{
			string stateKey = key;
			
			// Only apply the group key if specified.
			if (groupKey != null && groupKey != String.Empty)
				stateKey = groupKey + "_" + stateKey;
			
			return stateKey;
		}
		
		public int IndexOf(T item)
		{
			if (item != null)
			{
				for (int i = 0; i < Count; i ++)
				{
					if (item.Equals(this[i]))
						return i;
				}
			}
			
			return -1;
		}
		
		public void Insert(int index, T item)
		{
			this[index] = item;
		}
		
		public void RemoveAt(int index)
		{
			if (index > -1)
			{
				string key = Keys[index];
				
				RemoveStateValue(key);
				
				base.BaseRemove(key);
			}
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
		}
		
		public bool Remove(T item)
		{
			
			int index = IndexOf(item);
			
			if (index > -1)
			{
				string key = Keys[index];
				
				RemoveStateValue(key);
				
				base.BaseRemove(key);
				
				return true;
			}
			else
				return false;
		}
		
		public new IEnumerator<T> GetEnumerator()
		{
			foreach (string key in Keys)
			{
				yield return this[key];
			}
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			foreach (string key in Keys)
			{
				yield return this[key];
			}

		}
		
		public virtual T[] ToArray()
		{
			List<T> list = new List<T>();
			foreach (T item in this)
				list.Add(item);
			return list.ToArray();
		}
	}
}
