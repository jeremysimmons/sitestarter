using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Entities;
using System.IO;
using SoftwareMonkeys.SiteStarter.Configuration;
using System.Web;
using SoftwareMonkeys.SiteStarter.Web.State.Cookies;

namespace SoftwareMonkeys.SiteStarter.Web.State
{
	/// <summary>
	/// Initializes and acts as the the state management provider
	/// </summary>
    public class WebStateProvider : StateProvider 
    {
    	private string physicalApplicationPath = String.Empty;
    	/// <summary>
    	/// Gets/sets the full physical path to the room of the running application.
    	/// </summary>
    	public override string PhysicalApplicationPath
    	{
    		get { return physicalApplicationPath; }
    		set { physicalApplicationPath = value; }
    	}
		
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
        
		public override void RemoveApplication(string key)
		{
			if (ContainsApplication(key))
			{
				HttpContext.Current.Application.Remove(key);
			}
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
        
		public override void RemoveSession(string key)
		{
			if (ContainsSession(key))
			{
				HttpContext.Current.Session.Remove(key);
			}
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
        
		public override void RemoveRequest(string key)
		{
			if (ContainsRequest(key))
			{
				HttpContext.Current.Items.Remove(key);
			}
		}
        #endregion

        #region User state
        public override bool ContainsUser(string key)
        {
        	if (key == String.Empty)
        		throw new ArgumentException("The provided key cannot be null or String.Empty.");
        	
       		if (key != String.Empty
        	    && HttpContext.Current != null
                && HttpContext.Current.Request != null
        	    && HttpContext.Current.Request.Cookies[key] != null)
	            return true;
	        else
	        	return false;
        }
        
        public override void SetUser(string key, object value)
        {
        	SetUser(key, value, DateTime.MinValue);
        }

        
        public override void SetUser(string key, object value, DateTime expirationDate)
        {
        	if (key == String.Empty)
        		throw new ArgumentException("The provided key cannot be null or String.Empty.");
        	
            if (key != String.Empty
                && HttpContext.Current != null
                && HttpContext.Current.Request != null)
            {
        		HttpCookie cookie = new HttpCookie(key, value.ToString());
        		
        		cookie.Path = Config.Application.ApplicationPath;
        		
        		if (expirationDate > DateTime.MinValue)
        			cookie.Expires = expirationDate;
        		
        		HttpContext.Current.Response.Cookies.Add(HttpSecureCookie.Encode(cookie));
            }
        }

        public override object GetUser(string key)
        {
        	if (key == String.Empty)
        		throw new ArgumentException("The provided key cannot be null or String.Empty.");
        	
        	if (key != String.Empty && HttpContext.Current != null && HttpContext.Current.Request != null)
        	{
        		HttpCookie cookie = HttpContext.Current.Request.Cookies[key];
        		
        		if (cookie != null)
        		{
        			HttpCookie decodedCookie = HttpSecureCookie.Decode(cookie);
        			if (decodedCookie != null)
	        			return decodedCookie.Value;
        		}
        	}
        	
	        return null;
        }
        
		public override void RemoveUser(string key)
		{
			if (ContainsUser(key))
			{
				HttpContext.Current.Response.Cookies.Remove(key);
			}
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
            
            PhysicalApplicationPath = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath);
            
            base.Initialize(name, config);
        }

    }
}
