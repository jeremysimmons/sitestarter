using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Data.Tests.Entities
{
    /// <summary>
    /// Defines the interface for a user in the application.
    /// </summary>
    [DataStore("Users")]
    public interface ITestUser : IEntity
    {
        string Name { get; }
        string Username { get;set; }
        string Password { get;set; }
        string Email { get;set;}
        string FirstName { get;set; }
        string LastName { get;set; }
        ITestRole[] Roles { get;set; }
    }
}
