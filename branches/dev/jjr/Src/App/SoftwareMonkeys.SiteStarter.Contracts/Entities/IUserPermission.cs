using System;
using System.Data;
using System.Configuration;

namespace SoftwareMonkeys.SiteStarter.Entities
{
    /// <summary>
    /// Defines the interface for a user in the application.
    /// </summary>
    public interface IUserPermission : IEntity
    {
        string Verb { get;set; }
        string EntityType { get;set; }
    }
}