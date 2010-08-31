using System;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.State
{
	/// <summary>
	/// Persists data in application, session, or request scope state.
	/// </summary>
	public class StateNameValueCollection<T> : System.Collections.Specialized.NameObjectCollectionBase
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
		/// Gets/sets the state variable with the specified key.
		/// </summary>
		public T this[string key]
		{
			get { return GetStateValue(key); }
			set { SetStateValue(key, value); }
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
					value = (T)StateAccess.State.GetRequest(fullKey);
					break;
			}
			
			return value;
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
					StateAccess.State.SetRequest(fullKey, value);
					break;
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
			return groupKey + "_" + key;
		}
	}
}
