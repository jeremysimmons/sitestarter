using System;
using SoftwareMonkeys.SiteStarter.State;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Tests.State
{
	/// <summary>
	/// 
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
        		throw new ArgumentException("No application state data found with the key '" + key + "'.");
        	
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
        		throw new ArgumentException("No session state data found with the key '" + key + "'.");
        	
        	return SessionData[key];
        }
        #endregion

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {

        //    throw new Exception("The method or operation is not implemented.");
            SoftwareMonkeys.SiteStarter.State.StateAccess.State = this;

            base.Initialize(name, config);
        }
	}
}
