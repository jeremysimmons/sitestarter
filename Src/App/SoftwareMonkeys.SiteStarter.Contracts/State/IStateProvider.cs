using System;

namespace SoftwareMonkeys.SiteStarter.State
{
	/// <summary>
	/// Defines the interface of all state provider components.
	/// </summary>
	public interface IStateProvider
	{ 
		bool ContainsApplication(string key);
        void SetApplication(string key, object value);
        object GetApplication(string key);
        void RemoveApplication(string key);
        
        bool ContainsSession(string key);
        void SetSession(string key, object value);
        object GetSession(string key);
        void RemoveSession(string key);
        
        bool ContainsOperation(string key);
        void SetOperation(string key, object value);
        object GetOperation(string key);
        void RemoveOperation(string key);
        
        bool ContainsUser(string key);
        void SetUser(string key, object value);
        void SetUser(string key, object value, DateTime expirationDate);
        object GetUser(string key);
        void RemoveUser(string key);
		
		string PhysicalApplicationPath { get;set; }
	}
}
