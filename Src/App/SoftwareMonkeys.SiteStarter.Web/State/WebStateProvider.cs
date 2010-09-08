using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Entities;
using System.IO;
using SoftwareMonkeys.SiteStarter.Configuration;
using System.Web;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Web.State
{
	/// <summary>
	/// Initializes and acts as the the state management provider
	/// </summary>
    public class WebStateProvider : StateProvider 
    {
    	#region Application state
        public override bool ContainsApplication(string key)
        {
        	if (key == String.Empty)
        		throw new ArgumentException("The provided key cannot be null or String.Empty.");
        	
       		if (key != String.Empty
        	    && HttpContext.Current != null
                && HttpContext.Current.Application != null
                && HttpContext.Current.Application[key] != null)
	            return true;
	        else
	        	return false;
        }

        public override void SetApplication(string key, object value)
        {
        	if (key == String.Empty)
        		throw new ArgumentException("The provided key cannot be null or String.Empty.");
        	
            if (key != String.Empty && HttpContext.Current != null)
            {
                HttpContext.Current.Application[key] = value;
            }
        }

        public override object GetApplication(string key)
        {
        	if (key == String.Empty)
        		throw new ArgumentException("The provided key cannot be null or String.Empty.");
        	
       	if (key != String.Empty && HttpContext.Current != null)
	            return HttpContext.Current.Application[key];
	        else
	        	return null;
        }
        #endregion
        
        #region Session state
        public override bool ContainsSession(string key)
        {
        	if (key == String.Empty)
        		throw new ArgumentException("The provided key cannot be null or String.Empty.");
        	
       		if (key != String.Empty
        	    && HttpContext.Current != null
                && HttpContext.Current.Session != null
        	    && HttpContext.Current.Session[key] != null)
	            return true;
	        else
	        	return false;
        }
        
        public override void SetSession(string key, object value)
        {
        	if (key == String.Empty)
        		throw new ArgumentException("The provided key cannot be null or String.Empty.");
        	
            if (key != String.Empty
                && HttpContext.Current != null
                && HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session[key] = value;
            }
        }

        public override object GetSession(string key)
        {
        	if (key == String.Empty)
        		throw new ArgumentException("The provided key cannot be null or String.Empty.");
        	
        	if (key != String.Empty && HttpContext.Current != null && HttpContext.Current.Session != null)
	            return HttpContext.Current.Session[key];
	        else
	        	return null;
        }
        #endregion
        
        #region Request state
        public override bool ContainsRequest(string key)
        {
        	if (key == String.Empty)
        		throw new ArgumentException("The provided key cannot be null or String.Empty.");
        	
       		if (key != String.Empty
        	    && HttpContext.Current != null
                && HttpContext.Current.Items != null
        	    && HttpContext.Current.Items[key] != null)
	            return true;
	        else
	        	return false;
        }
        
        public override void SetRequest(string key, object value)
        {
        	if (key == String.Empty)
        		throw new ArgumentException("The provided key cannot be null or String.Empty.");
        	
            if (key != String.Empty
                && HttpContext.Current != null
                && HttpContext.Current.Request != null)
            {
                HttpContext.Current.Items[key] = value;
            }
        }

        public override object GetRequest(string key)
        {
        	if (key == String.Empty)
        		throw new ArgumentException("The provided key cannot be null or String.Empty.");
        	
        	if (key != String.Empty && HttpContext.Current != null && HttpContext.Current.Items != null)
	            return HttpContext.Current.Items[key];
	        else
	        	return null;
        }
        #endregion

        
		public override string[] GetKeys(StateScope scope)
		{
			List<string> keys = new List<string>();
			
			switch (scope)
			{
				case StateScope.Application:
					foreach (string key in HttpContext.Current.Application.AllKeys)
						keys.Add(key);
					break;
				case StateScope.Session:
					foreach (string key in HttpContext.Current.Session.Keys)
						keys.Add(key);
					break;
				case StateScope.Operation:
					foreach (string key in HttpContext.Current.Items.Keys)
						keys.Add(key);
					break;
			}
			
			return keys.ToArray();
		}
        
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            StateAccess.State = this;
            

            base.Initialize(name, config);
        }

    }
}
