using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Entities.Tests.Entities
{
    /// <summary>
    /// Defines the interface for a user role in the application.
    /// </summary>
    public interface ITestRole : IEntity
    {
        string Name { get;set; }
		IUserPermission[] Permissions { get;set; }
		ITestUser[] Users {get;set;}
		//Guid[] UserIDs {get;set;}
		//void AddUser(IUser user);
		//void RemoveUser(IUser user);
    }
}
