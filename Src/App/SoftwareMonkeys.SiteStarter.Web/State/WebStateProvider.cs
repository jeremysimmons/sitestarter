using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Web.State.Cookies;
using System.IO;
using System.Web;

namespace SoftwareMonkeys.SiteStarter.Web.State
{
	/// <summary>
	/// Initializes and acts as the the state management provider
	/// </summary>
    public class WebStateProvider : BaseStateProvider 
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
        	
       		if (HttpContext.Current != null
        	    && HttpContext.Current.Session != null)
        	{
        	    return HttpContext.Current.Session[key] != null;
        	}
        	else
        		throw new Exception("Can't check for session variable outside the scope of session state. Check that HttpContext.Current and HttpContext.Current.Session are not null.");
        }
        
        public override void SetSession(string key, object value)
        {
        	if (key == String.Empty)
        		throw new ArgumentException("The provided key cannot be null or String.Empty.");
        	
            if (HttpContext.Current != null
                && HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session[key] = value;
            }
        	else
        		throw new Exception("Can't set session variable outside the scope of session state.");
        }

        public override object GetSession(string key)
        {
        	if (key == String.Empty)
        		throw new ArgumentException("The provided key cannot be null or String.Empty.");
        	
        	if (HttpContext.Current != null && HttpContext.Current.Session != null)
	            return HttpContext.Current.Session[key];
        	else
        		throw new Exception("Can't get session variable outside the scope of session state.");
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
        
        [Obsolete("Use ContainsOperation function instead.")]
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
        
        [Obsolete("Use SetOperation function instead.")]
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

        [Obsolete("Use GetOperation function instead.")]
        public override object GetRequest(string key)
        {
        	if (key == String.Empty)
        		throw new ArgumentException("The provided key cannot be null or String.Empty.");
        	
        	if (key != String.Empty && HttpContext.Current != null && HttpContext.Current.Items != null)
	            return HttpContext.Current.Items[key];
	        else
	        	return null;
        }
        
        [Obsolete("Use RemoveOperation function instead.")]
		public override void RemoveRequest(string key)
		{
			if (ContainsRequest(key))
			{
				HttpContext.Current.Items.Remove(key);
			}
		}
        #endregion
        
        #region Operation state
        public override bool ContainsOperation(string key)
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
        
        public override void SetOperation(string key, object value)
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

        public override object GetOperation(string key)
        {
        	if (key == String.Empty)
        		throw new ArgumentException("The provided key cannot be null or String.Empty.");
        	
        	if (key != String.Empty && HttpContext.Current != null && HttpContext.Current.Items != null)
	            return HttpContext.Current.Items[key];
	        else
	        	return null;
        }
        
		public override void RemoveOperation(string key)
		{
			if (ContainsOperation(key))
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
        	// TODO: Allow the cookie expiry to be set in web.config
        	SetUser(key, value, DateTime.Now.AddHours(24));
        }

        
		public override void SetUser(string key, object value, DateTime expirationDate)
        {
			if (key == String.Empty)
        		throw new ArgumentException("The provided key cannot be null or String.Empty.");

            if (key != String.Empty
                && HttpContext.Current != null
                && HttpContext.Current.Request != null)
            {
				string encryptedValue = CookieCrypter.Encrypt(key, value.ToString(), expirationDate);
				
        		HttpCookie cookie = new HttpCookie(key, encryptedValue);
        		
        		cookie.Path = StateAccess.State.ApplicationPath;
        		
        		if (expirationDate > DateTime.MinValue)
        			cookie.Expires = expirationDate;
        		
        		HttpContext.Current.Response.Cookies.Add(cookie);
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
	        			return CookieCrypter.Decrypt(cookie.Value);
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
        	Initialize ();
            
            base.Initialize(name, config);
        }
        
        public override void Initialize()
        {
        	
            StateAccess.State = this;
            
            PhysicalApplicationPath = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath);
            ApplicationPath = HttpContext.Current.Request.ApplicationPath;
        }

    }
}
