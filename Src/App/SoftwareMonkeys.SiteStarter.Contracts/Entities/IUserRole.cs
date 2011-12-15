using System;
using System.Data;
using System.Configuration;

namespace SoftwareMonkeys.SiteStarter.Entities
{
    /// <summary>
    /// Defines the interface for a user role in the application.
    /// </summary>
    public interface IUserRole : ISimple
    {
        string Name { get;set; }
		//IUserPermission[] Permissions { get;set; }
		IUser[] Users {get;set;}
		//Guid[] UserIDs {get;set;}
		//void AddUser(IUser user);
		//void RemoveUser(IUser user);
    }
}