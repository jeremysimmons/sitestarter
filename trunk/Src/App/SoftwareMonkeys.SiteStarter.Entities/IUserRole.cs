using System;
using System.Data;
using System.Configuration;

namespace SoftwareMonkeys.SiteStarter.Entities
{
    /// <summary>
    /// Defines the interface for a user role in the application.
    /// </summary>
    public interface IUserRole
    {
        string Name { get;set; }
	UserPermission[] Permissions { get;set; }
    }
}