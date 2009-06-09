using System;
using System.Data;
using System.Configuration;

namespace SoftwareMonkeys.SiteStarter.Entities
{
    /// <summary>
    /// Defines the interface for a user in the application.
    /// </summary>
    [DataStore("Users")]
    public interface IUser : IEntity
    {
        string Name { get; }
        string Username { get;set; }
        string Password { get;set; }
        string Email { get;set;}
        string FirstName { get;set; }
        string LastName { get;set; }
        IUserRole[] Roles { get;set; }
        Guid[] RoleIDs { get;set; }
        
        void AddRole(IUserRole role);
        void RemoveRole(IUserRole role);
		//UserPermission[] Permissions { get;set; }
    }
}