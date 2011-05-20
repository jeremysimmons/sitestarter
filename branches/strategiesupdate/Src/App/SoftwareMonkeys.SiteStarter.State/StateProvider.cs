
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;
using System.IO;
using System.Web;

namespace SoftwareMonkeys.SiteStarter.State
{
	/// <summary>
	/// Initializes and acts as the the state management provider
	/// </summary>
	public class StateProvider : BaseStateProvider
	{
		private string physicalApplicationPath = String.Empty;
		/// <summary>
		/// Gets/sets the full physical path to the root of the running application.
		/// </summary>
		public override string PhysicalApplicationPath
		{
			get { return physicalApplicationPath; }
			set { physicalApplicationPath = value; }
		}
		
		private string applicationPath = String.Empty;
		/// <summary>
		/// Gets/sets the virtual path to the root of the running application.
		/// </summary>
		public override string ApplicationPath
		{
			get { return applicationPath; }
			set { applicationPath = value; }
		}
		
		#region Data properties
		private Dictionary<string, object> sessionData;
		protected Dictionary<string, object> SessionData
		{
			get
			{
				if (sessionData == null)
					sessionData = new Dictionary<string, object>();
				return sessionData; }
			set { sessionData = value; }
		}
		
		private Dictionary<string, object> applicationData;
		protected Dictionary<string, object> ApplicationData
		{
			get {
				if (applicationData == null)
					applicationData = new Dictionary<string, object>();
				return applicationData; }
			set { applicationData = value; }
		}
		
		
		private Dictionary<string, object> operationData;
		protected Dictionary<string, object> OperationData
		{
			get {
				if (operationData == null)
					operationData = new Dictionary<string, object>();
				return operationData; }
			set { operationData = value; }
		}
		
		private Dictionary<string, object> userData;
		protected Dictionary<string, object> UserData
		{
			get {
				if (userData == null)
					userData = new Dictionary<string, object>();
				return userData; }
			set { userData = value; }
		}
		#endregion
		
		public StateProvider()
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
		
		public override void RemoveApplication(string key)
		{
			if (ContainsApplication(key))
			{
				ApplicationData.Remove(key);
			}
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
		
		public override void RemoveSession(string key)
		{
			if (ContainsSession(key))
			{
				SessionData.Remove(key);
			}
		}
		#endregion


		#region Operation state
		public override bool ContainsOperation(string key)
		{
			return OperationData.ContainsKey(key);
		}
		
		public override void SetOperation(string key, object value)
		{
			OperationData[key] = value;
		}

		public override object GetOperation(string key)
		{
			if (!ContainsOperation(key))
			{
				return null;
				//throw new ArgumentException("No request state data found with the key '" + key + "'.");
			}
			
			return OperationData[key];
		}
		
		public override void RemoveOperation(string key)
		{
			if (ContainsOperation(key))
			{
				OperationData.Remove(key);
			}
		}
		#endregion
		
		#region Request state
		public override bool ContainsRequest(string key)
		{
			return ContainsOperation(key);
		}
		
		public override void SetRequest(string key, object value)
		{
			SetOperation(key, value);
		}

		public override object GetRequest(string key)
		{
			return GetOperation(key);
		}
		
		public override void RemoveRequest(string key)
		{
			RemoveOperation(key);
		}
		#endregion
		
		#region User state
		public override bool ContainsUser(string key)
		{
			return UserData.ContainsKey(key);
		}
		
		public override void SetUser(string key, object value)
		{
			UserData[key] = value;
		}
		
		public override void SetUser(string key, object value, DateTime expirationDate)
		{
			UserData[key] = value;
		}

		public override object GetUser(string key)
		{
			if (!ContainsUser(key))
			{
				return null;
			}
			
			return UserData[key];
		}
		
		public override void RemoveUser(string key)
		{
			if (ContainsUser(key))
			{
				UserData.Remove(key);
			}
		}
		#endregion

		
		public override string[] GetKeys(StateScope scope)
		{
			List<string> keys = new List<string>();
			
			switch (scope)
			{
				case StateScope.Application:
					foreach (string key in ApplicationData.Keys)
						if (key != null)
							keys.Add(key);
					break;
				case StateScope.Session:
					foreach (string key in SessionData.Keys)
						if (key != null)
							keys.Add(key);
					break;
				case StateScope.Operation:
					foreach (string key in OperationData.Keys)
						if (key != null)
							keys.Add(key);
					break;
				case StateScope.User:
					foreach (string key in UserData.Keys)
						if (key != null)
							keys.Add(key);
					break;
			}
			
			return keys.ToArray();
		}
		
		public override void Initialize()
		{
			StateAccess.State = this;
			
			PhysicalApplicationPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			ApplicationPath = PhysicalApplicationPath;
			
		}

	}
}
