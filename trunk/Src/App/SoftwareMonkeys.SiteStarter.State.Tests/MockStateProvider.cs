using System;
using SoftwareMonkeys.SiteStarter.State;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;

namespace SoftwareMonkeys.SiteStarter.State.Tests
{
	/// <summary>
	/// A mock state provider for use during testing.
	/// </summary>
	public class MockStateProvider : StateProvider
	{
		private Dictionary<string, object> sessionData;
		public Dictionary<string, object> SessionData
		{
			get
			{
				if (sessionData == null)
					sessionData = new Dictionary<string, object>();
				return sessionData; }
			set { sessionData = value; }
		}
		
		private Dictionary<string, object> applicationData;
		public Dictionary<string, object> ApplicationData
		{
			get {
				if (applicationData == null)
					applicationData = new Dictionary<string, object>();
				return applicationData; }
			set { applicationData = value; }
		}
		
		
		private Dictionary<string, object> requestData;
		public Dictionary<string, object> RequestData
		{
			get {
				if (requestData == null)
					requestData = new Dictionary<string, object>();
				return requestData; }
			set { requestData = value; }
		}
		
		public MockStateProvider()
		{
		}
		
		#region Application state
		public override bool ContainsApplication(string key)
		{
			return ApplicationData.ContainsKey(key);
		}

		public override void SetApplication(string key, object value)
		{
			ApplicationData[key] = value;
		}

		public override object GetApplication(string key)
		{
			if (!ContainsApplication(key))
			{
				return null;
				//throw new ArgumentException("No application state data found with the key '" + key + "'.");
			}
			
			return ApplicationData[key];
		}
		#endregion
		
		#region Session state
		public override bool ContainsSession(string key)
		{
			return SessionData.ContainsKey(key);
		}
		
		public override void SetSession(string key, object value)
		{
			SessionData[key] = value;
		}

		public override object GetSession(string key)
		{
			if (!ContainsSession(key))
			{
				return null;
				//throw new ArgumentException("No session state data found with the key '" + key + "'.");
			}
			
			return SessionData[key];
		}
		#endregion


		#region Request state
		public override bool ContainsRequest(string key)
		{
			return RequestData.ContainsKey(key);
		}
		
		public override void SetRequest(string key, object value)
		{
			RequestData[key] = value;
		}

		public override object GetRequest(string key)
		{
			if (!ContainsRequest(key))
			{
				return null;
				//throw new ArgumentException("No request state data found with the key '" + key + "'.");
			}
			
			return RequestData[key];
		}
		#endregion
		
		public override string[] GetKeys(StateScope scope)
		{
			List<string> keys = new List<string>();
			
			switch (scope)
			{
				case StateScope.Application:
					foreach (string key in ApplicationData.Keys)
						keys.Add(key);
					break;
				case StateScope.Session:
					foreach (string key in SessionData.Keys)
						keys.Add(key);
					break;
				case StateScope.Operation:
					foreach (string key in RequestData.Keys)
						keys.Add(key);
					break;
			}
			
			return keys.ToArray();
		}
		
		public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
		{

			//    throw new Exception("The method or operation is not implemented.");
			SoftwareMonkeys.SiteStarter.State.StateAccess.State = this;

			base.Initialize(name, config);
		}
	}
}
