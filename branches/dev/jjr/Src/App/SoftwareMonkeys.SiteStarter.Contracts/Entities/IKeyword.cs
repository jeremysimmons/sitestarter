using System;
using System.Data;
using System.Configuration;

namespace SoftwareMonkeys.SiteStarter.Entities
{
    /// <summary>
    /// Defines the interface for a keyword in the application.
    /// </summary>
    public interface IKeyword : IEntity
    {
        string Name { get; }
        string Description { get;set; }
        string[] Keywords { get;set; }
    }
}