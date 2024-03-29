﻿using System;
using SoftwareMonkeys.SiteStarter.State;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Tests;

namespace SoftwareMonkeys.SiteStarter.State.Tests
{
	/// <summary>
	/// A mock state provider for use during testing.
	/// </summary>
	public class MockStateProvider : BaseStateProvider
	{
		private BaseTestFixture fixture;
		public BaseTestFixture Fixture
		{
			get { return fixture; }
			set { fixture = value; }
		}
		
    	private string physicalApplicationPath = String.Empty;
    	/// <summary>
    	/// Gets/sets the full physical path to the room of the running application.
    	/// </summary>
    	public override string PhysicalApplicationPath
    	{
    		get { return physicalApplicationPath; }
    		set { physicalApplicationPath = value; }
    	}
    	
    	private string applicationPath = String.Empty;
    	/// <summary>
    	/// Gets/sets the virtual path to the room of the running application.
    	/// </summary>
    	public override string ApplicationPath
    	{
    		get { return applicationPath; }
    		set { applicationPath = value; }
    	}
    	
		private Hashtable sessionData;
		public Hashtable SessionData
		{
			get
			{
				if (sessionData == null)
					sessionData = new Hashtable();
				return sessionData; }
			set { sessionData = value; }
		}
		
		private Hashtable applicationData;
		public Hashtable ApplicationData
		{
			get {
				if (applicationData == null)
					applicationData = new Hashtable();
				return applicationData; }
			set { applicationData = value; }
		}
		
		
		private Hashtable operationData;
		public Hashtable OperationData
		{
			get {
				if (operationData == null)
					operationData = new Hashtable();
				return operationData; }
			set { operationData = value; }
		}
		
		private Hashtable userData;
		public Hashtable UserData
		{
			get {
				if (userData == null)
					userData = new Hashtable();
				return userData; }
			set { userData = value; }
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
						keys.Add(key);
					break;
				case StateScope.Session:
					foreach (string key in SessionData.Keys)
						keys.Add(key);
					break;
				case StateScope.Operation:
					foreach (string key in OperationData.Keys)
						keys.Add(key);
					break;
			}
			
			return keys.ToArray();
		}
		
		public MockStateProvider(BaseTestFixture fixture)
		{
			Fixture = fixture;
			Initialize();
		}
		
		public override void Initialize()
		{
			StateAccess.State = this;
			
			this.PhysicalApplicationPath = TestUtilities.GetTestApplicationPath(fixture, "MockApplication");
			this.ApplicationPath = "/MockApplication";

		}
	}
}
